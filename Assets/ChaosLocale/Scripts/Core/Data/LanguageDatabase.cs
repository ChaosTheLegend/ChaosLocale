using System;
using System.Collections.Generic;
using System.Linq;
using Locale.Scripts;
using Localization;
using UnityEngine;

namespace ChaosLocale.Scripts.Core.Data
{
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
            List<Locale.Scripts.LanguageCodes> list = new List<Locale.Scripts.LanguageCodes>();
            int count = 0;
            int max = System.Enum.GetNames(typeof(Locale.Scripts.LanguageCodes)).Length;
            while (count < max)
            {
                long isOne = (value & 1);
                value = value >> 1;
                if (isOne == 1)
                {
                    list.Add((Locale.Scripts.LanguageCodes)count);
                }
                count++;
            }
            Locale.Scripts.LanguageCodes sourcecode = (Locale.Scripts.LanguageCodes)sourceLanguage;
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
    }
}

