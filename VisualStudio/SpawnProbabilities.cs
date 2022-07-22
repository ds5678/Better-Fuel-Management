using GearSpawner;
using System;

namespace BetterFuelManagement;

internal static class SpawnProbabilities
{
	internal static void AddToModComponent()
	{
		SpawnTagManager.AddToTaggedFunctions("BetterFuelManagement", new Func<DifficultyLevel, FirearmAvailability, GearSpawnInfo, float>(GetProbability));
	}
	
	private static float GetProbability(DifficultyLevel difficultyLevel, FirearmAvailability firearmAvailability, GearSpawnInfo gearSpawnInfo)
	{
		return difficultyLevel switch
		{
			DifficultyLevel.Pilgram => Settings.options.pilgramSpawnExpectation / 70f * 100f,
			DifficultyLevel.Voyager => Settings.options.voyagerSpawnExpectation / 70f * 100f,
			DifficultyLevel.Stalker => Settings.options.stalkerSpawnExpectation / 70f * 100f,
			DifficultyLevel.Interloper => Settings.options.interloperSpawnExpectation / 70f * 100f,
			DifficultyLevel.Challenge => Settings.options.challengeSpawnExpectation / 70f * 100f,
			DifficultyLevel.Storymode => Settings.options.storySpawnExpectation / 70f * 100f,
			_ => 0f,
		};
	}
}
