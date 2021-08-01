using System;
using System.Collections;
using System.Collections.Generic;
using ChaosLocale.Editor;
using Locale.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ChaosLocale.Scripts;

public class LocaleEditor : EditorWindow
{
    private LocaleDatabase db;
    private const string DBPATH = @"Assets/ChaosLocale/WordDatabase.asset";
    private int openGroup;
    private bool editGroupName = false;
    private Vector2 scrollPos;
    private string tempTitle;
    private int tab;
    [MenuItem("Window/Localization/Text Localization")]
    public static void ShowWindow()
    {
        GetWindow<LocaleEditor>("Text Localization");
    }
    
    private void OnEnable()
    {
        if (db == null)
        {
            LoadDB();       
        }
    }

    private void LoadDB()
    {
        db = (LocaleDatabase) AssetDatabase.LoadAssetAtPath(DBPATH, typeof(LocaleDatabase));
    }


    private void DrawTabs()
    {
        tab = GUILayout.Toolbar(tab, new string[] {"Edit","Export"});
        
        EditorGUILayout.Space();
        
        switch (tab)
        {
            case 0:
                DrawEditor();
                break;
            case 1:
                DrawExporter();
                break;
        }
    }

    #region Exporter
    private void DrawExporter()
    {
        XMLExporter();
        //CsvExporter();
    }

    /// <summary>
    /// WIP
    /// </summary>
    private void CsvExporter()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Import CSV"))
        {
            
        }
        
        if (GUILayout.Button("Export CSV"))
        {
            
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void XMLExporter()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Import XML"))
        {
            var path = EditorUtility.OpenFilePanel("Import locale", @"Assets", "xml");
            if (!string.IsNullOrEmpty(path))
            {
                var newDb = ChaosLocale.Scripts.Localization.ImportXml(path);
                var baseLang = newDb.baseLanguage;
                if (db.Groups.Count > 1 || db.Groups[0].words.Count > 0)
                {
                    var choice = EditorUtility.DisplayDialogComplex("Import external file",
                        "You are trying to import new localization into existing one," +
                        "what should we do?",
                        "Append new groups to the old ones",
                        "Override current localization",
                        "Append only new content");

                    if (db.baseLanguage != newDb.baseLanguage)
                    {
                        var lang = EditorUtility.DisplayDialog("Base language",
                            "Your base language is different from the one in the localization file, which one do you prefer?",
                            $"{db.baseLanguage}", $"{newDb.baseLanguage}");
                        baseLang = lang ? db.baseLanguage : newDb.baseLanguage;
                    }

                    if (choice == 0)
                    {
                        db = LocaleDatabase.CrossJoin(db, newDb, baseLang);
                        //Debug.Log("cross Join");
                    }

                    if (choice == 1)
                    {
                        db.Load(newDb);
                        //Debug.Log("override");
                    }

                    if (choice == 2)
                    {
                        db = LocaleDatabase.LeftJoin(db, newDb, baseLang);
                        //Debug.Log("append");
                    }
                }
                else
                {
                    db.Load(newDb);
                }
                LoadDB();
                db.baseLanguage = baseLang;
                
                ShowWindow();
                
            }
        };
        
        if (GUILayout.Button("Export XML"))
        {
            var path = EditorUtility.SaveFilePanel("Export xml", @"Assets", "new Localization", "xml");
            ChaosLocale.Scripts.Localization.ExportXML(path);
            ShowNotification(new GUIContent("Exported"));
        };
        
        EditorGUILayout.EndHorizontal();
        
    }
    
    #endregion
    #region Editor
    private void DrawEditor()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Base Language:", GUILayout.Width(100));
        db.baseLanguage = (Languages) EditorGUILayout.EnumPopup(db.baseLanguage);
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Group: ", GUILayout.Width(50));
        var names = db.GetGroupNames();
        if (db.GetGroupNames().Count == 0)
        {
            db.AddGroup();
        }
        //names.Add("new group");
        if (!editGroupName)
        {
            var realList = new List<string>();
            foreach (var nm in names)
            {
                if (!realList.Contains(nm)) realList.Add(nm);
                else
                {
                    var i = 1;
                    var tempName = nm + $" ({i})";
                    while (realList.Contains(tempName))
                    {
                        i++;
                        tempName = nm + $" ({i})";
                    }

                    realList.Add(tempName);
                }
            }
            openGroup = EditorGUILayout.Popup(openGroup, realList.ToArray(), GUILayout.Width(200));
        }
        else
        {
            tempTitle = EditorGUILayout.TextField(tempTitle, GUILayout.Width(200));
            if(string.IsNullOrEmpty(tempTitle)) db.Groups[openGroup].title = "new group";
            else db.Groups[openGroup].title = tempTitle;
        }


