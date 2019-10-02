# Make a temp folder to move the created nugets to before we fat package them
mkdir -p ./tmp/nuget
mkdir -p ./tmp/output

# Move all of the nupkg files from output into the temp folder we just created
mv ./output/*.nupkg ./tmp/nuget

# Move the remaining output bits to a temp location so they don't get overwritten
mv ./output/* ./tmp/output

dotnet cake nuget.cake `
    --localSource=./tmp/nuget `
    --packagesPath=./tmp/pkgs `
    --workingPath=./tmp/working `
    --outputPath=./output `
    --incrementVersion=False `
    --packLatestOnly=True `
    --useExplicitVersion=True `
    --skipNuspecProcessing=True

# Move the other output bits back to the original output folder
mv ./tmp/output/* ./output
