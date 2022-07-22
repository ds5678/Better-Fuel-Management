extern alias Hinterland;
using HarmonyLib;
using Hinterland;
using UnityEngine;

namespace BetterFuelManagement
{
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

	[HarmonyPatch(typeof(Panel_Inventory_Examine), "OnRefuel")]
	internal class Panel_Inventory_Examine_OnRefuel
	{
		private static bool Prefix(Panel_Inventory_Examine __instance)
		{
			//Implementation.Log("Panel_Inventory_Examine - OnRefuel");

			if (!FuelUtils.IsFuelItem(__instance.m_GearItem)) return true;

			if (ButtonUtils.IsSelected(__instance.m_Button_Unload)) FuelUtils.Drain(__instance.m_GearItem);
			else FuelUtils.Refuel(__instance.m_GearItem);

			return false;
		}
	}

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

	[HarmonyPatch(typeof(Panel_Inventory_Examine), "RefreshRefuelPanel")]
	internal class Panel_Inventory_Examine_RefreshFuelPanel
	{
		private static bool Prefix(Panel_Inventory_Examine __instance)
		{
			//Implementation.Log("Panel_Inventory_Examine - RefreshRefuelPanel");

			if (!FuelUtils.IsFuelItem(__instance.m_GearItem)) return true;

			__instance.m_RefuelPanel.SetActive(false);
			__instance.m_Button_Refuel.gameObject.SetActive(true);

			float currentLiters = FuelUtils.GetIndividualCurrentLiters(__instance.m_GearItem);
			float capacityLiters = FuelUtils.GetIndividualCapacityLiters(__instance.m_GearItem);
			float totalCurrent = FuelUtils.GetTotalCurrentLiters(__instance.m_GearItem);
			float totalCapacity = FuelUtils.GetTotalCapacityLiters(__instance.m_GearItem);

			bool fuelIsAvailable = totalCurrent > FuelUtils.MIN_LITERS;
			bool canRefuel = fuelIsAvailable && !Mathf.Approximately(currentLiters, capacityLiters);

			__instance.m_Refuel_X.gameObject.SetActive(!canRefuel);
			__instance.m_Button_Refuel.gameObject.GetComponent<Panel_Inventory_Examine_MenuItem>().SetDisabled(!canRefuel);

			__instance.m_MouseRefuelButton.SetActive(canRefuel);
			__instance.m_RequiresFuelMessage.SetActive(false);

			__instance.m_LanternFuelAmountLabel.text =
				FuelUtils.GetLiquidQuantityStringNoOunces(currentLiters) +
				"/" +
				FuelUtils.GetLiquidQuantityStringNoOunces(capacityLiters);

			__instance.m_FuelSupplyAmountLabel.text =
				FuelUtils.GetLiquidQuantityStringNoOunces(totalCurrent) +
				"/" +
				FuelUtils.GetLiquidQuantityStringNoOunces(totalCapacity);

			__instance.UpdateWeightAndConditionLabels();

			return false;
		}
	}

	[HarmonyPatch(typeof(Panel_Inventory_Examine), "RefreshMainWindow")]
	internal class Panel_Inventory_Examine_RefreshMainWindow
	{
		private static void Postfix(Panel_Inventory_Examine __instance)
		{
			//Implementation.Log("Panel_Inventory_Examine - RefreshMainWindow");

			if (!FuelUtils.IsFuelItem(__instance.m_GearItem)) return;

			Vector3 position = ButtonUtils.GetBottomPosition(
				__instance.m_Button_Harvest,
				__instance.m_Button_Refuel,
				__instance.m_Button_Repair);
			position.y += __instance.m_ButtonSpacing;
			__instance.m_Button_Unload.transform.localPosition = position;

			__instance.m_Button_Unload.gameObject.SetActive(true);

			float litersToDrain = FuelUtils.GetLitersToDrain(__instance.m_GearItem);
			__instance.m_Button_Unload.GetComponent<Panel_Inventory_Examine_MenuItem>().SetDisabled(litersToDrain < FuelUtils.MIN_LITERS);
		}
	}

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



	internal static class DeductLiquidMethodTracker
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

	[HarmonyPatch(typeof(Inventory), "DestroyGear")]
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
