extern alias Hinterland;
using Hinterland;
using UnityEngine;

namespace BetterFuelManagement
{
	internal static class FuelUtils
	{
		public const float MIN_LITERS = 0.001f;
		private const string REFUEL_AUDIO = "Play_SndActionRefuelLantern";
		private const float REFUEL_TIME = 3;

		private static void AddLiters(GearItem gearItem, float liters)
		{
			if (IsKeroseneLamp(gearItem)) gearItem.m_KeroseneLampItem.m_CurrentFuelLiters += liters;
			else if (IsFuelContainer(gearItem)) gearItem.m_LiquidItem.m_LiquidLiters += liters;
		}

		private static void AddTotalCurrentLiters(float liters, GearItem excludeItem)
		{
			float remaining = liters;

			foreach (GameObject eachItem in GameManager.GetInventoryComponent().m_Items)
			{
				GearItem gearItem = eachItem.GetComponent<GearItem>();
				if (gearItem == null || gearItem == excludeItem) continue;

				LiquidItem liquidItem = gearItem.m_LiquidItem;
				if (liquidItem == null || liquidItem.m_LiquidType != GearLiquidTypeEnum.Kerosene) continue;

				float previousLiters = liquidItem.m_LiquidLiters;
				liquidItem.m_LiquidLiters = Mathf.Clamp(liquidItem.m_LiquidLiters + remaining, 0, liquidItem.m_LiquidCapacityLiters);
				float transferred = liquidItem.m_LiquidLiters - previousLiters;

				remaining -= transferred;

				if (Mathf.Abs(remaining) < MIN_LITERS) break;
			}
		}

		internal static void Drain(GearItem gearItem)
		{
			Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

			float currentLiters = GetIndividualCurrentLiters(panel.m_GearItem);
			if (currentLiters < MIN_LITERS)
			{
				HUDMessage.AddMessage(Localization.Get("GAMEPLAY_BFM_AlreadyEmpty"));
				GameAudioManager.PlayGUIError();
				return;
			}

			float totalCapacity = GetTotalCapacityLiters(panel.m_GearItem);
			float totalCurrent = GetTotalCurrentLiters(panel.m_GearItem);
			if (Mathf.Approximately(totalCapacity, totalCurrent))
			{
				HUDMessage.AddMessage(Localization.Get("GAMEPLAY_BFM_NoFuelCapacityAvailable"));
				GameAudioManager.PlayGUIError();
				return;
			}

			GameAudioManager.PlayGuiConfirm();
			InterfaceManager.m_Panel_GenericProgressBar.Launch(
				Localization.Get("GAMEPLAY_BFM_DrainingProgress"),
				REFUEL_TIME,
				0,
				0,
				REFUEL_AUDIO,
				null,
				false,
				true,
				new System.Action<bool, bool, float>(OnDrainFinished));

			// HACK: somehow this is needed to revert the button text to "Refuel", which will be active when draining finishes
			ButtonUtils.SetButtonLocalizationKey(panel.m_RefuelPanel.GetComponentInChildren<UIButton>(), "GAMEPLAY_Refuel");
		}

		/// <summary>
		/// Returns a liquid quantity string with respect to the game units;
		/// </summary>
		internal static string GetLiquidQuantityString(float quantityLiters)
		{
			return Utils.GetLiquidQuantityString(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, quantityLiters);
		}

		/// <summary>
		/// Returns a liquid quantity string with respect to the game units;
		/// </summary>
		internal static string GetLiquidQuantityStringNoOunces(float quantityLiters)
		{
			return Utils.GetLiquidQuantityStringNoOunces(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, quantityLiters);
		}

		/// <summary>
		/// Returns a liquid quantity string with respect to the game units;
		/// </summary>
		internal static string GetLiquidQuantityStringWithUnits(float quantityLiters)
		{
			return Utils.GetLiquidQuantityStringWithUnits(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, quantityLiters);
		}

