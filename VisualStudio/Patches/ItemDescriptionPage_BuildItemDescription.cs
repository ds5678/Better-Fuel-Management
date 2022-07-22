extern alias Hinterland;

using BetterFuelManagement;
using HarmonyLib;
using Hinterland;

namespace Patches;

[HarmonyPatch(typeof(ItemDescriptionPage), "BuildItemDescription")]
internal class ItemDescriptionPage_BuildItemDescription
{
    private static void Postfix(GearItem gi)
    {
        if (FuelUtils.IsFuelContainer(gi))
        {
            FuelUtils.SetConditionToMax(gi);
        }
    }
}

