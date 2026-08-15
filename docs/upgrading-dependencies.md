# Upgrading dependencies

This is a recurring task ("update the important packages, fix any test failures, one commit per category"). Follow this playbook — it encodes real breakage found while doing this by hand.

## 1. Figure out the commit grouping from `.github/dependabot.yml`

Groups defined there today:

- **osu-related**: `ppy.osu.Framework`, `ppy.osu.Framework.Android`, `ppy.osu.Framework.iOS` — always bump together to the same version, one commit.
- **dotnet-tools**: `cake.tool`, `jetbrains.resharper.globaltools`, `nvika`, `codefilesanity` (in `.config/dotnet-tools.json`) — one commit.
- Everything else (`Microsoft.NET.Test.Sdk`, `NUnit`, `NUnit3TestAdapter`, `Microsoft.CodeAnalysis.BannedApiAnalyzers` in `Directory.Build.props`, `NWaves`) is ungrouped — each gets its own commit, matching how dependabot PRs land individually for these.

Check `dotnet list <csproj> package --outdated` per project (`osu.Framework.Microphone`, `.Tests`, `.Android`, `.iOS`) to see what's actually behind — `--outdated` doesn't work on `.slnf` files directly, pass individual `.csproj` paths.

## 2. Cross-check versions against `ppy/osu-framework` before trusting "latest"

This repo's build tooling (`Directory.Build.props`, `.config/dotnet-tools.json`, CI structure) is forked from `ppy/osu-framework`'s own setup. For packages shared with that lineage, check what upstream currently pins — via `gh api repos/ppy/osu-framework/contents/<path> --jq .content | base64 -d` — before blindly taking NuGet's newest version:

