extern alias Hinterland;
using Hinterland;
using UnityEngine;

namespace BetterFuelManagement
{
	internal static class MessageUtils
	{
		internal static void SendLostMessageDelayed(float amount)
		{
			MelonLoader.MelonCoroutines.Start(SendDelayedLostMessageIEnumerator(amount));
		}

		private static System.Collections.IEnumerator SendDelayedLostMessageIEnumerator(float amount)
		{
			yield return new WaitForSeconds(1f);

			SendLostMessageImmediate(amount);
		}

		internal static void SendLostMessageImmediate(float amount)
		{
			GearMessage.AddMessage(
				"GEAR_JerrycanRusty",
				Localization.Get("GAMEPLAY_BFM_Lost"),
				" " + Localization.Get("GAMEPLAY_Kerosene") + " (" + Utils.GetLiquidQuantityStringWithUnitsNoOunces(InterfaceManager.m_Panel_OptionsMenu.m_State.m_Units, amount) + ")",
				Color.red,
				false);
		}
	}
}
