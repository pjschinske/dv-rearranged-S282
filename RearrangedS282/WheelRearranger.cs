using RearrangedS282;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV.ThingTypes;
using DV.Wheels;
using LocoSim.Implementations.Wheels;
using UnityEngine;
using LocoSim.Definitions;
using Random = System.Random;
using DV.ModularAudioCar;

namespace RearrangedS282
{
	internal static class WheelRearranger
	{

		private static Random rand = new System.Random();

		public static void SwitchWheelArrangementRandom(TrainCar loco)
		{
			int wa = rand.Next(WheelArrangement.WheelArrangementNames.Length);
			SwitchWheelArrangement(loco, wa);
		}

		private static bool skipWheelArrangementChange = false;

		public static void SwitchWheelArrangement(TrainCar loco, int wa)
		{
			//Was having some issues with multiple requests coming in at once for the same
			//loco. This will hopefully stop that from happening.
			if (skipWheelArrangementChange)
				return;
			skipWheelArrangementChange = true;

			WheelArrangementType waType = (WheelArrangementType)wa;

			Main.Logger.Log($"Switching an S282 to {waType.ToString()}");

			if (!validateLoco(loco))
			{
				return;
			}

			HideLeadingWheels(loco);
			RemoveFifthDriveAxle(loco);
			HideTrailingWheels(loco);

			FixExplodedModel(loco);

			//select leading axle configuration
			switch (waType)
			{
				case WheelArrangementType.a280:
				case WheelArrangementType.a282:
				case WheelArrangementType.a284:
				case WheelArrangementType.a2100:
				case WheelArrangementType.a2102:
				case WheelArrangementType.a2104:
					ShowTwoLeadingWheels(loco);
					break;
				case WheelArrangementType.a480:
				case WheelArrangementType.a482:
				case WheelArrangementType.a484:
				case WheelArrangementType.a4100:
				case WheelArrangementType.a4102:
				case WheelArrangementType.a4104:
					ShowFourLeadingWheels(loco);
					break;
				default:
					break;
			};

			//select drive axle configuration
			switch (waType)
			{
				case WheelArrangementType.a0100:
				case WheelArrangementType.a0102:
				case WheelArrangementType.a0104:
				case WheelArrangementType.a2100:
				case WheelArrangementType.a2102:
				case WheelArrangementType.a2104:
				case WheelArrangementType.a4100:
				case WheelArrangementType.a4102:
				case WheelArrangementType.a4104:
					AddFifthDriveAxle(loco);
					break;
				default:
					break;
			};

			//select trailing axle configuration
			switch (waType)
			{
				case WheelArrangementType.a082:
				case WheelArrangementType.a282:
				case WheelArrangementType.a482:
					switch (Main.settings.x82Options)
					{
						case Settings.X82Options.vanilla:
							ShowTwoTrailingWheelsVanilla(loco);
							break;
						case Settings.X82Options.small_wheels:
							ShowTwoTrailingWheels(loco);
							break;
						case Settings.X82Options.small_wheels_alternate:
							ShowTwoTrailingWheelsAlternate(loco);
							break;
					}
					break;
				case WheelArrangementType.a0102:
				case WheelArrangementType.a2102:
				case WheelArrangementType.a4102:
					switch (Main.settings.x102Options)
					{
						case Settings.X102Options.vanilla:
							ShowTwoTrailingWheelsFor10CoupledVanilla(loco);
							break;
						case Settings.X102Options.small_wheels:
							ShowTwoTrailingWheelsFor10Coupled(loco);
							break;
					}
					break;
				case WheelArrangementType.a084:
				case WheelArrangementType.a284:
				case WheelArrangementType.a484:
					ShowFourTrailingWheels(loco);
					break;
				case WheelArrangementType.a0104:
				case WheelArrangementType.a2104:
				case WheelArrangementType.a4104:
					ShowFourTrailingWheelsFor10Coupled(loco);
					break;
				default:
					break;
			};

			//setting this to -1 forces an update based on the number of axles in each Bogie.axles
			loco.numberOfAxles = -1;

			//setting these to null forces an update based on the number of [axle]s in each Bogie
			loco.transform.Find("Axle_F").GetComponent<Bogie>().axles = null;
			loco.transform.Find("Axle_R").GetComponent<Bogie>().axles = null;

			//reset audio sources for both bogies
			Transform interior = loco.interior;
			if (interior is null)
			{
				skipWheelArrangementChange = false;
				return;
			}
			Transform carBaseAudioModules = interior.Find("LocoS282A_Audio(Clone)/CarBaseAudioModules");
			if (carBaseAudioModules is null)
			{
				skipWheelArrangementChange = false;
				return;
			}
			CarRollingAudioModule carRollingAudioModule = carBaseAudioModules.GetComponent<CarRollingAudioModule>();
			if (carRollingAudioModule is null || carRollingAudioModule.car is null)
			{
				skipWheelArrangementChange = false;
				return;
			}
			carRollingAudioModule.ResetJointSoundValues();

			skipWheelArrangementChange = false;
		}