- `Microsoft.CodeAnalysis.BannedApiAnalyzers`: **do not** jump straight to latest (5.x). Every release from 4.14.0 up requires a newer Roslyn/SDK than a typical local `dotnet` install ships (fails with `CS9057`/`CS8032` loading the analyzer). osu-framework itself is still pinned at `3.3.3` — treat that as the practical ceiling until the local/CI SDK is deliberately upgraded.
- `jetbrains.resharper.globaltools`: match osu-framework's pin (check their `.config/dotnet-tools.json`) rather than absolute latest — reduces the chance of an unproven tooling regression (see the Sarif issue below, which affects this repo at *any* version ≥ 2025.x).
- `codefilesanity`: osu-framework pins `0.0.41` — **use exactly this**. NuGet's newest-listed version for this package (`15.0.0` as of 2026-07) is a mis-published stale 2018-era `netcoreapp2.1` build that doesn't even launch on a current machine ("You must install or update .NET to run this application"). Version-number-looks-newer ≠ actually newer; always run the tool after bumping it (`dotnet codefilesanity`), don't just trust `dotnet list package --outdated`.
- `ppy.osu.Framework`(`.Android`/`.iOS`): these three should always be the literal same version number as each other (they're released together upstream).
- `NUnit` / `NUnit3TestAdapter` / `Microsoft.NET.Test.Sdk`: osu-framework's own pins here are old (`NUnit 3.13.3`, `NUnit3TestAdapter 4.4.2`, `Test.Sdk 17.0.0` as of 2026-07) and don't reflect active maintenance — this repo's own dependabot history already tracks these forward independently of upstream, so keep following NuGet's latest for these three rather than rolling back to match osu-framework.

## 3. Known breaking changes when bumping `ppy.osu.Framework*`

- **iOS entry point**: older osu!framework versions used a static `GameApplication.Main(new VisualTestGame())` call in `Program.cs`/`Application.cs`. Current versions require a `GameApplicationDelegate` subclass instead:

  ```csharp
  // AppDelegate.cs
  [Register("AppDelegate")]
  public class AppDelegate : GameApplicationDelegate
  {
      protected override Game CreateGame() => new VisualTestGame();
  }

  // Program.cs
  public static class Program
  {
      public static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
  }
  ```

  Mirror `ppy/osu-framework`'s own `osu.Framework.Tests.iOS/AppDelegate.cs` + `Program.cs` if the exact shape has changed again — check via `gh api "search/code?q=repo:ppy/osu-framework+GameApplicationDelegate"`.

- **`workloads.json`**: this repo pins the iOS workload version (used by CI's `dotnet workload install ios --from-rollback-file workloads.json`) to a specific build. When `ppy.osu.Framework.iOS` bumps far enough, its NuGet package only ships assets for a newer iOS TFM (e.g. `net8.0-ios18.0`), and the pinned-old workload can't resolve a compile-time reference for it at all — `dotnet list package` shows nothing wrong, but `dotnet build` on the `.iOS.slnf` silently produces an *empty* compile item list for `ppy.osu.Framework.iOS` in `obj/project.assets.json`, surfacing as `CS0246: type or namespace not found` for framework types that definitely exist in the DLL. Fix: `dotnet workload update`, then read the resulting version from `dotnet workload list` and write it into `workloads.json`.
- **Android**: the `osu.Framework.Microphone.Android` library project currently has zero `.cs` files (pure packaging wrapper), so framework bumps are low-risk there. `osu.Framework.Microphone.Tests.Android/TestGameActivity.cs` uses the `AndroidGameActivity` + `CreateGame()` override pattern — check this still matches `ppy/osu-framework`'s `osu.Framework.Tests.Android/TestGameActivity.cs` after a big version jump.
- Whenever a framework bump produces a compile error referencing a removed API, don't just chase the compile error — search `ppy/osu-framework`'s git history for *why* it was removed (`gh api "repos/ppy/osu-framework/commits?path=<file>"`) so you know whether there's a real replacement API to call or whether the member was already dead code safe to delete.

## 4. `nvika` / `jetbrains.resharper.globaltools` Sarif incompatibility

Newer `jetbrains.resharper.globaltools` (checked: 2025.2.3 and 2026.1.4, likely all current versions) changed `jb inspectcode`'s default `--output` format to Sarif. `nvika` (stuck at 4.0.0 — no newer version has ever been published) cannot parse Sarif: `[Error] The adequate parser for this report was not found.` and exits 0 anyway (silently, so this can slip past unnoticed in CI logs).

Two fixes, pick one:
1. **Minimal**: add `--format=Xml` explicitly to the `dotnet jb inspectcode ...` invocation in `.github/workflows/ci.yml`. This is what's in place now.
2. **Upstream's actual fix**: `ppy/osu-framework` dropped `nvika` entirely and switched the `InspectCode` CI step to the `JetBrains/ReSharper-InspectCode` GitHub Action, which natively consumes Sarif and uploads to GitHub code scanning (needs `permissions: security-events: write`). This is a bigger CI redesign — worth doing eventually, but treat it as a separate change, not bundled into a routine package bump.

Whichever fix is live, **verify it actually works** locally before committing: `dotnet build ... -p:EnforceCodeStyleInBuild=true`, then `dotnet jb inspectcode <slnf> --no-build --output=report.xml --format=Xml --caches-home=inspectcode --verbosity=WARN`, then `dotnet nvika parsereport report.xml --treatwarningsaserrors` and confirm it prints `N issues was found` (a number), not a parser error. Delete the `inspectcode` cache dir and `report.xml` afterward — don't commit them.

## 5. Local verification checklist (mirrors `.github/workflows/ci.yml`)

```
dotnet restore osu-framework-microphone.Desktop.slnf
dotnet build -c Debug -warnaserror osu-framework-microphone.Desktop.slnf -p:EnforceCodeStyleInBuild=true
dotnet test <path-to-Tests.dll> --no-build   # or: dotnet test osu.Framework.Microphone.Tests/*.csproj --no-build
dotnet tool restore
dotnet codefilesanity
dotnet jb inspectcode ... --format=Xml ...   # see section 4
dotnet nvika parsereport ...

dotnet build osu-framework-microphone.iOS.slnf       # needs ios workload; see workloads.json note above
dotnet build osu-framework-microphone.Android.slnf -p:AndroidSdkDirectory="<path>"   # needs Android SDK *and* a JDK
```

CI runs the tests from the built `.dll` with `build/vstestconfig.runsettings` (a 300s per-test timeout). The baseline is **2 passing tests** (`TestSceneMicrophone`, `TestSceneGetMicrophoneList`) — a `NUnit3TestAdapter` bump that breaks test discovery still reports "success" with 0 tests run, so always confirm the passing *count*, not just that the command exited 0.

Environment gaps seen on a typical dev machine:
- Android build needs both an Android SDK (often present, e.g. `%LOCALAPPDATA%\Android\Sdk`) **and** a JDK (often *not* present — CI installs "Microsoft OpenJDK 11" explicitly). If no JDK is available, say so explicitly rather than claiming Android was verified — don't skip silently.
- iOS build works fine on Windows without a Mac (compiles against ref assemblies), but needs the matching `ios` workload installed — see section 3.

### Gotcha: building from a git worktree nested inside the repo

If your worktree lives *under* the main checkout (e.g. `<repo>/.orca/worktrees/<name>/<branch>`), Roslyn walks up the directory tree and picks up **both** `.globalconfig` files — the parent checkout's and the worktree's. Every duplicated key produces:

```
CSC : warning MultipleGlobalAnalyzerKeys: Multiple global analyzer config files set the same key
'dotnet_diagnostic.ide0004.severity' in section 'Global Section'. ...
```

That's 32 warnings, which `-warnaserror` promotes to 32 errors — a build failure with nothing to do with your dependency bump. Confirm the diagnosis by rebuilding without `-warnaserror` (`dotnet build -c Debug --no-incremental ...`): if you get `32 warnings, 0 errors` and every one is `MultipleGlobalAnalyzerKeys`, it's this. Fix it by putting the worktree **outside** the repo directory, or run the `-warnaserror` verification from the main checkout. CI is unaffected — it checks out a single clean copy.

## 6. Committing

- One commit per dependabot group/category (section 1), each with build+test passing before moving to the next.
- Do not commit unrelated pre-existing local changes (e.g. IDE-generated `.sln.DotSettings` diffs) — check `git status`/`git diff` before `git add` and stage files explicitly by name.

Then open a PR — see [opening-a-pull-request.md](opening-a-pull-request.md).