		/// <summary>
		/// Returns a liquid quantity string with respect to the game units;
		/// </summary>
		internal static string GetLiquidQuantityStringWithUnitsNoOunces(float quantityLiters)
		{
			return Utils.GetLiquidQuantityStringWithUnitsNoOunces(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, quantityLiters);
		}

		internal static float GetLitersToDrain(GearItem gearItem)
		{
			return Mathf.Min(
				GetIndividualCurrentLiters(gearItem), //available fuel
				GetTotalSpaceLiters(gearItem)); //available capacity
		}

		internal static float GetLitersToRefuel(GearItem gearItem)
		{
			return Mathf.Min(
				GetIndividualSpaceLiters(gearItem), //amount of space in the fuel container
				GetTotalCurrentLiters(gearItem)); //current amount of kerosene in other containers
		}

		/// <summary>
		/// Returns the current capacity (in liters) of kerosene for the gear item.
		/// </summary>
		internal static float GetIndividualCapacityLiters(GearItem gearItem)
		{
			if (IsFuelContainer(gearItem)) return gearItem.m_LiquidItem.m_LiquidCapacityLiters;
			else if (IsKeroseneLamp(gearItem)) return gearItem.m_KeroseneLampItem.m_MaxFuelLiters;
			else return 0;
		}

		/// <summary>
		/// Returns the current amount (in liters) of kerosene in the gear item.
		/// </summary>
		internal static float GetIndividualCurrentLiters(GearItem gearItem)
		{
			if (IsFuelContainer(gearItem)) return gearItem.m_LiquidItem.m_LiquidLiters;
			else if (IsKeroseneLamp(gearItem)) return gearItem.m_KeroseneLampItem.m_CurrentFuelLiters;
			else return 0;
		}

		/// <summary>
		/// Get the amount of space in the fuel container.
		/// </summary>
		/// <param name="gearItem">The fuel container being investigated.</param>
		/// <returns>The amount (in liters) of empty space in the fuel container.</returns>
		internal static float GetIndividualSpaceLiters(GearItem gearItem)
		{
			return GetIndividualCapacityLiters(gearItem) - GetIndividualCurrentLiters(gearItem);
		}

		/// <summary>
		/// Get the total capacity of all other fuel containers in the inventory.
		/// </summary>
		/// <param name="excludeItem">The gear item to be excluded from the calculations.</param>
		/// <returns>The total capacity (in liters) from inventory fuel containers.</returns>
		internal static float GetTotalCapacityLiters(GearItem excludeItem)
		{
			float result = 0;

			foreach (GameObject eachItem in GameManager.GetInventoryComponent().m_Items)
			{
				GearItem? gearItem = eachItem?.GetComponent<GearItem>();
				if (gearItem == null || gearItem == excludeItem || !IsFuelContainer(gearItem))
				{
					continue;
				}

				result += GetIndividualCapacityLiters(gearItem);
			}

			return result;
		}

		/// <summary>
		/// Get the total kerosene quantity of all other fuel containers in the inventory.
		/// </summary>
		/// <param name="excludeItem">The gear item to be excluded from the calculations.</param>
		/// <returns>The total kerosene quantity (in liters) from other inventory fuel containers.</returns>
		internal static float GetTotalCurrentLiters(GearItem excludeItem)
		{
			float result = 0;

			foreach (GameObject eachItem in GameManager.GetInventoryComponent().m_Items)
			{
				GearItem gearItem = eachItem.GetComponent<GearItem>();
				if (gearItem == null || gearItem == excludeItem || !IsFuelContainer(gearItem)) continue;

				result += GetIndividualCurrentLiters(gearItem);
			}

			return result;
		}