		// Exploded locos are handled separately from regular locos, so we need to alter
		// the model to look right when it gets exploded
		private static void FixExplodedModel(TrainCar loco)
		{
			ExplosionModelHandler explosionModelHandler = loco.GetComponent<ExplosionModelHandler>();

			//if we've already added the new axles to this loco, don't do it again
			if (explosionModelHandler.gameObjectsToReplace.Length > 4)
			{
				return;
			}
			GameObject frontAxle1 = loco.transform.Find("Axle_F/bogie_car/[axle] 1/axleF_model").gameObject;
			GameObject frontAxle2 = loco.transform.Find("Axle_F/bogie_car/[axle] 2/axleF_model").gameObject;
			Main.Logger.Log("Finished getting exploded front axles, now for the rear");
			//GameObject rearAxle1 = loco.transform.Find("Axle_R/bogie_car").GetComponentInChildren<MeshFilter>().gameObject;
			GameObject rearAxle2 = loco.transform.Find("Axle_R/bogie_car/[axle] 2/axleF_model").gameObject;
			GameObject rearAxle3 = loco.transform.Find("Axle_R/bogie_car/[axle] 3/axleF_model").gameObject;
			Main.Logger.Log("Finished getting exploded axles");

			explosionModelHandler.gameObjectsToReplace = explosionModelHandler.gameObjectsToReplace
				.Append(frontAxle1)
				.Append(frontAxle2)
				.Append(rearAxle2)
				.Append(rearAxle3)
				.ToArray();

			GameObject explodedFrontAxle = explosionModelHandler.replacePrefabsToSpawn[2];
			explosionModelHandler.replacePrefabsToSpawn = explosionModelHandler.replacePrefabsToSpawn
				.Append(explodedFrontAxle)
				.Append(explodedFrontAxle)
				.Append(explodedFrontAxle)
				.Append(explodedFrontAxle)
				.ToArray();
			Main.Logger.Log("Finished fixing exploded axles");
		}

