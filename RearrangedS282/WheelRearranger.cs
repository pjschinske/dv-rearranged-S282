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

namespace RearrangedS282
{
	internal static class WheelRearranger
	{
		//These colliders correspond to the location of the front-most and rear-most axle.
		//They are used to alter the physics of the engine.
		private static CapsuleCollider frontAxleCollider;
		private static CapsuleCollider rearAxleCollider;

		private static Random rand = new System.Random();

		public static void SwitchWheelArrangementRandom(TrainCar loco)
		{
			int wa = rand.Next(WheelArrangement.WheelArrangementNames.Length);
			SwitchWheelArrangement(loco, wa);
		}

		public static void SwitchWheelArrangement(TrainCar loco, int wa)
		{

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
		}

		private static void findAxleColliders(TrainCar loco)
		{
			if (frontAxleCollider != null)
			{
				return;
			}
			GameObject colliderGO = loco.transform.Find("[bogie]").gameObject;
			CapsuleCollider[] colliders = colliderGO.GetComponents<CapsuleCollider>();
			foreach (CapsuleCollider collider in colliders)
			{
				if (collider.radius == 0.575f)
				{   //if collider is the front collider
					frontAxleCollider = collider;
				}
				else
				{	//if collider is the rear collider
					rearAxleCollider = collider;
				}
			}
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
			GameObject frontAxle1 = loco.transform.Find("Axle_F/bogie_car").GetComponentInChildren<MeshFilter>().gameObject;
			GameObject frontAxle2 = loco.transform.Find("Axle_F/bogie_car_2").GetComponentInChildren<MeshFilter>().gameObject;
			//GameObject rearAxle1 = loco.transform.Find("Axle_R/bogie_car").GetComponentInChildren<MeshFilter>().gameObject;
			GameObject rearAxle2 = loco.transform.Find("Axle_R/bogie_car_2").GetComponentInChildren<MeshFilter>().gameObject;
			GameObject rearAxle3 = loco.transform.Find("Axle_R/bogie_car_3").GetComponentInChildren<MeshFilter>().gameObject;

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
		}

		//This hides any leading wheels, but also generates the second leading axle
		//if it hasn't been already.
		private static void HideLeadingWheels(TrainCar loco)
		{
			//Hide first axle and its support
			GameObject firstFrontAxle = loco.transform.Find("Axle_F/bogie_car").gameObject;
			GameObject firstFrontAxleSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject;
			GameObject firstFrontAxleSupportLOD = loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject;
			firstFrontAxle.SetActive(false);
			firstFrontAxleSupport.SetActive(false);
			firstFrontAxleSupportLOD.SetActive(false);
			//Stock axle support is actually misaligned by 0.03m, this sets it to the correct position
			firstFrontAxleSupport.transform.localPosition = new Vector3(0, 0.38f, 10.54f);
			firstFrontAxleSupportLOD.transform.localPosition = new Vector3(0, 0.38f, 10.55f);

			//look for the second front axle
			Transform secondFrontAxleTransform = loco.transform.Find("Axle_F/bogie_car_2");
			GameObject secondFrontAxle, secondFrontAxleSupport, secondFrontAxleSupportLOD;
			if (secondFrontAxleTransform == null)
			{   // if the second front axle isn't there, generate it
				secondFrontAxle = UnityEngine.Object.Instantiate(firstFrontAxle, firstFrontAxle.transform.parent);
				secondFrontAxle.name = "bogie_car_2";
				secondFrontAxle.transform.localPosition = new Vector3(0, 0.01f, -1.1f);

				//also generate the wheel support for the second front axle
				secondFrontAxleSupport = UnityEngine.Object.Instantiate(firstFrontAxleSupport, firstFrontAxleSupport.transform.parent);
				secondFrontAxleSupport.name = "s282_wheels_front_support_2";
				secondFrontAxleSupport.transform.localPosition = new Vector3(0, 0.38f, 9.44f);
				secondFrontAxleSupport.SetActive(false);
				secondFrontAxleSupportLOD = UnityEngine.Object.Instantiate(firstFrontAxleSupportLOD, firstFrontAxleSupportLOD.transform.parent);
				secondFrontAxleSupportLOD.name = "s282_wheels_front_support_2_LOD1";
				secondFrontAxleSupportLOD.transform.localPosition = new Vector3(0, 0.38f, 9.45f);
				secondFrontAxleSupportLOD.SetActive(false);

				//GameObject thirdAxle = UnityEngine.Object.Instantiate(firstFrontAxle, firstFrontAxle.transform.parent);
				//thirdAxle.name = "bogie_car_3";

				//get the second front axle to rotate
				WheelRotationViaCode[] wheelRotaters = loco.transform.Find("[wheel rotation]").GetComponents<WheelRotationViaCode>();
				foreach (WheelRotationViaCode wheelRotater in wheelRotaters)
				{
					if (wheelRotater.wheelRadius == 0.355f)
					{
						for (int i = 0; i < secondFrontAxle.transform.childCount; i++)
						{
							if (secondFrontAxle.transform.GetChild(i).GetComponent<PoweredWheel>() == null)
							{
								Transform realSecondAxle = secondFrontAxle.transform.GetChild(i);
								wheelRotater.transformsToRotate = wheelRotater.transformsToRotate.Append(realSecondAxle).ToArray();
							}
						}
					}
				}
			}
			else
			{   // if the second front axle is there, fetch it and hide it
				secondFrontAxle = secondFrontAxleTransform.gameObject;
				secondFrontAxle.SetActive(false);
				secondFrontAxleSupport = loco.transform.Find("LocoS282Body/Static_LOD0/s282_wheels_front_support_2").gameObject;
				secondFrontAxleSupport.SetActive(false);
			}
		}

