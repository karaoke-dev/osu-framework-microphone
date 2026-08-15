# Opening a pull request

**This repo has two remotes — check `git remote -v` before you start:**

- `karaoke` → `https://github.com/osu-karaoke/osu-framework-microphone.git` — **the canonical upstream repo. Branch from it, push to it, and open PRs against it.**
- `origin` → `https://github.com/andy840119/osu-framework-microphone.git` — an old personal fork/mirror. Not part of the normal workflow.

## The `karaoke` remote URL is stale but still correct

The `karaoke` remote still points at the **old org name** `osu-karaoke`. The org was renamed to `karaoke-dev`, and GitHub redirects transparently:

```
$ gh api repos/osu-karaoke/osu-framework-microphone --jq .full_name
karaoke-dev/osu-framework-microphone
```

So `git push karaoke` / `git fetch karaoke` work fine, but **`gh` commands must use the current name** — `--repo karaoke-dev/osu-framework-microphone`, not `osu-karaoke/...`. Don't "fix" the remote URL as a drive-by change in an unrelated PR, and don't infer from the remote *name* alone which repo is canonical. Confirm with:

```
gh api repos/karaoke-dev/osu-framework-microphone --jq .permissions
```

Push access implies it's a legitimate target.

## Workflow

Branches live on the canonical repo, so this is a same-repo PR (that's how recent work has landed — see #372, #373, #374, and dependabot's own branches):

```
git fetch karaoke master
git checkout -b <branch> karaoke/master
# ...make changes, commit...
git push -u karaoke <branch>
gh pr create --repo karaoke-dev/osu-framework-microphone --base master --head <branch>
```

Always branch from `karaoke/master`, never from a possibly-stale local `master` or from the `origin` mirror — compare `git rev-parse karaoke/master master` before starting. The fork has been left behind by dozens of commits before.

## Don't open the PR against the fork

A PR whose **base repo** is `andy840119/osu-framework-microphone` is wrong and easy to create by accident — it has happened here. PRs [#193](https://github.com/andy840119/osu-framework-microphone/pull/193) and [#194](https://github.com/andy840119/osu-framework-microphone/pull/194) (the dependabot `dotnet-tools` and `osu-related` bumps) were opened *and merged* on the fork, not upstream. Their commits only reached `karaoke-dev/osu-framework-microphone` much later, dragged in sideways by the merge commit in [#362](https://github.com/karaoke-dev/osu-framework-microphone/pull/362) — which is why `git log` on master shows `Merge pull request #193` sitting underneath a `Merge commit '2b0cf31'` from an unrelated branch.

Passing `--repo karaoke-dev/osu-framework-microphone` explicitly to `gh pr create` (as above) is what prevents this; without it, `gh` picks a base from whatever it infers about the remotes.

If you do work from the fork for some reason, the head must be qualified for a cross-repo PR: `--head andy840119:<branch>`. A bare `--head <branch>` looks for the branch inside `karaoke-dev/osu-framework-microphone` itself.

## CI on the PR

[`.github/workflows/ci.yml`](../.github/workflows/ci.yml) triggers on a bare `on: [push, pull_request]` with no branch filter, so any branch pushed to `karaoke` gets CI immediately, and the PR gets it again. Four jobs, all of which should be green before merging:

| Job | Runner | What it does |
| --- | --- | --- |
| **Code Quality** | `ubuntu-latest` | `-warnaserror` + `EnforceCodeStyleInBuild`, then CodeFileSanity, InspectCode (`--format=Xml`) and NVika (`--treatwarningsaserrors`) |
| **Test** | matrix | Windows / macOS / Linux × `SingleThread` / `MultiThreaded`, 8 legs |
| **Build only (Android)** | `windows-latest` | JDK 11 + `dotnet workload install android` |
| **Build only (iOS)** | `macos-latest` | `dotnet workload install ios --from-rollback-file workloads.json` |

The two platform jobs are the only automated check on the Android/iOS projects, and the macOS leg is the only thing that ever compiles the iOS target on a real Apple toolchain — so for a `ppy.osu.Framework*` bump, treat a red `Build only (iOS)` as the expected place to find breakage rather than a CI flake. See [upgrading-dependencies.md](upgrading-dependencies.md) §3.

Note that Actions are disabled outright on the `origin` fork:

```
$ gh api repos/andy840119/osu-framework-microphone/actions/permissions
{"enabled":false,...}
```

That's a repo-settings fact, not a broken workflow file — one more reason not to route work through the fork.
