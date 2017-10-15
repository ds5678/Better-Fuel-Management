using UnityEngine;

namespace BetterFuelManagement
{
    internal class BetterFuelManagement
    {
        public static void OnLoad()
        {
            Debug.Log("[Better-Fuel-Management]: Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

            AddTranslations();
        }

        internal static float GetLitersToDrain(Panel_Inventory_Examine panel)
        {
            float availableCapacity = GetRemainingFuelCapacityInInventory();
            float availableFuel = panel.m_GearItem.m_KeroseneLampItem.m_CurrentFuelLiters;
            return Mathf.Min(availableFuel, availableCapacity);
        }

        internal static float GetRemainingFuelCapacityInInventory()
        {
            float capacity = GameManager.GetPlayerManagerComponent().GetCapacityLiters(GearLiquidTypeEnum.Kerosene);
            float used = GameManager.GetPlayerManagerComponent().GetTotalLiters(GearLiquidTypeEnum.Kerosene);

            return capacity - used;
        }

        internal static bool IsExaminingKeroseneLamp(Panel_Inventory_Examine panel)
        {
            return panel && panel.m_GearItem && panel.m_GearItem.m_KeroseneLampItem;
        }

        internal static bool IsFuelContainer(GameObject gameObject)
        {
            if (!gameObject)
            {
                return false;
            }

            LiquidItem liquidItem = gameObject.GetComponent<LiquidItem>();
            if (!liquidItem)
            {
                return false;
            }

            return liquidItem.m_LiquidType == GearLiquidTypeEnum.Kerosene;
        }

        internal static bool WasCalledFromDeductLiquidFromInventory()
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(2, false);

            foreach (System.Diagnostics.StackFrame eachFrame in stackTrace.GetFrames())
            {
                if (eachFrame.GetMethod().Name == "DeductLiquidFromInventory" && eachFrame.GetMethod().DeclaringType == typeof(PlayerManager))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddTranslations()
        {
            string[] knownLanguages = Localization.knownLanguages;

            string[] translations = new string[knownLanguages.Length];
            for (int i = 0; i < knownLanguages.Length; i++)
            {
                switch (knownLanguages[i])
                {
                    case "English":
                        translations[i] = "No containers with remaining capacity available";
                        break;

                    case "German":
                        translations[i] = "Keine Behälter mit verbleibender Kapazität vorhanden";
                        break;

                    default:
                        translations[i] = "No containers with remaining capacity available\nHelp me translate this!\nVisit https://github.com/WulfMarius/Better-Fuel-Management";
                        break;
                }
            }
            Localization.dictionary.Add("GAMEPLAY_NoFuelCapacityAvailable", translations);

            translations = new string[knownLanguages.Length];
            for (int i = 0; i < knownLanguages.Length; i++)
            {
                switch (knownLanguages[i])
                {
                    case "English":
                        translations[i] = "Drain";
                        break;

                    case "German":
                        translations[i] = "Entleeren";
                        break;

                    default:
                        translations[i] = "Drain\nHelp me translate this!\nVisit https://github.com/WulfMarius/Better-Fuel-Management";
                        break;
                }
            }
            Localization.dictionary.Add("GAMEPLAY_Drain", translations);

            translations = new string[knownLanguages.Length];
            for (int i = 0; i < knownLanguages.Length; i++)
            {
                switch (knownLanguages[i])
                {
                    case "English":
                        translations[i] = "Draining...";
                        break;

                    case "German":
                        translations[i] = "Entleeren...";
                        break;

                    default:
                        translations[i] = "Draining...\nHelp me translate this!\nVisit https://github.com/WulfMarius/Better-Fuel-Management";
                        break;
                }
            }
            Localization.dictionary.Add("GAMEPLAY_DrainingProgress", translations);

            translations = new string[knownLanguages.Length];
            for (int i = 0; i < knownLanguages.Length; i++)
            {
                switch (knownLanguages[i])
                {
                    case "English":
                        translations[i] = "Lost";
                        break;

                    case "German":
                        translations[i] = "Verloren";
                        break;

                    default:
                        translations[i] = "Lost\nHelp me translate this!\nVisit https://github.com/WulfMarius/Better-Fuel-Management";
                        break;
                }
            }
            Localization.dictionary.Add("GAMEPLAY_Lost", translations);
        }
    }
}