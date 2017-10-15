using Harmony;
using UnityEngine;

namespace BetterFuelManagement
{
    [HarmonyPatch(typeof(Inventory), "DestroyGear")]
    public class Inventory_DestroyGear
    {
        public static bool Prefix(GameObject go)
        {
            if (!BetterFuelManagement.IsFuelContainer(go))
            {
                return true;
            }

            if (!BetterFuelManagement.WasCalledFromDeductLiquidFromInventory())
            {
                return true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "Enable")]
    public class Panel_Inventory_Examine_Enable
    {
        public static void Prefix(Panel_Inventory_Examine __instance, bool enable)
        {
            if (!enable)
            {
                return;
            }

            if (BetterFuelManagement.IsExaminingKeroseneLamp(__instance))
            {
                // repurpose the "Unload" button to "Drain"
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_Button_Unload, "GAMEPLAY_Drain");
                BetterFuelManagementUtils.SetButtonSprite(__instance.m_Button_Unload, "ico_lightSource_lantern");
            }
            else
            {
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_Button_Unload, "GAMEPLAY_Unload");
                BetterFuelManagementUtils.SetButtonSprite(__instance.m_Button_Unload, "ico_ammo_rifle");
            }
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "OnRefuel")]
    public class Panel_Inventory_Examine_OnRefuel
    {
        public static bool Prefix(Panel_Inventory_Examine __instance)
        {
            if (!BetterFuelManagement.IsExaminingKeroseneLamp(__instance))
            {
                return true;
            }

            if (!BetterFuelManagementUtils.IsSelected(__instance.m_Button_Unload))
            {
                return true;
            }

            KeroseneLampItem keroseneLamp = __instance.m_GearItem.m_KeroseneLampItem;
            if (keroseneLamp.m_CurrentFuelLiters == 0)
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Nofuelinlantern"));
                GameAudioManager.PlayGUIError();
                return false;
            }

            if (BetterFuelManagement.GetRemainingFuelCapacityInInventory() == 0)
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_NoFuelCapacityAvailable"));
                GameAudioManager.PlayGUIError();
                return false;
            }

            GameAudioManager.PlayGuiConfirm();
            InterfaceManager.m_Panel_GenericProgressBar.Launch(
                Localization.Get("GAMEPLAY_DrainingProgress"),
                keroseneLamp.m_RefuelTimeSeconds,
                0.0f,
                0.0f,
                keroseneLamp.m_RefuelAudio,
                null,
                false,
                true,
                new OnExitDelegate(Panel_Inventory_Examine_OnRefuel.OnDrainFinished));

            // HACK: somehow this is needed to revert the button text to "Refuel", which will be active when draining finishes
            BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_RefuelPanel.GetComponentInChildren<UIButton>(), "GAMEPLAY_Refuel");

            return false;
        }

        private static void OnDrainFinished(bool success, bool playerCancel, float progress)
        {
            Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

            KeroseneLampItem keroseneLamp = panel.m_GearItem.m_KeroseneLampItem;
            if (keroseneLamp != null)
            {
                float litersToDrain = BetterFuelManagement.GetLitersToDrain(panel) * progress;
                GameManager.GetPlayerManagerComponent().AddLiquidToInventory(litersToDrain, GearLiquidTypeEnum.Kerosene);
                keroseneLamp.m_CurrentFuelLiters -= litersToDrain;
            }

            panel.RefreshMainWindow();
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "RefreshMainWindow")]
    public class Panel_Inventory_Examine_RefreshMainWindow
    {
        public static void Postfix(Panel_Inventory_Examine __instance)
        {
            if (!BetterFuelManagement.IsExaminingKeroseneLamp(__instance))
            {
                return;
            }

            Vector3 position = BetterFuelManagementUtils.GetBottomPosition(
                __instance.m_Button_Harvest,
                __instance.m_Button_Refuel,
                __instance.m_Button_Repair);
            position.y += __instance.m_ButtonSpacing;
            __instance.m_Button_Unload.transform.localPosition = position;

            __instance.m_Button_Unload.gameObject.SetActive(true);

            float litersToDrain = BetterFuelManagement.GetLitersToDrain(__instance);
            __instance.m_Button_Unload.GetComponent<Panel_Inventory_Examine_MenuItem>().SetDisabled(litersToDrain < 0.001f);
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "SelectRefuelButton")]
    public class Panel_Inventory_Examine_SelectRefuelButton
    {
        public static void Prefix(Panel_Inventory_Examine __instance, bool selected)
        {
            if (!BetterFuelManagement.IsExaminingKeroseneLamp(__instance))
            {
                return;
            }

            if (selected)
            {
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_RefuelPanel.GetComponentInChildren<UIButton>(), "GAMEPLAY_Refuel");
            }
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "SelectUnloadButton")]
    public class Panel_Inventory_Examine_SelectUnloadButton
    {
        public static bool Prefix(Panel_Inventory_Examine __instance, bool selected)
        {
            if (!BetterFuelManagement.IsExaminingKeroseneLamp(__instance))
            {
                return true;
            }

            if (selected)
            {
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_RefuelPanel.GetComponentInChildren<UIButton>(), "GAMEPLAY_Drain");
            }

            __instance.m_RefuelPanel.SetActive(selected || BetterFuelManagementUtils.IsSelected(__instance.m_Button_Refuel));

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "AddLiquidToInventory")]
    public class PlayerManager_AddLiquidToInventory
    {
        public static void Postfix(PlayerManager __instance, float litersToAdd, GearLiquidTypeEnum liquidType, ref float __result)
        {
            if (liquidType == GearLiquidTypeEnum.Kerosene && __result != litersToAdd)
            {
                __instance.StartCoroutine(BetterFuelManagementUtils.SendDelayedLostMessage(litersToAdd - __result));

                // just pretend we added everything, so the original method will not generate new containers
                __result = litersToAdd;
            }
        }
    }

    internal class BetterFuelManagementUtils
    {
        internal static Vector3 GetBottomPosition(params Component[] components)
        {
            Vector3 result = new Vector3(0, 1000, 0);

            foreach (Component eachComponent in components)
            {
                if (eachComponent.gameObject.activeSelf && result.y > eachComponent.transform.localPosition.y)
                {
                    result = eachComponent.transform.localPosition;
                }
            }

            return result;
        }

        internal static bool IsSelected(UIButton button)
        {
            Panel_Inventory_Examine_MenuItem menuItem = button.GetComponent<Panel_Inventory_Examine_MenuItem>();
            if (menuItem == null)
            {
                return false;
            }

            System.Reflection.FieldInfo m_Selected = AccessTools.Field(menuItem.GetType(), "m_Selected");
            if (m_Selected == null)
            {
                return false;
            }

            return (bool)m_Selected.GetValue(menuItem);
        }

        internal static System.Collections.IEnumerator SendDelayedLostMessage(float amount)
        {
            yield return new WaitForSeconds(1f);

            GearMessage.AddMessage(
                "GEAR_JerrycanRusty",
                Localization.Get("GAMEPLAY_Lost"),
                " " + Localization.Get("GAMEPLAY_Kerosene") + " (" + Utils.GetLiquidQuantityStringWithUnitsNoOunces(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, amount) + ")",
                Color.red,
                false);
        }

        internal static void SetButtonLocalizationKey(UIButton button, string key)
        {
            if (button == null)
            {
                return;
            }

            bool wasActive = button.gameObject.activeSelf;
            button.gameObject.SetActive(false);

            UILocalize localize = button.GetComponentInChildren<UILocalize>();
            if (localize != null)
            {
                localize.key = key;
            }

            button.gameObject.SetActive(wasActive);
        }

        internal static void SetButtonSprite(UIButton button, string sprite)
        {
            if (button == null)
            {
                return;
            }

            button.normalSprite = sprite;
        }
    }
}