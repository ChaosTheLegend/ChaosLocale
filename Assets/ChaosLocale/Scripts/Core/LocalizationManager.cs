using System;
using System.Collections;
using System.Collections.Generic;
using ChaosLocale.Scripts.Core.Data;
using Locale.Scripts;
using UnityEngine;

namespace Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        
        public static LocalizationManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            if (PlayerPrefs.HasKey("lang"))
            {
                currentLanguage = (Languages) PlayerPrefs.GetInt("lang");
            }
            else
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Russian:
                        PlayerPrefs.SetInt("lang", 12);
                        currentLanguage = Languages.Russian;
                        break;
                    case SystemLanguage.English:
                        PlayerPrefs.SetInt("lang", 3);
                        currentLanguage = Languages.English;
                        break;
                    default:
                        PlayerPrefs.SetInt("lang", 3);
                        currentLanguage = Languages.English;
                        break;
                }
            }

            Translation.UpdateAllTranslations();
        }


        private static Languages currentLanguage;


        public string GetTranslation(string word)
        {
            return LanguageManager.Instance.GetMeaning(word, currentLanguage);
        }

        public string GetRegularTranslation(string word, params Translation.RegularTranslation[] expressions)
        {
            return LanguageManager.Instance.GetRegularMeaning(word, currentLanguage , expressions);
        }        

        public void SetLanguage(Languages langId)
        {
            PlayerPrefs.SetInt("lang", (int)langId);
            currentLanguage = langId;
            Translation.UpdateAllTranslations();
        }

        
        public void SetLanguage(int langId)
        {
            PlayerPrefs.SetInt("lang", langId);
            currentLanguage = (Languages) langId;
            Translation.UpdateAllTranslations();
        }

    }
}
