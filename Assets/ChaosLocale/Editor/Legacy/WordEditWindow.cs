using System;
using System.Collections;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using LanguageCodes = Locale.Scripts.LanguageCodes;
using WordLegacy = Locale.Scripts.WordLegacy;

namespace ChaosLocale.Editor
{
    public class WordEditWindow : EditorWindow
    {
        private WordLegacy wordLegacy;
        private LocaleDatabase db;
        private Vector2 scrollPos = Vector2.zero;
        public static void ShowWindow(WordLegacy wordLegacy, LocaleDatabase db)
        {
            var window = GetWindow<WordEditWindow>();
            window.wordLegacy = wordLegacy;
            window.db = db;
            window.titleContent = new GUIContent(wordLegacy.key);
            window.Show();
        }

        private void DrawHead()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key:", GUILayout.Width(90));
            wordLegacy.key = EditorGUILayout.TextField(wordLegacy.key, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Meaning:", GUILayout.Width(90));
            wordLegacy.baseTranslate = EditorGUILayout.TextArea(wordLegacy.baseTranslate, GUILayout.Width(200), GUILayout.Height(60));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawTranslations()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Language:", GUILayout.Width(90));
            EditorGUILayout.LabelField("Meaning:", GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            
            
            for (var i = 0; i < wordLegacy.translations.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var translation = wordLegacy.translations[i];
                EditorGUI.BeginDisabledGroup(translation.isLoading);
                translation.language = (Languages) EditorGUILayout.EnumPopup(translation.language ,GUILayout.Width(90));
                translation.meaning = EditorGUILayout.TextArea(translation.meaning, GUILayout.Width(200), GUILayout.Height(60));
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("Clone", GUILayout.Width(70)))
                {
                    CloneMeaning(translation);
                }
                
                if (GUILayout.Button("Translate", GUILayout.Width(70)))
                {
                    translation.isLoading = true;
                    EditorCoroutineUtility.StartCoroutine(Translate(wordLegacy.baseTranslate, db.baseLanguage, translation.language,
                        trans =>
                        {
                            translation.meaning = trans;
                            translation.isLoading = false;
                        },
                        () =>
                        {
                            translation.isLoading = false;
                        }
                        ), this);
                }
                
                if (GUILayout.Button("Delete", GUILayout.Width(70)))
                {
                    DeleteMeaning(i);
                }
                
                EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("+", GUILayout.Width(90)))
            {
                AddMeaning();   
            }
            
            
            EditorGUILayout.EndScrollView();
        }

        private void DeleteMeaning(int id)
        {
            wordLegacy.translations.RemoveAt(id);
        }
        
        private void CloneMeaning(WordMeaning meaning)
        {
            var newMeaning = new WordMeaning {language = meaning.language, meaning = meaning.meaning};
            wordLegacy.translations.Add(newMeaning);
        }
        
        private void AddMeaning()
        {
            wordLegacy.translations.Add(new WordMeaning());
            scrollPos = Vector2.positiveInfinity;
        }
        
        
        private void OnGUI()
        {
            DrawHead();
            EditorGUILayout.Space();
            DrawTranslations();
        }


        #region Translation


        private IEnumerator Translate(string baseTranslation, Languages baseLanguage, Languages targetLanguage,
            Action<string> onSuccess, Action onFail = null)
        {
            var baseLangCode = (LanguageCodes) baseLanguage;
            var targetLangCode = (LanguageCodes) targetLanguage;

            var url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                      + baseLangCode + "&tl=" + targetLangCode + "&dt=t&q=" +
                      UnityWebRequest.EscapeURL(baseTranslation);

            var www = UnityWebRequest.Get(url);

            yield return www.SendWebRequest();

            yield return new WaitUntil(() => www.isDone);
            if (www.isDone)
            {
                //junky way of unpacking translation
                var s1 = www.downloadHandler.text;
                var s2 = s1.Split('[');
                var s3 = s2[3];
                var s4 = s3.Split('"');
                var s5 = s4[1];
                var s6 = s5.Trim();
                
                onSuccess(s6);
            }
            else
            {
                //Debug.LogError(www.downloadHandler.error);
                onFail?.Invoke();
            }

        }

        #endregion
    }
}