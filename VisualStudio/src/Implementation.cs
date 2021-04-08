using UnityEngine;
using MelonLoader;

namespace BetterFuelManagement
{
    internal class Implementation : MelonMod
    {
        public const float MIN_LITERS = 0.001f;

        private const string NAME = "Durable-Whetstone";
        private const string REFUEL_AUDIO = "Play_SndActionRefuelLantern";
        private const float REFUEL_TIME = 3;

        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
            BetterFuelSettings.OnLoad();
        }

        internal static void AddLiters(GearItem gearItem, float liters)
        {
            if (gearItem && gearItem.m_KeroseneLampItem)
            {
                gearItem.m_KeroseneLampItem.m_CurrentFuelLiters += liters;
                return;
            }

            if (gearItem && gearItem.m_LiquidItem)
            {
                gearItem.m_LiquidItem.m_LiquidLiters += liters;
                return;
            }
        }

        internal static void AddTotalCurrentLiters(float liters, GearItem excludeItem)
        {
            float remaining = liters;

            foreach (GameObject eachItem in GameManager.GetInventoryComponent().m_Items)
            {
                GearItem gearItem = eachItem.GetComponent<GearItem>();
                if (gearItem == null || gearItem == excludeItem)
                {
                    continue;
                }

                LiquidItem liquidItem = gearItem.m_LiquidItem;
                if (liquidItem == null || liquidItem.m_LiquidType != GearLiquidTypeEnum.Kerosene)
                {
                    continue;
                }

                float previousLiters = liquidItem.m_LiquidLiters;
                liquidItem.m_LiquidLiters = Mathf.Clamp(liquidItem.m_LiquidLiters + remaining, 0, liquidItem.m_LiquidCapacityLiters);
                float transferred = liquidItem.m_LiquidLiters - previousLiters;

                remaining -= transferred;

                if (Mathf.Abs(remaining) < MIN_LITERS)
                {
                    break;
                }
            }
        }

        internal static void Drain(GearItem gearItem)
        {
            Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

            float currentLiters = GetCurrentLiters(panel.m_GearItem);
            if (currentLiters < MIN_LITERS)
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_AlreadyEmpty"));
                GameAudioManager.PlayGUIError();
                return;
            }

