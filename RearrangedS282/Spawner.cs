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

namespace RearrangedS282
{
	public class Spawner
	{
		//This fixes the S282 clipping through the rail by shifting and rotating everything in the S282.
		private static void fixS282ClippingThroughRail(ref TrainCar loco)
		{
			Main.Logger.Log("Fixing S282 clipping through rails");
			loco.transform.Find("LocoS282A_Body").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("LocoS282A_Body").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("[interior LOD]").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[interior LOD]").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("[buffers]").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[buffers]").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("[cab lights] - gearlights").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[cab lights] - gearlights").localEulerAngles = new Vector3(0.35f, 0, 0);

			loco.transform.Find("LocoS282A_Headlights").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("LocoS282A_Headlights").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("LocoS282A_Particles").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("LocoS282A_Particles").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("[car plate anchor1]").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("[car plate anchor2]").localEulerAngles = new Vector3(-0.35f, 180f, 0);
			loco.transform.Find("[collision]").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[collision]").localEulerAngles = new Vector3(0.35f, 0, 0);
			loco.transform.Find("[bogies]").localPosition = new Vector3(0, 0.09f, 0);
			loco.transform.Find("[bogies]").localEulerAngles = new Vector3(0.35f, 0, 0);

			//now for the interior
			Main.Logger.Log("Fixing an S282 interior");
			loco.interior.Find("[walkable]").localPosition = new Vector3(0, 0.09f, 0);
			loco.interior.Find("[items]").localPosition = new Vector3(0, 0.09f, 0);
			//Main.Logger.Log("Halfway through");
			loco.interior.Find("[camera dampening]").localPosition = new Vector3(0, 0.09f, 0);
			loco.interior.Find("[cab]").localPosition += new Vector3(0, 0.09f, 0);
			Main.Logger.Log("Finished fixing S282 clipping through rails");
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

