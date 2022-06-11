using System;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEngine;

namespace ChaosLocale.Scripts.AssetLocalization
{
    [CreateAssetMenu(fileName = "new localized sound", menuName = "Localization/Localized Sound", order = 5)]
    public class LocalizedSound : ScriptableObject
    {
        public AudioClip fallback;
        public List<SoundTranslation> translations = new List<SoundTranslation>();


        public void CloneSound(int i)
        {
            var trans = translations[i];
            var newTrans = new SoundTranslation(trans.lang, trans.sound);
            translations.Add(newTrans);
        }

        public void DeleteSound(int i)
        {
            translations.RemoveAt(i);
        }

        public void NewSound()
        {
            translations.Add(new SoundTranslation());
        }
        
        public AudioClip GetSound()
        {
            var lang = Localization.GetLanguage();

            var translation = translations.Find(trans => trans.lang == lang);

            if (translation == null) return fallback;

            return translation.sound;
        }
    }

    [Serializable]
    public class SoundTranslation
    {
        protected bool Equals(SoundTranslation other)
        {
            return lang == other.lang;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SoundTranslation) obj);
        }

        public override int GetHashCode()
        {
            return (int) lang;
        }

        public Languages lang;
        public AudioClip sound;


        public SoundTranslation(Languages lang, AudioClip sound)
        {
            this.lang = lang;
            this.sound = sound;
        }


        public SoundTranslation()
        {
            lang = Languages.English;
        }
        
        
        
    }
}