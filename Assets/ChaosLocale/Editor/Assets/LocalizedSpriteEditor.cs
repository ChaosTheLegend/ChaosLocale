using ChaosLocale.Scripts.AssetLocalization;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEditor;
using UnityEngine;

namespace ChaosLocale.Editor.Assets
{
    ///
    /// This code needs refactoring
    ///

    
    [CustomEditor(typeof(LocalizedSprite))]
    public class LocalizedSpriteEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var spr = target as LocalizedSprite;

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", GUILayout.Width(90));
            spr.name = EditorGUILayout.TextField(spr.name);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fallback Sprite:", GUILayout.Width(90));
            spr.fallback = (Sprite) EditorGUILayout.ObjectField(spr.fallback, typeof(Sprite), false );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Translations:", GUILayout.Width(90));
            var sprites = spr.translations;
            
            for (var i = 0; i < sprites.Count; i++)
            {
                var trans = sprites[i];
                EditorGUILayout.BeginHorizontal();
                trans.lang = (Languages) EditorGUILayout.EnumPopup(trans.lang, GUILayout.Width(100));
                trans.sprite = (Sprite) EditorGUILayout.ObjectField(trans.sprite, typeof(Sprite), false);
                
                if (GUILayout.Button("Clone", GUILayout.Width(50)))
                {
                    spr.CloneSprite(i);
                }
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    spr.DeleteSprite(i);
                }
                
                
                
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                spr.NewSprite();
            }
            
            
            EditorGUILayout.EndVertical();
        }
    }
}