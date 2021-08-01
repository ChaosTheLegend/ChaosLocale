using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Localization
{
    public class Translation : MonoBehaviour
    {
        private static List<Translation> allTranslators = new List<Translation>();

        public static void UpdateAllTranslations()
        {
            foreach (var trans in allTranslators)
            {
                trans.UpdateLabels();
            }
        }

        [Serializable]
        public class RegularTranslation
        {
            public string  expression;
            public string Value
            {
                get
                {
                    if (recursive)
                    {
                        return LocalizationManager.Instance.GetTranslation(key);
                    }
                    else
                    {
                        return key; 
                    }

                    return key;
                }   
                set
                {
                    key = value;
                }
            }
            public string key;
            public bool recursive;

            
            /// <summary>
            /// Constructor for RegularTranslation
            /// </summary>
            /// <param name="expression">name of the expression to set the value for</param>
            /// <param name="value">text with which expression will be replaced</param>
            /// <param name="recursive">should replaced text also be translated?</param>
            public RegularTranslation(string expression, string value, bool recursive = false)
            {
                this.recursive = recursive;
                this.expression = expression;
                Value = value;
            }
            
            public RegularTranslation(){}

        }
        
        [Serializable]
        public class LabelTranslation
        {
            public TextMeshProUGUI label = null;
            private string lastTranslation = "";
            public string key = "";
            public string text = "";

            public List<RegularTranslation> RegularExpressions
            {
                get
                {
                    if (regExp == null) regExp = new List<RegularTranslation>();
                    return regExp;
                }
                set => regExp = value;
            }
            [SerializeField] private List<RegularTranslation> regExp;
            public bool foldout = false;
            public void UpdateLabel()
            {
                if (label == null)
                {
                    Debug.LogError($"label for key {key} is not set!");
                    return;
                }

                if (RegularExpressions.Count == 0) lastTranslation = LocalizationManager.Instance.GetTranslation(key);
                else lastTranslation = LocalizationManager.Instance.GetRegularTranslation(key, RegularExpressions.ToArray());
                
                var output = text.Replace("{t}", lastTranslation);
                label.text = output;
            }

            /// <summary>
            /// Updates label without updating a key for translation
            /// Useful for timers 
            /// </summary>
            public void SoftUpdate()
            {
                var output = text.Replace("{t}", lastTranslation);
                label.text = output;
            }
        }

        [SerializeField] public List<LabelTranslation> translationFields;
        
        public void SetText(int labelId, string key, string text = "")
        {
            translationFields[labelId].key = key;
            if (!string.IsNullOrEmpty(text)) translationFields[labelId].text = text;
            translationFields[labelId].UpdateLabel();
        }
        
        private void SetTextWithoutUpdate(int labelId, string key, string text = "")
        {
            translationFields[labelId].key = key;
            if (!string.IsNullOrEmpty(text)) translationFields[labelId].text = text;
        }
        /// <summary>
        /// Updates the key for translation
        /// </summary>
        /// <param name="label"></param>
        /// <param name="key"></param>
        /// <param name="text"></param>
        public void SetText(TextMeshProUGUI label, string key, string text = "")
        {
            var id = translationFields.FindIndex((translation => translation.label == label));
            if (id == -1) return;
            SetText(id, key, text);
        }

        public void SetRegularText(int labelId, string key, params RegularTranslation[] expressions) 
        {
            SetTextWithoutUpdate(labelId, key);
            SetRegularExpressions(labelId, expressions);
            translationFields[labelId].UpdateLabel();
        }
        
        public void SetRegularText(int labelId, string key, string text = "", params RegularTranslation[] expressions) 
        {
            SetTextWithoutUpdate(labelId, key, text);
            SetRegularExpressions(labelId, expressions);
            translationFields[labelId].UpdateLabel();
        }

        public void SetRegularExpressions(int labelId, params RegularTranslation[] expressions)
        {
            var field = translationFields[labelId];
            field.RegularExpressions = expressions.ToList();
        }
        
        
        /// <summary>
        /// Sets new text to label without updating the key for translation
        /// </summary>
        /// <param name="labelId"></param>
        /// <param name="text"></param>
        public void SetTextQuick(int labelId, string text)
        {
            translationFields[labelId].text = text;
            translationFields[labelId].SoftUpdate();
        }

        /// <summary>
        /// Sets new text to label without updating the key for translation
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        public void SetTextQuick(TextMeshProUGUI label, string text)
        {
            var id = translationFields.FindIndex((translation => translation.label == label));
            if (id == -1) return;
            SetTextQuick(id, text);
        }

        private void Awake()
        {
            allTranslators.Add(this);
            if (LocalizationManager.Instance != null)
            {
                UpdateLabels();
            }
            else
            {
                Debug.LogWarning("No localization manager found on scene");
            }
        }

        public void UpdateLabels()
        {
            foreach (var field in translationFields)
            {
                field.UpdateLabel();
            }
        }
    }
}
