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
    internal class BetterFuelSettings : JsonModSettings
    {
        [Section("Gameplay Settings")]
        [Name("Use Radial Menu")]
        [Description("Enables a new radial menu for you to easily access your fuel containers.")]
        public bool enableRadial = false;

        [Name("Key for Radial Menu")]
        [Description("The key you press to show the new menu.")]
        public KeyCodeAlphabet keyCodeAlphabet = KeyCodeAlphabet.G;

        [Section("Spawn Settings")]
        [Name("Pilgram / Very High Loot Custom")]
        [Description("The expected number of times a gas can will randomly spawn in the world based on statistics. Setting to zero disables them on this game mode.  Recommended is 40.")]
        [Slider(0f, 50f, 101)]
        public float pilgramSpawnExpectation = 40f;

        [Name("Voyager / High Loot Custom")]
        [Description("The expected number of times a gas can will randomly spawn in the world based on statistics. Setting to zero disables them on this game mode.  Recommended is 30.")]
        [Slider(0f, 50f, 101)]
        public float voyagerSpawnExpectation = 30f;

        [Name("Stalker / Medium Loot Custom")]
        [Description("The expected number of times a gas can will randomly spawn in the world based on statistics. Setting to zero disables them on this game mode.  Recommended is 15.")]
        [Slider(0f, 50f, 101)]
        public float stalkerSpawnExpectation = 15f;

        [Name("Interloper / Low Loot Custom")]
        [Description("The expected number of times a gas can will randomly spawn in the world based on statistics. Setting to zero disables them on this game mode.  Recommended is 5.")]
        [Slider(0f, 50f, 101)]
        public float interloperSpawnExpectation = 5f;

        [Name("Wintermute")]
        [Description("The expected number of times a gas can will randomly spawn in the world based on statistics. Setting to zero disables them on this game mode.  Recommended is 40.")]
        [Slider(0f, 50f, 101)]
        public float storySpawnExpectation = 40f;

        [Name("Challenges")]
        [Description("The expected number of times a gas can will randomly spawn in the world based on statistics. Setting to zero disables them on this game mode.  Recommended is 40.")]
        [Slider(0f, 50f, 101)]
        public float challengeSpawnExpectation = 40f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if(field.Name == nameof(enableRadial))
            {
                Settings.SetFieldVisible((bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            base.OnConfirm();
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeAlphabet.ToString());
            Settings.radialMenu.SetValues(keyCode,enableRadial);
        }
    }
    
    internal static class Settings
    {
        internal static readonly BetterFuelSettings options = new BetterFuelSettings();
        internal static CustomRadialMenu radialMenu;

        public static void OnLoad()
        {
            options.AddToModSettings("Better Fuel Management");
            SetFieldVisible(options.enableRadial);
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), options.keyCodeAlphabet.ToString());
            radialMenu = new CustomRadialMenu(keyCode, CustomRadialMenuType.AllOfEach, new string[] { "GEAR_JerrycanRusty", "GEAR_LampFuel", "GEAR_LampFuelFull" }, options.enableRadial);
        }
        internal static void SetFieldVisible(bool visible)
        {
            FieldInfo[] fields = options.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < fields.Length; ++i)
            {
                if(fields[i].Name == nameof(options.keyCodeAlphabet))
                {
                    options.SetFieldVisible(fields[i], visible);
                }
            }
        }
    }
}
