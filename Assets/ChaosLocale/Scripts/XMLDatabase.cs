using System;
using System.Collections.Generic;
using Locale.Scripts;

namespace ChaosLocale.Scripts
{
    [Serializable]
    public class XMLDatabase
    {
        public Languages baseLanguage;
        public List<WordGroup> groups;
    }
}