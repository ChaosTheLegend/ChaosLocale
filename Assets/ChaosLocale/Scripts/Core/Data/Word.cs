using System;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;

namespace Locale.Scripts
{
    [Serializable]
    public class Word
    {
        public string key;
        public string baseTranslate;
        public List<WordMeaning> translations;


        public Word()
        {
            if(translations == null) translations = new List<WordMeaning>();            
        }
    }

    [Serializable]
    public class WordMeaning
    {
        public string meaning;
        public Languages language;
        [NonSerialized] public bool isLoading = false;
    }
}