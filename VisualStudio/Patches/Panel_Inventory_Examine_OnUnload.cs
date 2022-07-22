extern alias Hinterland;

using BetterFuelManagement;
using HarmonyLib;
using Hinterland;

namespace Patches;

[HarmonyPatch(typeof(Panel_Inventory_Examine), "OnUnload")]
internal class Panel_Inventory_Examine_OnUnload
{
    private static bool Prefix(Panel_Inventory_Examine __instance)
    {
        //Implementation.Log("Panel_Inventory_Examine - OnUnload");
        if (FuelUtils.IsFuelItem(__instance.m_GearItem))
        {
            FuelUtils.Drain(__instance.m_GearItem);
            return false;
        }
        return true;
    }
}

