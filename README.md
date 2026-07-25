# PatchHarbor

PatchHarbor is a free, self-hosted vulnerability disclosure platform for game-server communities. It is written in C# with ASP.NET Core and stores reports in a local JSON file for the first MVP.

## MVP capabilities

- Public vulnerability report submission via `POST /api/reports`
- Severity, affected component, version, and reproduction-step fields
- Moderator-protected report listing and detail views
- Separate administrator and moderator access keys
- Admin filtering by status, severity, search text, and page size
- Local audit history for submissions and status transitions
- Local community branding and disclosure settings
- Redacted public advisory pages for explicitly disclosed reports
- Admin status workflow: submitted, acknowledged, investigating, resolved, disclosed, closed
- No external services or paid dependencies
- Data stays on the community's own host
- Built-in per-IP rate limiting for public report intake

## Run locally

```powershell
$env:PATCHHARBOR_ADMIN_KEY = "change-me-before-production"
dotnet run --project .\PatchHarbor.Web
```

Health check: `GET http://localhost:5000/health`.

Public report intake is limited to 10 submissions per IP per minute by default. Adjust the policy in `Program.cs` for a trusted internal deployment, or place PatchHarbor behind a reverse proxy with additional abuse controls.

Submit a report (the API returns a receipt without echoing private reporter details):

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5000/api/reports -ContentType "application/json" -Body '{"title":"Example issue","description":"Explain the impact.","serverName":"Example Community","severity":"High","affectedComponent":"Moderation plugin","affectedVersion":"1.0.0","reproductionSteps":"1. Join the server. 2. Run the command.","reporterContact":"security@example.org"}'
```

List reports with the moderator or administrator key:

```powershell
Invoke-RestMethod -Headers @{"X-Admin-Key"=$env:PATCHHARBOR_ADMIN_KEY} http://localhost:5000/api/reports
```

Set both roles in production. Keep these values outside source control:

```powershell
$env:PATCHHARBOR_ADMIN_KEY = "long-random-administrator-key"
$env:PATCHHARBOR_MODERATOR_KEY = "long-random-moderator-key"
```

Moderators can triage reports and read audit history. Only administrators can update community settings.

When an administrator enables `publicDisclosureEnabled`, reports moved to `Disclosed` appear through the redacted advisory API:

```powershell
Invoke-RestMethod http://localhost:5000/api/advisories
Invoke-RestMethod http://localhost:5000/api/advisories/{advisory-id}
```

Advisories never return descriptions, reproduction steps, reporter contacts, or private audit events.

Filter triage results:

```powershell
Invoke-RestMethod -Headers @{"X-Admin-Key"=$env:PATCHHARBOR_ADMIN_KEY} "http://localhost:5000/api/reports?severity=High&status=Submitted&page=1&pageSize=25"
```

Read or update community settings:

```powershell
Invoke-RestMethod http://localhost:5000/api/community
Invoke-RestMethod -Method Put -Headers @{"X-Admin-Key"=$env:PATCHHARBOR_ADMIN_KEY} -Uri http://localhost:5000/api/community -ContentType "application/json" -Body '{"name":"Example Game Community","description":"Security disclosures for Example Game.","securityContactUrl":"https://example.org/security","publicDisclosureEnabled":false}'
```

View a report's audit history:

```powershell
Invoke-RestMethod -Headers @{"X-Admin-Key"=$env:PATCHHARBOR_ADMIN_KEY} http://localhost:5000/api/reports/{report-id}/audit
```

## Safety notes

PatchHarbor is a disclosure workflow, not an exploit-hosting system. Communities should define their own authorization, retention, moderation, and coordinated-disclosure policies before production use. Replace the development admin-key approach with proper identity and role management before exposing the service to the internet.

Back up the `PatchHarbor.Web/data` directory. It contains community settings, reports, and audit history in the current alpha storage model.