        if (GUILayout.Button(!editGroupName? "edit" : "submit", GUILayout.Width(50)))
        {
            editGroupName = !editGroupName;
        }
       
        EditorGUI.BeginDisabledGroup(editGroupName);
        if (GUILayout.Button("add", GUILayout.Width(40)))
        {
            db.AddGroup();
            openGroup = db.Groups.Count - 1;
            editGroupName = !editGroupName;
        }
        EditorGUI.EndDisabledGroup();
        
        EditorGUI.BeginDisabledGroup(db.Groups.Count < 2 || editGroupName);
        if (GUILayout.Button("X", GUILayout.Width(20)))
        {
            if (db.GetGroupWords(openGroup).Count > 0)
            {
                if (EditorUtility.DisplayDialog("Delete group?", $"This group has {db.GetGroupWords(openGroup).Count} words, are you sure you want to delete it?", 
                    "Yes, delete group", "No, keep the group"))
                {
                    if (db.GetGroupWords(openGroup).Count > 50)
                    {
                        if (!EditorUtility.DisplayDialog("Are you sure?",
                            $"!!!WARNING!!! This group has more than 50 words, are you really sure you want to delete it? You won't be able to undo this action!",
                            "No, cancel deletion", "Yes, I am absolutely sure"))
                        {
                            db.Groups.RemoveAt(openGroup);
                            openGroup = db.Groups.Count - 1;
                        }
                    }
                    else
                    {
                        db.Groups.RemoveAt(openGroup);
                        openGroup = db.Groups.Count - 1;
                    }
                }
            }
            else
            {
                db.Groups.RemoveAt(openGroup);
                openGroup = db.Groups.Count - 1;
            }
        }
        EditorGUI.EndDisabledGroup();
        
        
        EditorGUILayout.EndHorizontal();
        
        DrawWordsGUI();
    }

    private void DrawWordsGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key: ", GUILayout.Width(100));
        EditorGUILayout.LabelField("Base Translation: ", GUILayout.Width(200));
        
        EditorGUILayout.EndHorizontal();

        
        
        var words = db.GetGroupWords(openGroup);
        var wordId = 0;
        for (var i = 0; i < words.Count; i++)
        {
            var word = words[i];
            EditorGUILayout.BeginHorizontal();
            word.key = EditorGUILayout.TextField(word.key, GUILayout.Width(100));
            word.baseTranslate = EditorGUILayout.TextField(word.baseTranslate, GUILayout.Width(200));
            if (GUILayout.Button("edit", GUILayout.Width(50)))
            {
                WordEditWindow.ShowWindow(word, db);
            };
            if (GUILayout.Button("copy", GUILayout.Width(50))) CloneWord(wordId);
            if (GUILayout.Button("X", GUILayout.Width(20))) RemoveWord(wordId);
            EditorGUILayout.EndHorizontal();
            wordId++;
        }

        if(GUILayout.Button("+", GUILayout.Width(100))) AddWord();
        EditorGUILayout.EndScrollView();
    }

    private void CloneWord(int id)
    {
        var words = db.GetGroupWords(openGroup);
        var word = words[id];
        var newWord = new Word {key = word.key, baseTranslate = word.baseTranslate, translations = word.translations};

        db.AddWord(openGroup, newWord);
    }
    private void AddWord()
    {
        db.AddWord(openGroup);
        scrollPos = Vector2.positiveInfinity;
    }

    private void RemoveWord(int id)
    {
        db.RemoveWord(openGroup, id);
    }
    #endregion
    private void OnGUI()
    {
        DrawTabs();
    }
}
