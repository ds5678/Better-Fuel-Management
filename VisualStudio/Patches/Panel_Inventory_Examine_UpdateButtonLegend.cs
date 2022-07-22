extern alias Hinterland;

using BetterFuelManagement;
using HarmonyLib;
using Hinterland;

namespace Patches;

[HarmonyPatch(typeof(Panel_Inventory_Examine), "UpdateButtonLegend")]
internal class Panel_Inventory_Examine_UpdateButtonLegend
{
    private static void Postfix(Panel_Inventory_Examine __instance)
    {
        //Implementation.Log("Panel_Inventory_Examine - UpdateButtonLegend");

        if (FuelUtils.IsFuelItem(__instance.m_GearItem) && ButtonUtils.IsSelected(__instance.m_Button_Unload))
        {
            __instance.m_ButtonLegendContainer.UpdateButton("Continue", "GAMEPLAY_BFM_Drain", true, 1, true);
        }
    }
}

