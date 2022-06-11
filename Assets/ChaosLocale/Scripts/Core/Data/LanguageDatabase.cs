
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChaosLocale.Scripts.Core.Data
{
    [CreateAssetMenu(menuName = "ChaosLocale/Word Database")]
    public class LanguageDatabase : ScriptableObject
    {
        [field: SerializeField] public List<Language> RegisteredLanguages { get; set; }
        [field: SerializeField] public Language FallbackLanguage { get; set; }
        [field: SerializeField] public List<Word> Words { get; set; }
    }

    [Serializable]
    public class Word
    {
        [field: SerializeField] public string Key { get; set; }
        [field: SerializeField] public List<Translation> Translations { get; set; }
    }
    
    [Serializable]
    public class Translation
    {
        [field: SerializeField] public Language Language { get; set; }
        [field: SerializeField] public string Meaning { get; set; }
    }
}
