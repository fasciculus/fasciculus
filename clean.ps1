
@("dotnet", "msbuild") | Foreach-Object { Get-Process $_ -ErrorAction Ignore | ForEach-Object { $_.Kill() }}

$sourceDir = New-Object System.IO.DirectoryInfo("source")
$projectDirs = $sourceDir.GetDirectories()

$projectDirs | ForEach-Object {

    $binDirs = $_.GetDirectories("bin")
    $objDirs = $_.GetDirectories("obj")

    if ($binDirs.Length -gt 0) { $binDirs[0].Delete($true) }
    if ($objDirs.Length -gt 0) { $objDirs[0].Delete($true) }
}
