param(
    [string]$DataDirectory = (Join-Path $PSScriptRoot "..\PatchHarbor.Web\data"),
    [string]$WorkingDirectory = (Join-Path $PSScriptRoot "..\.backup-verification")
)

$resolvedData = (Resolve-Path -LiteralPath $DataDirectory -ErrorAction Stop).Path
$resolvedWork = [IO.Path]::GetFullPath($WorkingDirectory)
$archive = Join-Path $resolvedWork "patchharbor-data.zip"
$restored = Join-Path $resolvedWork "restored"

if (-not (Test-Path -LiteralPath $resolvedData -PathType Container)) { throw "Data directory does not exist: $resolvedData" }
New-Item -ItemType Directory -Force -Path $resolvedWork | Out-Null
if (Test-Path -LiteralPath $archive) { Remove-Item -LiteralPath $archive -Force }
if (Test-Path -LiteralPath $restored) { Remove-Item -LiteralPath $restored -Recurse -Force }

$files = @(Get-ChildItem -LiteralPath $resolvedData -File)
if ($files.Count -eq 0) { throw "No data files found to verify." }
Compress-Archive -Path (Join-Path $resolvedData "*") -DestinationPath $archive -CompressionLevel Optimal
Expand-Archive -LiteralPath $archive -DestinationPath $restored
$restoredFiles = @(Get-ChildItem -LiteralPath $restored -File)

if ($restoredFiles.Count -ne $files.Count) { throw "Restore file count does not match backup file count." }
foreach ($file in $files) {
    $restoredFile = Join-Path $restored $file.Name
    if (-not (Test-Path -LiteralPath $restoredFile)) { throw "Missing restored file: $($file.Name)" }
    $sourceHash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
    $restoredHash = (Get-FileHash -LiteralPath $restoredFile -Algorithm SHA256).Hash
    if ($sourceHash -ne $restoredHash) { throw "Checksum mismatch: $($file.Name)" }
}

[pscustomobject]@{ status = "ok"; files = $files.Count; archive = $archive }
