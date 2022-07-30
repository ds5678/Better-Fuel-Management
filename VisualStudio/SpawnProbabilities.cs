using GearSpawner;

namespace BetterFuelManagement;

internal static class SpawnProbabilities
{
	internal static void AddToModComponent()
	{
		SpawnTagManager.AddFunction("BetterFuelManagement", GetProbability);
	}
	
	private static float GetProbability(DifficultyLevel difficultyLevel, FirearmAvailability firearmAvailability, GearSpawnInfo gearSpawnInfo)
	{
		return difficultyLevel switch
		{
			DifficultyLevel.Pilgram => BetterFuelSettings.instance.pilgramSpawnExpectation / 70f * 100f,
			DifficultyLevel.Voyager => BetterFuelSettings.instance.voyagerSpawnExpectation / 70f * 100f,
			DifficultyLevel.Stalker => BetterFuelSettings.instance.stalkerSpawnExpectation / 70f * 100f,
			DifficultyLevel.Interloper => BetterFuelSettings.instance.interloperSpawnExpectation / 70f * 100f,
			DifficultyLevel.Challenge => BetterFuelSettings.instance.challengeSpawnExpectation / 70f * 100f,
			DifficultyLevel.Storymode => BetterFuelSettings.instance.storySpawnExpectation / 70f * 100f,
			_ => 0f,
		};
	}
}
