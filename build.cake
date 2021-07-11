// Tools needed by cake addins
#tool nuget:?package=vswhere&version=2.7.1

// Cake Addins
#addin nuget:?package=Cake.FileHelpers&version=3.1.0

using System.Xml.Linq;
using System.Text.RegularExpressions;

var TARGET = Argument ("t", Argument ("target", "Default"));
var BUILD_CONFIG = Argument ("config", "Release");
var VERBOSITY = Argument ("v", Argument ("verbosity", Verbosity.Normal));
var MAX_CPU_COUNT = Argument("maxcpucount", 0);

// Lists all the artifacts and their versions for com.android.support.*
// https://dl.google.com/dl/android/maven2/com/android/support/group-index.xml
// Master list of all the packages in the repo:
// https://dl.google.com/dl/android/maven2/master-index.xml

var REF_DOCS_URL = "https://bosstoragemirror.blob.core.windows.net/android-docs-scraper/ea/ea65204c51cf20873c17c32584f3b12ed390ac55/android-support.zip";

// Resolve Xamarin.Android installation
var XAMARIN_ANDROID_PATH = EnvironmentVariable ("XAMARIN_ANDROID_PATH");
var ANDROID_SDK_BASE_VERSION = "v1.0";
var ANDROID_SDK_VERSION = "v9.0";
string AndroidSdkBuildTools = $"29.0.2";

if (string.IsNullOrEmpty(XAMARIN_ANDROID_PATH)) {
	if (IsRunningOnWindows()) {
		var vsInstallPath = VSWhereLatest(new VSWhereLatestSettings 
		{ 
			Requires = "Component.Xamarin",
			IncludePrerelease = true
		});
		XAMARIN_ANDROID_PATH = vsInstallPath.Combine("Common7/IDE/ReferenceAssemblies/Microsoft/Framework/MonoAndroid").FullPath;
	} else {
		if (DirectoryExists("/Library/Frameworks/Xamarin.Android.framework/Versions/Current/lib/xamarin.android/xbuild-frameworks/MonoAndroid"))
			XAMARIN_ANDROID_PATH = "/Library/Frameworks/Xamarin.Android.framework/Versions/Current/lib/xamarin.android/xbuild-frameworks/MonoAndroid";
		else
			XAMARIN_ANDROID_PATH = "/Library/Frameworks/Xamarin.Android.framework/Versions/Current/lib/xbuild-frameworks/MonoAndroid";
	}
}
if (!DirectoryExists($"{XAMARIN_ANDROID_PATH}/{ANDROID_SDK_VERSION}"))
	throw new Exception($"Unable to find Xamarin.Android {ANDROID_SDK_VERSION} at {XAMARIN_ANDROID_PATH}.");

// Load all the git variables
var BUILD_COMMIT = EnvironmentVariable("BUILD_SOURCEVERSION") ?? "DEV";
var BUILD_NUMBER = EnvironmentVariable("BUILD_NUMBER") ?? "DEBUG";
var BUILD_TIMESTAMP = DateTime.UtcNow.ToString();

var REQUIRED_DOTNET_TOOLS = new [] {
	"xamarin-android-binderator",
	"xamarin.androidx.migration.tool"
};

string[] Configs = new []
{
	"Debug",
	"Release"
};

// Log some variables
Information ("XAMARIN_ANDROID_PATH: {0}", XAMARIN_ANDROID_PATH);
Information ("ANDROID_SDK_VERSION:  {0}", ANDROID_SDK_VERSION);
Information ("BUILD_COMMIT:         {0}", BUILD_COMMIT);
Information ("BUILD_NUMBER:         {0}", BUILD_NUMBER);
Information ("BUILD_TIMESTAMP:      {0}", BUILD_TIMESTAMP);

// You shouldn't have to configure anything below here
// ######################################################

void RunProcess(FilePath fileName, string processArguments)
{
	var exitCode = StartProcess(fileName, processArguments);
	if (exitCode != 0)
		throw new Exception ($"Process {fileName} exited with code {exitCode}.");
}

string[] RunProcessWithOutput(FilePath fileName, string processArguments)
{
	var exitCode = StartProcess(fileName, new ProcessSettings {
		Arguments = processArguments,
		RedirectStandardOutput = true,
		RedirectStandardError = true
	}, out var procOut);
	if (exitCode != 0)
		throw new Exception ($"Process {fileName} exited with code {exitCode}.");
	return procOut.ToArray();;
}

Task("javadocs")
	.Does(() =>
{
	if (!FileExists("./externals/docs.zip"))
		DownloadFile(REF_DOCS_URL, "./externals/docs.zip");

	if (!DirectoryExists("./externals/docs"))
		Unzip ("./externals/docs.zip", "./externals/docs");

	var astJar = new FilePath("./util/JavaASTParameterNames-1.0.jar");
	var sourcesJars = GetFiles("./externals/**/*-sources.jar");

	foreach (var srcJar in sourcesJars) {
		var srcJarPath = MakeAbsolute(srcJar).FullPath;
		var outTxtPath = srcJarPath.Replace("-sources.jar", "-paramnames.txt");
		var outXmlPath = srcJarPath.Replace("-sources.jar", "-paramnames.xml");

		RunProcess("java", "-jar \"" + MakeAbsolute(astJar).FullPath + "\" --text \"" + srcJarPath + "\" \"" + outTxtPath + "\"");
		RunProcess("java", "-jar \"" + MakeAbsolute(astJar).FullPath + "\" --xml \"" + srcJarPath + "\" \"" + outXmlPath + "\"");
	}
});