		private static void RemoveFifthDriveAxle(TrainCar loco)
		{
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
				fifthLeftDriveWheel.transform.localPosition = new Vector3(0, 0.72f, 1.5f);
				fifthRightDriveWheel.transform.localPosition = new Vector3(0, 0.72f, 1.5f);
				//hide LODs
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
				//hide LODs
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
				fifthLeftDriveWheel = fifthLeftDriveWheelTransform.gameObject;
				fifthRightDriveWheel = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_wheels_driving_5").gameObject;
				if (fifthRightDriveWheel is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: fifthRightDriveWheel is null");

				Transform leftSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
				Transform rightSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_mech_wheels_connect/s282_mech_wheels_connect(Clone)");
				if (leftSideRod is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: leftSideRod clone not found");
				if (rightSideRod is null)
					Main.Logger.Error("Error: RemoveFifthDriveAxle: rightSideRod clone not found");
				leftSideRod.gameObject.SetActive(false);
				rightSideRod.gameObject.SetActive(false);
			}
			fifthLeftDriveWheel.SetActive(false);
			fifthRightDriveWheel.SetActive(false);
			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 4;
		}

		//This hides any trailing wheels, but also generates the two small trailing axles
		//if they haven't been already.
		private static void HideTrailingWheels(TrainCar loco)
		{
			//generate the rear axle supports
			GameObject firstFrontAxleSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject;
			//GameObject firstFrontAxleSupportLOD = loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject;

			//hide first rear axle
			GameObject firstRearAxle = loco.transform.Find("Axle_R/bogie_car").gameObject;
			firstRearAxle.SetActive(false);

			//look for the second rear axle
			GameObject secondRearAxle, thirdRearAxle,
				secondRearAxleSupport/*, secondRearAxleSupportLOD,
				thirdRearAxleSupport, thirdRearAxleSupportLOD*/;
			Transform secondRearAxleTransform = loco.transform.Find("Axle_R/bogie_car_2");
			if (secondRearAxleTransform == null)
			{	//if the second rear axle isn't there, generate it
				GameObject firstFrontAxle = loco.transform.Find("Axle_F/bogie_car").gameObject;
				secondRearAxle = UnityEngine.Object.Instantiate(firstFrontAxle, firstRearAxle.transform.parent);
				secondRearAxle.name = "bogie_car_2";
				secondRearAxle.transform.localPosition = new Vector3(0, 0.01f, 0.95f);
				thirdRearAxle = UnityEngine.Object.Instantiate(secondRearAxle, secondRearAxle.transform.parent);
				thirdRearAxle.name = "bogie_car_3";
				thirdRearAxle.transform.localPosition = new Vector3(0, 0.01f, 0);

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


				//get the second rear axle to rotate
				WheelRotationViaCode[] wheelRotaters = loco.transform.Find("[wheel rotation]").GetComponents<WheelRotationViaCode>();
				foreach (WheelRotationViaCode wheelRotater in wheelRotaters)
				{
					if (wheelRotater.wheelRadius == 0.355f)
					{
						//found the wheel rotater, now we need to find the second axle
						for (int i = 0; i < secondRearAxle.transform.childCount; i++)
						{
							if (secondRearAxle.transform.GetChild(i).GetComponent<PoweredWheel>() == null)
							{
								Transform realSecondAxle = secondRearAxle.transform.GetChild(i);
								wheelRotater.transformsToRotate = wheelRotater.transformsToRotate.Append(realSecondAxle).ToArray();
							}
						}

						//now finding the third axle and adding it to the rotater
						for (int i = 0; i < thirdRearAxle.transform.childCount; i++)
						{
							if (thirdRearAxle.transform.GetChild(i).GetComponent<PoweredWheel>() == null)
							{
								Transform realThirdAxle = thirdRearAxle.transform.GetChild(i);
								wheelRotater.transformsToRotate = wheelRotater.transformsToRotate.Append(realThirdAxle).ToArray();
							}
						}
					}
				}
			}
			else
			{	//if the second rear axle is there, fetch it and hide it
				secondRearAxle = secondRearAxleTransform.gameObject;
				secondRearAxle.SetActive(false);
				thirdRearAxle = loco.transform.Find("Axle_R/bogie_car_3").gameObject;
				thirdRearAxle.SetActive(false);
				GameObject firstRearAxleSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
				firstRearAxleSupport.SetActive(false);
			}
		}

		private static void ShowTwoLeadingWheels(TrainCar loco)
		{
			loco.transform.Find("Axle_F/bogie_car").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject.SetActive(true);
		}

		private static void ShowFourLeadingWheels(TrainCar loco)
		{
			loco.transform.Find("Axle_F/bogie_car").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1").gameObject.SetActive(true);
			loco.transform.Find("Axle_F/bogie_car_2").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support_2").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_2_LOD1").gameObject.SetActive(true);

			//loco.transform.Find("Axle_F/bogie_car_3").gameObject.SetActive(true);
		}

