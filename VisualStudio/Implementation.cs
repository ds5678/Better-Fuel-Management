using MelonLoader;

namespace BetterFuelManagement;

internal sealed class Implementation : MelonMod
{
	public override void OnApplicationStart()
	{
		Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
		BetterFuelSettings.OnLoad();
		SpawnProbabilities.AddToModComponent();
	}

	internal static void Log(string message, params object[] parameters) => MelonLogger.Msg(message, parameters);
	internal static void LogWarning(string message, params object[] parameters) => MelonLogger.Warning(message, parameters);
	internal static void LogError(string message, params object[] parameters) => MelonLogger.Error(message, parameters);
}
