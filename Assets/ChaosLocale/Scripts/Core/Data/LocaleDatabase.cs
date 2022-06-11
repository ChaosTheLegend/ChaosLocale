using System;
using System.Collections.Generic;
using System.Linq;
using ChaosLocale.Scripts.Core.Data;
using UnityEngine;

namespace Locale.Scripts
{
    public class LocaleDatabase : ScriptableObject
    {
        public Languages baseLanguage;
        [SerializeField] private List<WordGroup> _groups;

        public List<WordGroup> Groups => _groups;

        public void SetGroups(List<WordGroup> g)
        {
            _groups = g;
        }
        
        public List<string> GetGroupNames()
        {
            return Groups.Select(@group => @group.title).ToList();
        }

        public void AddGroup(string title = "new group")
        {
            var group = new WordGroup();
            group.words = new List<Word>();
            Groups.Add(group);
        }
        
        public List<Word> GetGroupWords(int groupId)
        {
            return Groups[groupId].words;
        }

        public void Load(LocaleDatabase db)
        {
            baseLanguage = db.baseLanguage;
            _groups = db.Groups;
        }
            
        public void AddWord(int groupId)
        {
            var word = new Word();
            word.key = "new key";
            word.translations = new List<WordMeaning>();
            word.baseTranslate = "";
            Groups[groupId].words.Add(word);
        }

        public void AddWord(int groupId, Word word)
        {
            Groups[groupId].words.Add(word);
        }
        
        public void RemoveWord(int groupId, int wordId)
        {
            Groups[groupId].words.RemoveAt(wordId);
        }

        public static LocaleDatabase CrossJoin(LocaleDatabase db1, LocaleDatabase db2, Languages baseLang)
        {
            var db3 = ScriptableObject.CreateInstance<LocaleDatabase>();
            db3.SetGroups(db1.Groups);
            db3.baseLanguage = baseLang;
            foreach (var @group in db2.Groups)
            {
                db3.Groups.Add(@group);
            }
            
            return db3;
        }
        public static LocaleDatabase LeftJoin(LocaleDatabase db1, LocaleDatabase db2, Languages baseLang)
        {
            var db3 = ScriptableObject.CreateInstance<LocaleDatabase>();
            db3.SetGroups(db1.Groups);
            db3.baseLanguage = baseLang;
            foreach (var @group in db2.Groups)
            {
                var gp1 = db3.Groups.Find(gp => gp.title == @group.title);
                if (gp1 != null)
                {
                    var gp2 = @group;
                    foreach (var word in gp2.words)
                    {
                        var word1 = gp1.words.Find(w => w.key == word.key);
                        if (word1 !=null)
                        {
                            var word2 = word;
                            foreach (var trans in word2.translations)
                            {
                                if (word1.translations.Find(wd => wd.language == trans.language) == null)
                                {
                                    word1.translations.Add(trans);   
                                }
                            }
                        }
                        else
                        {
                            gp1.words.Add(word);    
                        }
                    }
                }
                else
                {
                    db3.Groups.Add(@group);
                }
            }
            return db3;
        }
        
        
        public static LocaleDatabase RightJoin(LocaleDatabase db1, LocaleDatabase db2, Languages baseLang)
        {
            return LeftJoin(db2, db1, baseLang);
        }
    }
}
