// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using ManagedBass;
using osu.Framework.Extensions.TypeExtensions;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.Input
{
    public class MicrophoneManager
    {
        private readonly List<string> microphoneDeviceNames;

        /// <summary>
        /// The names of all available audio devices.
        /// </summary>
        /// <remarks>
        /// This property does not contain the names of disabled audio devices.
        /// </remarks>
        public IEnumerable<string> MicrophoneDeviceNames => microphoneDeviceNames;

        public MicrophoneManager()
        {
            // Get device on ctor
            var microphoneDevices = EnumerateAllDevices().ToList();
            microphoneDeviceNames = microphoneDevices.Where(d => d.IsEnabled && d.Type == DeviceType.Microphone)
                                                     .Select(d => d.Name).ToList();
        }

        protected virtual IEnumerable<DeviceInfo> EnumerateAllDevices()
        {
            int deviceCount = Bass.RecordingDeviceCount;
            for (int i = 0; i < deviceCount; i++)
                yield return Bass.RecordGetDeviceInfo(i);
        }

        public override string ToString() => $@"{GetType().ReadableName()}";
    }
}
