using BepInEx;
using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ItemsReworked.Handlers
{
    public class AudioInfo
    {
        public bool IsPlaying { get; set; }
        public AudioSource AudioSource { get; set; }

    }
    public class AudioHandler
    {
        public static Dictionary<PlayerControllerB, AudioInfo> AudioInfoMap = new Dictionary<PlayerControllerB, AudioInfo>();

        public static void PlaySound(PlayerControllerB player, string soundFileName, float minDistance = 3f, float maxDistance = 10f, float volume = 1f, float spacialBlend = 1f, float dopplerLevel = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Linear)
        {
            StopAllSounds();

            if (!AudioInfoMap.TryGetValue(player, out var audioInfo))
            {
                audioInfo = new AudioInfo
                {
                    IsPlaying = false,
                    AudioSource = player.gameObject.AddComponent<AudioSource>()
                };

                // AudioSource properties
                audioInfo.AudioSource.spatialBlend = spacialBlend;
                audioInfo.AudioSource.dopplerLevel = dopplerLevel;
                audioInfo.AudioSource.rolloffMode = rolloffMode;
                audioInfo.AudioSource.minDistance = minDistance;
                audioInfo.AudioSource.maxDistance = maxDistance;
                audioInfo.AudioSource.volume = volume;

                AudioInfoMap[player] = audioInfo;
            }

            if (!audioInfo.IsPlaying)
            {
                string path = $"file://{Paths.PluginPath}\\8-ItemsReworked\\SFX\\{soundFileName}";
                UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);
                audioClip.SendWebRequest();

                while (!audioClip.isDone)
                {
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(audioClip);
                audioInfo.AudioSource.clip = clip;
                audioInfo.AudioSource.PlayOneShot(clip);
                audioInfo.IsPlaying = true;

                // Used to stop the sound after a certain time
                float time = 5.0f;
                Task.Run(async delegate
                {
                    await Task.Delay(TimeSpan.FromSeconds(time));
                    audioInfo.AudioSource.Stop();
                    audioInfo.IsPlaying = false;
                });
            }
        }

        public static void StopAllSounds()
        {
            foreach (var audioInfo in AudioInfoMap.Values)
            {
                if (audioInfo.IsPlaying)
                {
                    audioInfo.AudioSource.Stop();
                    audioInfo.IsPlaying = false;
                }
            }
        }

    }
}

