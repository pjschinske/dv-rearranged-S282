using System;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace RearrangedS282
{
	public static class Main
	{
		public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
		public static string ModPath { get; private set; }
		public static Settings settings { get; private set; }

		public static bool IsGaugeModInstalled { get; private set; }

		// Unity Mod Manage Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Logger = modEntry.Logger;
			ModPath = modEntry.Path;
			Harmony harmony = null;

			settings = Settings.Load<Settings>(modEntry);
			settings.OnChange();
			modEntry.OnGUI = OnGUI;
			modEntry.OnSaveGUI = OnSaveGUI;

			IsGaugeModInstalled = UnityModManager.FindMod("Gauge") is not null;

			try
			{
				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll();

			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				//harmony?.UnpatchAll();
				return false;
			}

			return true;
		}

		static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			settings.Draw(modEntry);
			//GUILayout.BeginVertical();
			//settings.spawnRandomWA = GUILayout.Toggle(settings.spawnRandomWA, "Spawn S282 with random wheel arrangement");

		}

		static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			settings.Save(modEntry);
		}
	}
}
