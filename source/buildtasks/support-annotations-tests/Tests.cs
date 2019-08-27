using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Xamarin.Android.Support.BuildTasks.Tests
{
	public class Tests
	{
		[Fact]
		public void Test_Get_Project_Asset_Nugets()
		{
			var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "TestData");

			var packageVersions = new Dictionary<string, string>();
			NugetPackages.GatherProjectJsonVersions(
				VerifyVersionsTask.PACKAGE_ID_PREFIX,
				path,
				VerifyVersionsTask.ExcludedPackages,
				new Version(8, 1),
				packageVersions);

			Assert.NotEmpty(packageVersions);
		}

		[Fact]
		public void Test_Should_Detect_Multiple_Versions_Project_Assets()
		{
			var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "TestData");

			var packageVersions = new Dictionary<string, string>();
			NugetPackages.GatherProjectJsonVersions(
				VerifyVersionsTask.PACKAGE_ID_PREFIX,
				path,
				VerifyVersionsTask.ExcludedPackages,
				new Version(8, 1),
				packageVersions);

			Assert.NotEmpty(packageVersions);

			var distinctVersions = NugetPackages.GetDistinctVersions(VerifyVersionsTask.PACKAGE_ID_PREFIX, VerifyVersionsTask.ExcludedPackages, packageVersions);

			Assert.True(distinctVersions > 1);
		}

		[Theory]
		[InlineData(23, "23.*")]
		[InlineData(24, "24.*")]
		[InlineData(25, "25.*")]
		[InlineData(26, "26.*")]
        [InlineData(29, "28.*")]
		public void Test_Recommended_NuGet_Version(int apiLevel, string expectedVersion)
		{
			var v = NugetPackages.GetRecommendedSupportPackageVersion(apiLevel);

			Assert.Equal(expectedVersion, v);
		}
	}
}