		//This hides any leading wheels, but also generates the second leading axle
		//if it hasn't been already.
		private static void HideLeadingWheels(TrainCar loco)
		{
			//Hide first axle and its support
			Transform firstFrontBogie = loco.transform.Find("Axle_F/bogie_car");
			Transform firstFrontAxle = firstFrontBogie.Find("[axle] 1");

			GameObject firstFrontAxleSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject;
			GameObject firstFrontAxleSupportLOD = loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject;
			firstFrontAxleSupport.SetActive(false);
			firstFrontAxleSupportLOD.SetActive(false);
			//Stock axle support is actually misaligned by 0.03m, this sets it to the correct position
			firstFrontAxleSupport.transform.localPosition = new Vector3(0, 0.38f, 10.54f);
			firstFrontAxleSupportLOD.transform.localPosition = new Vector3(0, 0.38f, 10.55f);

			//look for the second front axle
			GameObject secondFrontAxle, secondFrontAxleSupport, secondFrontAxleSupportLOD;
			Transform driveAxleTransform = null;
			int driveAxleCounter = 0;
			if (firstFrontAxle == null)
			{
				//rename front axles to something reasonable
				for (int i = 0; i < firstFrontBogie.transform.childCount; i++)
				{
					if (firstFrontBogie.transform.GetChild(i).localPosition.z > 2.4)
					{//if the ith front axle is the first front axle, grab it
						firstFrontAxle = firstFrontBogie.transform.GetChild(i);
						firstFrontAxle.gameObject.name = "[axle] 1";
						firstFrontAxle.localPosition = new Vector3(firstFrontAxle.localPosition.x, 0.36f, firstFrontAxle.localPosition.z);
					}
					else
					{//while we're here, grab the extra drive axles and rename them so we can easily access them later
						driveAxleTransform = firstFrontBogie.GetChild(i);
						driveAxleTransform.gameObject.name = $"[axle] drive {driveAxleCounter}";
						driveAxleCounter++;
					}
				}

				// if the second front axle isn't there, generate it
				secondFrontAxle = UnityEngine.Object.Instantiate(firstFrontAxle.gameObject, firstFrontAxle.parent);
				secondFrontAxle.name = "[axle] 2";
				secondFrontAxle.transform.localPosition = new Vector3(0, 0.36f, 1.45f);
				secondFrontAxle.gameObject.SetActive(false);
				secondFrontAxle.transform.Find("axleF_modelLOD1").gameObject.SetActive(false);

				//also generate the wheel support for the second front axle
				secondFrontAxleSupport = UnityEngine.Object.Instantiate(firstFrontAxleSupport, firstFrontAxleSupport.transform.parent);
				secondFrontAxleSupport.name = "s282_wheels_front_support_2";
				secondFrontAxleSupport.transform.localPosition = new Vector3(0, 0.38f, 9.44f);
				secondFrontAxleSupport.SetActive(false);
				secondFrontAxleSupportLOD = UnityEngine.Object.Instantiate(firstFrontAxleSupportLOD, firstFrontAxleSupportLOD.transform.parent);
				secondFrontAxleSupportLOD.name = "s282_wheels_front_support_2_LOD1";
				secondFrontAxleSupportLOD.transform.localPosition = new Vector3(0, 0.38f, 9.45f);
				secondFrontAxleSupportLOD.SetActive(false);

				//get the second front axle to rotate
				WheelRotationViaCode[] wheelRotaters = loco.transform.Find("[wheel rotation]").GetComponents<WheelRotationViaCode>();
				foreach (WheelRotationViaCode wheelRotater in wheelRotaters)
				{
					if (wheelRotater.wheelRadius == 0.355f)
					{
						wheelRotater.transformsToRotate = wheelRotater.transformsToRotate.Append(secondFrontAxle.transform).ToArray();
					}
				}
			}
			else
			{   // if the second front axle is there, fetch it and hide it
				secondFrontAxle = firstFrontBogie.Find("[axle] 2").gameObject;
				//TODO: get this to work
				/*secondFrontAxleSupport = loco.transform.Find("LocoS282Body/Static_LOD0/s282_wheels_front_support_2").gameObject;
				secondFrontAxleSupport.SetActive(false);*/
			}

			firstFrontAxle.gameObject.SetActive(false);
			secondFrontAxle.SetActive(false);
			firstFrontAxleSupport.SetActive(false);
			firstFrontAxleSupportLOD.SetActive(false);
		}

		//When testing, I ran into a bug where the locomotive would "hop" forward or
		//backward slightly when changing between an 8-coupled and a 10-coupled or
		//vice versa. This method turns off physics updates during that transition so
		//that that "hop" doesn't happen.
		private static async void setBogieKinematicLater(Rigidbody bogie)
		{
			bool isKinematic = bogie.isKinematic;
			bogie.isKinematic = true;
			Main.Logger.Log("Setting bogie kinematic true...");
			await Task.Delay(100);//delay 1 frame @ 10fps
			bogie.isKinematic = isKinematic;
			Main.Logger.Log("Setting bogie kinematic back to its original value");
			await Task.Delay(100);//delay 1 frame @ 10fps
			bogie.isKinematic = isKinematic;
			Main.Logger.Log("Setting bogie kinematic again");
		}

