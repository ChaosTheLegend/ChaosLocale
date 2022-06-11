using System;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEngine;
using Word = ChaosLocale.Scripts.Core.Data.Word;

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

        public static explicit operator LanguageDatabase(JsonLanguageDatabase jsonDb)
        {
            var langDb = ScriptableObject.CreateInstance<LanguageDatabase>();
            langDb.sourceLanguage = jsonDb.sourceLanguage;
            langDb.database = new List<Word>();
            var jsonWords = jsonDb.database;
            foreach (var jsonWord in jsonWords)
            {
                langDb.database.Add((Word) jsonWord);
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
        
        public static explicit operator Word(JsonWord jsonWord)
        {
            var wd = new Word();
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