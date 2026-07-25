# PatchHarbor early-alpha roadmap

The target is a genuinely usable early alpha for small game-server communitiesâ€”not a throwaway demo.

## Sprint 1 â€” Disclosure foundation

- [x] Public report submission API
- [x] Admin-protected listing and detail endpoints
- [x] Status workflow from submission to disclosure
- [x] Local, self-hosted persistence
- [x] Build and smoke-test coverage for the core flow

## Sprint 2 â€” Community-ready workflow

- [ ] Server/community configuration and branding
- [ ] Report intake acknowledgements
- [ ] Severity, affected component, reproduction steps, and attachments metadata
- [ ] Search, filtering, pagination, and audit events

## Sprint 3 â€” Safe access and moderation

- [ ] Pluggable authentication and roles for owners, moderators, and reporters
- [ ] Rate limiting, anti-spam controls, and abuse reporting
- [ ] Reporter privacy controls and contact verification
- [ ] Secrets and production configuration guidance

## Sprint 4 â€” Coordinated disclosure

- [ ] Private report discussion and moderator notes
- [ ] Disclosure timeline and publication approval
- [ ] Public advisory pages with redaction controls
- [ ] Email/webhook notification adapters

## Sprint 5 â€” Self-hosted release quality

- [ ] Docker image and compose deployment
- [ ] Upgrade and backup/restore instructions
- [ ] Health checks and structured logs
- [ ] Automated tests for authorization, validation, persistence, and disclosure transitions
- [ ] Security review and alpha release checklist

## Alpha exit criteria

PatchHarbor is ready for early-alpha users when a community can deploy it from documented instructions, submit and triage reports safely, control who can view private reports, publish a redacted advisory, back up its data, and upgrade without losing reports.
