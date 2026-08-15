# osu.Framework.Microphone

Unofficial osu!framework extension for using a microphone as an input device. Desktop (net8.0) + Android (net8.0-android) + iOS (net8.0-ios) targets sharing one core library (`osu.Framework.Microphone`), each driven by its own `.slnf` filter. Only the desktop library is published to nuget.org.

## Playbooks

Detailed, recurring-task playbooks live under [`docs/`](docs/) so this file stays short:

- [docs/upgrading-dependencies.md](docs/upgrading-dependencies.md) — the "update the important packages, fix any test failures, one commit per category" task: dependabot commit grouping, cross-checking pins against `ppy/osu-framework` (several packages must *not* go to latest), known `ppy.osu.Framework*` breakages including the `workloads.json` iOS trap, the nvika/InspectCode Sarif incompatibility, and the local verification checklist.
- [docs/opening-a-pull-request.md](docs/opening-a-pull-request.md) — this repo has two remotes (`karaoke` = canonical upstream, still pointing at the old `osu-karaoke` org name; `origin` = a stale personal fork). Branch from and push to `karaoke`, and always pass `--repo karaoke-dev/osu-framework-microphone` to `gh pr create`.
- [docs/tag-releases.md](docs/tag-releases.md) — tag naming convention (`YYYY.MMDD.PATCH`), why release tags must be pushed to `karaoke` (the fork has Actions disabled and no nuget.org API key), and why the release run needs babysitting.
