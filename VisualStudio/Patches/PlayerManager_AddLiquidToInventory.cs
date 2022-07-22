extern alias Hinterland;

using BetterFuelManagement;
using HarmonyLib;
using Hinterland;

namespace Patches;

[HarmonyPatch(typeof(PlayerManager), "AddLiquidToInventory")]
internal class PlayerManager_AddLiquidToInventory
{
    private static void Postfix(PlayerManager __instance, float litersToAdd, GearLiquidTypeEnum liquidType, ref float __result)
    {
        //Implementation.Log("PlayerManager - AddLiquidToInventory");

        if (liquidType == GearLiquidTypeEnum.Kerosene && __result != litersToAdd)
        {
            MessageUtils.SendLostMessageDelayed(litersToAdd - __result);

            // just pretend we added everything, so the original method will not generate new containers
            __result = litersToAdd;
        }
    }
}

