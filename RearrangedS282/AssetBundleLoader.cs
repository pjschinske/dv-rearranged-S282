using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RearrangedS282
{
	internal class AssetBundleLoader
	{
		private static GameObject branchPipe;
		private static GameObject branchPipe4444;
		private static GameObject franklinBValveGear;
		private static GameObject franklinBValveGear4444;
		private static GameObject franklinBReverseGear;
		private static GameObject franklinBReverseGear4444;
		private static GameObject franklinBReverseGearRear;
		private static RuntimeAnimatorController franklinBAnimCtrl;
		private static RuntimeAnimatorController franklinBAnimCtrl4444;

		public static GameObject BranchPipe
		{
			get
			{
				if (branchPipe is null)
				{
					loadAssets();
				}
				return branchPipe;
			}
			private set { }
		}

		public static GameObject BranchPipe4444
		{
			get
			{
				if (branchPipe4444 is null)
				{
					loadAssets();
				}
				return branchPipe4444;
			}
			private set { }
		}

		public static GameObject FranklinBValveGear
		{
			get
			{
				if (franklinBValveGear is null)
				{
					loadAssets();
				}
				return franklinBValveGear;
			}
			private set { }
		}

		public static GameObject FranklinBValveGear4444
		{
			get
			{
				if (franklinBValveGear4444 is null)
				{
					loadAssets();
				}
				return franklinBValveGear4444;
			}
			private set { }
		}

		public static GameObject FranklinBReverseGear
		{
			get
			{
				if (franklinBReverseGear is null)
				{
					loadAssets();
				}
				return franklinBReverseGear;
			}
			private set { }
		}

		public static GameObject FranklinBReverseGear4444
		{
			get
			{
				if (franklinBReverseGear4444 is null)
				{
					loadAssets();
				}
				return franklinBReverseGear4444;
			}
			private set { }
		}

		public static GameObject FranklinBReverseGearRear
		{
			get
			{
				if (franklinBReverseGearRear is null)
				{
					loadAssets();
				}
				return franklinBReverseGearRear;
			}
			private set { }
		}

		public static RuntimeAnimatorController FranklinBAnimCtrl
		{
			get
			{
				if (franklinBAnimCtrl is null)
				{
					loadAssets();
				}
				return franklinBAnimCtrl;
			}
			private set { }
		}

		public static RuntimeAnimatorController FranklinBAnimCtrl4444
		{
			get
			{
				if (franklinBAnimCtrl4444 is null)
				{
					loadAssets();
				}
				return franklinBAnimCtrl4444;
			}
			private set { }
		}

		private static void loadAssets()
		{
			Main.Logger.Log("Loading assets...");
			AssetBundle drivetrain = AssetBundle.LoadFromFile(Path.Combine(Main.ModPath, @"assets\s282_drivetrain"));
			if (drivetrain == null)
			{
				Main.Logger.Error("Failed to load drivetrain assetbundle");
			}

			branchPipe = (GameObject)drivetrain.LoadAsset("branch_pipe");
			if (branchPipe == null)
			{
				Main.Logger.Error("Failed to load branch_pipe in drivetrain assetbundle");
			}

			branchPipe4444 = (GameObject)drivetrain.LoadAsset("branch_pipe_4444");
			if (branchPipe4444 == null)
			{
				Main.Logger.Error("Failed to load branch_pipe_4444 in drivetrain assetbundle");
			}

			franklinBValveGear = (GameObject)drivetrain.LoadAsset("franklin_type_b");
			if (franklinBValveGear == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b in drivetrain assetbundle");
			}

			franklinBValveGear4444 = (GameObject)drivetrain.LoadAsset("franklin_type_b_4444");
			if (franklinBValveGear4444 == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b_4444 in drivetrain assetbundle");
			}

			franklinBReverseGear = (GameObject)drivetrain.LoadAsset("franklin_type_b_reverse");
			if (franklinBReverseGear == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b_reverse in drivetrain assetbundle");
			}

			franklinBReverseGear4444 = (GameObject)drivetrain.LoadAsset("franklin_type_b_reverse_4444");
			if (franklinBReverseGear4444 == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b_reverse_4444 in drivetrain assetbundle");
			}

			franklinBReverseGearRear = (GameObject)drivetrain.LoadAsset("franklin_type_b_reverse_rear");
			if (franklinBReverseGearRear == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b_reverse_rear in drivetrain assetbundle");
			}

			franklinBAnimCtrl = (RuntimeAnimatorController)drivetrain.LoadAsset("franklin_b_anim_ctrl");
			if (franklinBAnimCtrl == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b_anim in drivetrain assetbundle");
			}

			franklinBAnimCtrl4444 = (RuntimeAnimatorController)drivetrain.LoadAsset("franklin_b_anim_ctrl_4444");
			if (franklinBAnimCtrl == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b_anim_4444 in drivetrain assetbundle");
			}

			Object.DontDestroyOnLoad(branchPipe);
			Object.DontDestroyOnLoad(branchPipe4444);
			Object.DontDestroyOnLoad(franklinBValveGear);
			Object.DontDestroyOnLoad(franklinBValveGear4444);
			Object.DontDestroyOnLoad(franklinBReverseGear);
			Object.DontDestroyOnLoad(franklinBReverseGear4444);
			Object.DontDestroyOnLoad(franklinBReverseGearRear);
			Object.DontDestroyOnLoad(franklinBAnimCtrl);
			Object.DontDestroyOnLoad(franklinBAnimCtrl4444);

			Main.Logger.Log("Finished loading assets");
		}
	}
}
