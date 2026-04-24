using GameFramework;
using GameFramework.Sound;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public static class SoundExtension
    {
        private const float FadeVolumeDuration = 1f;
        private static int? _musicSerialId;

        public static int? PlayMusic(this SoundComponent soundComponent, string assetName, object userData = null)
        {
            soundComponent.StopMusic();
            var playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = 64;
            playSoundParams.Loop = true;
            playSoundParams.VolumeInSoundGroup = 1f;
            playSoundParams.FadeInSeconds = FadeVolumeDuration;
            playSoundParams.SpatialBlend = 0f;
            _musicSerialId = soundComponent.PlaySound(assetName, "Music", Constant.AssetPriority.MusicAsset, playSoundParams, null, userData);
            return _musicSerialId;
        }

        public static void StopMusic(this SoundComponent soundComponent)
        {
            if (!_musicSerialId.HasValue)
            {
                return;
            }

            soundComponent.StopSound(_musicSerialId.Value, FadeVolumeDuration);
            _musicSerialId = null;
        }

        public static int? PlaySound(this SoundComponent soundComponent, string assetName, Entity bindingEntity = null, object userData = null)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Log.Warning("Can not load sound '{0}' from data table.", assetName);
                return null;
            }

            var playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = 0;
            playSoundParams.Loop = false;
            playSoundParams.VolumeInSoundGroup = 1;
            playSoundParams.SpatialBlend = 1;

            var soundAssetName = assetName;
            return soundComponent.PlaySound(soundAssetName, "Sound", Constant.AssetPriority.SoundAsset, playSoundParams,
                bindingEntity != null ? bindingEntity : null, userData);
        }

        public static int? PlayUISound(this SoundComponent soundComponent, string assetName, float volume = 1, int priority = 0, object userData = null)
        {
            var playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = priority;
            playSoundParams.Loop = false;
            playSoundParams.VolumeInSoundGroup = volume;
            playSoundParams.SpatialBlend = 0f;
            return soundComponent.PlaySound(assetName, "UISound", Constant.AssetPriority.UISoundAsset, playSoundParams, userData);
        }

        public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return true;
            }

            var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return true;
            }

            return soundGroup.Mute;
        }

        public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Mute = mute;

            GameModule.Setting.SetBool(Utility.Text.Format(Constant.Setting.SoundGroupMuted, soundGroupName), mute);
            GameModule.Setting.Save();
        }

        public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return 0f;
            }

            var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return 0f;
            }

            return soundGroup.Volume;
        }

        public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Volume = volume;

            GameModule.Setting.SetFloat(Utility.Text.Format(Constant.Setting.SoundGroupVolume, soundGroupName), volume);
            GameModule.Setting.Save();
        }
    }
}