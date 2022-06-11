using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEngine;
using UnityEditor;
using Word = ChaosLocale.Scripts.Core.Data.Word;

namespace Localization
{
    public class LanguageDatabaseEditor : EditorWindow
    {
        private LanguageDatabase languageDatabase;
        private List<Word> dataList;
        private const string DATABASE_PATH = @"Assets/LanguageDatabase.asset";
        private int count = 0;
        private int _maxScrollHeight;
        private long lastTranslate;
        private string searchText;
        private Dictionary<string, bool> groupIsOpen; 
        
        [MenuItem("Window/ChaosLocalization")]
        public static void Init()
        {
            LanguageDatabaseEditor window = EditorWindow.GetWindow<LanguageDatabaseEditor>();
            window.minSize = new Vector2(440, 400);
            window.titleContent = new GUIContent("Localization Editor");
            window.Show();
        }

        void OnEnable()
        {
            if (languageDatabase == null)
            {
                LoadDatabase();
            }
            dataList = new List<Word>(languageDatabase.GetDB());
            count = dataList.Count;
        }
        
        void LoadDatabase()
        {
            languageDatabase =
                (LanguageDatabase) AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(LanguageDatabase));

            if (languageDatabase == null)
                CreateDatabase();
        }

        private void OverrideDatabase(LanguageDatabase newDb)
        {
            languageDatabase = newDb;
            AssetDatabase.DeleteAsset(DATABASE_PATH);
            AssetDatabase.CreateAsset(languageDatabase, DATABASE_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        void CreateDatabase()
        {
            languageDatabase = ScriptableObject.CreateInstance<LanguageDatabase>();
            AssetDatabase.CreateAsset(languageDatabase, DATABASE_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            DisplayMainArea();
            EditorGUILayout.EndVertical();
        }

        private Vector2 _scrollPosition;
        bool isTranslating;
        bool isSave;

        void AddWord()
        {
            var wd = new Word();
            wd.word = "new word";
            wd.wordTranslation = new List<WordTranslation>();
            var trans = new WordTranslation();
            trans.country = Languages.English;
            trans.meaning = "new word";
            wd.wordTranslation.Add(trans);
            languageDatabase.Add(wd);
            dataList = languageDatabase.GetDB();
            count = dataList.Count;
        }

        private void Reload()
        {
            dataList = new List<Word>(languageDatabase.GetDB());
            count = dataList.Count;
            Repaint();
        }

        private void SearchForWord(string word)
        {
            var index = dataList.FindIndex((wd) => wd.word.Contains(word));
            if (index == -1) return;
            var predict = -792 + 29 * index;
            if (predict < 0) predict = 0;
            _scrollPosition = new Vector2(0, predict);
            Reload();
        }

        void DisplayMainArea()
        {

            
            //Set Source Text
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            languageDatabase.sourceLanguage = (Languages) EditorGUI.EnumPopup(
                GUILayoutUtility.GetRect(0, 12.0f, GUILayout.Width(300)), "Source Lanaguage",
                languageDatabase.sourceLanguage);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Refresh"))
            {
                Reload();
            }

            if (GUILayout.Button("Import from json"))
            {
                var path = EditorUtility.OpenFilePanel("Select import file", "", "json");
                Import(path);
                Reload();
            }

            if (GUILayout.Button("Export to json"))
            {
                var path = EditorUtility.SaveFilePanel("Select path to export to", "", "LanguageDatabase", "json");
                Export(path);
                Reload();
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label("Search:", GUILayout.Width(64));
            searchText = EditorGUILayout.TextField(searchText, GUILayout.Width(260));
            if (GUILayout.Button("Find", GUILayout.Width(64)))
            {
                SearchForWord(searchText);
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            // Get Each Word 
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, "box", GUILayout.ExpandHeight(true));
            lastTranslate = 0;
            
            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                var greenStyle = new GUIStyle(EditorStyles.label) {normal = {textColor = Color.green}};
                var yellowStyle = new GUIStyle(EditorStyles.label) {normal = {textColor = Color.yellow}};
                if (!string.IsNullOrEmpty(searchText) && dataList[i].word.Contains(searchText))
                    GUILayout.Label(i + " Word", greenStyle, GUILayout.Width(64));
                else if (!string.IsNullOrEmpty(searchText) &&
                         dataList[i].wordTranslation.Find((trans) => trans.meaning.Contains(searchText)) != null)
                    GUILayout.Label(i + " Word", yellowStyle, GUILayout.Width(64));
                else GUILayout.Label(i + " Word", GUILayout.Width(64));
                dataList[i].word = EditorGUILayout.TextField(new GUIContent(""), dataList[i].word, GUILayout.Width(200))
                    .ToLower();
                if (GUILayout.Button("Edit", GUILayout.Width(64)))
                {
                    EditWordMeaning(i);
                }

                if (GUILayout.Button("Remove", GUILayout.Width(64)))
                {
                    if (count == 1)
                        return;
                    count--;
                    dataList.RemoveAt(i);
                    languageDatabase.Remove(i);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();

            // After type word Press Add Button
            if (GUILayout.Button("Add Word", GUILayout.Width(100)))
            {
                AddWord();
            }

            // Save All
            if (GUILayout.Button("Save All", GUILayout.Width(100)))
            {
                isSave = true;
                EditorUtility.SetDirty(languageDatabase);
                ShowNotification(new GUIContent("SAVE !"));
            }
            
            if (GUILayout.Button("Clear all", GUILayout.Width(100)))
            {
                if(EditorUtility.DisplayDialog("Clear all data?", "Do you really want to clear all translation data?", "no, fuck no!", "yes :)")) return;
                languageDatabase.ClearAll();
                Reload();
            }

            // Press Translate and Editor will play
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();


        }

        private void EditWordMeaning(int index)
        {
            EditWordMeaningEditor window = EditorWindow.GetWindow<EditWordMeaningEditor>();
            window.minSize = new Vector2(200, 200);
            window.Show();
            window.SetValue(index);
        }

        public void Export(string path)
        {
            ProtectFromStupidPeople();
            languageDatabase.UpdateLangNames();
            var prettyPrint = EditorUtility.DisplayDialog("Export", "Are you evil?", "yes", "no");
            var jsonDb = (JsonLanguageDatabase) languageDatabase;
            var json = JsonUtility.ToJson(jsonDb, !prettyPrint);
            var fl = File.Create(path);
            var writer = new StreamWriter(fl);
            writer.Write(json);
            writer.Close();
            fl.Close();
            Debug.Log("Exported db to json");
        }

        private void ProtectFromStupidPeople()
        {
            foreach (var wd in dataList)
            {
                var baseTrans = wd.word;
                if (wd.wordTranslation.Find((word) => word.country == languageDatabase.sourceLanguage) !=
                    null) continue;
                var trans = new WordTranslation {country = languageDatabase.sourceLanguage, meaning = wd.word};
                wd.wordTranslation.Add(trans);
            }
        }

        public void Import(string path)
        {
            var fl = File.Open(path, FileMode.Open);
            var reader = new StreamReader(fl);
            var json = reader.ReadToEnd();
            var jsonDb = JsonUtility.FromJson<JsonLanguageDatabase>(json);
            languageDatabase = (LanguageDatabase) jsonDb;
            OverrideDatabase(languageDatabase);
            EditorUtility.SetDirty(languageDatabase);
            reader.Close();
            fl.Close();
            Reload();
            Debug.Log("Imported db to from json");
        }

    }
}
