using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using DV.RemoteControls;
using DV.Wheels;
using DV.Simulation.Cars;
using DV.ThingTypes;
using LocoSim.Implementations;
using UnityEngine;
using static Oculus.Avatar.CAPI;
using DV.ModularAudioCar;
using JetBrains.Annotations;

namespace RearrangedS282
{
	public class Spawner
	{
		//This fixes the S282 clipping through the rail by shifting and rotating everything in the S282.
		private static void fixS282ClippingThroughRail(ref TrainCar loco)
		{
			/*Main.Logger.Log("Fixing S282 clipping through rails");
			loco.transform.Find("LocoS282A_Body/Static_LOD0").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("LocoS282A_Body/Static_LOD0").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("LocoS282A_Body/MovingParts_LOD0").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("LocoS282A_Body/MovingParts_LOD0").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("[interior LOD]").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[interior LOD]").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("[buffers]").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[buffers]").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("[cab lights] - gearlights").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[cab lights] - gearlights").localEulerAngles = new Vector3(0.22f, 0, 0);

			loco.transform.Find("LocoS282A_Headlights").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("LocoS282A_Headlights").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("LocoS282A_Particles").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("LocoS282A_Particles").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("[car plate anchor1]").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("[car plate anchor2]").localEulerAngles = new Vector3(-0.22f, 180f, 0);
			//loco.transform.Find("[collision]").localPosition = new Vector3(0, 0.09f, 0);
			//loco.transform.Find("[collision]").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("[bogies]").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[bogies]").localEulerAngles = new Vector3(0.22f, 0, 0);
			loco.transform.Find("[coupler rear]").localEulerAngles += new Vector3(0, 0.09f, 0);
			loco.transform.Find("[coupler front]").localEulerAngles += new Vector3(0, 0.09f, 0);

			//now for the interior
			Main.Logger.Log("Fixing an S282 interior");
			loco.interior.Find("[walkable]").localPosition = new Vector3(0, 0.09f, 0);
			loco.interior.Find("[items]").localPosition = new Vector3(0, 0.09f, 0);
			//Main.Logger.Log("Halfway through");
			loco.interior.Find("[camera dampening]").localPosition = new Vector3(0, 0.09f, 0);
			loco.interior.Find("[cab]").localPosition += new Vector3(0, 0.09f, 0);
			Main.Logger.Log("Finished fixing S282 clipping through rails");*/
		}

		[HarmonyPatch(typeof(TrainCar), nameof(TrainCar.Start))]
		class RandomizeWA
		{
			[HarmonyAfter("Gauge")]
			static void Postfix(ref TrainCar __instance)
			{
				if (__instance is not null
					&& __instance.carType == TrainCarType.LocoSteamHeavy)
				{
					fixS282ClippingThroughRail(ref __instance);

					__instance.gameObject.AddComponent<WheelRearranger>();
				}
			}
		}

		/*//Moves S282's external interactables (windows, outside hand valves, etc) up to match the body
		//We need to move these up because we're moving the body upwards
		[HarmonyPatch(typeof(TrainCar), nameof(TrainCar.LoadExternalInteractables))]
		class MoveS282ExternalInteractablesUp
		{
			static void Postfix(ref TrainCar __instance)
			{
				if (__instance is not null
					&& __instance.carType == TrainCarType.LocoSteamHeavy)
				{
					__instance.loadedExternalInteractables.transform.localPosition = new Vector3(0, 0.09f, 0);
					__instance.loadedExternalInteractables.transform.localEulerAngles = new Vector3(0.22f, 0, 0);
					//this seems to be throwing a null pointer exception, but it also seems to be working...
					//added this "off and on again" because it seems to be necessary to make the external
					//interactables actually show up in the right place.
					//TODO: Maybe I could make this a prefix to get rid of these lines?
					__instance.loadedExternalInteractables.gameObject.SetActive(false);
					__instance.loadedExternalInteractables.gameObject.SetActive(true);
				}
			}
		}*/

