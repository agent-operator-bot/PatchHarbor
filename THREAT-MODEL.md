# PatchHarbor alpha threat model

## Assets

- Private vulnerability reports and reproduction details
- Reporter contact information
- Community configuration and security contact
- Moderator/admin credentials
- Audit history and disclosure decisions

## Trust boundaries

1. The public internet submits untrusted report content.
2. PatchHarbor stores that content on the community host.
3. Moderators and administrators cross the private management boundary using alpha API keys.
4. Public advisory readers must receive only deliberately redacted data.

## Main threats and mitigations

| Threat | Alpha mitigation |
|---|---|
| Spam or submission flooding | Per-IP fixed-window rate limit and input length limits |
| Unauthorized private report access | Moderator/admin key checks on private endpoints |
| Moderator overreach | Separate moderator and administrator keys; audit status changes |
| Accidental disclosure | Public endpoint only returns `Disclosed` reports and a redacted projection |
| Data loss | Documented backup of `PatchHarbor.Web/data`; restore automation is a follow-up gate |
| Credential leakage | Keys are supplied through environment variables and excluded from source control |
| Stored malicious content | PatchHarbor stores report text as data; a UI must HTML-encode it before rendering |

## Explicit alpha limitations

- Header-key authentication is suitable only for a controlled early alpha; use an identity provider before broad public deployment.
- JSON-file storage is single-instance storage and has no concurrent multi-node coordination.
- Attachments, email notifications, and private discussion threads are not enabled yet.
- Operators remain responsible for HTTPS, reverse-proxy controls, backups, retention, and incident response.
