using ModSettings;
using RadialMenuUtilities;
using System.Reflection;

namespace BetterFuelManagement;

internal sealed class BetterFuelSettings : JsonModSettings
{
	internal static readonly BetterFuelSettings instance = new BetterFuelSettings();
	internal static CustomRadialMenu? radialMenu;
	
	[Section("Gameplay Settings")]
	[Name("Use Radial Menu")]
	[Description("Enables a new radial menu for you to easily access your fuel containers.")]
	public bool enableRadial = false;

	[Name("Key for Radial Menu")]
	[Description("The key you press to show the new menu.")]
	public KeyCode keyCode = KeyCode.G;

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
		if (field.Name == nameof(enableRadial))
		{
			SetFieldsVisibility((bool)newValue);
		}
	}

	protected override void OnConfirm()
	{
		base.OnConfirm();
		radialMenu!.SetValues(keyCode, enableRadial);
	}

	private void SetFieldsVisibility(bool visible)
	{
		FieldInfo[] fields = typeof(BetterFuelSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);

		for (int i = 0; i < fields.Length; ++i)
		{
			if (fields[i].Name == nameof(keyCode))
			{
				SetFieldVisible(fields[i], visible);
			}
		}
	}

	public static void OnLoad()
	{
		instance.AddToModSettings("Better Fuel Management");
		instance.SetFieldsVisibility(instance.enableRadial);
		radialMenu = new CustomRadialMenu(instance.keyCode, CustomRadialMenuType.AllOfEach, new string[] { "GEAR_JerrycanRusty", "GEAR_LampFuel", "GEAR_LampFuelFull" }, instance.enableRadial);
	}
}
