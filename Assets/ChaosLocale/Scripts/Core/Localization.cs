using System.CodeDom.Compiler;
using System.IO;
using System.Xml.Serialization;
using ChaosLocale.Scripts.Core.Data;
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


        public static LocaleDatabase GetDB()
        {
            db = (LocaleDatabase) AssetDatabase.LoadAssetAtPath(DBPATH, typeof(LocaleDatabase));
            return db;
        }

        #region Exports
        

        public static void ExportCsv(string exportPath)
        {
            var path = exportPath;
            File.WriteAllText(path, "");
            
        }

        #endregion
    }
}