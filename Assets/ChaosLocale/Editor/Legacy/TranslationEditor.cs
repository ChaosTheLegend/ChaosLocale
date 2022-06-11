using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using TMPro;
using UnityEditor;
using UnityEngine;
using Word = ChaosLocale.Scripts.Core.Data.Word;

namespace Localization
{
    [CustomEditor(typeof(Translation))]
    public class TranslationEditor : Editor
    {
        private static readonly string DATABASE_PATH = @"Assets/LanguageDatabase.asset";
        private Vector2 scrollPosition;
        private string lastKey;
        private Languages testLang;
        private Dictionary<string, string> regularExpressionCache = new Dictionary<string, string>();
        public override void OnInspectorGUI()
        {
            var translation = (Translation) target;
            var labels = translation.translationFields;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Test language:", GUILayout.Width(100));
            testLang = (Languages) EditorGUILayout.EnumPopup(testLang);
            EditorGUILayout.EndHorizontal();
            DrawLabels(labels);
        }

        private void DrawLabels(List<Translation.LabelTranslation> labels)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, "box", GUILayout.MaxHeight(240));
            
            var style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            style.margin = new RectOffset(10, 10, 0, 0);
            style.normal.textColor = Color.white;
            for (var i = 0; i < labels.Count; i++)
            {
                var label = labels[i];
                EditorGUILayout.BeginHorizontal();
                var labelName = label.key;
                var labelObjectName = "Null";
                if (label.label != null) labelObjectName = label.label.gameObject.name;
                if (string.IsNullOrEmpty(label.key)) labelName = "new label";
                label.foldout = EditorGUILayout.Foldout(label.foldout, $"{labelObjectName}: {labelName}", true, style);
                if(!label.foldout) if (GUILayout.Button("Delete",GUILayout.Width(60))) DeleteLabel(label);
                EditorGUILayout.EndHorizontal();
                if(label.foldout)DrawLabel(label);
            }
            EditorGUILayout.EndScrollView();
            if(GUILayout.Button("Add")) AddLabel();
        }

        private void AddLabel()
        {
            var translation = (Translation) target;
            var trans = new Translation.LabelTranslation();
            translation.translationFields.Add(trans);
            Repaint();
        }

        private void DeleteLabel(Translation.LabelTranslation label)
        {
            var translation = (Translation) target;
            translation.translationFields.Remove(label);
            Repaint();
        }
        private void DrawLabel(Translation.LabelTranslation label)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Label:", GUILayout.Width(60));
            label.label = (TextMeshProUGUI) EditorGUILayout.ObjectField(label.label, typeof(TextMeshProUGUI), true);
            if (GUILayout.Button("Delete",GUILayout.Width(60))) DeleteLabel(label);
            EditorGUILayout.EndHorizontal();


            var hasKey = !string.IsNullOrEmpty(label.key) && Database.HasKey(label.key);
            EditorGUILayout.BeginHorizontal();
            var style = new GUIStyle();
            var tooltip = "";
            style.normal.textColor = Color.green;
            if (!hasKey) 
            {
                if (string.IsNullOrEmpty(label.key))
                {
                    style.normal.textColor = Color.yellow;
                    tooltip = $"Key is blank";
                }
                else
                {
                    style.normal.textColor = Color.red;
                    tooltip = $"There is no key {label.key} in database";
                }
            }
            EditorGUILayout.LabelField(new GUIContent("key:", tooltip), style, GUILayout.Width(60));
            label.key = EditorGUILayout.TextField(label.key);
            EditorGUILayout.EndHorizontal();
            DrawText(label);
            
            if (label.key != lastKey) OnKeyChanged(label, label.key);
            lastKey = label.key;
                        
            if (hasKey)
            {
                var word = Database.GetWord(label.key);
                if (word != null)
                {
                    if (word.hasRegularExpression) DrawRegularLabel(label, word);
                    else DrawNonRegularLabel(label, word);
                }
            }
            
            
            EditorGUILayout.EndVertical();
        }

        private void DrawText(Translation.LabelTranslation label)
        {
            var style = new GUIStyle();
            var tooltip = "";
            style.normal.textColor = Color.green;
            if (string.IsNullOrEmpty(label.text))
            {
                style.normal.textColor = Color.red;
                tooltip = "Text is blank, label wont display anything";
            }
            else if (!label.text.Contains("{t}"))
            {
                style.normal.textColor = new Color(1f, 0.56f, 0f);
                tooltip = "There is no {t} tag in text, translated text will not be inserted";
            }
            else
            {
                var count = Regex.Matches(label.text, "{t}").Count;
                if (count > 1)
                {
                    style.normal.textColor = new Color(0.72f, 1f, 0f);
                    tooltip = "There are multiple {t} tags in text, translated text will be inserted multiple times, this might be intentional";
                }
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Text:", tooltip), style, GUILayout.Width(60));
            label.text = EditorGUILayout.TextField(label.text);
            EditorGUILayout.EndHorizontal();
        }
        private void DrawNonRegularLabel(Translation.LabelTranslation label, Word word){
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preview:", GUILayout.Width(60));
            var meaning = Database.GetMeaning(word.word, testLang);
            var trueText = label.text.Replace("{t}", meaning);
            EditorGUILayout.LabelField($"{trueText}");
            EditorGUILayout.EndHorizontal();
        }

        private void OnKeyChanged(Translation.LabelTranslation label, string newKey)
        {
            if (string.IsNullOrEmpty(newKey)) return;
            if(!Database.HasKey(newKey)) return;
            var word = Database.GetWord(label.key);
            if(word.hasRegularExpression) UpdateLabelExpressions(label, word);
        }
        private void UpdateLabelExpressions(Translation.LabelTranslation label, Word word)
        {
            var newExpressions = word.regularExpressions;
            var oldExpressions = label.RegularExpressions;

            
            //Remove old
            for (int i = 0; i < oldExpressions.Count; i++)
            {
                var exp = oldExpressions[i];
                var find = newExpressions.Find((e) => e.key == exp.expression);
                if (find == null)
                {
                    regularExpressionCache[exp.expression] = exp.key;
                    oldExpressions.Remove(exp);
                    i--;
                }
            }
            
            //add new
            foreach (var exp in newExpressions)
            {
                var find = oldExpressions.Find((e) => e.expression == exp.key);
                if (find == null)
                {
                    var trans = new Translation.RegularTranslation(exp.key, "");
                    if (regularExpressionCache.ContainsKey(exp.key))
                    {
                        trans.key = regularExpressionCache[exp.key];
                    }
                    label.RegularExpressions.Add(trans);
                }
            }
            Repaint();
        }
        private void DrawRegularLabel(Translation.LabelTranslation label, Word word){
            for (int i = 0; i < label.RegularExpressions.Count; i++)
            {
                var exp = label.RegularExpressions[i];
                EditorGUILayout.BeginHorizontal();
                if (exp.recursive)
                {
                    var hasKey = !string.IsNullOrEmpty(exp.key) && Database.HasKey(exp.key);
                    var nonRecursive = hasKey && Database.GetWord(exp.key).hasRegularExpression;
                    var style = new GUIStyle();
                    var tooltip = "";
                    style.normal.textColor = Color.green;
                    if (!hasKey)
                    {
                        if (string.IsNullOrEmpty(exp.key))
                        {
                            style.normal.textColor = Color.yellow;
                            tooltip = $"Key is blank";
                        }
                        else
                        {
                            style.normal.textColor = Color.red;
                            tooltip = $"There is no key {exp.key} in database";
                        }
                    }
                    else if(nonRecursive)
                    {
                        style.normal.textColor = Color.yellow;
                        tooltip = $"Key {exp.key} already has regular expressions and cannot be recursive";
                    }
                    EditorGUILayout.LabelField(new GUIContent($"{exp.expression}:", tooltip), style, GUILayout.Width(60));
                }
                else
                {
                    EditorGUILayout.LabelField($"{exp.expression}:", GUILayout.Width(60));
                }

                exp.key = EditorGUILayout.TextField(exp.key);

               

                EditorGUILayout.LabelField($"Recursive?:", GUILayout.Width(60));
                exp.recursive = EditorGUILayout.Toggle(exp.recursive, GUILayout.Width(14));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preview:", GUILayout.Width(60));
            var meaning = Database.GetRegularTranslation(word.word, testLang, label.RegularExpressions.ToArray());
            var trueText = label.text.Replace("{t}", meaning);
            EditorGUILayout.LabelField($"{trueText}");
            EditorGUILayout.EndHorizontal();

        }

        private LanguageDatabase database;
        private LanguageDatabase Database => GetDB();
        private LanguageDatabase GetDB()
        {
            if (database == null)
            {
                database = (LanguageDatabase) AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(LanguageDatabase));
            }
            return database;
        }
    }
}