					if (Main.settings.spawnRandomWA)
					{
						WheelRearranger.SwitchWheelArrangementRandom(__instance);
					}
					else
					{
						WheelRearranger.SwitchWheelArrangement(__instance, (int)WheelArrangementType.a282);
					}
				}
			}
		}

		//Moves S282's external interactables (windows, outside hand valves, etc) up to match the body
		//We need to move these up because we're moving the body upwards
		[HarmonyPatch(typeof(TrainCar), nameof(TrainCar.LoadExternalInteractables))]
		class MoveS282ExternalInteractablesUp
		{
			static void Postfix(ref TrainCar __instance)
			{
				if (__instance.carType == TrainCarType.LocoSteamHeavy)
				{
					__instance.loadedExternalInteractables.transform.localPosition = new Vector3(0, 0.1f, 0);
					__instance.loadedExternalInteractables.transform.localEulerAngles = new Vector3(0.35f, 0, 0);
					//this seems to be throwing a null pointer exception, but it also seems to be working...
					//added this "off and on again" because it seems to be necessary to make the external
					//interactables actually show up in the right place.
					//TODO: Maybe I could make this a prefix to get rid of these lines?
					__instance.loadedExternalInteractables.gameObject.SetActive(false);
					__instance.loadedExternalInteractables.gameObject.SetActive(true);

				}
			}
		}

		//Moves S282's interior up to match the body
		//Needs to be a patch because LocoS282A_Interior(Clone) gets respawned every time you enter the cab
		[HarmonyPatch(typeof(TrainCar), nameof(TrainCar.LoadInterior))]
		class MoveS282InteriorUp
		{
			static void Postfix(ref TrainCar __instance)
			{
				if (__instance.carType == TrainCarType.LocoSteamHeavy)
				{
					Transform interior = __instance.loadedInterior.transform;
					interior.localPosition = new Vector3(0, 0.1f, 0);
					interior.localEulerAngles = new Vector3(0.35f, 0, 0);
				}
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
					return;
				}
				Main.Logger.Log("Exploding an S282");

				Transform leftDrivers = loco.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L");
				Transform rightDrivers = loco.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R");
				Transform fifthLeftDriveWheel = leftDrivers.Find("s282_wheels_driving_5");
				Transform fifthRightDriveWheel = rightDrivers.Find("s282_wheels_driving_5");
				Transform explodedBody = loco.Find("LocoS282AExploded_Body(Clone)");
				Transform leftExplodedDrivers = explodedBody.Find("MovingParts_LOD0/DriveMechanism L");
				Transform rightExplodedDrivers = explodedBody.Find("MovingParts_LOD0/DriveMechanism R");
				Transform fifthLeftExplodedDriveWheel = leftExplodedDrivers.Find("s282_wheels_driving_5");

				if (fifthLeftDriveWheel is not null && fifthLeftDriveWheel.gameObject.activeSelf)
				{//if the fifth drive axle was actually being used
					Main.Logger.Log("Exploding a 10-coupled S282");

					if (fifthLeftExplodedDriveWheel is null)
					{   //We haven't spawned the exploded fifth drive wheels yet, so create them
						Main.Logger.Log("Exploding a 10-coupled S282 that hasn't been exploded before");

						GameObject fourthLeftExplodedDriver = leftExplodedDrivers.Find("s282_wheels_driving_4").gameObject;
						GameObject fourthRightExplodedDriver = rightExplodedDrivers.Find("s282_wheels_driving_4").gameObject;
						GameObject leftExplodedSideRod = leftExplodedDrivers.Find("s282_mech_wheels_connect").gameObject;
						GameObject rightExplodedSideRod = rightExplodedDrivers.Find("s282_mech_wheels_connect").gameObject;
						Transform leftExtraSideRod = leftDrivers.Find("s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
						Transform rightExtraSideRod = rightDrivers.Find("s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");

						//clone exploded drivers for fifth drive axle
						GameObject fifthLeftExplodedDriver = UnityEngine.Object.Instantiate(
							fourthLeftExplodedDriver,
							fifthLeftDriveWheel.position,
							fourthLeftExplodedDriver.transform.rotation,
							leftExplodedDrivers);
						GameObject fifthRightExplodedDriver = UnityEngine.Object.Instantiate(
							fourthRightExplodedDriver,
							fifthRightDriveWheel.position,
							fourthRightExplodedDriver.transform.rotation,
							rightExplodedDrivers);
						fifthLeftExplodedDriver.name = "s282_wheels_driving_5";
						fifthRightExplodedDriver.name = "s282_wheels_driving_5";
						//hide LODs
						fifthLeftExplodedDriver.transform.GetChild(0).gameObject.SetActive(false);
						fifthRightExplodedDriver.transform.GetChild(0).gameObject.SetActive(false);

						//clone siderods
						GameObject newLeftSideRod = UnityEngine.Object.Instantiate(
							leftExplodedSideRod,
							leftExplodedSideRod.transform);
						GameObject newRightSideRod = UnityEngine.Object.Instantiate(
							rightExplodedSideRod,
							rightExplodedSideRod.transform);
						newLeftSideRod.transform.localPosition = new Vector3(0.003f, 0.004f, -3.1f);
						newRightSideRod.transform.localPosition = new Vector3(0.003f, 0.004f, -3.1f);
						newLeftSideRod.transform.localEulerAngles = leftExtraSideRod.localEulerAngles;
						newRightSideRod.transform.localEulerAngles = rightExtraSideRod.localEulerAngles;
						//hide LODs
						newLeftSideRod.transform.GetChild(0).gameObject.SetActive(false);
						newRightSideRod.transform.GetChild(0).gameObject.SetActive(false);

						//get rid of all the extra valve gear that we accidentally copied when cloning siderods
						for (int i = 0; i < newLeftSideRod.transform.childCount; i++)
						{
							GameObject child = newLeftSideRod.transform.GetChild(i).gameObject;
							//kill all the children except our favorite
							if (!child.name.Equals("s282_mech_wheels_connect_LOD1"))
							{
								UnityEngine.Object.DestroyImmediate(child);
								i--; //gotta offset i because we deleted an object
							}
						}
						for (int i = 0; i < newRightSideRod.transform.childCount; i++)
						{
							GameObject child = newRightSideRod.transform.GetChild(i).gameObject;
							//kill all the children except our favorite
							if (!child.name.Equals("s282_mech_wheels_connect_LOD1"))
							{
								UnityEngine.Object.DestroyImmediate(child);
								i--; //gotta offset i because we deleted an object
							}
						}
					}
					else
					{   // if the fifth exploded drive wheels have been spawned already, we just need to show them.
						//This would happen if we exploded a 10-coupled loco, then repaired it, then exploded it again.
						Main.Logger.Log("Exploding a 10-coupled S282 that has been exploded before as a 10-coupled");

						Transform fifthRightExplodedDriveWheel = rightExplodedDrivers.Find("s282_wheels_driving_5");
						Transform extraLeftExplodedSideRod = leftExplodedDrivers.Find("s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
						Transform extraRightExplodedSideRod = rightExplodedDrivers.Find("s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
						fifthLeftExplodedDriveWheel.gameObject.SetActive(true);
						fifthRightExplodedDriveWheel.gameObject.SetActive(true);
						extraLeftExplodedSideRod.gameObject.SetActive(true);
						extraRightExplodedSideRod.gameObject.SetActive(true);
					}
				}
				else
				{
					if (fifthLeftExplodedDriveWheel is not null)
					{   //if we've exploded a 10-coupled locomotive, then repaired it, then made it an 8-coupled,
						//we now need to hide the fifth exploded axle.
						Main.Logger.Log("Exploding an 8-coupled S282 that has been exploded before as a 10-coupled");

						Transform fifthRightExplodedDriveWheel = rightExplodedDrivers.Find("s282_wheels_driving_5");
						Transform extraLeftExplodedSideRod = leftExplodedDrivers.Find("s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
						Transform extraRightExplodedSideRod = rightExplodedDrivers.Find("s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
						fifthLeftExplodedDriveWheel.gameObject.SetActive(false);
						fifthRightExplodedDriveWheel.gameObject.SetActive(false);
						extraLeftExplodedSideRod.gameObject.SetActive(false);
						extraRightExplodedSideRod.gameObject.SetActive(false);
					}
					//else, we exploded a 8-coupled locomotive that has always been an 8-coupled locomotive:
					//no need to do anything because we've never spawned the fifth drive axle
				}

				//show trailing axle support if we need to
				Transform rearTrailingAxleSupport = loco.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1");
				if (rearTrailingAxleSupport is not null)
				{
					Transform explodedFrontAxleSupport = explodedBody.Find("Static_LOD0/s282_wheels_front_support");

					GameObject explodedTrailingAxleSupport = UnityEngine.Object.Instantiate(
						explodedFrontAxleSupport.gameObject,
						rearTrailingAxleSupport.position,
						rearTrailingAxleSupport.rotation,
						explodedFrontAxleSupport.parent
					);
					explodedTrailingAxleSupport.transform.localScale = rearTrailingAxleSupport.localScale;
				}
			}
		}
	}
}
