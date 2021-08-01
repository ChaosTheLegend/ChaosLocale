#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [CustomEditor(typeof(LanguageExporter))]
    public class LanguageExporterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Export to json"))
            {
                var path = EditorUtility.SaveFilePanel("Select path to export to", "", "LanguageDatabase", "json");
                var exporter = target as LanguageExporter;
                exporter.Export(path);
            }

            if (GUILayout.Button("Import from json"))
            {
                var path = EditorUtility.OpenFilePanel("Select import file", "", "json");
                var exporter = target as LanguageExporter;
                exporter.Import(path);
            }
        }
    }

    [ExecuteInEditMode]
    public class LanguageExporter : MonoBehaviour
    {
        [SerializeField] private LanguageDatabase db;

        public void Export(string path)
        {
            var json = JsonUtility.ToJson(db, true);
            var fl = File.Create(path);
            var writer = new StreamWriter(fl);
            writer.Write(json);
            writer.Close();
            fl.Close();
            print("Exported db to json");
        }

        public void Import(string path)
        {
            var fl = File.Open(path, FileMode.Open);
            var reader = new StreamReader(fl);
            var json = reader.ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, db);
            reader.Close();
            fl.Close();
            print("Imported db to from json");
        }
    }
}
#endif