		private static void RemoveFifthDriveAxle(TrainCar loco)
		{
			Transform axleR = loco.transform.Find("Axle_R");

			ConfigurableJoint axleRJoint = axleR.GetComponent<ConfigurableJoint>();
			if (axleRJoint is null)
			{
				Main.Logger.Log($"Couldn't find ConfigurableJoint on Axle_R.");
			}
			else
			{
				axleRJoint.autoConfigureConnectedAnchor = false;
				axleRJoint.connectedAnchor = new Vector3(axleRJoint.connectedAnchor.x, axleRJoint.connectedAnchor.y, 3.2f);
			}
			Rigidbody axleRRigidbody = axleR.GetComponent<Rigidbody>();
			if (axleRRigidbody is not null)
				setBogieKinematicLater(axleRRigidbody);
			axleR.localPosition = new Vector3(axleR.localPosition.x, axleR.localPosition.y, 3.2f);

			Transform axleRDrive2 = axleR.transform.Find("bogie_car/[axle] drive 2");
			if (axleRDrive2 is not null)
			{
				axleRDrive2.gameObject.SetActive(false);
			}

			//move axleRDrive0 to line up with the 3rd drive axle
			Transform axleRDrive0 = axleR.transform.Find("bogie_car/[axle] drive 0");
			if (axleRDrive0 is not null)
			{
				axleRDrive0.localPosition = new Vector3(axleRDrive0.localPosition.x, axleRDrive0.localPosition.y, 1.7f);
			}

			Transform fourthLeftDriveWheel =			loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_wheels_driving_4");
			Transform fourthRightDriveWheel =			loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_wheels_driving_4");
			Transform fifthLeftDriveWheelTransform =	loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_wheels_driving_5");

			if (fourthLeftDriveWheel is null)
				Main.Logger.Error("Error: RemoveFifthDriveAxle: fourthLeftDriveWheel not found");
			if (fourthRightDriveWheel is null)
				Main.Logger.Error("Error: RemoveFifthDriveAxle: fourthRightDriveWheel not found");

			GameObject fifthLeftDriveWheel, fifthRightDriveWheel;
			if (fifthLeftDriveWheelTransform == null)
			{   //if the fifth drive axle isn't there, then generate it
				fifthLeftDriveWheel = UnityEngine.Object.Instantiate(fourthLeftDriveWheel.gameObject, fourthLeftDriveWheel.parent);
				fifthRightDriveWheel = UnityEngine.Object.Instantiate(fourthRightDriveWheel.gameObject, fourthRightDriveWheel.parent);
				fifthLeftDriveWheel.name = "s282_wheels_driving_5";
				fifthRightDriveWheel.name = "s282_wheels_driving_5";
				fifthLeftDriveWheel.transform.localPosition = new Vector3(0, 0.71f, 1.5f);
				fifthRightDriveWheel.transform.localPosition = new Vector3(0, 0.71f, 1.5f);

				//hide fifth drive axle LODs
				fifthLeftDriveWheel.transform.GetChild(0).gameObject.SetActive(false);
				fifthRightDriveWheel.transform.GetChild(0).gameObject.SetActive(false);

				//get fifth drive axle to rotate
				fifthLeftDriveWheel.AddComponent<PoweredWheelRotater>().Init(fourthLeftDriveWheel.gameObject);
				fifthRightDriveWheel.AddComponent<PoweredWheelRotater>().Init(fourthRightDriveWheel.gameObject);

				//duplicate side rods and extend the copies to the fifth axle
				Transform leftSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_mech_wheels_connect");
				Transform rightSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_mech_wheels_connect");
				if (leftSideRod is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: leftSideRod not found");
				if (rightSideRod is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: rightSideRod not found");
				GameObject newLeftSideRod = UnityEngine.Object.Instantiate(leftSideRod.gameObject, leftSideRod);
				GameObject newRightSideRod = UnityEngine.Object.Instantiate(rightSideRod.gameObject, rightSideRod);
				newLeftSideRod.SetActive(false);
				newRightSideRod.SetActive(false);
				newLeftSideRod.transform.localPosition = new Vector3(0.003f, 0.004f, -3.1f);
				newLeftSideRod.transform.localEulerAngles = new Vector3(0, 180f, 180f);
				newRightSideRod.transform.localPosition = new Vector3(0.003f, 0.004f, -3.1f);
				newRightSideRod.transform.localEulerAngles = new Vector3(0, 180f, 180f);

				//hide siderod LODs
				newLeftSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
				newRightSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);

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
			{
				Main.Logger.Log("Found fifth left drive wheel transform");
				fifthLeftDriveWheel = fifthLeftDriveWheelTransform.gameObject;
				Main.Logger.Log("Found fifth left drive wheel gameobject");
				fifthRightDriveWheel = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_wheels_driving_5").gameObject;
				if (fifthRightDriveWheel is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: fifthRightDriveWheel is null");
				Main.Logger.Log("Found fifth right drive wheel");

				Transform leftSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
				Main.Logger.Log("Found left siderod");
				Transform rightSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
				Main.Logger.Log("Found right siderod");
				if (leftSideRod is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: leftSideRod clone not found");
				if (rightSideRod is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: rightSideRod clone not found");
				leftSideRod.gameObject.SetActive(false);
				rightSideRod.gameObject.SetActive(false);
				Main.Logger.Log("Hid siderods");
			}

			fifthLeftDriveWheel.SetActive(false);
			fifthRightDriveWheel.SetActive(false);
			Main.Logger.Log("Hid fifth drive wheels");
			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 4;
			Main.Logger.Log("Set poweredAxles to 4");

			//remove sparks from 5th drive axle
			Transform wheelSparks = loco.transform.Find("LocoS282A_Particles/[wheel sparks]");
			if (wheelSparks is null)
			{
				Main.Logger.Log("[wheel sparks] is null");
				return;
			}
			WheelslipSparksController wheelslipSparks = wheelSparks.GetComponent<WheelslipSparksController>();
			WheelSlideSparksController wheelSlideSparks = wheelSparks.GetComponent<WheelSlideSparksController>();
			if (wheelslipSparks is not null
				&& wheelslipSparks.wheelSparks.Length > 4)
			{
				wheelslipSparks.wheelSparks[4].sparksLeftPS.Stop();
				wheelslipSparks.wheelSparks[4].sparksRightPS.Stop();
				wheelslipSparks.wheelSparks = wheelslipSparks.wheelSparks.Take(4).ToArray();
			}
			if (wheelSlideSparks is not null
				&& wheelSlideSparks.sparkAnchors.Length > 8)
			{
				wheelSlideSparks.sparkAnchors = wheelSlideSparks.sparkAnchors.Take(8).ToArray();
			}

			Transform spark9 = wheelSparks.Find("spark9");
			//create spark GOs for the fifth axle if they haven't been already
			if (spark9 is null)
			{
				GameObject spark9GO = new GameObject("spark9");
				spark9GO.transform.SetParent(wheelSparks);
				spark9GO.transform.localPosition = new Vector3(-0.75f, 0, 0.86f);
				spark9GO.transform.localEulerAngles = Vector3.zero;
				GameObject spark10GO = new GameObject("spark10");
				spark10GO.transform.SetParent(wheelSparks);
				spark10GO.transform.localPosition = new Vector3(0.75f, 0, 0.86f);
				spark10GO.transform.localEulerAngles = Vector3.zero;
			}
		}

		//This hides any trailing wheels, but also generates the two small trailing axles
		//if they haven't been already.
		private static void HideTrailingWheels(TrainCar loco)
		{
			//generate the rear axle supports
			GameObject firstFrontAxleSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject;
			//GameObject firstFrontAxleSupportLOD = loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject;

			Transform firstRearBogie = loco.transform.Find("Axle_R/bogie_car");
			Transform firstRearAxle = firstRearBogie.Find("[axle] 1");
			Transform secondRearAxle, thirdRearAxle;
			GameObject secondRearAxleSupport;
			Transform driveAxleTransform;
			int driveAxleCounter = 0;
			if (firstRearAxle is null)
			{   //if the first axle hasn't been renamed, rename it and generate the second and third rear axle
				for (int i = 0; i < firstRearBogie.childCount; i++)
				{
					if (firstRearBogie.GetChild(i).localPosition.z < -2.4f)
					{//if the ith rear axle is the rear front axle, grab it
						firstRearAxle = firstRearBogie.GetChild(i);
						firstRearAxle.gameObject.name = "[axle] 1";
					}
					else
					{//while we're here, grab the extra drive axles and rename them so we can easily access them later
						driveAxleTransform = firstRearBogie.GetChild(i);
						driveAxleTransform.gameObject.name = $"[axle] drive {driveAxleCounter}";
						driveAxleCounter++;
					}
				}

				//generate second and third axle
				GameObject firstFrontAxle = loco.transform.Find("Axle_F/bogie_car/[axle] 1").gameObject;
				secondRearAxle = UnityEngine.Object.Instantiate(firstFrontAxle, firstRearBogie).transform;
				secondRearAxle.gameObject.name = "[axle] 2";
				secondRearAxle.localPosition = new Vector3(0, 0.38f, -1.32f);
				secondRearAxle.transform.Find("axleF_modelLOD1").gameObject.SetActive(false);
				thirdRearAxle = UnityEngine.Object.Instantiate(secondRearAxle, firstRearBogie).transform;
				thirdRearAxle.gameObject.name = "[axle] 3";
				thirdRearAxle.localPosition = new Vector3(0, 0.38f, -2.52f);
				thirdRearAxle.transform.Find("axleF_modelLOD1").gameObject.SetActive(false);

				//also generate the second and third axle supports
				secondRearAxleSupport = UnityEngine.Object.Instantiate(firstFrontAxleSupport, firstFrontAxleSupport.transform.parent);
				secondRearAxleSupport.name = "s282_wheels_rear_support_1";
				secondRearAxleSupport.transform.localPosition = new Vector3(0, 0.38f, 9.44f);
				secondRearAxleSupport.transform.localScale = new Vector3(1f, 1.6f, 1f);
				/*secondRearAxleSupportLOD = UnityEngine.Object.Instantiate(firstFrontAxleSupportLOD, firstFrontAxleSupportLOD.transform.parent);
				secondRearAxleSupportLOD.name = "s282_wheels_rear_support_1_LOD1";
				secondRearAxleSupportLOD.transform.localPosition = new Vector3(0, 0.38f, 9.45f);
				secondRearAxleSupportLOD.transform.localScale = new Vector3(1f, 1.6f, 1f);*/
				/*thirdRearAxleSupport = UnityEngine.Object.Instantiate(firstFrontAxleSupport, firstFrontAxleSupport.transform.parent);
				thirdRearAxleSupport.name = "s282_wheels_rear_support_2";
				thirdRearAxleSupport.transform.localPosition = new Vector3(0, 0.38f, 9.44f);
				thirdRearAxleSupportLOD = UnityEngine.Object.Instantiate(firstFrontAxleSupportLOD, firstFrontAxleSupportLOD.transform.parent);
				thirdRearAxleSupportLOD.name = "s282_wheels_rear_support_2_LOD1";
				thirdRearAxleSupportLOD.transform.localPosition = new Vector3(0, 0.38f, 9.45f);*/

				//make wheels rotate
				WheelRotationViaCode[] wheelRotaters = loco.transform.Find("[wheel rotation]").GetComponents<WheelRotationViaCode>();
				foreach (WheelRotationViaCode wheelRotater in wheelRotaters)
				{
					if (wheelRotater.wheelRadius == 0.355f)
					{
						wheelRotater.transformsToRotate
							= wheelRotater.transformsToRotate
							.Append(secondRearAxle)
							.Append(thirdRearAxle)
							.ToArray();
					}
					else if (wheelRotater.wheelRadius == 0.575f)
					{
						wheelRotater.transformsToRotate = wheelRotater.transformsToRotate
							.Append(firstRearBogie.Find("[axle] 1"))
							.ToArray();
					}
				}
			}
			else
			{   //if the second rear axle is there, fetch it and hide it
				secondRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 2");
				thirdRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 3");
				GameObject firstRearAxleSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
				firstRearAxleSupport.SetActive(false);
			}

			firstRearAxle.gameObject.SetActive(false);
			secondRearAxle.gameObject.SetActive(false);
			thirdRearAxle.gameObject.SetActive(false);
		}

		private static void ShowTwoLeadingWheels(TrainCar loco)
		{
			loco.transform.Find("Axle_F/bogie_car/[axle] 1").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject.SetActive(true);
		}

		private static void ShowFourLeadingWheels(TrainCar loco)
		{
			loco.transform.Find("Axle_F/bogie_car/[axle] 1").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject.SetActive(true);
			Transform secondFrontAxle = loco.transform.Find("Axle_F/bogie_car/[axle] 2");
			secondFrontAxle.gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support_2").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_2_LOD1").gameObject.SetActive(true);
		}

		private static void AddFifthDriveAxle(TrainCar loco)
		{
			GameObject axleR = loco.transform.Find("Axle_R").gameObject;
			ConfigurableJoint axleRJoint = axleR.GetComponent<ConfigurableJoint>();
			if (axleRJoint is null)
			{
				Main.Logger.Log($"Couldn't find ConfigurableJoint on ten-coupled Axle_R.");
			}
			else
			{
				axleRJoint.autoConfigureConnectedAnchor = false;
				axleRJoint.connectedAnchor = new Vector3(axleRJoint.connectedAnchor.x, axleRJoint.connectedAnchor.y, 1.5f);
			}
			axleR.transform.localPosition =
				new Vector3(
					axleR.transform.localPosition.x,
					axleR.transform.localPosition.y,
					1.5f);

			Transform axleDrive0 = axleR.transform.Find("bogie_car/[axle] drive 0");
			//Move  ten-coupled [axle] drive 0 rearward so that it lines up with the third driver
			axleDrive0.localPosition = new Vector3(axleDrive0.localPosition.x, axleDrive0.localPosition.y, 1.7f);

			//add fifth axle to ten-coupled rear bogie
			Transform axleRDrive2 = axleR.transform.Find("bogie_car/[axle] drive 2");
			if (axleRDrive2 is null)
			{
				GameObject axleDrive2 = UnityEngine.Object.Instantiate(axleDrive0.gameObject, axleDrive0.parent);
				axleDrive2.name = "[axle] drive 2";
				axleDrive2.transform.localPosition
					= new Vector3(
						axleDrive2.transform.localPosition.x,
						axleDrive2.transform.localPosition.y,
						3.25f);
				axleRDrive2 = axleDrive2.transform;
			}
			else
			{
				axleRDrive2.gameObject.SetActive(true);
			}

			GameObject fifthLeftDriveWheel = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_wheels_driving_5").gameObject;
			GameObject fifthRightDriveWheel = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_wheels_driving_5").gameObject;
			fifthLeftDriveWheel.SetActive(true);
			fifthRightDriveWheel.SetActive(true);

			//show added side rods
			GameObject leftSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)").gameObject;
			GameObject rightSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)").gameObject;
			leftSideRod.SetActive(true);
			rightSideRod.SetActive(true);

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 5;

			//show sparks at fifth drive axle
			Transform wheelSparks = loco.transform.Find("LocoS282A_Particles/[wheel sparks]");
			if (wheelSparks is null)
			{
				return;
			}
			Transform spark9 = wheelSparks.Find("spark9");
			Transform spark10 = wheelSparks.Find("spark10");
			WheelslipSparksController wheelslipSparks = wheelSparks.GetComponent<WheelslipSparksController>();
			WheelSlideSparksController wheelSlideSparks = wheelSparks.GetComponent<WheelSlideSparksController>();
			if (wheelslipSparks is not null)
			{
				Main.Logger.Log("Adding wheelslip sparks to S282");
				var axle5Sparks = new WheelslipSparksController.WheelSparksDefinition();
				axle5Sparks.poweredWheel = axleRDrive2.GetComponent<PoweredWheel>();
				axle5Sparks.sparksLeftAnchor = spark9;
				axle5Sparks.sparksRightAnchor = spark10;
				axle5Sparks.Init();
				wheelslipSparks.wheelSparks = wheelslipSparks.wheelSparks
					.Append(axle5Sparks)
					.ToArray();
			}
			if (wheelSlideSparks is not null)
			{
				Main.Logger.Log("Adding wheel slide sparks to S282");
				wheelSlideSparks.sparkAnchors = wheelSlideSparks.sparkAnchors
					.Append(spark9)
					.Append(spark10)
					.ToArray();
				wheelSlideSparks.sparks = null;
			}
		}

		//Show the vanilla big trailing axle
		private static void ShowTwoTrailingWheelsVanilla(TrainCar loco)
		{
			GameObject firstRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 1").gameObject;
			firstRearAxle.transform.localPosition = new Vector3(0, 0.58f, -2.52f);
			firstRearAxle.SetActive(true);
		}

		//Show the small trailing axle where the vanilla trailing axle is
		private static void ShowTwoTrailingWheels(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 2").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -2.52f);
			secondRearAxle.SetActive(true);
		}

		//Show the small trailing axle at the position suggested by discord
		private static void ShowTwoTrailingWheelsAlternate(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 2").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -2.25f);
			secondRearAxle.SetActive(true);
		}

		//the ten-coupled engines (x-10-2) need a trailing axle further back than the
		//trailing axle for the eight coupled engines (x-8-2).
		private static void ShowTwoTrailingWheelsFor10Coupled(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 2").gameObject;
			GameObject rearWheelSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -1.45f);
			rearWheelSupport.transform.localPosition = new Vector3(0, 0.46f, 0.05f);
			rearWheelSupport.transform.localScale = new Vector3(1, 1, 1);
			secondRearAxle.SetActive(true);
			rearWheelSupport.SetActive(true);
		}

		//show the vanilla training wheels in the position needed for the x-10-2 engines
		private static void ShowTwoTrailingWheelsFor10CoupledVanilla(TrainCar loco)
		{
			GameObject firstRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 1").gameObject;
			GameObject rearWheelSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
			firstRearAxle.transform.localPosition = new Vector3(0, 0.58f, -1.45f);
			rearWheelSupport.transform.localPosition = new Vector3(0, 0.54f, 0.05f);
			rearWheelSupport.transform.localScale = new Vector3(1, 1.4f, 1);
			firstRearAxle.SetActive(true);
			rearWheelSupport.SetActive(true);
		}

		//Show four trailing wheels for 8-coupled engines
		private static void ShowFourTrailingWheels(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 2").gameObject;
			GameObject thirdRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 3").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -2.52f);
			thirdRearAxle.transform.localPosition = new Vector3(0, 0.36f, -1.32f);
			secondRearAxle.SetActive(true);
			thirdRearAxle.SetActive(true);
		}

		private static void ShowFourTrailingWheelsFor10Coupled(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 2").gameObject;
			GameObject thirdRearAxle = loco.transform.Find("Axle_R/bogie_car/[axle] 3").gameObject;
			GameObject rearWheelSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
			//GameObject rearWheelSupportLOD = loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_rear_support_1_LOD1").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -1.15f);
			thirdRearAxle.transform.localPosition = new Vector3(0, 0.36f, -2f);
			rearWheelSupport.transform.localPosition = new Vector3(0, 0.46f, -0.5f);
			rearWheelSupport.transform.localScale = new Vector3(1, 1, 1);
			//rearWheelSupportLOD.transform.localPosition = new Vector3(0, 0.46f, -0.45f);
			secondRearAxle.SetActive(true);
			thirdRearAxle.SetActive(true);
			rearWheelSupport.SetActive(true);
			//rearWheelSupportLOD.SetActive(true);
			/*loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_2").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_rear_support_2_LOD1").gameObject.SetActive(true);*/
		}

		private static bool validateLoco(TrainCar loco)
		{
			if (loco == null)
			{
				Main.Logger.Error("Tried to alter a null traincar.");
				return false;
			}
			if (loco.carType != TrainCarType.LocoSteamHeavy)
			{
				Main.Logger.Error("TrainCar is not an S282.");
				return false;
			}
			return true;
		}
	}
}
