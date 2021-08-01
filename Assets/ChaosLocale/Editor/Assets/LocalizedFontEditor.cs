using ChaosLocale.Scripts.AssetLocalization;
using Locale.Scripts;
using UnityEditor;
using UnityEngine;

namespace ChaosLocale.Editor.Assets
{
    ///
    /// This code needs refactoring
    ///
    
    [CustomEditor(typeof(LocalizedFont))]
    public class LocalizedFontEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var font = target as LocalizedFont;

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", GUILayout.Width(90));
            font.name = EditorGUILayout.TextField(font.name);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fallback Font:", GUILayout.Width(90));
            font.fallback = (Font) EditorGUILayout.ObjectField(font.fallback, typeof(Font), false );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Translations:", GUILayout.Width(90));
            var sprites = font.translations;
            
            for (var i = 0; i < sprites.Count; i++)
            {
                var trans =  (FontTranslation) sprites[i];
                EditorGUILayout.BeginHorizontal();
                trans.lang = (Languages) EditorGUILayout.EnumPopup(trans.lang, GUILayout.Width(100));
                trans.font = (Font) EditorGUILayout.ObjectField(trans.font, typeof(Font), false);
                
                if (GUILayout.Button("Clone", GUILayout.Width(50)))
                {
                    font.CloneAsset(i);
                }
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    font.DeleteAsset(i);
                }
                
                
                
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                font.NewAsset();
            }
            
            
            EditorGUILayout.EndVertical();
        }
    }
}