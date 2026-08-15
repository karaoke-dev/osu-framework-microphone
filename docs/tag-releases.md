# Tag releases

Pushing a tag to the canonical repo triggers [`.github/workflows/deploy-pack.yml`](../.github/workflows/deploy-pack.yml) (`Tagged Release`), which runs two jobs:

1. **Set Package Version** (`ubuntu-latest`) — hand-clones the repo shallowly (no `actions/checkout`) and derives the version from `git describe --exact-match --tags HEAD`. If HEAD isn't exactly on a tag it aborts with exit 128.
2. **Pack (Framework)** (`windows-latest`) — `dotnet build` + `dotnet pack -c Release osu.Framework.Microphone /p:Version=<tag> /p:GenerateDocumentationFile=true`, uploads the `.nupkg` as a build artifact, then exchanges the job's GitHub OIDC token for a short-lived nuget.org API key via `NuGet/login@v1` and `dotnet nuget push`es with it — all in the same job.

Two things worth knowing before you tag:

- **Only `osu.Framework.Microphone` is packed and published.** The `.Android` and `.iOS` projects are compile-only — they don't set `IsPackable`, the workflow never names them, and neither `osu.Framework.Microphone.Android` nor `osu.Framework.Microphone.iOS` exists on nuget.org.
- **There's no `--skip-duplicate`.** Pushing a version that already exists on nuget.org fails the job rather than being quietly ignored, so a re-run of the same tag is not a no-op.

The `<Version>1.2.0</Version>` in `osu.Framework.Microphone.csproj` is dead weight — `/p:Version=<tag>` always overrides it. Don't bother bumping it as part of a release.

## Tag naming convention

`YYYY.MMDD.PATCH`, matching this repo's own version history (`git tag -l | sort -V`), e.g.:

```
2023.0627.0
2024.0219.0
2025.0614.0
2025.0614.1
2025.0614.2
```

- `YYYY` — 4-digit year.
- `MMDD` — 4-digit, zero-padded month + day (e.g. June 14th → `0614`, not `614`).
- `PATCH` — starts at `0` for the first release of that day; bump to `1`, `2`, ... if another release goes out the same day (`2022.1227.0`/`.1`/`.2` and `2025.0614.0`/`.1`/`.2` both happened).

There's no `v` prefix — tags are the bare version string, since that string is passed straight through as the package's `Version`.

NuGet **normalises away the leading zero** in the `MMDD` segment: the tag `2025.0614.2` is published as package version `2025.614.2`. That's expected — don't tag `2025.614.2` to "match" nuget.org, since it would sort wrongly against every other tag.

## Which remote to push the tag to

Push tags to `karaoke` (which resolves to `karaoke-dev/osu-framework-microphone` — see [opening-a-pull-request.md](opening-a-pull-request.md)), **not** `origin` (the personal fork). Two independent reasons, not just convention:

1. **Actions are disabled on the fork.** `gh api repos/andy840119/osu-framework-microphone/actions/permissions` returns `{"enabled":false}`, so a tag pushed to `origin` triggers nothing at all — and no failure notification either, it just silently does nothing.
2. **Trusted Publishing is locked to this exact repo.** Publishing uses [nuget.org Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing) (OIDC, via `NuGet/login@v1`) rather than a long-lived API key secret. The policy on nuget.org is bound to a specific `(repository owner, repository, workflow file)` triple — `karaoke-dev` / `osu-framework-microphone` / `deploy-pack.yml`. A run from any other repo, including the personal fork, cannot exchange its OIDC token for a NuGet API key even with a byte-identical workflow file.

```
git fetch karaoke master
git tag 2026.0815.0        # on the commit you want released, following the convention above
git push karaoke 2026.0815.0
```

Then watch it:

```
gh run list --repo karaoke-dev/osu-framework-microphone --workflow=deploy-pack.yml --limit 1
```

## Expect to babysit the run

The last release was a three-tag fight. On 2025-06-14, `deploy-pack.yml` was edited **four times** and three tags were pushed:

| Time (+0800) | Event | Result |
| --- | --- | --- |
| 22:14 | workflow fix `5222f59` | |
| 22:32 | tag `2025.0614.0` | no package on nuget.org |
| 22:43 | workflow fix `a454039` | |
| 23:16 | workflow fix `4905288`, tag `2025.0614.1` | no package on nuget.org |
| 23:22 | workflow fix `30f8111`, tag `2025.0614.2` | published 23:25 |

Tag `2025.0614.2` points exactly at that last fix commit — that shape of the workflow is the one that succeeded.

Since then the publish step has been swapped from a long-lived API key to Trusted Publishing, so **the authentication half is again unproven by a real release**. The rest of the workflow still carries rot that a future runner-image change can break:

- `::set-output` (superseded by `$GITHUB_OUTPUT`)
- `actions/checkout@v2`, `actions/setup-dotnet@v3`

So: push the tag, watch the run, and be ready to fix the workflow and re-tag with a bumped `PATCH` — that's the established pattern here, and re-pushing the same tag won't re-trigger cleanly. Note also that `Tagged Release` run history is not a reliable audit trail; GitHub ages runs out, and the 2025-06-14 runs have already been purged (`total_count: 0`).

If the `NuGet login` step fails to exchange its token, the cause is almost always on the nuget.org side rather than in this repo: the Trusted Publishing policy must exist for the package, be bound to `karaoke-dev` / `osu-framework-microphone` / `deploy-pack.yml`, and be owned by the account named in the step's `user:` input (`andy840119`). Policies for a package that doesn't exist yet also expire if unused — re-check it in nuget.org account settings before assuming the workflow is at fault.
