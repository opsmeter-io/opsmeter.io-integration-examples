# Package Rename Strategy (`opsmeter.io` identity)

## Goal

Align package names with the domain brand while keeping installs stable for existing users.

- Current Node package: `@opsmeter/node`
- Target Node package: `@opsmeter.io/node`
- Current Python package: `opsmeter-sdk`
- Target Python package: `opsmeter-io-sdk`

## Principles

1. No breaking API change during rename.
2. Old names stay installable for a transition period.
3. Deprecation is explicit with migration commands.
4. Docs switch only after new names are proven in production.

## Rollout Plan

## Phase 1 — New names published

- Publish `@opsmeter.io/node` from the same source/API as `@opsmeter/node`.
- Publish `opsmeter-io-sdk` from the same source/API as `opsmeter-sdk`.
- Keep provider/model rules unchanged: use [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog).

Success criteria:
- Both new packages install and pass existing test suites.
- Integration examples work with new names.

## Phase 2 — Dual support window

- Keep releasing both old and new package names.
- Add README migration notice in both SDK repos.
- Keep import style unchanged for Python runtime:

```python
import opsmeter_sdk as opsmeter
```

Success criteria:
- Existing users can keep old package names without breakage.
- New users can start with new package names.

## Phase 3 — Deprecation

- npm: deprecate old scope package with clear message.

```bash
npm deprecate @opsmeter/node@"<next_major" "Package moved to @opsmeter.io/node. Install: npm i @opsmeter.io/node"
```

- PyPI: publish migration note on `opsmeter-sdk` releases and README pointing to `opsmeter-io-sdk`.

Success criteria:
- Installers of old names always see migration direction.

## Phase 4 — Docs default switch

- Make docs/examples default to:
  - `npm install @opsmeter.io/node`
  - `pip install opsmeter-io-sdk`
- Keep compatibility notes for old names until sunset date.

## Sunset decision

After adoption threshold is reached (e.g. >90% installs from new names for 2 consecutive releases):

- Freeze old names (no feature updates).
- Keep security/critical fixes only for a limited period.

## Operational Checklist

- Verify ownership/access:
  - npm org scope `@opsmeter.io`
  - PyPI project `opsmeter-io-sdk`
- Ensure CI release workflows can publish each target package.
- Add smoke tests for both old/new install commands.
- Announce in changelogs and repo release notes.
