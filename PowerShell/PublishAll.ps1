# Use command line toggle --incremental to skip deleting publish folder and skip creating archive, thus making build faster.
$PrevPath = Get-Location

Write-Host "Publish for Final Packaging build."
Set-Location $PSScriptRoot

$PublishFolder = "$PSScriptRoot\..\Publish"
$LibraryPublishFolder = "$PublishFolder\Libraries"
$NugetPublishFolder = "$PublishFolder\Nugets"

# Delete current data
if ($Args[0] -ne '--incremental') {
    Remove-Item $PublishFolder -Recurse -Force
}

# Publish Executables
$PublishExecutables = @(
    "Cast\Cast.csproj"
)
foreach ($Item in $PublishExecutables)
{
    dotnet publish $PSScriptRoot\..\Solution\NewWorldFoundation\$Item --use-current-runtime --output $PublishFolder
}
# Publish Windows-only Executables
$PublishWindowsExecutables = @(
)
foreach ($Item in $PublishWindowsExecutables)
{
    dotnet publish $PSScriptRoot\..\Solution\NewWorldFoundation\$Item --runtime win-x64 --self-contained --output $PublishFolder
}
# Publish Loose Libraries
$PublishLibraries = @(
    "Shared\NWF.Shared\NWF.Shared.csproj"
    "Shared\EverythingService\EverythingService.csproj"
)
foreach ($Item in $PublishLibraries)
{
    dotnet publish $PSScriptRoot\..\Solution\NewWorldFoundation\$Item --use-current-runtime --output $LibraryPublishFolder
}
# Publish Windows-only Library Components
$PublishWindowsLibraryComponents = @(
)
foreach ($Item in $PublishWindowsLibraryComponents)
{
    dotnet publish $PSScriptRoot\..\Solution\NewWorldFoundation\$Item --runtime win-x64 --self-contained --output $PublishFolder
}
# Publish Nugets
$PublishNugets = @(
    "Shared\NWF.Shared\NWF.Shared.csproj"
    "Shared\EverythingService\EverythingService.csproj"
)
foreach ($Item in $PublishNugets)
{
    dotnet pack $PSScriptRoot\..\Solution\NewWorldFoundation\$Item --output $NugetPublishFolder
}

# Validation
$sampleExePath = Join-Path $PublishFolder "Cast.exe"
if (-Not (Test-Path $sampleExePath))
{
    Write-Host "Build failed."
    Exit
}

# Create archive
if ($Args[0] -ne '--incremental') {
    $Date = Get-Date -Format yyyyMMdd
    $ArchiveFolder = "$PublishFolder\..\Packages"
    $ArchivePath = "$ArchiveFolder\NWF_Distribution_Windows_B$Date.zip"
    New-Item -ItemType Directory -Force -Path $ArchiveFolder
    Compress-Archive -Path $PublishFolder\* -DestinationPath $ArchivePath -Force
}

Set-Location $PrevPath