using System;
using System.Collections.Generic;
using Locale.Scripts;
using UnityEngine;

namespace ChaosLocale.Scripts.AssetLocalization
{
    public abstract class LocalizedAsset : ScriptableObject
    {
        public List<AssetTranslation> translations = new List<AssetTranslation>();
        
        public abstract void CloneAsset(int i);

        public abstract void DeleteAsset(int i);
        
        public abstract void NewAsset();
    }

    [Serializable]
    public abstract class AssetTranslation
    {
        public Languages lang;

        protected bool Equals(AssetTranslation other)
        {
            return lang == other.lang;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetTranslation) obj);
        }

        public override int GetHashCode()
        {
            return (int) lang;
        }

        protected AssetTranslation(Languages lang)
        {
            this.lang = lang;
        }
    }
}