// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using AVFoundation;
using osu.Framework.Input.Handlers.Microphone;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace osu.Framework.iOS.Input;

public class IOSMicrophoneHandler : MicrophoneHandler
{
    public override bool IsActive => throw new System.NotImplementedException();

    public IOSMicrophoneHandler()
        : base(-1)
    {
    }

    public override bool Initialize(GameHost host)
    {
        var session = AVAudioSession.SharedInstance();
        bool success = false;

        Logger.Log("Begin Recording", LoggingTarget.Information);

        session.RequestRecordPermission(granted =>
        {
            Logger.Log($"Audio Permission: {granted}", LoggingTarget.Information);

            if (granted)
            {
                var error = session.SetCategory(AVAudioSessionCategory.Record);

                if (error == null)
                {
                    session.SetActive(true, out error);
                    success = base.Initialize(host);
                    Logger.Log($"Microphone get permission status : {success}", LoggingTarget.Information);
                }
                else
                {
                    Logger.Log(error.LocalizedDescription, LoggingTarget.Information, LogLevel.Error);
                }
            }
            else
            {
                Logger.Log("YOU MUST ENABLE MICROPHONE PERMISSION", LoggingTarget.Information, LogLevel.Error);
            }
        });

        Logger.Log($"Checking : {success}", LoggingTarget.Information, LogLevel.Error);
        return success;
    }
}
