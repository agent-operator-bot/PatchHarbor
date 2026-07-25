param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$AdminKey = $env:PATCHHARBOR_ADMIN_KEY
)

if ([string]::IsNullOrWhiteSpace($AdminKey)) { throw "Set PATCHHARBOR_ADMIN_KEY before running the smoke test." }

$payload = @{
    title = "Smoke-test report"
    description = "A synthetic report used to verify the disclosure workflow."
    serverName = "PatchHarbor smoke test"
    affectedVersion = "test"
    reporterContact = "smoke-test@example.invalid"
} | ConvertTo-Json

$created = Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/reports" -ContentType "application/json" -Body $payload
$headers = @{ "X-Admin-Key" = $AdminKey }
$listed = @(Invoke-RestMethod -Headers $headers -Uri "$BaseUrl/api/reports")
$updated = Invoke-RestMethod -Method Patch -Uri "$BaseUrl/api/reports/$($created.id)/status" -Headers $headers -ContentType "application/json" -Body '{"status":"Investigating"}'

[pscustomobject]@{
    reportId = $created.id
    listedReports = $listed.Count
    finalStatus = $updated.status
}
