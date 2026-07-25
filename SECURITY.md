# PatchHarbor security policy

## Reporting a vulnerability in PatchHarbor

Do not open a public issue containing exploit details, credentials, private report data, or a working attack chain.

Until a dedicated private reporting address is published, contact the maintainers through the private channel associated with the repository. Include:

- affected version or commit
- deployment type
- impact and affected component
- safe reproduction steps
- any suggested mitigation

Allow maintainers reasonable time to investigate and release a fix before public disclosure.

## Secure deployment guidance

- Put PatchHarbor behind HTTPS and a trusted reverse proxy.
- Set long, random administrator and moderator keys through secret management.
- Restrict access to the admin API at the network layer where possible.
- Back up and protect `PatchHarbor.Web/data`.
- Do not enable public disclosure until reports have been reviewed and redacted.
- Replace the alpha header-key access model with an identity provider before a large or public deployment.