		//Moves S282's interior up to match the body
		//Needs to be a patch because LocoS282A_Interior(Clone) gets respawned every time you enter the cab
		[HarmonyPatch(typeof(TrainCar), nameof(TrainCar.LoadInterior))]
		class MoveS282InteriorUp
		{
			static void Postfix(ref TrainCar __instance)
			{
				/*if (__instance.carType == TrainCarType.LocoSteamHeavy)
				{
					Transform interior = __instance.loadedInterior.transform;
					interior.localPosition = new Vector3(0, 0.09f, 0);
					interior.localEulerAngles = new Vector3(0.22f, 0, 0);
					//The "draft" (i.e the damper) doesn't work well when its moved up.
					//So we move it back down to where it is in the vanilla game. 
					Transform draft = interior.Find("Center/Draft");
					draft.localPosition = new Vector3(0.42f, 1.27f, -0.13f); //(y -= 0.09)
					//But we make it look like it's where it should be
					Transform draftModelParent = draft.Find("C_Draft_pivot/C_Damper");
					draftModelParent.Find("model").localPosition = new Vector3(0, -0.57f, 0);
					draftModelParent.Find("handle collider").localPosition = new Vector3(0, 0.09f, 0.07f);
					draftModelParent.Find("body collider").localPosition = new Vector3(0, -0.24f, 0);

					//also fix the injector handle and the valves on the hydrostatic lubricator
					Transform valves = interior.Find("Left/Valves");
					if (valves is null)
					{
						Main.Logger.Error($"Valve parent can't be found");
						return;
					}

					Transform c_blowdown = valves.Find("Blowdown/C_Blowdown");
					Transform c_bell = valves.Find("Bell/C_Bell");
					Transform c_blower = valves.Find("Blower/C_Blower");
					if (c_blowdown is null || c_bell is null || c_blower is null)
					{
						Main.Logger.Error("Valves can't be found");
						return;
					}

					HingeJoint c_blowdown_hinge = c_blowdown.GetComponent<HingeJoint>();
					HingeJoint c_bell_hinge = c_bell.GetComponent<HingeJoint>();
					HingeJoint c_blower_hinge = c_blower.GetComponent<HingeJoint>();
					if (c_blowdown_hinge is null || c_bell_hinge is null || c_blower_hinge is null)
					{
						Main.Logger.Error("Couldn't find valve hinges");
						return;
					}

					c_blowdown_hinge.autoConfigureConnectedAnchor = false;
					c_bell_hinge.autoConfigureConnectedAnchor = false;
					c_blower_hinge.autoConfigureConnectedAnchor = false;
					c_blowdown_hinge.connectedAnchor += new Vector3(0, 0.09f, 0.008f);
					c_bell_hinge.connectedAnchor += new Vector3(0, 0.09f, 0.008f);
					c_blower_hinge.connectedAnchor += new Vector3(0, 0.09f, 0.008f);

					//also do the injector handle
					Transform c_injector = valves.parent.Find("Injector/C_Injector");
					HingeJoint c_injector_hinge = c_injector.GetComponent<HingeJoint>();
					c_injector_hinge.autoConfigureConnectedAnchor = false;
					c_injector_hinge.connectedAnchor += new Vector3(0, 0.09f, 0.08f);

					//also do the reverser
					Transform c_reverser = interior.Find("Right/CutoffReverser/C_CutoffReverser");
					HingeJoint c_reverser_hinge = c_reverser.GetComponent<HingeJoint>();
					c_reverser_hinge.autoConfigureConnectedAnchor = false;
					c_reverser_hinge.connectedAnchor += new Vector3(0, 0.09f, 0);
				}*/
			}
		}

