using System;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEngine;
using WordLegacy = ChaosLocale.Scripts.Core.Data.WordLegacy;

namespace Localization
{
    public class JsonLanguageDatabase
    {
        public List<JsonWord> database;
        public Languages sourceLanguage;

        public static explicit operator JsonLanguageDatabase(LanguageDatabaseLegacy databaseLegacy)
        {
            var jsonDb = new JsonLanguageDatabase();
            var langDb = databaseLegacy.GetDB();
            jsonDb.database = new List<JsonWord>();
            jsonDb.sourceLanguage = databaseLegacy.sourceLanguage;
            foreach (var word in langDb)
            {
                var jsonWord = new JsonWord();
                jsonWord.key = word.word;
                jsonWord.regular_expressions = new List<string>();
                var wordExpressions = word.regularExpressions;
                foreach (var expression in wordExpressions)
                {
                    jsonWord.regular_expressions.Add(expression.key);
                }

                jsonWord.meanings = new List<JsonTranslation>();
                var wordTranslations = word.wordTranslation;
                foreach (var translation in wordTranslations)
                {
                    var jsonTrans = new JsonTranslation();
                    jsonTrans.language = Enum.GetName(typeof(Languages), translation.country);
                    jsonTrans.translation = translation.meaning;
                    jsonWord.meanings.Add(jsonTrans);
                }

                jsonDb.database.Add(jsonWord);
            }
            
            return jsonDb;
        }

        public static explicit operator LanguageDatabaseLegacy(JsonLanguageDatabase jsonDb)
        {
            var langDb = ScriptableObject.CreateInstance<LanguageDatabaseLegacy>();
            langDb.sourceLanguage = jsonDb.sourceLanguage;
            langDb.database = new List<WordLegacy>();
            var jsonWords = jsonDb.database;
            foreach (var jsonWord in jsonWords)
            {
                langDb.database.Add((WordLegacy) jsonWord);
            }

            return langDb;
        }
        
    }

    [Serializable]
    public class JsonWord
    {
        public string key;
        public List<JsonTranslation> meanings;
        public List<string> regular_expressions;
        
        public static explicit operator WordLegacy(JsonWord jsonWord)
        {
            var wd = new WordLegacy();
            wd.word = jsonWord.key;
            wd.regularExpressions = new List<RegularExpression>();
            foreach (var exp in jsonWord.regular_expressions)
            {
                wd.regularExpressions.Add((RegularExpression) exp);
            }
            wd.hasRegularExpression = wd.regularExpressions.Count > 0;
            wd.wordTranslation = new List<WordTranslation>();
            foreach (var trans in jsonWord.meanings)
            {
                wd.wordTranslation.Add((WordTranslation) trans);
            }

            return wd;
        }    
    }

    [Serializable]
    public class JsonTranslation
    {
        public string language;
        public string translation;
        
        public static explicit operator WordTranslation(JsonTranslation jsonTranslation)
        {
            var trans = new WordTranslation();
            trans.country = (Languages) Enum.Parse(typeof(Languages) ,jsonTranslation.language);
            trans.meaning = jsonTranslation.translation;
            return trans; 
        }   
    }
    
    
}