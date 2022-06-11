using System;
using System.Collections;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using LanguageCodes = Locale.Scripts.LanguageCodes;
using WordLegacy = ChaosLocale.Scripts.Core.Data.WordLegacy;

namespace Localization
{
    public class EditWordMeaningEditor : EditorWindow
    {
        private LanguageDatabaseLegacy languageDatabaseLegacy;
        private List<WordLegacy> dataList;
        private bool _isTranslating = false; 
        private const string DATABASE_PATH = @"Assets/LanguageDatabase.asset";
        private int count = 0;
        private Vector2 expressionView;

        void OnEnable()
        {
            if (languageDatabaseLegacy == null)
            {
                LoadDatabase();
            }

            dataList = new List<WordLegacy>(languageDatabaseLegacy.GetDB());
            count = dataList.Count;
        }

        void LoadDatabase()
        {
            languageDatabaseLegacy =
                (LanguageDatabaseLegacy) AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(LanguageDatabaseLegacy));

            if (languageDatabaseLegacy == null)
                CreateDatabase();
        }

        void CreateDatabase()
        {
            languageDatabaseLegacy = ScriptableObject.CreateInstance<LanguageDatabaseLegacy>();
            AssetDatabase.CreateAsset(languageDatabaseLegacy, DATABASE_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        int index = 0;

        public void SetValue(int _index)
        {
            index = _index;
        }

        void OnGUI()
        {
            titleContent = new GUIContent("Word Editor");
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            DisplayMainArea();
            EditorGUILayout.EndVertical();
        }

        private Vector2 _scrollPosition;

        private void AddTranslation()
        {
            var trans = new WordTranslation();
            trans.country = Languages.English;
            trans.meaning = "";
            dataList[index].wordTranslation.Add(trans);
            DisplayMainArea();
        }

        private void TranslateVoid(WordLegacy wordLegacy, WordTranslation translation)
        {
            var sourceText = wordLegacy.word;
            var baseTranslation =
                wordLegacy.wordTranslation.Find((trans) => trans.country == languageDatabaseLegacy.sourceLanguage);
            if (baseTranslation != null) sourceText = baseTranslation.meaning;
            
            EditorCoroutineUtility.StartCoroutine(Translate(sourceText, languageDatabaseLegacy.sourceLanguage, translation.country,
                (trans) =>
                {
                    _isTranslating = false;
                    translation.meaning = trans;
                }, () => { _isTranslating = false;}), this);
        }
        
        void DisplayMainArea()
        {
            // Set Source Text
            if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(true)))
            {
                dataList = new List<WordLegacy>(languageDatabaseLegacy.GetDB());
                count = dataList.Count;
                DisplayMainArea();
            }

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label("Word:", GUILayout.Width(40));
            GUILayout.Label(dataList[index].word.ToLower());
            dataList[index].hasRegularExpression = GUILayout.Toggle(dataList[index].hasRegularExpression, "Enable Regular Expressions", GUILayout.Width(190));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            //-----------------------------------------------

            // Get Meaning of Source Text
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            // Meanings
            EditorGUILayout.LabelField("Translations:");
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, "box", GUILayout.ExpandHeight(true));
            var word = dataList[index];
            EditorGUI.BeginDisabledGroup(_isTranslating);
            for (int j = 0; j < dataList[index].wordTranslation.Count; j++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                EditorGUILayout.BeginVertical(GUILayout.Width(100));
                dataList[index].wordTranslation[j].country =
                    (Languages) EditorGUILayout.EnumPopup(dataList[index].wordTranslation[j].country,
                        GUILayout.Width(100));
                
                var translation = dataList[index].wordTranslation[j];
                if (GUILayout.Button("Translate", GUILayout.Width(100)))
                {
                    _isTranslating = true;
                    TranslateVoid(word, translation);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    dataList[index].wordTranslation.RemoveAt(j);
                }
                
                EditorGUILayout.EndVertical();
                
                dataList[index].wordTranslation[j].meaning = EditorGUILayout.TextArea(
                        dataList[index].wordTranslation[j].meaning, GUILayout.ExpandWidth(true), GUILayout.Height(60));
    
                /*
                EditorGUILayout.BeginVertical(GUILayout.Width(100));
                if (GUILayout.Button("Translate", GUILayout.Width(100)))
                {
                    _isTranslating = true;
                    EditorCoroutineUtility.StartCoroutine(Translate(word.word, languageDatabase.sourceLanguage, translation.country,
                        (trans) =>
                        {
                            _isTranslating = false;
                            translation.meaning = trans;
                        }, () => { _isTranslating = false;}), this);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    dataList[index].wordTranslation.RemoveAt(j);
                }
                EditorGUILayout.EndVertical();
                */
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
            
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("add"))
            {
                AddTranslation();
            }
            

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            //-----------------------------------------------
            
            if(dataList[index].hasRegularExpression) DrawRegularExpressions();

            
            EditorGUILayout.EndHorizontal();

            
            if (GUILayout.Button("Done"))
            {
                EditorUtility.SetDirty(languageDatabaseLegacy);
                this.Close();
            }
        }

        private void DrawRegularExpressions()
        {
            var word = dataList[index];
            EditorGUILayout.BeginVertical(GUILayout.Width(224));
 
            EditorGUILayout.LabelField("Regular Expressions:", GUILayout.Width(180));
            
            expressionView = EditorGUILayout.BeginScrollView(expressionView, "box", GUILayout.Width(224), GUILayout.ExpandHeight(true));
            for (var i = 0; i < word.regularExpressions.Count; i++)
            {
                var exp = word.regularExpressions[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("{", GUILayout.Width(6));
                exp.key = GUILayout.TextField(exp.key,GUILayout.Width(140) );
                EditorGUILayout.LabelField("}", GUILayout.Width(6));

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveRegularExpression(exp);
                }    
                
                EditorGUILayout.EndHorizontal();
            }

            if (word.regularExpressions.Count < 30)
            {
                if (GUILayout.Button("add", GUILayout.Width(200)))
                {
                    AddRegularExpression();
                }
            }

            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }

        private void RemoveRegularExpression(RegularExpression exp)
        {
            dataList[index].regularExpressions.Remove(exp);
        }

        private void AddRegularExpression()
        {
            var word = dataList[index];
            word.regularExpressions.Add(new RegularExpression());
        }


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
    }
}
