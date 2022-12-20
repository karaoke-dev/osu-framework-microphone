// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Foundation;
using osu.Framework.iOS;

namespace osu.Framework.Tests;

[Register("AppDelegate")]
public class AppDelegate : GameAppDelegate
{
    protected override Game CreateGame() => new VisualTestGame();
}
