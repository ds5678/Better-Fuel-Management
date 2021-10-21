using GearSpawner;
using System;

namespace BetterFuelManagement
{
	internal static class SpawnProbabilities
	{
		internal static void AddToModComponent()
		{
			SpawnTagManager.AddToTaggedFunctions("BetterFuelManagement", new Func<DifficultyLevel, FirearmAvailability, GearSpawnInfo, float>(GetProbability));
		}
		private static float GetProbability(DifficultyLevel difficultyLevel, FirearmAvailability firearmAvailability, GearSpawnInfo gearSpawnInfo)
		{
			switch (difficultyLevel)
			{
				case DifficultyLevel.Pilgram:
					return Settings.options.pilgramSpawnExpectation / 70f * 100f;
				case DifficultyLevel.Voyager:
					return Settings.options.voyagerSpawnExpectation / 70f * 100f;
				case DifficultyLevel.Stalker:
					return Settings.options.stalkerSpawnExpectation / 70f * 100f;
				case DifficultyLevel.Interloper:
					return Settings.options.interloperSpawnExpectation / 70f * 100f;
				case DifficultyLevel.Challenge:
					return Settings.options.challengeSpawnExpectation / 70f * 100f;
				case DifficultyLevel.Storymode:
					return Settings.options.storySpawnExpectation / 70f * 100f;
				default:
					return 0f;
			}
		}
	}
}
