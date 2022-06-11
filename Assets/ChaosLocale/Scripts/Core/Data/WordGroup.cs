using System;
using System.Collections.Generic;

namespace Locale.Scripts
{
    [Serializable]
    public class WordGroup
    {
        public string title;
        public List<Word> words;

        public WordGroup() : this("new group") { }

        public WordGroup(string title)
        {
            this.title = title;
            if (words == null) words = new List<Word>();
        }
        
    }
}