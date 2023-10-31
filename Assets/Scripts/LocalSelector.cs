using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalSelector : MonoBehaviour
{
    public Dropdown languageDropdown;
    private void Start()
    {
        languageDropdown.onValueChanged.AddListener(LanguageChanged);
    }

    private void LanguageChanged(int value)
    {
        if (value == 0)
        {
            // Ukraine
            StartCoroutine(SetLocale(0));
        }
        else
        {
            // English
            StartCoroutine(SetLocale(1));
        }
    }

    private IEnumerator SetLocale(int localeID)
    {  
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];     
    }
}