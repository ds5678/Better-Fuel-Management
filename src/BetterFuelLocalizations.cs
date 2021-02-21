using System.Collections.Generic;

namespace BetterFuelManagement
{
    class BetterFuelLocalizations
    {
        private static bool hasBeenLoaded = false;

        private static readonly string locId1 = "GAMEPLAY_NoFuelCapacityAvailable";
        private static readonly Dictionary<string, string> locDict1 = new Dictionary<string, string>
        {
            ["English"] = "No containers with remaining capacity available",
            ["German"]  = "Keine Behälter mit verbleibender Kapazität vorhanden",
            ["Russian"] = "Отсутствует емкость для топлива",
            ["French (France)"] = "Tous vos récipients sont déjà pleins",
            ["Japanese"] = "空きのある容器がありません",
            ["Swedish"] = "Inga behållare med tillgängligt utrymme",
            ["Spanish (Spain)"] = "No hay contenedores disponibles con suficiente capacidad",
            ["Turkish"] = "Kalan kapasiteye sahip konteyner yok",
            ["Dutch"] = "Er is geen container in je inventory met genoeg opslag",
            ["Finnish"] = "Ei säiliötä, joissa on jäljellä käytettävää tilaa"
        };

        private static readonly string locId2 = "GAMEPLAY_Drain";
        private static readonly Dictionary<string, string> locDict2 = new Dictionary<string, string>
        {
            ["English"] = "Drain",
            ["German"] = "Entleeren",
            ["Russian"] = "Слить",
            ["French (France)"] = "Vider",
            ["Japanese"] = "中身を抜き取る",
            ["Swedish"] = "Tom",
            ["Spanish (Spain)"] = "Vaciar",
            ["Turkish"] = "Boşaltmak",
            ["Dutch"] = "Leeg afvoeren",
            ["Finnish"] = "Tyhjennä"
        };

        private static readonly string locId3 = "GAMEPLAY_DrainingProgress";
        private static readonly Dictionary<string, string> locDict3 = new Dictionary<string, string>
        {
            ["English"] = "Draining...",
            ["German"] = "Entleeren...",
            ["Russian"] = "Сливает...",
            ["French (France)"] = "Vidange...",
            ["Japanese"] = "抜き取っています...",
            ["Swedish"] = "Tömning...",
            ["Spanish (Spain)"] = "Vaciando...",
            ["Turkish"] = "Boşaltma...",
            ["Dutch"] = "Aftappen...",
            ["Finnish"] = "Tyhjennetään..."
        };

        private static readonly string locId4 = "GAMEPLAY_Lost";
        private static readonly Dictionary<string, string> locDict4 = new Dictionary<string, string>
        {
            ["English"] = "Lost",
            ["German"] = "Verloren",
            ["Russian"] = "Разрушено",
            ["French (France)"] = "Perdu",
            ["Japanese"] = "失われました",
            ["Swedish"] = "Förlorat",
            ["Spanish (Spain)"] = "Perdidos",
            ["Turkish"] = "Boşa harcandı",
            ["Dutch"] = "Verspild",
            ["Finnish"] = "Menetetty"
        };

        private static readonly string locId5 = "GAMEPLAY_AlreadyEmpty";
        private static readonly Dictionary<string, string> locDict5 = new Dictionary<string, string>
        {
            ["English"] = "Already empty",
            ["German"] = "Schon leer",
            ["Russian"] = "Ёмкость пуста",
            ["French (France)"] = "Déjà vide",
            ["Japanese"] = "既に容器が空です",
            ["Swedish"] = "Det är redan tomt",
            ["Spanish (Spain)"] = "Ya está vacío",
            ["Turkish"] = "Zaten boş",
            ["Dutch"] = "Het is al leeg",
            ["Finnish"] = "On jo tyhjä"
        };

        private static readonly string locId6 = "GAMEPLAY_AlreadyFilled";
        private static readonly Dictionary<string, string> locDict6 = new Dictionary<string, string>
        {
            ["English"] = "Already full",
            ["German"] = "Schon voll",
            ["Russian"] = "Ёмкость заполнена",
            ["French (France)"] = "Déjà plein",
            ["Japanese"] = "既に容器が一杯です",
            ["Swedish"] = "Det är redan fullt",
            ["Spanish (Spain)"] = "Ya está lleno",
            ["Turkish"] = "Zaten dolu",
            ["Dutch"] = "Het is al vol",
            ["Finnish"] = "On jo täysi"
        };

        internal static void AddLocalizations()
        {
            LocalizationUtils.LoadLocalization(locId1, locDict1, true);
            LocalizationUtils.LoadLocalization(locId2, locDict2, true);
            LocalizationUtils.LoadLocalization(locId3, locDict3, true);
            LocalizationUtils.LoadLocalization(locId4, locDict4, true);
            LocalizationUtils.LoadLocalization(locId5, locDict5, true);
            LocalizationUtils.LoadLocalization(locId6, locDict6, true);
            hasBeenLoaded = true;
        }

        internal static bool IsLoaded()
        {
            return hasBeenLoaded;
        }
    }
}
