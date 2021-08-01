using System;
using System.Collections;
using System.Collections.Generic;
using Localization;
using UnityEngine;

public class TestLocale : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private string text;

    [SerializeField] private string food;
    [SerializeField] private string person;

    [SerializeField] private Translation trans;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            trans.SetRegularText(0, key, text, new Translation.RegularTranslation("food", food, true), 
                new Translation.RegularTranslation("person", person, true));
        }
    }
}