            float totalCapacity = GetTotalCapacityLiters(panel.m_GearItem);
            float totalCurrent = GetTotalCurrentLiters(panel.m_GearItem);
            if (Mathf.Approximately(totalCapacity, totalCurrent))
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_NoFuelCapacityAvailable"));
                GameAudioManager.PlayGUIError();
                return;
            }

            GameAudioManager.PlayGuiConfirm();
            InterfaceManager.m_Panel_GenericProgressBar.Launch(
                Localization.Get("GAMEPLAY_DrainingProgress"),
                REFUEL_TIME,
                0,
                0,
                REFUEL_AUDIO,
                null,
                false,
                true,
                //new OnExitDelegate(OnDrainFinished));
                new System.Action<bool, bool, float>(OnDrainFinished));

            // HACK: somehow this is needed to revert the button text to "Refuel", which will be active when draining finishes
            BetterFuelManagementUtils.SetButtonLocalizationKey(panel.m_RefuelPanel.GetComponentInChildren<UIButton>(), "GAMEPLAY_Refuel");
        }

        internal static float GetCapacityLiters(GearItem gearItem)
        {
            if (gearItem && gearItem.m_LiquidItem)
            {
                return gearItem.m_LiquidItem.m_LiquidCapacityLiters;
            }

            if (gearItem && gearItem.m_KeroseneLampItem)
            {
                return gearItem.m_KeroseneLampItem.m_MaxFuelLiters;
            }

            return 0;
        }

        internal static float GetCurrentLiters(GearItem gearItem)
        {
            if (gearItem && gearItem.m_LiquidItem)
            {
                return gearItem.m_LiquidItem.m_LiquidLiters;
            }

            if (gearItem && gearItem.m_KeroseneLampItem)
            {
                return gearItem.m_KeroseneLampItem.m_CurrentFuelLiters;
            }

            return 0;
        }

        internal static float GetLitersToDrain(GearItem gearItem)
        {
            float availableFuel = GetCurrentLiters(gearItem);
            float availableCapacity = GetTotalCapacityLiters(gearItem) - GetTotalCurrentLiters(gearItem);

            return Mathf.Min(availableFuel, availableCapacity);
        }

        internal static float GetLitersToRefuel(GearItem gearItem)
        {
            float currentLiters = Implementation.GetCurrentLiters(gearItem);
            float capacityLiters = Implementation.GetCapacityLiters(gearItem);
            float totalCurrent = Implementation.GetTotalCurrentLiters(gearItem);

            return Mathf.Min(capacityLiters - currentLiters, totalCurrent);
        }

        internal static float GetTotalCapacityLiters(GearItem excludeItem)
        {
            float result = 0;

            foreach (GameObject eachItem in GameManager.GetInventoryComponent().m_Items)
            {
                GearItem gearItem = eachItem.GetComponent<GearItem>();
                if (gearItem == null || gearItem == excludeItem)
                {
                    continue;
                }

                LiquidItem liquidItem = gearItem.m_LiquidItem;
                if (liquidItem == null || liquidItem.m_LiquidType != GearLiquidTypeEnum.Kerosene)
                {
                    continue;
                }

                result += liquidItem.m_LiquidCapacityLiters;
            }

            return result;
        }

        internal static float GetTotalCurrentLiters(GearItem excludeItem)
        {
            float result = 0;

            foreach (GameObject eachItem in GameManager.GetInventoryComponent().m_Items)
            {
                GearItem gearItem = eachItem.GetComponent<GearItem>();
                if (gearItem == null || gearItem == excludeItem)
                {
                    continue;
                }

                LiquidItem liquidItem = gearItem.m_LiquidItem;
                if (liquidItem == null || liquidItem.m_LiquidType != GearLiquidTypeEnum.Kerosene)
                {
                    continue;
                }

                result += liquidItem.m_LiquidLiters;
            }

            return result;
        }

        internal static bool IsFuelContainer(GearItem gearItem)
        {
            if (gearItem == null || gearItem.m_LiquidItem == null)
            {
                return false;
            }

            return gearItem.m_LiquidItem.m_LiquidType == GearLiquidTypeEnum.Kerosene;
        }

        internal static bool IsFuelItem(GearItem gearItem)
        {
            return IsFuelContainer(gearItem) || IsKeroseneLamp(gearItem);
        }

        internal static bool IsKeroseneLamp(GearItem gearItem)
        {
            return gearItem && gearItem.m_KeroseneLampItem;
        }

        internal static void Log(string message)
        {
            MelonLogger.Log(message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format(message , parameters);
            Log(preformattedMessage);
        }

        internal static void Refuel(GearItem gearItem)
        {
            Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

            float currentLiters = GetCurrentLiters(panel.m_GearItem);
            float capacityLiters = GetCapacityLiters(panel.m_GearItem);
            if (Mathf.Approximately(currentLiters, capacityLiters))
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_AlreadyFilled"), false);
                return;
            }

            float totalCurrent = GetTotalCurrentLiters(panel.m_GearItem);
            if (totalCurrent < Implementation.MIN_LITERS)
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
                //new OnExitDelegate(OnRefuelFinished));
                new System.Action<bool,bool,float>(OnRefuelFinished));
        }

        

        private static void OnDrainFinished(bool success, bool playerCancel, float progress)
        {
            Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

            if (Implementation.IsFuelItem(panel.m_GearItem))
            {
                float litersToDrain = Implementation.GetLitersToDrain(panel.m_GearItem) * progress;
                Implementation.AddTotalCurrentLiters(litersToDrain, panel.m_GearItem);
                Implementation.AddLiters(panel.m_GearItem, -litersToDrain);
            }

            panel.RefreshMainWindow();
        }

        private static void OnRefuelFinished(bool success, bool playerCancel, float progress)
        {
            Panel_Inventory_Examine panel = InterfaceManager.m_Panel_Inventory_Examine;

            if (Implementation.IsFuelItem(panel.m_GearItem))
            {
                float litersToTransfer = Implementation.GetLitersToRefuel(panel.m_GearItem) * progress;
                Implementation.AddTotalCurrentLiters(-litersToTransfer, panel.m_GearItem);
                Implementation.AddLiters(panel.m_GearItem, litersToTransfer);
            }

            panel.RefreshMainWindow();
        }

        public static void SetConditionToMax(GearItem gearItem)
        {
            if(gearItem.m_CurrentHP != gearItem.m_MaxHP)
            {
                gearItem.m_CurrentHP = gearItem.m_MaxHP;
            }
        }
    }
}