		/*
		 * If a ten-coupled locomotive explodes, add the fifth drive axle and the extra
		 * side rod to the model.
		 * We can't do this beforehand because the exploded locomotive prefab hasn't
		 * been spawned in yet, so we need to alter it as the exploded locomotive body
		 * prefab is spawned in.
		 */
		[HarmonyPatch(typeof(ExplosionModelHandler), nameof(ExplosionModelHandler.HandleExplosionModelChange))]
		class ExplodeDriveAxles
		{
			static void Postfix(ref ExplosionModelHandler __instance)
			{
				Transform loco = __instance.transform;
				TrainCar locoTrainCar = loco.GetComponent<TrainCar>();
				if (locoTrainCar is null || locoTrainCar.carType != TrainCarType.LocoSteamHeavy)
				{//only continue if we're working with an S282
					Main.Logger.Error("Tried to explode something that wasn't an S282");
					return;
				}
				Main.Logger.Log("Exploding an S282");

				//copy unexplodedMovingParts over
				Transform unexplodedMovingParts = loco.Find("LocoS282A_Body/MovingParts_LOD0");
				Transform oldExplodedMovingParts = loco.Find("LocoS282AExploded_Body(Clone)/MovingParts_LOD0");
				oldExplodedMovingParts.gameObject.SetActive(false);
				Transform newMovingParts = UnityEngine.Object.Instantiate(unexplodedMovingParts.gameObject, oldExplodedMovingParts.parent).transform;

				//get exploded material. Doesn't matter from what part, they all have the same texture
				var explodedMaterial = oldExplodedMovingParts
					.Find("DriveMechanism L/s282_wheels_driving_1")
					.GetComponent<MeshRenderer>()
					.material;

				//replace all textures in newMovingParts with exploded texture
				//This code was heavily inspired by code from Skin Manager (https://github.com/derail-valley-modding/skin-manager)
				foreach (var renderer in newMovingParts.GetComponentsInChildren<MeshRenderer>(true))
				{
					if (renderer.material is null)
						continue;

					//apply exploded material to renderer
					renderer.material = explodedMaterial;
				}

				//hide siderods if they were already hidden
				Transform oldLeftSideRod = unexplodedMovingParts.Find("DriveMechanism L/s282_mech_wheels_connect");
				Transform oldRightSideRod = unexplodedMovingParts.Find("DriveMechanism R/s282_mech_wheels_connect");
				Transform newLeftSideRod = newMovingParts.Find("DriveMechanism L/s282_mech_wheels_connect");
				Transform newRightSideRod = newMovingParts.Find("DriveMechanism R/s282_mech_wheels_connect");
				newLeftSideRod.GetComponent<MeshRenderer>().enabled = oldLeftSideRod.GetComponent<MeshRenderer>().enabled;
				newRightSideRod.GetComponent<MeshRenderer>().enabled = oldRightSideRod.GetComponent<MeshRenderer>().enabled;
				newLeftSideRod.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
				newRightSideRod.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);

				//hide some LODs to get rid of z-fighting on the drivers
				loco.Find("Axle_F/bogie_car/[axle] 1/axleF_modelLOD1").gameObject.SetActive(false);
				loco.Find("Axle_R/bogie_car/[axle] 1/axleR_modelLOD1").gameObject.SetActive(false);

				Transform firstLeftDriveWheel = newMovingParts.Find("DriveMechanism L/s282_wheels_driving_1");
				Transform secondLeftDriveWheel = newMovingParts.Find("DriveMechanism L/s282_wheels_driving_2");
				Transform thirdLeftDriveWheel = newMovingParts.Find("DriveMechanism L/s282_wheels_driving_3");
				Transform fourthLeftDriveWheel = newMovingParts.Find("DriveMechanism L/s282_wheels_driving_4");
				Transform fifthLeftDriveWheel = newMovingParts.Find("DriveMechanism L/s282_wheels_driving_5");
				Transform sixthLeftDriveWheel = newMovingParts.Find("DriveMechanism L/s282_wheels_driving_6");
				Transform firstRightDriveWheel = newMovingParts.Find("DriveMechanism R/s282_wheels_driving_1");
				Transform secondRightDriveWheel = newMovingParts.Find("DriveMechanism R/s282_wheels_driving_2");
				Transform thirdRightDriveWheel = newMovingParts.Find("DriveMechanism R/s282_wheels_driving_3");
				Transform fourthRightDriveWheel = newMovingParts.Find("DriveMechanism R/s282_wheels_driving_4");
				Transform fifthRightDriveWheel = newMovingParts.Find("DriveMechanism R/s282_wheels_driving_5");
				Transform sixthRightDriveWheel = newMovingParts.Find("DriveMechanism R/s282_wheels_driving_6");

				//Hide more LODs
				firstLeftDriveWheel.GetChild(0).gameObject.SetActive(false);
				secondLeftDriveWheel.GetChild(0).gameObject.SetActive(false);
				thirdLeftDriveWheel.GetChild(0).gameObject.SetActive(false);
				fourthLeftDriveWheel.GetChild(0).gameObject.SetActive(false);
				fifthLeftDriveWheel.GetChild(0).gameObject.SetActive(false);
				sixthLeftDriveWheel.GetChild(0).gameObject.SetActive(false);
				firstRightDriveWheel.GetChild(0).gameObject.SetActive(false);
				secondRightDriveWheel.GetChild(0).gameObject.SetActive(false);
				thirdRightDriveWheel.GetChild(0).gameObject.SetActive(false);
				fourthRightDriveWheel.GetChild(0).gameObject.SetActive(false);
				fifthRightDriveWheel.GetChild(0).gameObject.SetActive(false);
				sixthRightDriveWheel.GetChild(0).gameObject.SetActive(false);

