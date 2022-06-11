using System.IO;
using System.Xml.Serialization;
using Locale.Scripts;
using UnityEngine;

namespace ChaosLocale.Scripts.Export
{
    public static class LocaleExport
    {
        public static LocaleDatabase ImportXml(string path)
        {
            var filestream = new FileStream(path, FileMode.Open);
            var serializer = new XmlSerializer(typeof(XMLDatabase));
            var inst = ScriptableObject.CreateInstance<LocaleDatabase>();
            var database = (XMLDatabase) serializer.Deserialize(filestream);
            inst.SetGroups(database.groups);
            inst.baseLanguage = database.baseLanguage;
            filestream.Close();
            return inst;
        }
        
        public static void ExportXML(string exportPath)
        {
            var db = Localization.GetDB();
            
            if(string.IsNullOrEmpty(exportPath)) return;
            
            var path = exportPath;
            File.WriteAllText(path, "");
            var database = new XMLDatabase();
            
            database.groups = db.Groups;
            database.baseLanguage = db.baseLanguage;
            
            var serializer = new XmlSerializer(typeof(XMLDatabase));
            var filestream = new FileStream(path, FileMode.OpenOrCreate);
            serializer.Serialize(filestream, database);
        }
        
        public static void ExportXML(this LocaleDatabase db, string exportPath)
        {
            
            if(string.IsNullOrEmpty(exportPath)) return;
            
            var path = exportPath;
            File.WriteAllText(path, "");
            var database = new XMLDatabase();
            
            database.groups = db.Groups;
            database.baseLanguage = db.baseLanguage;
            
            var serializer = new XmlSerializer(typeof(XMLDatabase));
            var filestream = new FileStream(path, FileMode.OpenOrCreate);
            serializer.Serialize(filestream, database);
        }
    }
}