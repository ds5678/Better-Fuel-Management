﻿using Harmony;
using UnityEngine;

namespace BetterFuelManagement
{
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

            //Traverse field = Traverse.Create(menuItem).Field("m_Selected");
            //return field.FieldExists() && field.GetValue<bool>();
            return menuItem.m_Selected;
        }

        internal static System.Collections.IEnumerator SendDelayedLostMessage(float amount)
        {
            yield return new WaitForSeconds(1f);

            SendLostMessage(amount);
        }

        internal static void SendLostMessage(float amount)
        {
            GearMessage.AddMessage(
                "GEAR_JerrycanRusty",
                Localization.Get("GAMEPLAY_BFM_Lost"),
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

        internal static void SetTexture(Component component, Texture2D texture)
        {
            if (!component)
            {
                return;
            }

            UITexture uiTexture = component.GetComponent<UITexture>();
            if (!uiTexture)
            {
                return;
            }

            uiTexture.mainTexture = texture;
        }
    }

    //Register Localizations after Localization Initialized
    /*[HarmonyPatch(typeof(GameManager),"Update")]
    internal class LoadLocalizations
    {
        private static void Postfix()
        {
            if (!BetterFuelLocalizations.IsLoaded() && Localization.IsInitialized())
            {
                BetterFuelLocalizations.AddLocalizations();
            }
        }
    }*/

    [HarmonyPatch(typeof(ItemDescriptionPage), "CanExamine")]
    internal class ItemDescriptionPage_CanExamine
    {
        public static bool Prefix(GearItem gi, ref bool __result)
        {
            //Implementation.Log("ItemDescriptionPage - CanExamine");

            if (Implementation.IsFuelContainer(gi))
            {
                Implementation.SetConditionToMax(gi);
            }
            
            if (Implementation.IsFuelItem(gi))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "Enable")]
    internal class Panel_Inventory_Examine_Enable
    {
        public static void Prefix(Panel_Inventory_Examine __instance, bool enable)
        {
            //Implementation.Log("Panel_Inventory_Examine - Enable");
            if (!enable)
            {
                return;
            }

            if (Implementation.IsFuelItem(__instance.m_GearItem))
            {
                // repurpose the "Unload" button to "Drain"
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_Button_Unload, "GAMEPLAY_BFM_Drain");
                BetterFuelManagementUtils.SetButtonSprite(__instance.m_Button_Unload, "ico_lightSource_lantern");

                Transform lanternTexture = __instance.m_RefuelPanel.transform.Find("FuelDisplay/Lantern_Texture");
                BetterFuelManagementUtils.SetTexture(lanternTexture, Utils.GetInventoryIconTexture(__instance.m_GearItem));
            }
            else
            {
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_Button_Unload, "GAMEPLAY_Unload");
                BetterFuelManagementUtils.SetButtonSprite(__instance.m_Button_Unload, "ico_ammo_rifle");
            }
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "OnRefuel")]
    internal class Panel_Inventory_Examine_OnRefuel
    {
        public static bool Prefix(Panel_Inventory_Examine __instance)
        {
            //Implementation.Log("Panel_Inventory_Examine - OnRefuel");
            
            if (!Implementation.IsFuelItem(__instance.m_GearItem))
            {
                return true;
            }

            if (BetterFuelManagementUtils.IsSelected(__instance.m_Button_Unload))
            {
                Implementation.Drain(__instance.m_GearItem);
            }
            else
            {
                Implementation.Refuel(__instance.m_GearItem);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "OnUnload")]
    internal class Panel_Inventory_Examine_OnUnload
    {
        public static bool Prefix(Panel_Inventory_Examine __instance)
        {
            //Implementation.Log("Panel_Inventory_Examine - OnUnload");

            if (!Implementation.IsFuelItem(__instance.m_GearItem))
            {
                return true;
            }

            Implementation.Drain(__instance.m_GearItem);
            return false;
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "RefreshRefuelPanel")]
    internal class Panel_Inventory_Examine_RefreshFuelPanel
    {
        public static bool Prefix(Panel_Inventory_Examine __instance)
        {
            //Implementation.Log("Panel_Inventory_Examine - RefreshRefuelPanel");

            if (!Implementation.IsFuelItem(__instance.m_GearItem))
            {
                return true;
            }

            __instance.m_RefuelPanel.SetActive(false);
            __instance.m_Button_Refuel.gameObject.SetActive(true);

            float currentLiters = Implementation.GetCurrentLiters(__instance.m_GearItem);
            float capacityLiters = Implementation.GetCapacityLiters(__instance.m_GearItem);

            var fuelAvailable = Implementation.GetTotalCurrentLiters(__instance.m_GearItem) > Implementation.MIN_LITERS;
            bool canRefuel = !Mathf.Approximately(currentLiters, capacityLiters) && fuelAvailable;
            __instance.m_Refuel_X.gameObject.SetActive(!canRefuel);
            __instance.m_Button_Refuel.gameObject.GetComponent<Panel_Inventory_Examine_MenuItem>().SetDisabled(!canRefuel);

            __instance.m_MouseRefuelButton.SetActive(canRefuel);
            __instance.m_RequiresFuelMessage.SetActive(false);

            __instance.m_LanternFuelAmountLabel.text =
                Utils.GetLiquidQuantityString(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, Implementation.GetCurrentLiters(__instance.m_GearItem)) +
                "/" +
                Utils.GetLiquidQuantityStringWithUnitsNoOunces(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, Implementation.GetCapacityLiters(__instance.m_GearItem));

            float totalCurrent = Implementation.GetTotalCurrentLiters(__instance.m_GearItem);
            float totalCapacity = Implementation.GetTotalCapacityLiters(__instance.m_GearItem);
            __instance.m_FuelSupplyAmountLabel.text =
                Utils.GetLiquidQuantityString(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, totalCurrent) +
                "/" +
                Utils.GetLiquidQuantityStringWithUnitsNoOunces(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, totalCapacity);

            //Traverse method = Traverse.Create(__instance).Method("UpdateCondition");
            //if (method.MethodExists())
            //{
            //    method.GetValue();
            //}

            return false;
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "RefreshMainWindow")]
    internal class Panel_Inventory_Examine_RefreshMainWindow
    {
        public static void Postfix(Panel_Inventory_Examine __instance)
        {
            //Implementation.Log("Panel_Inventory_Examine - RefreshMainWindow");

            if (!Implementation.IsFuelItem(__instance.m_GearItem))
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

            float litersToDrain = Implementation.GetLitersToDrain(__instance.m_GearItem);
            __instance.m_Button_Unload.GetComponent<Panel_Inventory_Examine_MenuItem>().SetDisabled(litersToDrain < Implementation.MIN_LITERS);
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "SelectRefuelButton")] //inlined patch
    internal class Panel_Inventory_Examine_SelectRefuelButton
    {
        public static void Prefix(Panel_Inventory_Examine __instance, bool selected)
        {
            //Implementation.Log("Panel_Inventory_Examine - SelectRefuelButton");

            if (!Implementation.IsFuelItem(__instance.m_GearItem))
            {
                return;
            }

            if (selected)
            {
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_RefuelPanel.GetComponentInChildren<UIButton>(), "GAMEPLAY_Refuel");
            }
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "SelectUnloadButton")] //inlined patch
    internal class Panel_Inventory_Examine_SelectUnloadButton
    {
        public static bool Prefix(Panel_Inventory_Examine __instance, bool selected)
        {
            //Implementation.Log("Panel_Inventory_Examine - SelectUnloadButton");

            if (!Implementation.IsFuelItem(__instance.m_GearItem))
            {
                return true;
            }

            if (selected)
            {
                BetterFuelManagementUtils.SetButtonLocalizationKey(__instance.m_RefuelPanel.GetComponentInChildren<UIButton>(), "GAMEPLAY_BFM_Drain");
            }

            __instance.m_RefuelPanel.SetActive(selected || BetterFuelManagementUtils.IsSelected(__instance.m_Button_Refuel));

            return false;
        }
    }

    [HarmonyPatch(typeof(Panel_Inventory_Examine), "UpdateButtonLegend")]
    internal class Panel_Inventory_Examine_UpdateButtonLegend
    {
        public static void Postfix(Panel_Inventory_Examine __instance)
        {
            //Implementation.Log("Panel_Inventory_Examine - UpdateButtonLegend");

            if (Implementation.IsFuelItem(__instance.m_GearItem) && BetterFuelManagementUtils.IsSelected(__instance.m_Button_Unload))
            {
                __instance.m_ButtonLegendContainer.UpdateButton("Continue", "GAMEPLAY_BFM_Drain", true, 1, true);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "AddLiquidToInventory")]
    internal class PlayerManager_AddLiquidToInventory
    {
        public static void Postfix(PlayerManager __instance, float litersToAdd, GearLiquidTypeEnum liquidType, ref float __result)
        {
            //Implementation.Log("PlayerManager - AddLiquidToInventory");

            if (liquidType == GearLiquidTypeEnum.Kerosene && __result != litersToAdd)
            {
                //__instance.StartCoroutine(BetterFuelManagementUtils.SendDelayedLostMessage(litersToAdd - __result));
                BetterFuelManagementUtils.SendLostMessage(litersToAdd - __result);

                // just pretend we added everything, so the original method will not generate new containers
                __result = litersToAdd;
            }
        }
    }

    

    internal class DeductLiquidMethodTracker
    {
        internal static bool isExecuting = false;
    }

    [HarmonyPatch(typeof(PlayerManager), "DeductLiquidFromInventory")]
    internal class PlayerManager_DeductLiquidFromInventory
    {
        private static void Prefix()
        {
            DeductLiquidMethodTracker.isExecuting = true;
        }
        private static void Postfix()
        {
            DeductLiquidMethodTracker.isExecuting = false;
        }
    }

    [HarmonyPatch(typeof(Inventory),"DestroyGear")]
    internal class PreventLiquidItemDestruction
    {
        private static bool Prefix(GameObject go)
        {
            if (DeductLiquidMethodTracker.isExecuting)
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
