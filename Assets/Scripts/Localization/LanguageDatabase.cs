using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Localization
{
[Serializable]
public enum Languages
{
    Arabic = 0,
    Chinese = 1,
    Dutch = 2,
    English = 3,
    Filipino = 4,
    French = 5,
    German = 6,
    Hindi = 7,
    Indonesian = 8,
    Italian = 9,
    Japanese = 10,
    Portuguese = 11,
    Russian = 12,
    Spanish = 13,
    Turkish = 14,
    Urdu = 15
}

    public enum LanguageCodes
    {
        ar = 0,
        zh_CN = 1,
        nl = 2,
        en = 3,
        tl = 4,
        fr = 5,
        de = 6,
        hi = 7,
        id = 8,
        it = 9,
        ja = 10,
        pt_BR = 11,
        ru = 12,
        es = 13,
        tr = 14,
        ur = 15

    }

    [ExecuteInEditMode]
    public class LanguageDatabase : ScriptableObject
    {

        public List<Word> database;
        public Languages sourceLanguage;
        
        public void UpdateLangNames()
        {
            foreach (var wd in database)
            {
                wd.UpdateLangNames();
            }
        }
        public void Add(Word d)
        {
            database.Add(d);
        }

        public void Remove(int index)
        {
            if (index < database.Count)
            {
                database.RemoveAt(index);
            }
        }

        public List<Word> GetDB()
        {
            if (database == null) database = new List<Word>();
            return database;
        }

        public bool HasKey(string key)
        {
            var find = database.Find((word) => word.word == key);
            return find != null;
        }

        public Word GetWord(string key)
        {
            var find = database.Find((word) => word.word == key);
            return find;
        }
        public void ClearAll()
        {
            database.Clear();
        }
        
        private void GetTargetLanguage(long selectedEnum, string word)
        {

            long value = (long)selectedEnum;
            List<LanguageCodes> list = new List<LanguageCodes>();
            int count = 0;
            int max = System.Enum.GetNames(typeof(LanguageCodes)).Length;
            while (count < max)
            {
                long isOne = (value & 1);
                value = value >> 1;
                if (isOne == 1)
                {
                    list.Add((LanguageCodes)count);
                }
                count++;
            }
            LanguageCodes sourcecode = (LanguageCodes)sourceLanguage;
        }
        
        
        public string GetMeaning(string sourceText, Languages targetLanguage){
            sourceText = sourceText.ToLower ();
            Word word = GetDB ().Find (x => x.word.Equals (sourceText));
            if (word == null)
                return sourceText;
            WordTranslation wordTranslation=  word.wordTranslation.Find (x => x.country == targetLanguage);
            if (wordTranslation == null)
            {
                targetLanguage = sourceLanguage;
            }
            wordTranslation=  word.wordTranslation.Find (x => x.country == targetLanguage);
            if (wordTranslation == null) return sourceText;
            return ArabicTranslation(wordTranslation.meaning,targetLanguage);
        }
        private string ArabicTranslation(string word,Languages targetLanguage){
            if (targetLanguage == Languages.Arabic || targetLanguage == Languages.Urdu) {
                return ArabicFixer.Fix (word);
            }
            return word;
        }
        
        public string GetRegularTranslation(string word, Languages targetLanguage,  params Translation.RegularTranslation[] expressions)
        {
            var translation = GetMeaning(word, targetLanguage);
            var wd = GetWord(word);
            var regExpressions = wd.regularExpressions;
            foreach (var expression in expressions)
            {
                var key = expression.expression;
                var value = (expression.recursive)? GetMeaning(expression.key, targetLanguage) : expression.Value;
                var exp = regExpressions.Find((e) => e.key == key);
                if (exp != null)
                {
                    if (translation.Contains(key))
                    {
                        translation = translation.Replace("{"+key+"}", value); 
                    }
                }
            }
            
            return  translation;
        }
        
        
        public static explicit operator LanguageDatabase(JsonLanguageDatabase jsonDb)
        {
            var langDb = CreateInstance<LanguageDatabase>();
            langDb.sourceLanguage = jsonDb.sourceLanguage;
            langDb.database = new List<Word>();
            var jsonWords = jsonDb.database;
            foreach (var jsonWord in jsonWords)
            {
                langDb.database.Add((Word) jsonWord);
            }

            return langDb;
        }

        public List<string> GetAllGroups()
        {
            var groups = new List<string>();
            var db = GetDB();
            foreach (var word in db.Where(word => !groups.Contains(word.@group)))
            {
                groups.Add(word.@group);
            }

            return groups;
        }

        public List<Word> GetWordsInGroup(string group)
        {
            var find = GetDB().FindAll((word) => word.@group == group);
            return find;
        }
    }

    [System.Serializable]
    public class LanguageData
    {
        public long translatedTo;
        public string word;

        public LanguageData()
        {
            word = "";
        }
    }

    [System.Serializable]
    public class Word
    {
        public string word;
        public List<WordTranslation> wordTranslation;
        public bool hasRegularExpression;
        public List<RegularExpression> regularExpressions = new List<RegularExpression>();
        public string group;

        public void UpdateLangNames()
        {
            foreach (var wd in wordTranslation)
            {
                wd.UpdateLangNames();
            }
        }

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
    public class RegularExpression
    {
        public string key;

        public static explicit operator RegularExpression(string key)
        {
            var exp = new RegularExpression() {key = key};
            return exp;
        }
    }
    
    [System.Serializable]
    public class WordTranslation
    {
        // translation of word in different Languages
        public Languages country;
        [SerializeField] private string language;
        public string meaning;

        public void UpdateLangNames()
        {
            language = Enum.GetName(typeof(Languages), country);
        }
        
        public static explicit operator WordTranslation(JsonTranslation jsonTranslation)
        {
            var trans = new WordTranslation();
            trans.country = (Languages) Enum.Parse(typeof(Languages) ,jsonTranslation.language);
            trans.meaning = jsonTranslation.translation;
            return trans; 
        }    

    }
}

