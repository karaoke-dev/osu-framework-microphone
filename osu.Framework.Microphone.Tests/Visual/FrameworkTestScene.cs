// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.IO.Stores;
using osu.Framework.Testing;

namespace osu.Framework.Tests.Visual;

public abstract partial class FrameworkTestScene : TestScene
{
    protected override ITestSceneTestRunner CreateRunner() => new FrameworkTestSceneTestRunner();

    private partial class FrameworkTestSceneTestRunner : TestSceneTestRunner
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(FrameworkTestScene).Assembly), "Resources"));
        }
    }
}