Task("check-tools")
	.Does(() =>
{
	var installedTools = RunProcessWithOutput("dotnet", "tool list -g");
	foreach (var toolName in REQUIRED_DOTNET_TOOLS) {
		if (installedTools.All(l => l.IndexOf(toolName, StringComparison.OrdinalIgnoreCase) == -1))
			throw new Exception ($"Missing dotnet tool: {toolName}");
	}
});

Task("binderate")
	.Does(() =>
{
	var configFile = MakeAbsolute(new FilePath("./config.json")).FullPath;
	var basePath = MakeAbsolute(new DirectoryPath ("./")).FullPath;

	// Run the dotnet tool for binderator
	RunProcess("xamarin-android-binderator",
		$"--config=\"{configFile}\" --basepath=\"{basePath}\"");

	// format the targets file so they are pretty in the package
	var targetsFiles = GetFiles("generated/**/*.targets");
	var xmlns = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
	foreach (var targets in targetsFiles) {
		var xdoc = XDocument.Load(targets.FullPath);
		xdoc.Save(targets.FullPath);
	}
});

Task("libs")
	.Does(() =>
{
	foreach(string config in Configs)
	{
		MSBuild("./generated/AndroidSupport.sln", c => {
			c.Configuration = config;
			c.Restore = true;
			c.Properties.Add("DesignTimeBuild", new [] { "false" });
		});
	}
});

Task("nuget")
	.IsDependentOn("libs")
	.Does(() =>
{
	MSBuild ("./generated/AndroidSupport.sln", c => {
		c.Configuration = "Release";
		c.MaxCpuCount = MAX_CPU_COUNT;
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.Properties.Add("NoBuild", new [] { "true" });
	});
});

Task("samples")
	.IsDependentOn("nuget")
	.Does(() =>
{
	// TODO: make this actually work with more than just this sample

	// make a big .targets file that pulls in everything
	var xmlns = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
	var itemGroup = new XElement(xmlns + "ItemGroup");
	foreach (var nupkg in GetFiles("./output/*.nupkg")) {
		// Skip Wear as it has special implications requiring more packages to be used properly in an app
		if (nupkg.FullPath.Contains(".Wear."))
			continue;
		// Skip the migration packages as that is not meant forto be used here
		if (nupkg.FullPath.Contains("Xamarin.AndroidX.Migration"))
			continue;
		var filename = nupkg.GetFilenameWithoutExtension();
		var match = Regex.Match(filename.ToString(), @"(.+?)\.(\d+[\.0-9\-a-zA-Z]+)");
		itemGroup.Add(new XElement(xmlns + "PackageReference",
			new XAttribute("Include", match.Groups[1]),
			new XAttribute("Version", match.Groups[2])));
	}
	var xdoc = new XDocument(new XElement(xmlns + "Project", itemGroup));
	xdoc.Save("./output/AllPackages.targets");

	// clear the packages folder so we always use the latest
	var packagesPath = MakeAbsolute((DirectoryPath)"./samples/packages").FullPath;
	EnsureDirectoryExists(packagesPath);
	CleanDirectories(packagesPath);


	foreach(string config in Configs)
	{
		// build the samples
		var settings = new MSBuildSettings()
			.SetConfiguration(config)
			.SetVerbosity(VERBOSITY)
			.SetMaxCpuCount(0)
			.EnableBinaryLogger("./output/samples.binlog")
			.WithRestore()
			.WithProperty("RestorePackagesPath", packagesPath)
			.WithProperty("DesignTimeBuild", "false")
			.WithProperty("AndroidSdkBuildToolsVersion", AndroidSdkBuildTools);

		MSBuild("./samples/BuildAll/BuildAll.sln", settings);
	}
});

Task ("merge")
	.IsDependentOn ("libs")
	.Does (() =>
{
	var allDlls = GetFiles ($"./generated/*/bin/{BUILD_CONFIG}/monoandroid*/Xamarin.*.dll");
	var mergeDlls = allDlls
		.GroupBy(d => new FileInfo(d.FullPath).Name)
		.Select(g => g.FirstOrDefault())
		.ToList();

	EnsureDirectoryExists("./output/");
	RunProcess("androidx-migrator",
		$"merge" +
		$"  --assembly " + string.Join(" --assembly ", mergeDlls) +
		$"  --output ./output/AndroidSupport.Merged.dll" +
		$"  --search \"{XAMARIN_ANDROID_PATH}/{ANDROID_SDK_VERSION}\" " +
		$"  --search \"{XAMARIN_ANDROID_PATH}/{ANDROID_SDK_BASE_VERSION}\" " +
		$"  --inject-assemblyname");
});

Task("inject-variables")
	.WithCriteria(!BuildSystem.IsLocalBuild)
	.Does(() =>
{
	var glob = "./source/AssemblyInfo.cs";

	ReplaceTextInFiles(glob, "{BUILD_COMMIT}", BUILD_COMMIT);
	ReplaceTextInFiles(glob, "{BUILD_NUMBER}", BUILD_NUMBER);
	ReplaceTextInFiles(glob, "{BUILD_TIMESTAMP}", BUILD_TIMESTAMP);
});

Task ("clean")
	.Does (() =>
{
	if (DirectoryExists ("./externals"))
		DeleteDirectory ("./externals", new DeleteDirectorySettings { Recursive = true, Force = true });

	if (DirectoryExists ("./generated"))
		DeleteDirectory ("./generated", new DeleteDirectorySettings { Recursive = true, Force = true });

	if (DirectoryExists ("./util/binderator"))
		DeleteDirectory ("./util/binderator", new DeleteDirectorySettings { Recursive = true, Force = true });

	CleanDirectories ("./**/packages");
});

Task ("ci")
	.IsDependentOn ("check-tools")
	.IsDependentOn ("inject-variables")
	.IsDependentOn ("binderate")
	.IsDependentOn ("merge")
	.IsDependentOn ("nuget");

RunTarget (TARGET);
