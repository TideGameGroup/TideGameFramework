using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Tide.Core
{
    public class UAudio : UComponent
    {
        private Dictionary<string, SoundEffect> soundTable 
            = new Dictionary<string, SoundEffect>();
        private Dictionary<SoundEffectInstance, settingChangedEvent> soundEventTable 
            = new Dictionary<SoundEffectInstance, settingChangedEvent>();

        private readonly UContentManager content;
        private readonly USettings settings;

        public UAudio(UContentManager content, USettings settings)
        {
            this.content = content;
            this.settings = settings;
        }

        public void Cache(string sound)
        {
            try
            {
                SoundEffect soundEffect = content.Load<SoundEffect>(sound);
                soundTable.TryAdd(sound, soundEffect);
            }
            catch (ContentLoadException)
            {
                return;
            }
        }

        private SoundEffect Get(string sound)
        {
            if (soundTable.ContainsKey(sound) == false)
            {
                Cache(sound);
            }

            if (soundTable.ContainsKey(sound))
            {
                return soundTable[sound];
            }

            return null;
        }

        public void PlaySingle(string sound)
        {
            SoundEffect effect = Get(sound);
            if (effect != null)
            {
                effect.Play(settings["volume"].f, 0.0f, 0.0f);
            }
        }

        public SoundEffectInstance Play(string sound)
        {
            SoundEffectInstance instance = Get(sound).CreateInstance();
            instance.Play();

            settingChangedEvent volumeEvent = new settingChangedEvent(() =>
            {
                instance.Volume = settings["volume"].f;
            });

            settings.SetOnChangedCallback("volume", volumeEvent);
            soundEventTable[instance] = volumeEvent;

            volumeEvent.Invoke();
            return instance;
        }

        public void Stop(SoundEffectInstance instance)
        {
            if (soundEventTable.ContainsKey(instance))
            {
                settings.RemoveOnChangedCallback("volume", soundEventTable[instance]);
            }
        }
    }
}
