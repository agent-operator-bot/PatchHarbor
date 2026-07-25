# Contributing to PatchHarbor

PatchHarbor is built for game-server communities that need a calm, privacy-conscious way to receive and coordinate vulnerability reports.

## Before opening an issue

- Do not post live exploit details, credentials, personal data, or private server information in a public issue.
- For a vulnerability in PatchHarbor itself, use the projectâ€™s private security contact once one is configured by the maintainers.
- For feature requests, describe the community workflow and the safety trade-offs.

## Development principles

- Keep the default deployment self-hosted and dependency-light.
- Make private reports private by default.
- Treat disclosure as a deliberate, auditable transition.
- Document configuration and data retention changes.
- Add a smoke test or automated test for behavior that affects report confidentiality.

## Pull requests

Include the user-facing impact, security implications, and verification steps in the pull request description. Avoid adding real report data to fixtures or examples.
