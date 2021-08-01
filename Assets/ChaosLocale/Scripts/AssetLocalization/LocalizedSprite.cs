using System;
using System.Collections.Generic;
using Locale.Scripts;
using UnityEngine;

namespace ChaosLocale.Scripts.AssetLocalization
{
    [CreateAssetMenu(fileName = "new localized sprite", menuName = "Localization/Localized Sprite", order = 5)]
    public class LocalizedSprite : ScriptableObject
    {
        public Sprite fallback;
        public List<SpriteTranslation> translations = new List<SpriteTranslation>();

        public void NewSprite()
        {
            translations.Add(new SpriteTranslation());
        }

        public void DeleteSprite(int id)
        {
            translations.RemoveAt(id);
        }

        public void CloneSprite(int id)
        {
            var trans = translations[id];
            var newTrans = new SpriteTranslation(trans.lang, trans.sprite);
            translations.Add(newTrans);
        }
        
        public Sprite GetSprite()
        {
            var lang = Localization.GetLanguage();

            var translation = translations.Find(trans => trans.lang == lang);

            if (translation == null) return fallback;

            return translation.sprite;
        }
    }

    [Serializable]
    public class SpriteTranslation
    {
        protected bool Equals(SpriteTranslation other)
        {
            return lang == other.lang;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpriteTranslation) obj);
        }

        public override int GetHashCode()
        {
            return (int) lang;
        }

        public Languages lang;
        public Sprite sprite;


        public SpriteTranslation(Languages lang, Sprite sprite)
        {
            this.lang = lang;
            this.sprite = sprite;
        }

        public SpriteTranslation()
        {
            lang = Languages.English;
            sprite = null;
        }
        
    }
}