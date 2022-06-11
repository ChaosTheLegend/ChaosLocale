using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChaosLocale.Editor.Updated
{
    public class TextLocalizationWindow : OdinEditorWindow
    {
        [MenuItem("Window/Localization/v3.0", priority = 1)]
        public static void OpenWindow()
        {
            GetWindow<TextLocalizationWindow>().Show();
        }

        [SerializeField] private LanguageDatabase database;

        [TableList]
        [ShowInInspector]
        private List<Word> Words
        {
            get => database.Words;
            set => database.Words = value;
        }
        
    }
}
