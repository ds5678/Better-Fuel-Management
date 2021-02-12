using System.Collections.Generic;

namespace BetterFuelManagement
{
    class BetterFuelLocalizations
    {
        private static bool hasBeenLoaded = false;

        private static string locId1 = "GAMEPLAY_NoFuelCapacityAvailable";
        private static Dictionary<string, string> locDict1 = new Dictionary<string, string>
        {
            ["English"] = "No containers with remaining capacity available",
            ["German"]  = "Keine Behälter mit verbleibender Kapazität vorhanden",
            ["Russian"] = "Отсутствует емкость для топлива",
            ["Japanese"] = "空きのある容器がありません",
            ["Spanish (Spain)"] = "No hay contenedores disponibles con suficiente capacidad"
        };

        private static string locId2 = "GAMEPLAY_Drain";
        private static Dictionary<string, string> locDict2 = new Dictionary<string, string>
        {
            ["English"] = "Drain",
            ["German"] = "Entleeren",
            ["Russian"] = "Слить",
            ["Japanese"] = "中身を抜き取る",
            ["Spanish (Spain)"] = "Vaciar"
        };

        private static string locId3 = "GAMEPLAY_DrainingProgress";
        private static Dictionary<string, string> locDict3 = new Dictionary<string, string>
        {
            ["English"] = "Draining...",
            ["German"] = "Entleeren...",
            ["Russian"] = "Сливает...",
            ["Japanese"] = "抜き取っています...",
            ["Spanish (Spain)"] = "Vaciando..."
        };

        private static string locId4 = "GAMEPLAY_Lost";
        private static Dictionary<string, string> locDict4 = new Dictionary<string, string>
        {
            ["English"] = "Lost",
            ["German"] = "Verloren",
            ["Russian"] = "Разрушено",
            ["Japanese"] = "失われました",
            ["Spanish (Spain)"] = "Perdidos"
        };

        private static string locId5 = "GAMEPLAY_AlreadyEmpty";
        private static Dictionary<string, string> locDict5 = new Dictionary<string, string>
        {
            ["English"] = "Already empty",
            ["German"] = "Schon leer",
            ["Russian"] = "Ёмкость пуста",
            ["Japanese"] = "既に容器が空です",
            ["Spanish (Spain)"] = "Ya está vacío"
        };

        private static string locId6 = "GAMEPLAY_AlreadyFilled";
        private static Dictionary<string, string> locDict6 = new Dictionary<string, string>
        {
            ["English"] = "Already full",
            ["German"] = "Schon voll",
            ["Russian"] = "Ёмкость заполнена",
            ["Japanese"] = "既に容器が一杯です",
            ["Spanish (Spain)"] = "Ya está lleno"
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
