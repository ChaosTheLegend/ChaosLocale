using ChaosLocale.Scripts.AssetLocalization;
using Locale.Scripts;
using UnityEditor;
using UnityEngine;

namespace ChaosLocale.Editor.Assets
{
    ///
    /// This code needs refactoring
    ///

    [CustomEditor(typeof(LocalizedPrefab))]
    public class LocalizedPrefabEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var prefab = target as LocalizedPrefab;

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", GUILayout.Width(90));
            prefab.name = EditorGUILayout.TextField(prefab.name);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fallback Prefab:", GUILayout.Width(90));
            prefab.fallback = (GameObject) EditorGUILayout.ObjectField(prefab.fallback, typeof(GameObject), false );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Translations:", GUILayout.Width(90));
            var sprites = prefab.translations;
            
            for (var i = 0; i < sprites.Count; i++)
            {
                var trans = sprites[i];
                EditorGUILayout.BeginHorizontal();
                trans.lang = (Languages) EditorGUILayout.EnumPopup(trans.lang, GUILayout.Width(100));
                trans.gameObject = (GameObject) EditorGUILayout.ObjectField(trans.gameObject, typeof(GameObject), false);
                
                if (GUILayout.Button("Clone", GUILayout.Width(50)))
                {
                    prefab.CloneSprite(i);
                }
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    prefab.DeleteSprite(i);
                }
                
                
                
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                prefab.NewSprite();
            }
            
            
            EditorGUILayout.EndVertical();
        }
    }
}