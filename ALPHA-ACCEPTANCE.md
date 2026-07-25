# PatchHarbor early-alpha acceptance criteria

PatchHarbor is ready for an early-alpha release when all of these are true.

## Product workflow

- [x] A reporter can submit a disclosure without an account.
- [x] A moderator can review, filter, and triage private reports.
- [x] Status transitions are recorded in an audit timeline.
- [x] An administrator can configure community identity and disclosure settings.
- [x] An administrator can deliberately publish a redacted advisory.
- [x] Public advisories exclude reporter contact, reproduction details, descriptions, and audit events.

## Safety and operations

- [x] Admin and moderator access are separate and documented.
- [x] Public intake has input limits and rate limiting.
- [x] Health checks exist for deployment monitoring.
- [x] Report, audit, and community data are stored under one documented data directory.
- [x] Docker deployment and backup guidance are available.
- [x] Backup restore has a repeatable scripted verification.
- [ ] Production identity integration replaces alpha header keys.

## Verification gates

- [x] Clean .NET build with zero warnings and errors.
- [x] End-to-end report submission and triage smoke test.
- [x] Persistence across an application restart.
- [x] Role separation test.
- [x] Controlled advisory publication test.
- [x] Health and rate-limit test.

The unchecked items are explicitly deferred alpha hardening, not hidden requirements. They block a production claim but do not block a clearly labeled early-alpha release.
