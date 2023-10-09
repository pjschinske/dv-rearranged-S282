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
		private static GameObject franklinBValveGear;
		private static RuntimeAnimatorController franklinBAnimCtrl;

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

		private static void loadAssets()
		{
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
			franklinBValveGear = (GameObject)drivetrain.LoadAsset("franklin_type_b");
			if (franklinBValveGear == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b in drivetrain assetbundle");
			}
			franklinBAnimCtrl = (RuntimeAnimatorController)drivetrain.LoadAsset("franklin_b_anim_ctrl");
			if (franklinBValveGear == null)
			{
				Main.Logger.Error("Failed to load franklin_type_b_anim in drivetrain assetbundle");
			}
		}
	}
}
