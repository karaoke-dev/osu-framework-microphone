# Tag releases

Pushing a tag to the canonical repo triggers [`.github/workflows/deploy-pack.yml`](../.github/workflows/deploy-pack.yml) (`Tagged Release`), which runs two jobs:

1. **Set Package Version** (`ubuntu-latest`) — hand-clones the repo shallowly (no `actions/checkout`) and derives the version from `git describe --exact-match --tags HEAD`. If HEAD isn't exactly on a tag it aborts with exit 128.
2. **Pack (Framework)** (`windows-latest`) — `dotnet build` + `dotnet pack -c Release osu.Framework.Microphone /p:Version=<tag> /p:GenerateDocumentationFile=true`, uploads the `.nupkg` as a build artifact, then `dotnet nuget push --api-key ${{ secrets.NUGET_AUTH_TOKEN }}` in the same job.

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
2. **The nuget.org API key only exists for the canonical repo.** The publish step needs `secrets.NUGET_AUTH_TOKEN`. It isn't a repo-level secret on `karaoke-dev/osu-framework-microphone` (`gh api repos/karaoke-dev/osu-framework-microphone/actions/secrets` → `total_count: 0`), so it's inherited from the `karaoke-dev` org. Forks can't read org secrets, so even with Actions enabled the push would fail to authenticate.

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

Tag `2025.0614.2` points exactly at that last fix commit, and `deploy-pack.yml` hasn't changed since — so the workflow **as it currently stands is the one that succeeded**, not an untested draft. Don't preemptively rewrite it.

But it's still carrying rot that a future runner-image change can break:

- `::set-output` (superseded by `$GITHUB_OUTPUT`)
- `actions/checkout@v2`, `actions/setup-dotnet@v3`

So: push the tag, watch the run, and be ready to fix the workflow and re-tag with a bumped `PATCH` — that's the established pattern here, and re-pushing the same tag won't re-trigger cleanly. Note also that `Tagged Release` run history is not a reliable audit trail; GitHub ages runs out, and the 2025-06-14 runs have already been purged (`total_count: 0`).

If you're modernising the workflow, consider switching the publish step to [nuget.org Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing) as the sibling `osu-framework-font` repo did — it drops the long-lived API key, at the cost of locking publishing to the `(karaoke-dev, osu-framework-microphone, deploy-pack.yml)` triple. Do it as its own PR, not bundled into a release.
