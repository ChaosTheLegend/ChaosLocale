using System;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using TMPro;
using UnityEngine;

namespace ChaosLocale.Scripts.AssetLocalization
{
    [CreateAssetMenu(fileName = "new localized font asset", menuName = "Localization/Localized Font Asset", order = 5)]
    public class LocalizedFontAsset : ScriptableObject
    {
        public TMP_FontAsset fallback;
        public List<FontAssetTranslation> translations = new List<FontAssetTranslation>();

        public void CloneSprite(int i)
        {
            var trans = translations[i];
            translations.Add(new FontAssetTranslation(trans.lang, trans.font));
        }

        public void DeleteSprite(int i)
        {
            translations.RemoveAt(i);
        }

        public void NewSprite()
        {
            translations.Add(new FontAssetTranslation());
        }
        
        public TMP_FontAsset GetFont()
        {
            var lang = Localization.GetLanguage();

            var translation = translations.Find(trans => trans.lang == lang);

            if (translation == null) return fallback;

            return translation.font;
        }
    }

    [Serializable]
    public class FontAssetTranslation
    {
        protected bool Equals(FontAssetTranslation other)
        {
            return lang == other.lang;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FontAssetTranslation) obj);
        }

        public override int GetHashCode()
        {
            return (int) lang;
        }

        public Languages lang;
        public TMP_FontAsset font;

        public FontAssetTranslation(Languages lang, TMP_FontAsset font)
        {
            this.lang = lang;
            this.font = font;
        }

        public FontAssetTranslation()
        {
            lang = Languages.English;
            font = null;
        }
        
        
    }
}