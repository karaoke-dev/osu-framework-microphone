// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.IO.Stores;
using osu.Framework.Testing;

namespace osu.Framework.Tests.Visual
{
    public abstract class FrameworkTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new FrameworkTestSceneTestRunner();

        private class FrameworkTestSceneTestRunner : TestSceneTestRunner
        {
            [BackgroundDependencyLoader]
            private void load()
            {
                Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(FrameworkTestScene).Assembly), "Resources"));
            }
        }
    }
}