		/// <summary>
		/// Get the total empty space of all other fuel containers in the inventory.
		/// </summary>
		/// <param name="excludeItem">The gear item to be excluded from the calculations.</param>
		/// <returns>The total empty space (in liters) from other inventory fuel containers.</returns>
		internal static float GetTotalSpaceLiters(GearItem excludeItem)
		{
			float result = 0;

			foreach (GameObject eachItem in GameManager.GetInventoryComponent().m_Items)
			{
				GearItem gearItem = eachItem.GetComponent<GearItem>();
				if (gearItem != null && gearItem != excludeItem && IsFuelContainer(gearItem))
				{
					result += GetIndividualSpaceLiters(gearItem);
				}
			}

			return result;
		}

		/// <summary>
		/// Is the gear item purely a container for kerosene?
		/// </summary>
		/// <returns>True if gearItem.m_LiquidItem is not null and is for kerosene.</returns>
		internal static bool IsFuelContainer(GearItem gearItem)
		{
			return gearItem?.m_LiquidItem != null && gearItem.m_LiquidItem.m_LiquidType == GearLiquidTypeEnum.Kerosene;
		}

		/// <summary>
		/// Can the gear item hold kerosene?
		/// </summary>
		/// <returns>True if the gear item is a fuel container or is a kerosene lamp.</returns>
		internal static bool IsFuelItem(GearItem gearItem)
		{
			return IsFuelContainer(gearItem) || IsKeroseneLamp(gearItem);
		}

		/// <summary>
		/// Is the gear item a kerosene lamp?
		/// </summary>
		/// <returns>True if gearItem.m_KeroseneLampItem is not null.</returns>
		internal static bool IsKeroseneLamp(GearItem gearItem)
		{
			return gearItem?.m_KeroseneLampItem != null;
		}

		internal static void Refuel(GearItem gearItem)
		{
			Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

			float currentLiters = GetIndividualCurrentLiters(panel.m_GearItem);
			float capacityLiters = GetIndividualCapacityLiters(panel.m_GearItem);
			if (Mathf.Approximately(currentLiters, capacityLiters))
			{
				GameAudioManager.PlayGUIError();
				HUDMessage.AddMessage(Localization.Get("GAMEPLAY_BFM_AlreadyFilled"), false);
				return;
			}

			float totalCurrent = GetTotalCurrentLiters(panel.m_GearItem);
			if (totalCurrent < MIN_LITERS)
			{
				GameAudioManager.PlayGUIError();
				HUDMessage.AddMessage(Localization.Get("GAMEPLAY_NoKeroseneavailable"), false);
				return;
			}

			GameAudioManager.PlayGuiConfirm();
			InterfaceManager.m_Panel_GenericProgressBar.Launch(
				Localization.Get("GAMEPLAY_RefuelingProgress"),
				REFUEL_TIME,
				0,
				0,
				REFUEL_AUDIO,
				null,
				false,
				true,
				new System.Action<bool, bool, float>(OnRefuelFinished));
		}

		private static void OnDrainFinished(bool success, bool playerCancel, float progress)
		{
			Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

			if (IsFuelItem(panel.m_GearItem))
			{
				float litersToDrain = GetLitersToDrain(panel.m_GearItem) * progress;
				AddTotalCurrentLiters(litersToDrain, panel.m_GearItem);
				AddLiters(panel.m_GearItem, -litersToDrain);
			}

			panel.RefreshMainWindow();
		}

		private static void OnRefuelFinished(bool success, bool playerCancel, float progress)
		{
			Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

			if (IsFuelItem(panel.m_GearItem))
			{
				float litersToTransfer = GetLitersToRefuel(panel.m_GearItem) * progress;
				AddTotalCurrentLiters(-litersToTransfer, panel.m_GearItem);
				AddLiters(panel.m_GearItem, litersToTransfer);
			}

			panel.RefreshMainWindow();
		}

		public static void SetConditionToMax(GearItem gearItem)
		{
			if (gearItem.m_CurrentHP != gearItem.m_MaxHP)
			{
				gearItem.m_CurrentHP = gearItem.m_MaxHP;
			}
		}
	}
}
