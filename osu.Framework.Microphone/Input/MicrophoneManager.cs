// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using ManagedBass;
using osu.Framework.Extensions.TypeExtensions;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.Input
{
    public class MicrophoneManager
    {
        private List<DeviceInfo> audioDevices = new List<DeviceInfo>();
        private List<string> audioDeviceNames = new List<string>();

        /// <summary>
        /// The names of all available audio devices.
        /// </summary>
        /// <remarks>
        /// This property does not contain the names of disabled audio devices.
        /// </remarks>
        public IEnumerable<string> AudioDeviceNames => audioDeviceNames;

        public MicrophoneManager()
        {
            // Get device on ctor
            audioDevices = EnumerateAllDevices().ToList();
            audioDeviceNames = audioDevices.Skip(1).Where(d => d.IsEnabled).Select(d => d.Name).ToList();
        }

        protected virtual IEnumerable<DeviceInfo> EnumerateAllDevices()
        {
            int deviceCount = Bass.RecordingDeviceCount;
            for (int i = 0; i < deviceCount; i++)
                yield return Bass.RecordGetDeviceInfo(i);
        }

        protected virtual bool IsCurrentDeviceValid()
        {
            var deviceIndex = Bass.CurrentRecordingDevice;
            var device = deviceIndex == Bass.DefaultDevice ? default : Bass.RecordGetDeviceInfo(deviceIndex);

            return device.IsEnabled && device.IsInitialized;
        }

        public override string ToString() => $@"{GetType().ReadableName()}";
    }
}
