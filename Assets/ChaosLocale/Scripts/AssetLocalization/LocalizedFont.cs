using System;
using System.Collections.Generic;
using Locale.Scripts;
using UnityEngine;

namespace ChaosLocale.Scripts.AssetLocalization
{
    [CreateAssetMenu(fileName = "new localized font", menuName = "Localization/Localized Font", order = 5)]
    public class LocalizedFont : LocalizedAsset
    {
        public Font fallback;

        public override void CloneAsset(int i)
        {
            var trans = translations[i] as FontTranslation;
            translations.Add(new FontTranslation(trans.lang, trans.font));
        }

        public override void DeleteAsset(int i)
        {
            translations.RemoveAt(i);
        }

        public override void NewAsset()
        {
            translations.Add(new FontTranslation());
        }

        public Font GetFont()
        {
            var lang = Localization.GetLanguage();

            var translation = translations.Find(trans => trans.lang == lang);

            if (translation == null) return fallback;

            var font = translation;
            
            return (translation as FontTranslation)?.font;
        }
    }

    [Serializable]
    public class FontTranslation : AssetTranslation
    {
        public Font font;
        public FontTranslation(Languages lang, Font font) : base(lang)
        {
            this.font = font;
        }

        public FontTranslation() : base(Languages.English)
        {
            this.font = null;
        }
    }
}