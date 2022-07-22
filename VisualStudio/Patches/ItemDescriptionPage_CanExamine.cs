extern alias Hinterland;

using BetterFuelManagement;
using HarmonyLib;
using Hinterland;

namespace Patches;

[HarmonyPatch(typeof(ItemDescriptionPage), "CanExamine")]
internal class ItemDescriptionPage_CanExamine
{
    private static bool Prefix(GearItem gi, ref bool __result)
    {
        if (FuelUtils.IsFuelItem(gi))
        {
            __result = true;
            return false;
        }

        return true;
    }
}

