// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.iOS;

namespace osu.Framework.Tests;

public static class Application
{
    // This is the main entry point of the application.
    public static void Main(string[] args) => GameApplication.Main(new VisualTestGame());
}
