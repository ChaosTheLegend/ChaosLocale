using System;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChaosLocale.Scripts.AssetLocalization
{
    [CreateAssetMenu(fileName = "new localized prefab", menuName = "Localization/Localized Prefab", order = 5)]
    public class LocalizedPrefab : ScriptableObject
    {
        public GameObject fallback;
        public List<PrefabTranslation> translations = new List<PrefabTranslation>();

        public void CloneSprite(int i)
        {
            var trans = translations[i];
            translations.Add(new PrefabTranslation(trans.lang, trans.gameObject));
        }

        public void DeleteSprite(int i)
        {
            translations.RemoveAt(i);
        }

        public void NewSprite()
        {
            translations.Add(new PrefabTranslation());
        }

        public GameObject GetPrefab()
        {
            var lang = Localization.GetLanguage();

            var prefab = translations.Find(trans => trans.lang == lang);

            if (prefab == null) return fallback;

            return prefab.gameObject;
        }
    }

    [Serializable]
    public class PrefabTranslation
    {
        protected bool Equals(PrefabTranslation other)
        {
            return lang == other.lang;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PrefabTranslation) obj);
        }

        public override int GetHashCode()
        {
            return (int) lang;
        }

        public Languages lang;
        public GameObject gameObject;

        public PrefabTranslation()
        {
            lang = Languages.English;
            gameObject = null;
        }

        public PrefabTranslation(Languages lang, GameObject gameObject)
        {
            this.lang = lang;
            this.gameObject = gameObject;
        }
    }
}