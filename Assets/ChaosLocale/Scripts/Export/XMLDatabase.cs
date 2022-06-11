using System;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
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