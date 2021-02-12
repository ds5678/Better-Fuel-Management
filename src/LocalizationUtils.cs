using System.Collections.Generic;

namespace BetterFuelManagement
{
    class LocalizationUtils
    {
        public static void LoadLocalization(string localizationID, Dictionary<string, string> translationDictionary, bool useEnglishAsDefault = false)
        {
            string[] knownLanguages = Localization.GetLanguages().ToArray();


            string[] translations = new string[knownLanguages.Length];
            for (int i = 0; i < knownLanguages.Length; i++)
            {
                string language = knownLanguages[i];

                if (translationDictionary.ContainsKey(language))
                {
                    translations[i] = translationDictionary[language];
                }
                else if (useEnglishAsDefault && translationDictionary.ContainsKey("English"))
                {
                    translations[i] = translationDictionary["English"];
                }
            }

            var key = localizationID;
            if (!Localization.s_CurrentLanguageStringTable.DoesKeyExist(key))
            {
                Localization.s_CurrentLanguageStringTable.AddEntryForKey(key);
            }
            for (int j = 0; j < translations.Length; j++)
            {
                Localization.s_CurrentLanguageStringTable.GetEntryFromKey(key).m_Languages[j] = translations[j];
            }
        }
    }
}
