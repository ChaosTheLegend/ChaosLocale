using ChaosLocale.Scripts.AssetLocalization;
using Locale.Scripts;
using UnityEditor;
using UnityEngine;

namespace ChaosLocale.Editor.Assets
{
    ///
    /// This code needs refactoring
    ///

    
    [CustomEditor(typeof(LocalizedSound))]
    public class LocalizedSoundEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var sound = target as LocalizedSound;

            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", GUILayout.Width(90));
            sound.name = EditorGUILayout.TextField(sound.name);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fallback Sound:", GUILayout.Width(90));
            sound.fallback = (AudioClip) EditorGUILayout.ObjectField(sound.fallback, typeof(AudioClip), false );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Translations:", GUILayout.Width(90));
            var sounds = sound.translations;
            
            for (var i = 0; i < sounds.Count; i++)
            {
                var trans = sounds[i];
                EditorGUILayout.BeginHorizontal();
                trans.lang = (Languages) EditorGUILayout.EnumPopup(trans.lang, GUILayout.Width(100));
                trans.sound = (AudioClip) EditorGUILayout.ObjectField(trans.sound, typeof(AudioClip), false);
                
                if (GUILayout.Button("Clone", GUILayout.Width(50)))
                {
                    sound.CloneSound(i);
                }
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    sound.DeleteSound(i);
                }
                
                
                
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                sound.NewSound();
            }
            
            
            EditorGUILayout.EndVertical();
        }
    }
}