				//rotate 5th and 6th drive wheels to be in the right position
				fifthLeftDriveWheel.gameObject.GetComponent<PoweredWheelRotater>().Init(fourthLeftDriveWheel.gameObject);
				fifthRightDriveWheel.gameObject.GetComponent<PoweredWheelRotater>().Init(fourthRightDriveWheel.gameObject);
				sixthLeftDriveWheel.gameObject.GetComponent<PoweredWheelRotater>().Init(fourthLeftDriveWheel.gameObject);
				sixthRightDriveWheel.gameObject.GetComponent<PoweredWheelRotater>().Init(fourthRightDriveWheel.gameObject);

				//offset the right side drivetrain
				loco.Find("[wheel rotation]").GetComponent<PoweredWheelRotationViaAnimation>().animatorSetups[0].startTimeOffset = 0.75f;

				//copy axle supports over
				UnityEngine.Object.Instantiate(loco.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support_2").gameObject, loco.Find("LocoS282AExploded_Body(Clone)/Static_LOD0/"));
				UnityEngine.Object.Instantiate(loco.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject, loco.Find("LocoS282AExploded_Body(Clone)/Static_LOD0/"));

				//hide the vanilla brake shoes, since we've made our own
				loco.Find("LocoS282AExploded_Body(Clone)/Static_LOD0/s282_brake_shoes").gameObject.SetActive(false);

				//replace main loco mesh
				MeshFilter filter = loco.transform
				.Find("LocoS282AExploded_Body(Clone)/Static_LOD0/s282_locomotive_body")
				.GetComponent<MeshFilter>();
				if (filter is null)
				{
					Main.Logger.Warning("MeshFilter was null on an exploded S282");
					return;
				}
				Mesh mesh = filter.sharedMesh;
				if (mesh is null)
				{
					Main.Logger.Warning("Mesh was null on an exploded S282");
					return;
				}
				Mesh newS282Mesh = UnityEngine.Object.Instantiate(MeshFinder.Instance.S282Mesh);
				//for some reason the OBJloader flips the mesh left to right, so we have to flip it back
				filter.transform.localScale = new Vector3(-1, 1, 1);
				newS282Mesh.UploadMeshData(true);
				filter.sharedMesh = newS282Mesh;
				Main.Logger.Log("Loaded new exploded S282 mesh");

				//make exploded drivetrain rotate
				/*if (Main.settings.explodedDrivetrainRotate)
				{
					var drivetrainRotater = loco.Find("[wheel rotation]")
					.GetComponent<PoweredWheelRotationViaAnimation>();
					Animator leftAnimation = newMovingParts.Find("DriveMechanism L")
						.GetComponent<Animator>();
					Animator rightAnimation = newMovingParts.Find("DriveMechanism R")
						.GetComponent<Animator>();
					PoweredWheelRotationViaAnimation.AnimatorStartTimeOffsetPair leftAnimationOffset = new()
					{
						animator = leftAnimation,
						startTimeOffset = 0
					};
					PoweredWheelRotationViaAnimation.AnimatorStartTimeOffsetPair rightAnimationOffset = new()
					{
						animator = rightAnimation,
						startTimeOffset = 0.75f
					};
					drivetrainRotater.animatorSetups.AddItem(leftAnimationOffset);
					drivetrainRotater.animatorSetups.AddItem(rightAnimationOffset);
				}*/
				
			}
		}

		[HarmonyPatch(typeof(ExplosionModelHandler), nameof(ExplosionModelHandler.RevertToUnexplodedModel))]
		class DeexplodeLocomotive
		{
			static void Postfix(ref ExplosionModelHandler __instance)
			{
				Transform loco = __instance.transform;
				var wheelRearranger = loco.GetComponent<WheelRearranger>();
				wheelRearranger.SwitchWheelArrangement((int) wheelRearranger.currentWA);
				//We hid these LODs earlier, but now they're useful again
				loco.Find("Axle_F/bogie_car/[axle] 1/axleF_modelLOD1").gameObject.SetActive(true);
				loco.Find("Axle_R/bogie_car/[axle] 1/axleR_modelLOD1").gameObject.SetActive(true);
				UnityEngine.Object.Destroy(loco.Find("LocoS282AExploded_Body(Clone)/MovingParts_LOD0(Clone)").gameObject);
			}
		}
	}
}
