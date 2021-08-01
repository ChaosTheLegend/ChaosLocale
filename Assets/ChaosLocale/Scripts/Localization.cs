using System.CodeDom.Compiler;
using System.IO;
using System.Xml.Serialization;
using Locale.Scripts;
using UnityEditor;
using UnityEngine;

namespace ChaosLocale.Scripts
{
    public class Localization
    {
        //private static string ExportPath  = @"Assets/ChaosLocale/";
        private static string DBPATH = @"Assets/ChaosLocale/WordDatabase.asset";
        private static LocaleDatabase db;
        private static Languages currentLanguage = Languages.English;
        
        public static void SetLanguage(Languages newLang)
        {
            currentLanguage = newLang;
        }
        
        public static Languages GetLanguage()
        {
            return currentLanguage;
        }

        public static string GetTranslation(string group, string key, Languages lang)
        {
            if (db == null) GetDB();
            var wordGroup = db.Groups.Find(group1 => group1.title == @group);
            if (wordGroup == null) return "Group not found";
            var word = wordGroup.words.Find(word1 => word1.key == @key);
            if (word == null) return "Word not found";
            if (lang == db.baseLanguage) return word.baseTranslate;
            var translation = word.translations.Find(trans1 => trans1.language == @lang);
            if (translation == null) return word.baseTranslate;
            return translation.meaning;
        }

        public static string GetTranslation(string group, string key)
        {
            return GetTranslation(group, key, currentLanguage);
        }


        private static void GetDB()
        {
            db = (LocaleDatabase) AssetDatabase.LoadAssetAtPath(DBPATH, typeof(LocaleDatabase));
        }

        #region Exports
        public static void ExportXML(string exportPath)
        {
            if(string.IsNullOrEmpty(exportPath)) return;
            
            if (db == null) GetDB();
            var path = exportPath;
            File.WriteAllText(path, "");
            var database = new XMLDatabase();
            
            database.groups = db.Groups;
            database.baseLanguage = db.baseLanguage;
            
            var serializer = new XmlSerializer(typeof(XMLDatabase));
            var filestream = new FileStream(path, FileMode.OpenOrCreate);
            serializer.Serialize(filestream, database);
        }

        public static void ExportCsv(string exportPath)
        {
            var path = exportPath;
            File.WriteAllText(path, "");
            
        }

        #endregion

        #region Imports

        public static LocaleDatabase ImportXml(string path)
        {
            var filestream = new FileStream(path, FileMode.Open);
            var serializer = new XmlSerializer(typeof(XMLDatabase));
            var inst = ScriptableObject.CreateInstance<LocaleDatabase>();
            var database = (XMLDatabase) serializer.Deserialize(filestream);
            inst.SetGroups(database.groups);
            inst.baseLanguage = database.baseLanguage;
            filestream.Close();
            return inst;
        }
        
        
        
        
        

        #endregion
    }
}