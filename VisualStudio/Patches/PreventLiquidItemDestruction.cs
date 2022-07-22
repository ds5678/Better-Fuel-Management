extern alias Hinterland;

using BetterFuelManagement;
using HarmonyLib;
using Hinterland;

namespace Patches;

internal static class PreventLiquidItemDestruction
{
    private static int deductLiquidFromInventoryCallDepth = 0;

    [HarmonyPatch(typeof(PlayerManager), "DeductLiquidFromInventory")]
    private static class PlayerManager_DeductLiquidFromInventory
    {
        private static void Prefix()
        {
            deductLiquidFromInventoryCallDepth++;
        }
        private static void Postfix()
        {
            deductLiquidFromInventoryCallDepth--;
        }
    }

    [HarmonyPatch(typeof(Inventory), "DestroyGear")]
    private static class Inventory_DestroyGear
    {
        private static bool Prefix(GameObject go)
        {
            if (deductLiquidFromInventoryCallDepth > 0)
            {
                Implementation.Log("TLD is trying to destroy {0}", go.name);

                LiquidItem liquidItem = go.GetComponent<LiquidItem>();

                if (liquidItem != null && liquidItem.m_LiquidType == GearLiquidTypeEnum.Kerosene)
                {
                    Implementation.Log("Prevented destruction");
                    return false;
                }
            }

            return true;
        }
    }
}