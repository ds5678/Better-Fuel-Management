extern alias Hinterland;

using BetterFuelManagement;
using HarmonyLib;
using Hinterland;

namespace Patches;

[HarmonyPatch(typeof(Panel_Inventory_Examine), "Enable", new System.Type[] { typeof(bool), typeof(ComingFromScreenCategory) })]
internal class Panel_Inventory_Examine_Enable
{
    private static void Prefix(Panel_Inventory_Examine __instance, bool enable)
    {
        //Implementation.Log("Panel_Inventory_Examine - Enable");
        if (!enable) return;

        if (FuelUtils.IsFuelItem(__instance.m_GearItem))
        {
            // repurpose the left "Unload" button to "Drain"
            ButtonUtils.SetButtonLocalizationKey(__instance.m_Button_Unload, "GAMEPLAY_BFM_Drain");
            ButtonUtils.SetButtonSprite(__instance.m_Button_Unload, "ico_lightSource_lantern");

            // rename the bottom right "Unload" button to "Drain"
            ButtonUtils.SetUnloadButtonLabel(__instance, "GAMEPLAY_BFM_Drain");

            Transform lanternTexture = __instance.m_RefuelPanel.transform.Find("FuelDisplay/Lantern_Texture");
            ButtonUtils.SetTexture(lanternTexture, Utils.GetInventoryIconTexture(__instance.m_GearItem));
        }
        else
        {
            ButtonUtils.SetButtonLocalizationKey(__instance.m_Button_Unload, "GAMEPLAY_Unload");
            ButtonUtils.SetButtonSprite(__instance.m_Button_Unload, "ico_ammo_rifle");
            ButtonUtils.SetUnloadButtonLabel(__instance, "GAMEPLAY_Unload");
        }
    }
}