		private static void AddFifthDriveAxle(TrainCar loco)
		{
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
			//also maybe add fifth [axle], add to [poweredWheels]
		}

		//Show the vanilla big trailing axle
		private static void ShowTwoTrailingWheelsVanilla(TrainCar loco)
		{
			GameObject firstRearAxle = loco.transform.Find("Axle_R/bogie_car").gameObject;
			firstRearAxle.transform.localPosition = new Vector3(0, 0.01f, 0);
			firstRearAxle.SetActive(true);
		}

		//Show the small trailing axle where the vanilla trailing axle is
		private static void ShowTwoTrailingWheels(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car_2").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.01f, -5.08f);
			secondRearAxle.SetActive(true);
		}

		//Show the small trailing axle at the position suggested by discord
		private static void ShowTwoTrailingWheelsAlternate(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car_2").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.01f, -4.9f);
			secondRearAxle.SetActive(true);
		}

		//the ten-coupled engines (x-10-2) need a trailing axle further back than the
		//trailing axle for the eight coupled engines (x-8-2).
		private static void ShowTwoTrailingWheelsFor10Coupled(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car_2").gameObject;
			GameObject rearWheelSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0.01f, -5.6f);
			rearWheelSupport.transform.localPosition = new Vector3(0, 0.46f, 0.1f);
			secondRearAxle.SetActive(true);
			rearWheelSupport.SetActive(true);
		}

		//show the vanilla training wheels in the position needed for the x-10-2 engines
		private static void ShowTwoTrailingWheelsFor10CoupledVanilla(TrainCar loco)
		{
			GameObject firstRearAxle = loco.transform.Find("Axle_R/bogie_car").gameObject;
			GameObject rearWheelSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
			rearWheelSupport.transform.localPosition = new Vector3(0, 0.46f, 0.1f);
			firstRearAxle.transform.localPosition = new Vector3(0, 0.01f, -0.56f);
			firstRearAxle.SetActive(true);
			rearWheelSupport.SetActive(true);
		}

		//Show four trailing wheels for 8-coupled engines
		private static void ShowFourTrailingWheels(TrainCar loco)
		{

			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car_2").gameObject;
			GameObject thirdRearAxle = loco.transform.Find("Axle_R/bogie_car_3").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0, -3.883f);
			thirdRearAxle.transform.localPosition = new Vector3(0, 0.01f, -5.08f);
			secondRearAxle.SetActive(true);
			thirdRearAxle.SetActive(true);
		}

		private static void ShowFourTrailingWheelsFor10Coupled(TrainCar loco)
		{
			GameObject secondRearAxle = loco.transform.Find("Axle_R/bogie_car_2").gameObject;
			GameObject thirdRearAxle = loco.transform.Find("Axle_R/bogie_car_3").gameObject;
			GameObject rearWheelSupport = loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_rear_support_1").gameObject;
			//GameObject rearWheelSupportLOD = loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_rear_support_1_LOD1").gameObject;
			secondRearAxle.transform.localPosition = new Vector3(0, 0, -5.4f);
			thirdRearAxle.transform.localPosition = new Vector3(0, 0.01f, -6.25f);
			rearWheelSupport.transform.localPosition = new Vector3(0, 0.46f, -0.5f);
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
