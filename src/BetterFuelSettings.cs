using ModSettings;
using System;
using UnityEngine;
using RadialMenuUtilities;
using System.Reflection;

namespace BetterFuelManagement
{
    internal enum KeyCodeAlphabet
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z
    }
    internal class Settings : JsonModSettings
    {
        [Name("Use Radial Menu")]
        [Description("Enables a new radial menu for you to easily access your fuel containers.")]
        public bool enableRadial = true;

        [Name("Key for Radial Menu")]
        [Description("The key you press to show the new menu.")]
        public KeyCodeAlphabet keyCodeAlphabet = KeyCodeAlphabet.G;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if(field.Name == nameof(enableRadial))
            {
                BetterFuelSettings.SetFieldVisible((bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            base.OnConfirm();
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeAlphabet.ToString());
            BetterFuelSettings.radialMenu.SetValues(keyCode,enableRadial);
        }
    }
    
    internal static class BetterFuelSettings
    {
        internal static readonly Settings settings = new Settings();
        internal static CustomRadialMenu radialMenu;

        public static void OnLoad()
        {
            settings.AddToModSettings("Better Fuel Management");
            SetFieldVisible(settings.enableRadial);
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), settings.keyCodeAlphabet.ToString());
            radialMenu = new CustomRadialMenu(keyCode, CustomRadialMenuType.AllOfEach, new string[] { "GEAR_JerrycanRusty", "GEAR_LampFuel", "GEAR_LampFuelFull" }, settings.enableRadial);
        }
        internal static void SetFieldVisible(bool visible)
        {
            FieldInfo[] fields = settings.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < fields.Length; ++i)
            {
                if(fields[i].Name == nameof(settings.keyCodeAlphabet))
                {
                    settings.SetFieldVisible(fields[i], visible);
                }
            }
        }
    }
}
