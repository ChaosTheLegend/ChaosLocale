using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    public class JsonLanguageDatabase
    {
        public List<JsonWord> database;
        public Languages sourceLanguage;

        public static explicit operator JsonLanguageDatabase(LanguageDatabase database)
        {
            var jsonDb = new JsonLanguageDatabase();
            var langDb = database.GetDB();
            jsonDb.database = new List<JsonWord>();
            jsonDb.sourceLanguage = database.sourceLanguage;
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

        
    }

    [Serializable]
    public class JsonWord
    {
        public string key;
        public List<JsonTranslation> meanings;
        public List<string> regular_expressions;
    }

    [Serializable]
    public class JsonTranslation
    {
        public string language;
        public string translation;
    }

}