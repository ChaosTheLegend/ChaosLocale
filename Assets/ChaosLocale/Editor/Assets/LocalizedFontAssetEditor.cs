using ChaosLocale.Scripts.AssetLocalization;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace ChaosLocale.Editor.Assets
{
    ///
    /// This code needs refactoring
    ///
    
    [CustomEditor(typeof(LocalizedFontAsset))]
    public class LocalizedFontAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var font = target as LocalizedFontAsset;

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", GUILayout.Width(90));
            font.name = EditorGUILayout.TextField(font.name);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fallback Font:", GUILayout.Width(90));
            font.fallback = (TMP_FontAsset) EditorGUILayout.ObjectField(font.fallback, typeof(TMP_FontAsset), false );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Translations:", GUILayout.Width(90));
            var sprites = font.translations;
            
            for (var i = 0; i < sprites.Count; i++)
            {
                var trans = sprites[i];
                EditorGUILayout.BeginHorizontal();
                trans.lang = (Languages) EditorGUILayout.EnumPopup(trans.lang, GUILayout.Width(100));
                trans.font = (TMP_FontAsset) EditorGUILayout.ObjectField(trans.font, typeof(TMP_FontAsset), false);
                
                if (GUILayout.Button("Clone", GUILayout.Width(50)))
                {
                    font.CloneSprite(i);
                }
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    font.DeleteSprite(i);
                }
                
                
                
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                font.NewSprite();
            }
            
            
            EditorGUILayout.EndVertical();
        }
    }
}