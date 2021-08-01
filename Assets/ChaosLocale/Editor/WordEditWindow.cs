using System;
using System.Collections;
using Locale.Scripts;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ChaosLocale.Editor
{
    public class WordEditWindow : EditorWindow
    {
        private Word _word;
        private LocaleDatabase db;
        private Vector2 scrollPos = Vector2.zero;
        public static void ShowWindow(Word word, LocaleDatabase db)
        {
            var window = GetWindow<WordEditWindow>();
            window._word = word;
            window.db = db;
            window.titleContent = new GUIContent(word.key);
            window.Show();
        }

        private void DrawHead()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key:", GUILayout.Width(90));
            _word.key = EditorGUILayout.TextField(_word.key, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Meaning:", GUILayout.Width(90));
            _word.baseTranslate = EditorGUILayout.TextArea(_word.baseTranslate, GUILayout.Width(200), GUILayout.Height(60));
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
            
            
            for (var i = 0; i < _word.translations.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var translation = _word.translations[i];
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
                    EditorCoroutineUtility.StartCoroutine(Translate(_word.baseTranslate, db.baseLanguage, translation.language,
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
            _word.translations.RemoveAt(id);
        }
        
        private void CloneMeaning(WordMeaning meaning)
        {
            var newMeaning = new WordMeaning {language = meaning.language, meaning = meaning.meaning};
            _word.translations.Add(newMeaning);
        }
        
        private void AddMeaning()
        {
            _word.translations.Add(new WordMeaning());
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