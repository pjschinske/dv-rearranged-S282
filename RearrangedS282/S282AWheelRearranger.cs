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
using DV.Simulation.Ports;
using DV.Simulation.Cars;
using DV.Logic.Job;
using System.Data;
using LocoSim.Implementations;
using DV.Simulation.Brake;
using HarmonyLib;
using DV.VRTK_Extensions;
using LocoMeshSplitter.MeshLoaders;
using DV.Simulation.Controllers;
using static DV.Simulation.Controllers.CylinderCockParticlePortReader;
using static DV.Wheels.PoweredWheelRotationViaAnimation;
using DV.Damage;
using UnityEngine.Events;
using DV;
using System.Runtime.ConstrainedExecution;
using RearrangedS282.Sim.SimDuplex;

//abandon all hope ye who enter here

namespace RearrangedS282
{
	internal class S282AWheelRearranger : MonoBehaviour
	{
		private TrainCar loco;
		private S282AMeshLoader splitS282A;

		private Transform
			splitLoco,
			frontBogie,
			firstFrontAxle,
			firstFrontAxleSupport,
			firstFrontAxleSupportLOD,
			secondFrontAxle,
			secondFrontAxleSupport,
			secondFrontAxleSupportLOD,

			driveL,
			driveR,
			firstLeftDriveWheel,
			firstRightDriveWheel,
			secondLeftDriveWheel,
			secondRightDriveWheel,
			thirdLeftDriveWheel,
			thirdRightDriveWheel,
			fourthLeftDriveWheel,
			fourthRightDriveWheel,
			fifthLeftDriveWheel,
			fifthRightDriveWheel,
			sixthLeftDriveWheel,
			sixthRightDriveWheel,

			leftSideRod,
			rightSideRod,
			leftTwoAxleSideRod,
			rightTwoAxleSideRod,
			leftTwoAxleDuplexSideRod,
			rightTwoAxleDuplexSideRod,
			lfThreeAxleSideRod,
			rfThreeAxleSideRod,
			lrThreeAxleSideRod,
			rrThreeAxleSideRod,
			leftFiveAxleSideRod,
			rightFiveAxleSideRod,
			leftSixAxleSideRod,
			rightSixAxleSideRod,
			leftReachRod,
			rightReachRod,
			leftMainRod,
			rightMainRod,
			leftCrosshead,
			rightCrosshead,
			frontHandrail,

			leftFirstBrakeCaliper,
			rightFirstBrakeCaliper,
			leftSecondBrakeCaliper,
			rightSecondBrakeCaliper,
			leftThirdBrakeCaliper,
			rightThirdBrakeCaliper,
			leftFourthBrakeCaliper,
			rightFourthBrakeCaliper,
			leftFifthBrakeCaliper,
			rightFifthBrakeCaliper,
			leftSixthBrakeCaliper,
			rightSixthBrakeCaliper,

			rightBrakeShoes,
			leftBrakeShoes,
			leftFirstBrakeShoe,
			rightFirstBrakeShoe,
			leftSecondBrakeShoe,
			rightSecondBrakeShoe,
			leftThirdBrakeShoe,
			rightThirdBrakeShoe,
			leftFourthBrakeShoe,
			rightFourthBrakeShoe,
			leftFifthBrakeShoe,
			rightFifthBrakeShoe,
			leftSixthBrakeShoe,
			rightSixthBrakeShoe,

			//for duplex
			driveL2,
			driveR2,
			leftSeventhDriveWheel,
			rightSeventhDriveWheel,
			lfCrossheadBracket,
			rfCrossheadBracket,
			lLiftingArmSupport,
			rLiftingArmSupport,
			lrCrossheadBracket,
			rrCrossheadBracket,
			lfFranklinValveGear,
			rfFranklinValveGear,
			lfFranklinValveGear4444,
			rfFranklinValveGear4444,
			lBranchPipe,
			rBranchPipe,
			lBranchPipe4444,
			rBranchPipe4444,
			lFranklinBReverseGear,
			rFranklinBReverseGear,
			lFranklinBReverseGear4444,
			rFranklinBReverseGear4444,

			//lubrication
			lubricator,
			lubricatorSightGlass,
			lubricatorHandle,
			lubricatorSupport,
			lubricatorLabel,
			lubricatorOilLevel,
			rOilLines,
			fOilLines,

			//air
			lAirTank,
			rAirTank,
			airPump,
			airPumpInputPipe,
			airPumpOutputPipe,
			airValveLabel,

			lfCylinder,
			rfCylinder,
			cylinderCocks,
			lDryPipe,
			rDryPipe,

			lCrosshead,
			rCrosshead,

			reachRodSupport,

			/*
			 * These drive axles are misleadingly named.
			 * 
			 * For a 10-coupled loco, the whole rear bogie gets moved back so that driveAxle4
			 * lines up with the fifth drive wheels.
			 * For a 12-coupled loco:
			 *	- driveAxle4 lines up with the sixth drivers
			 *	- driveAxle3 lines up with the fifth drivers
			 *	- driveAxle5 lines up with the fourth drivers
			 *	- driveAxle6 lines up with the third drivers
			 *	
			 * So driveAxle3 and 4 are always the last two drivers, and driveAxle5 and 6
			 * are added to the middle as needed.
			 */
			driveAxle1,
			driveAxle2,
			driveAxle3,
			driveAxle4,
			driveAxle5,
			driveAxle6,

			//wheel spark locations
			spark1L,
			spark2L,
			spark3L,
			spark4L,
			spark5L,
			spark6L,
			spark1R,
			spark2R,
			spark3R,
			spark4R,
			spark5R,
			spark6R,

			//duplex wheel spark locations
			sparkD1L,
			sparkD2L,
			sparkD3L,
			sparkD4L,
			sparkD1R,
			sparkD2R,
			sparkD3R,
			sparkD4R,

			rearBogie,
			firstRearAxle,
			secondRearAxle,
			secondRearAxleSupport,
			thirdRearAxle,

			//cylinder cock particle effects
			oldCylCockWaterDrip,
			oldCylCockSteam,
			rearCylCockWaterDrip,
			rearCylCockSteam,

			//sounds
			valveGearMechanism,

			duplexSteamEngine,
			duplexTraction,
			oldSim,
			duplexSim;

		private HingeJoint lubricatorHandValveJoint, airValveJoint;

		SimController oldSimCtrlr;
		DuplexSimController duplexSimCtrlr;

		PoweredWheelRotationViaAnimation oldDrivetrainRotater,
			duplexDrivetrainRotaterF, duplexDrivetrainRotaterR;

		private ReciprocatingSteamEngine steamEngine, newSteamEngine;

		private Mesh flangedDriver, blindDriver, blindDriverBigWeight,
			mainRod, shortMainRod,
			lCrossheadGuide, lDuplexCrossheadGuide,
			rCrossheadGuide, rDuplexCrossheadGuide;

		//This stuff will be used to switch between the default valve gear animation,
		//and our franklin valve gear animation.
		//We switch between the two by setting Animator.runtimeAnimatorController to
		//one or the other.
		private RuntimeAnimatorController
			lfDefaultAnimatorCtrl,
			rfDefaultAnimatorCtrl,
			lfFranklinAnimatorCtrl,
			rfFranklinAnimatorCtrl,
			lfFranklinAnimatorCtrl4444,
			rfFranklinAnimatorCtrl4444;

		private Animator lfAnimator, rfAnimator, lrAnimator, rrAnimator;

		private bool skipWheelArrangementChange = false;

		private static readonly Random rand = new();

		public S282AWheelArrangementType currentWA
		{get; private set; }

		private float derailModifier;

		public float GetDerailModifier()
		{
			return derailModifier;
		}

		void Start()
		{
			loco = GetComponent<TrainCar>();
			splitS282A = GetComponent<S282AMeshLoader>();

			if (loco == null )
			{
				Main.Logger.Error("S282AWheelRearranger initialized on something that isn't a train car");
				return;
			}
			if (loco.carType != TrainCarType.LocoSteamHeavy)
			{
				Main.Logger.Error("A282AWheelRearranger initialized on something that isn't an S282A");
				return;
			}

			if (splitS282A == null)
			{
				Main.Logger.Error("WheelRearranger can't find S282AMeshLoader");
				return;
			}

			SetupBody();
			SetupLubrication();
			SetupAir();
			SetupFrontBogie();
			SetupRearBogie();
			SetupDrivers();

			//set locomotive to initial wheel arrangement
			if (Main.settings.spawnRandomWA)
			{
				SwitchWheelArrangementRandom();
			}
			else
			{
				SwitchWheelArrangement((int)S282AWheelArrangementType.s282);
			}
		}

		//Sets the wheel radius in the sim
		private void setWheelRadius(float radius)
		{
			Transform traction = transform.Find("[sim]/traction");
			TractionPortsFeeder tractionPortsFeeder = traction.GetComponent<TractionPortsFeeder>();
			DrivingForce drivingForce = traction.GetComponent<DrivingForce>();
			if (tractionPortsFeeder is null)
			{
				Main.Logger.Error("Couldn't find TractionPortsFeeder");
				return;
			}
			if (drivingForce is null)
			{
				Main.Logger.Error("Couldn't find DrivingForce");
				return;
			}
			//These three lines basically redo the math that is done in the Init method of TractionPortsFeeder
			float num = 2f * radius * (float)Math.PI;
			tractionPortsFeeder.speedMsToWheelRpmConst = 60f / num;
			tractionPortsFeeder.wheelRpmToSpeedKmhConst = 1f / tractionPortsFeeder.speedMsToWheelRpmConst * 3.6f;

			drivingForce.wheelRadius = radius;
		}

		private void SetupBody()
		{
			splitLoco = transform.Find("LocoS282A_Body/Static_LOD0/s282a_split_body(Clone)");
			lfCrossheadBracket = splitLoco.Find("s282a_crosshead_guide_l");
			rfCrossheadBracket = splitLoco.Find("s282a_crosshead_guide_r");
			lCrossheadGuide = lfCrossheadBracket.GetComponent<MeshFilter>().mesh;
			rCrossheadGuide = rfCrossheadBracket.GetComponent<MeshFilter>().mesh;
			lDuplexCrossheadGuide = MeshFinder.alterCrossheadGuideLMesh(lCrossheadGuide);
			rDuplexCrossheadGuide = MeshFinder.alterCrossheadGuideRMesh(rCrossheadGuide);

			lrCrossheadBracket = Instantiate(lfCrossheadBracket.gameObject, splitLoco).transform;
			rrCrossheadBracket = Instantiate(rfCrossheadBracket.gameObject, splitLoco).transform;
			lrCrossheadBracket.GetComponent<MeshFilter>().mesh = lDuplexCrossheadGuide;
			rrCrossheadBracket.GetComponent<MeshFilter>().mesh = rDuplexCrossheadGuide;

			lrCrossheadBracket.gameObject.SetActive(false);
			rrCrossheadBracket.gameObject.SetActive(false);
			lrCrossheadBracket.gameObject.name = "s282a_crosshead_guide_lr";
			rrCrossheadBracket.gameObject.name = "s282a_crosshead_guide_rr";
			lrCrossheadBracket.localPosition += new Vector3(0, 0.07f, -4.58f);
			rrCrossheadBracket.localPosition += new Vector3(0, 0.07f, -4.58f);

			lLiftingArmSupport = splitLoco.Find("s282a_lifting_arm_support_l");
			rLiftingArmSupport = splitLoco.Find("s282a_lifting_arm_support_r");
			reachRodSupport = splitLoco.Find("s282a_reach_rod_support");

			lfCylinder = splitLoco.Find("s282a_cylinder_l");
			rfCylinder = splitLoco.Find("s282a_cylinder_r");
			cylinderCocks = splitLoco.Find("s282a_cylinder_cocks");

			lDryPipe = splitLoco.Find("s282a_dry_pipe_l");
			rDryPipe = splitLoco.Find("s282a_dry_pipe_r");

			frontHandrail = splitLoco.Find("s282a_handrail_f");
		}

		private void SetupLubrication()
		{
			lubricator = splitLoco.Find("s282a_lubricator");
			lubricatorSupport = splitLoco.Find("s282a_lubricator_support");
			lubricatorSightGlass = transform.Find("LocoS282A_Body/Static_LOD0/s282_gauge_lubricator_glass");
			lubricatorHandle = lubricatorSightGlass.parent.Find("s282_handle_lubricator");
			rOilLines = splitLoco.Find("s282a_oil_lines_r");
			fOilLines = splitLoco.Find("s282a_oil_lines_f");
			loco.ExternalInteractableLoaded += LoadExternalInteractables;
			loco.ExternalInteractableAboutToBeUnloaded += UnloadExternalInteractables;
		}

		private void LoadExternalInteractables(GameObject externalInteractables)
		{
			//if externalInteractables is null, this function has been called because
			//the dummy ExternalInteractables have been spawned.
			//The dummy ExternalInteractables are just the regular ones but without the
			//scripts, to save resources when locomotives are far away.
			//But if the locomotives are that far away, no one is going to notice a
			//lubricator valve in the wrong place. So we don't need to bother moving them.
			if (externalInteractables is null) {
				return;
			}

			lubricatorLabel = externalInteractables.transform.Find("LocoS282A_ExternalInteractables_Labels/Lubricator");
			if (lubricatorLabel is null)
			{
				Main.Logger.Error("No lubricator label found");
			}
			lubricatorOilLevel = externalInteractables.transform.Find("Gauges");
			if (lubricatorOilLevel is null)
			{
				Main.Logger.Error("No lubricator oil level found");
			}

			lubricatorHandValveJoint = externalInteractables.transform
				.Find("Interactables/C_Lubricator")
				.GetComponent<HingeJoint>();
			if (lubricatorHandValveJoint is null)
			{
				Main.Logger.Error("No lubricator valve HingeJoint found");
			}
			lubricatorHandValveJoint.autoConfigureConnectedAnchor = false;

			airValveLabel = externalInteractables.transform.Find("LocoS282A_ExternalInteractables_Labels/AirPump");
			if (airValveLabel is null)
			{
				Main.Logger.Error("No air valve label found");
			}
			airValveJoint = externalInteractables.transform
				.Find("Interactables/C_BrakeCompressor")
				.GetComponent<HingeJoint>();
			if (airValveJoint is null)
			{
				Main.Logger.Error("No air pump hand valve HingeJoint found");
			}
			airValveJoint.autoConfigureConnectedAnchor = false;

			//show correct lubricator placement depending on wheel arrangement
			lubricatorLabel.localPosition += lubricator.localPosition;
			lubricatorOilLevel.localPosition = lubricator.localPosition;
			lubricatorHandValveJoint.connectedAnchor += lubricator.localPosition;

			//also move the air pump hand valve and label as well
			airValveLabel.localPosition += airPumpInputPipe.localPosition;
			airValveJoint.connectedAnchor += airPumpInputPipe.localPosition;
		}

		private void UnloadExternalInteractables(GameObject externalInteractables)
		{
			lubricatorLabel = null;
			lubricatorOilLevel = null;
			lubricatorHandValveJoint = null;
			airValveLabel = null;
			airValveJoint = null;
		}

		private void SetupAir()
		{
			lAirTank = splitLoco.Find("s282a_air_tank_l");
			rAirTank = splitLoco.Find("s282a_air_tank_r");
			airPump = splitLoco.Find("s282a_air_pump");
			airPumpInputPipe = splitLoco.Find("s282a_air_pump_input_pipe");
			airPumpOutputPipe = splitLoco.Find("s282a_air_pump_output_pipe");
		}

		private void SetupFrontBogie()
		{
			//Hide first axle and its support
			frontBogie = transform.Find("Axle_F/bogie_car");
			firstFrontAxle = frontBogie.Find("[axle] 1");
			firstFrontAxleSupport = transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support");
			firstFrontAxleSupportLOD = transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_LOD1");
			firstFrontAxleSupport.gameObject.SetActive(false);
			firstFrontAxleSupportLOD.gameObject.SetActive(false);

			//Stock axle support is actually misaligned by 0.03m, this sets it to the correct position
			firstFrontAxleSupport.localPosition = new Vector3(0, 0.38f, 10.54f);
			firstFrontAxleSupportLOD.localPosition = new Vector3(0, 0.38f, 10.55f);

			//rename front axles to something reasonable
			Transform driveAxleTransform = null;
			int driveAxleCounter = 0;
			for (int i = 0; i < frontBogie.transform.childCount; i++)
			{
				if (frontBogie.transform.GetChild(i).localPosition.z > 2.4)
				{//if the ith front axle is the first front axle, grab it
					firstFrontAxle = frontBogie.transform.GetChild(i);
					firstFrontAxle.gameObject.name = "[axle] 1";
					firstFrontAxle.localPosition = new Vector3(firstFrontAxle.localPosition.x, 0.36f, firstFrontAxle.localPosition.z);
				}
				else
				{//while we're here, grab the extra drive axles and rename them so we can easily access them later
					driveAxleTransform = frontBogie.GetChild(i);
					driveAxleTransform.gameObject.name = $"[axle] drive {driveAxleCounter}";
					driveAxleCounter++;
				}
			}
			driveAxle1 = frontBogie.Find("[axle] drive 0");
			driveAxle2 = frontBogie.Find("[axle] drive 1");

			//Generate the second front axle
			GameObject secondFrontAxleGO = Instantiate(firstFrontAxle.gameObject, firstFrontAxle.parent);
			secondFrontAxleGO.SetActive(false);
			secondFrontAxleGO.name = "[axle] 2";
			secondFrontAxle = secondFrontAxleGO.transform;
			secondFrontAxle.localPosition = new Vector3(0, 0.36f, 1.45f);
			secondFrontAxle.Find("axleF_modelLOD1").gameObject.SetActive(false);

			//also generate the wheel support for the second front axle
			secondFrontAxleSupport = Instantiate(firstFrontAxleSupport.gameObject, firstFrontAxleSupport.parent).transform;
			secondFrontAxleSupport.gameObject.SetActive(false);
			secondFrontAxleSupport.gameObject.name = "s282_wheels_front_support_2";
			secondFrontAxleSupport.localPosition = new Vector3(0, 0.38f, 9.44f);
			secondFrontAxleSupportLOD = Instantiate(firstFrontAxleSupportLOD.gameObject, firstFrontAxleSupportLOD.parent).transform;
			secondFrontAxleSupportLOD.gameObject.SetActive(false);
			secondFrontAxleSupportLOD.gameObject.name = "s282_wheels_front_support_2_LOD1";
			secondFrontAxleSupportLOD.localPosition = new Vector3(0, 0.38f, 9.45f);
			
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

		//Should be run after SetupRearBogie()
		private void SetupDrivers()
		{
			steamEngine = GetComponent<SimController>().simFlow.orderedSimComps.OfType<ReciprocatingSteamEngine>().First();
			newSteamEngine = GetComponent<SimController>().simFlow.orderedSimComps.OfType<ReciprocatingSteamEngine>().Last();

			driveL = transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L");
			driveR = transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R");

			//I refuse to use Altfuture's names for the valve gear parts
			lCrosshead = driveL.Find("s282_mech_push_rod");
			rCrosshead = driveR.Find("s282_mech_push_rod");

			lfFranklinValveGear = Instantiate(AssetBundleLoader.FranklinBValveGear, driveL).transform;
			rfFranklinValveGear = Instantiate(AssetBundleLoader.FranklinBValveGear, driveR).transform;

			lfAnimator = driveL.GetComponent<Animator>();
			rfAnimator = driveR.GetComponent<Animator>();
			lfDefaultAnimatorCtrl = lfAnimator.runtimeAnimatorController;
			rfDefaultAnimatorCtrl = rfAnimator.runtimeAnimatorController;
			lfFranklinAnimatorCtrl = Instantiate(AssetBundleLoader.FranklinBAnimCtrl);
			rfFranklinAnimatorCtrl = Instantiate(AssetBundleLoader.FranklinBAnimCtrl);
			lfFranklinAnimatorCtrl4444 = Instantiate(AssetBundleLoader.FranklinBAnimCtrl4444);
			rfFranklinAnimatorCtrl4444 = Instantiate(AssetBundleLoader.FranklinBAnimCtrl4444);

			driveL2 = Instantiate(driveL.gameObject, driveL.parent).transform;
			driveL2.gameObject.name = "DriveMechanism LR";
			driveR2 = Instantiate(driveR.gameObject, driveR.parent).transform;
			driveR2.gameObject.name = "DriveMechanism RR";
			driveL2.gameObject.SetActive(false);
			driveR2.gameObject.SetActive(false);
			driveL2.localPosition = new Vector3(0, 0, -2.75f);
			driveR2.localPosition = driveL2.localPosition;
			driveL2.localScale = new Vector3(-1, 0.8f, 0.8f);
			driveR2.localScale = new Vector3(1, 0.8f, 0.8f);

			lrAnimator = driveL2.GetComponent<Animator>();
			rrAnimator = driveR2.GetComponent<Animator>();
			lrAnimator.runtimeAnimatorController =
				Instantiate(AssetBundleLoader.FranklinBAnimCtrl);
			rrAnimator.runtimeAnimatorController =
				Instantiate(AssetBundleLoader.FranklinBAnimCtrl);

			lfFranklinValveGear4444 = Instantiate(AssetBundleLoader.FranklinBValveGear4444, driveL).transform;
			rfFranklinValveGear4444 = Instantiate(AssetBundleLoader.FranklinBValveGear4444, driveR).transform;

			lBranchPipe = Instantiate(AssetBundleLoader.BranchPipe, driveL2).transform;
			rBranchPipe = Instantiate(AssetBundleLoader.BranchPipe, driveR2).transform;
			lBranchPipe4444 = Instantiate(AssetBundleLoader.BranchPipe4444, driveL2).transform;
			rBranchPipe4444 = Instantiate(AssetBundleLoader.BranchPipe4444, driveR2).transform;

			lFranklinBReverseGear = Instantiate(AssetBundleLoader.FranklinBReverseGear, driveL2).transform;
			rFranklinBReverseGear = Instantiate(AssetBundleLoader.FranklinBReverseGear, driveR2).transform;
			lFranklinBReverseGear4444 = Instantiate(AssetBundleLoader.FranklinBReverseGear4444, driveL2).transform;
			rFranklinBReverseGear4444 = Instantiate(AssetBundleLoader.FranklinBReverseGear4444, driveR2).transform;
			Instantiate(AssetBundleLoader.FranklinBReverseGearRear, driveR2);

			//Hide extra parts
			driveL2.Find("s282_mech_cutoff_boomerang_high").gameObject.SetActive(false);
			driveL2.Find("s282_mech_cutoff_rail").gameObject.SetActive(false);
			driveL2.Find("s282_mech_cutoff_rod").gameObject.SetActive(false);
			driveL2.Find("s282_mech_cutoff_rod_hinge").gameObject.SetActive(false);
			driveL2.Find("s282_mech_cylinder_rod").gameObject.SetActive(false);
			driveL2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_rod").gameObject.SetActive(false);
			driveL2.Find("s282a_brake_shoes(Clone)").gameObject.SetActive(false);
			driveR2.Find("s282_mech_cutoff_boomerang_high").gameObject.SetActive(false);
			driveR2.Find("s282_mech_cutoff_rail").gameObject.SetActive(false);
			driveR2.Find("s282_mech_cutoff_rod").gameObject.SetActive(false);
			driveR2.Find("s282_mech_cutoff_rod_hinge").gameObject.SetActive(false);
			driveR2.Find("s282_mech_cylinder_rod").gameObject.SetActive(false);
			driveR2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_rod").gameObject.SetActive(false);
			driveR2.Find("s282a_brake_shoes(Clone)").gameObject.SetActive(false);
			driveR2.Find("LubricatorParts").gameObject.SetActive(false);

			//Hide LODs
			driveL2.Find("s282_wheels_driving_1").GetChild(0).gameObject.SetActive(false);
			driveL2.Find("s282_wheels_driving_2").GetChild(0).gameObject.SetActive(false);
			driveL2.Find("s282_wheels_driving_3").GetChild(0).gameObject.SetActive(false);
			driveL2.Find("s282_wheels_driving_4").GetChild(0).gameObject.SetActive(false);
			driveL2.Find("s282_mech_wheels_connect/s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			driveL2.Find("s282_mech_wheels_connect/s282_mech_push_rod_to_connect").GetChild(0).gameObject.SetActive(false);
			driveL2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_base_LOD1").gameObject.SetActive(false);
			driveR2.Find("s282_mech_wheels_connect/s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			driveR2.Find("s282_mech_wheels_connect/s282_mech_push_rod_to_connect").GetChild(0).gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_1").GetChild(0).gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_2").GetChild(0).gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_3").GetChild(0).gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_4").GetChild(0).gameObject.SetActive(false);
			driveR2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_base_LOD1").gameObject.SetActive(false);

			leftSeventhDriveWheel = driveL2.Find("s282_wheels_driving_4");
			rightSeventhDriveWheel = driveR2.Find("s282_wheels_driving_4");
			leftSeventhDriveWheel.gameObject.SetActive(false);
			rightSeventhDriveWheel.gameObject.SetActive(false);

			leftReachRod = driveL.Find("s282_mech_cutoff_rod");
			rightReachRod = driveR.Find("s282_mech_cutoff_rod");

			firstLeftDriveWheel = driveL.Find("s282_wheels_driving_1");
			firstRightDriveWheel = driveR.Find("s282_wheels_driving_1");

			secondLeftDriveWheel = driveL.Find("s282_wheels_driving_2");
			secondRightDriveWheel = driveR.Find("s282_wheels_driving_2");

			thirdLeftDriveWheel = driveL.Find("s282_wheels_driving_3");
			thirdRightDriveWheel = driveR.Find("s282_wheels_driving_3");

			fourthLeftDriveWheel = driveL.Find("s282_wheels_driving_4");
			fourthRightDriveWheel = driveR.Find("s282_wheels_driving_4");

			//generate the fifth drive axle
			fifthLeftDriveWheel = Instantiate(fourthLeftDriveWheel.gameObject, fourthLeftDriveWheel.parent).transform;
			fifthRightDriveWheel = Instantiate(fourthRightDriveWheel.gameObject, fourthRightDriveWheel.parent).transform;
			fifthLeftDriveWheel.gameObject.name = "s282_wheels_driving_5";
			fifthRightDriveWheel.gameObject.name = "s282_wheels_driving_5";
			fifthLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 1.65f);
			fifthRightDriveWheel.localPosition = new Vector3(0, 0.71f, 1.65f);

			//hide fifth drive axle LODs
			fifthLeftDriveWheel.transform.GetChild(0).gameObject.SetActive(false);
			fifthRightDriveWheel.transform.GetChild(0).gameObject.SetActive(false);

			//generate the sixth drive axle
			sixthLeftDriveWheel = Instantiate(fourthLeftDriveWheel.gameObject, fourthLeftDriveWheel.parent).transform;
			sixthRightDriveWheel = Instantiate(fourthRightDriveWheel.gameObject, fourthRightDriveWheel.parent).transform;
			sixthLeftDriveWheel.gameObject.name = "s282_wheels_driving_6";
			sixthRightDriveWheel.gameObject.name = "s282_wheels_driving_6";
			sixthLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 0.1f);
			sixthRightDriveWheel.localPosition = new Vector3(0, 0.71f, 0.1f);

			//hide sixth drive axle LODs
			sixthLeftDriveWheel.transform.GetChild(0).gameObject.SetActive(false);
			sixthRightDriveWheel.transform.GetChild(0).gameObject.SetActive(false);

			//get fifth and sixth drive axle to rotate
			fifthLeftDriveWheel.gameObject.AddComponent<PoweredWheelRotater>().Init(fourthLeftDriveWheel.gameObject);
			fifthRightDriveWheel.gameObject.AddComponent<PoweredWheelRotater>().Init(fourthRightDriveWheel.gameObject);
			sixthLeftDriveWheel.gameObject.AddComponent<PoweredWheelRotater>().Init(fourthLeftDriveWheel.gameObject);
			sixthRightDriveWheel.gameObject.AddComponent<PoweredWheelRotater>().Init(fourthRightDriveWheel.gameObject);

			//create driveAxles for fifth and sixth axles
			driveAxle5 = Instantiate(driveAxle4.gameObject, rearBogie).transform;
			driveAxle5.gameObject.name = "[axle] drive 2";
			driveAxle5.localPosition = new Vector3(
				driveAxle5.localPosition.x,
				driveAxle5.localPosition.y,
				3.25f);
			driveAxle6 = Instantiate(driveAxle4.gameObject, frontBogie).transform;
			driveAxle6.gameObject.name = "[axle] drive 3";
			driveAxle6.localPosition = new Vector3(
				driveAxle5.localPosition.x,
				driveAxle5.localPosition.y,
				-0.05f); //TODO: GET THE RIGHT POSITION OF THE SIXTH DRIVE AXLE

			flangedDriver = firstRightDriveWheel.GetComponent<MeshFilter>().sharedMesh;
			blindDriver = secondRightDriveWheel.GetComponent<MeshFilter>().sharedMesh;
			blindDriverBigWeight = thirdRightDriveWheel.GetComponent<MeshFilter>().sharedMesh;

			//Set skins for franklin valve gear, external branch pipes
			var lfMeshRenderers = lfFranklinValveGear.GetComponentsInChildren<MeshRenderer>();
			var rfMeshRenderers = rfFranklinValveGear.GetComponentsInChildren<MeshRenderer>();
			var lf4444MeshRenderers = lfFranklinValveGear4444.GetComponentsInChildren<MeshRenderer>();
			var rf4444MeshRenderers = rfFranklinValveGear4444.GetComponentsInChildren<MeshRenderer>();
			var lrMeshRenderers = driveL2.Find("franklin_type_b(Clone)").GetComponentsInChildren<MeshRenderer>();
			var rrMeshRenderers = driveR2.Find("franklin_type_b(Clone)").GetComponentsInChildren<MeshRenderer>();
			var lrReverseRenderers = lFranklinBReverseGear.GetComponentsInChildren<MeshRenderer>();
			var rrReverseRenderers = rFranklinBReverseGear.GetComponentsInChildren<MeshRenderer>();
			var lrReverse4444Renderers = lFranklinBReverseGear4444.GetComponentsInChildren<MeshRenderer>();
			var rrReverse4444Renderers = rFranklinBReverseGear4444.GetComponentsInChildren<MeshRenderer>();
			var rrReverseRearRenderers = driveR2.Find("franklin_type_b_reverse_rear(Clone)").GetComponentsInChildren<MeshRenderer>();
			var lBranchPipeRenderers = lBranchPipe.GetComponentsInChildren<MeshRenderer>();
			var rBranchPipeRenderers = rBranchPipe.GetComponentsInChildren<MeshRenderer>();
			var lBranchPipe4444Renderers = lBranchPipe4444.GetComponentsInChildren<MeshRenderer>();
			var rBranchPipe4444Renderers = rBranchPipe4444.GetComponentsInChildren<MeshRenderer>();
			var s282Material = fifthLeftDriveWheel.GetComponent<MeshRenderer>().sharedMaterial;
			foreach (var meshRenderer in lfMeshRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rfMeshRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in lf4444MeshRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rf4444MeshRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in lrMeshRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rrMeshRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in lrReverseRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rrReverseRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in lrReverse4444Renderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rrReverse4444Renderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rrReverseRearRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in lBranchPipeRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rBranchPipeRenderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in lBranchPipe4444Renderers)
			{
				meshRenderer.material = s282Material;
			}
			foreach (var meshRenderer in rBranchPipe4444Renderers)
			{
				meshRenderer.material = s282Material;
			}

			//duplicate side rods and extend the copies to the fifth axle
			leftSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism L/s282_mech_wheels_connect");
			rightSideRod = loco.transform.Find("LocoS282A_Body/MovingParts_LOD0/DriveMechanism R/s282_mech_wheels_connect");
			if (leftSideRod is null)
				Main.Logger.Error("Error: RemoveFifthDriveAxle: leftSideRod not found");
			if (rightSideRod is null)
				Main.Logger.Error("Error: RemoveFifthDriveAxle: rightSideRod not found");
			leftFiveAxleSideRod = Instantiate(leftSideRod.gameObject, leftSideRod).transform;
			rightFiveAxleSideRod = Instantiate(rightSideRod.gameObject, rightSideRod).transform;
			leftFiveAxleSideRod.gameObject.SetActive(false);
			rightFiveAxleSideRod.gameObject.SetActive(false);
			leftFiveAxleSideRod.transform.localPosition = new Vector3(0.003f, 0.004f, -3.1f);
			leftFiveAxleSideRod.transform.localEulerAngles = new Vector3(0, 180, 0);
			rightFiveAxleSideRod.transform.localPosition = new Vector3(0.003f, 0.004f, -3.1f);
			rightFiveAxleSideRod.transform.localEulerAngles = new Vector3(0, 180, 0);

			//hide siderod LODs
			leftFiveAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			rightFiveAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);

			//create rear side rods for 12-coupled locomotives
			leftSixAxleSideRod = Instantiate(leftSideRod.gameObject, leftSideRod).transform;
			rightSixAxleSideRod = Instantiate(rightSideRod.gameObject, rightSideRod).transform;
			leftSixAxleSideRod.gameObject.SetActive(false);
			rightSixAxleSideRod.gameObject.SetActive(false);
			leftSixAxleSideRod.transform.localPosition = new Vector3(-0.003f, -0.004f, -3.1f);
			leftSixAxleSideRod.transform.localEulerAngles = new Vector3(0, 0, 180);
			rightSixAxleSideRod.transform.localPosition = new Vector3(-0.003f, -0.004f, -3.1f);
			rightSixAxleSideRod.transform.localEulerAngles = new Vector3(0, 0, 180);

			//hide siderod LODs
			leftSixAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			rightSixAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);


			//create side rods for 6-coupled locomotives
			lfThreeAxleSideRod = Instantiate(leftSideRod.gameObject, leftSideRod).transform;
			rfThreeAxleSideRod = Instantiate(rightSideRod.gameObject, rightSideRod).transform;
			lfThreeAxleSideRod.gameObject.SetActive(false);
			rfThreeAxleSideRod.gameObject.SetActive(false);
			lfThreeAxleSideRod.transform.localPosition = new Vector3(0, 0, -3.11f);
			lfThreeAxleSideRod.transform.localEulerAngles = new Vector3(0, 180, 0);
			rfThreeAxleSideRod.transform.localPosition = new Vector3(0, 0, -3.11f);
			rfThreeAxleSideRod.transform.localEulerAngles = new Vector3(0, 180, 0);

			lrThreeAxleSideRod = Instantiate(leftSideRod.gameObject, leftSideRod).transform;
			rrThreeAxleSideRod = Instantiate(rightSideRod.gameObject, rightSideRod).transform;
			lrThreeAxleSideRod.gameObject.SetActive(false);
			rrThreeAxleSideRod.gameObject.SetActive(false);
			lrThreeAxleSideRod.transform.localPosition = new Vector3(0, 0, 0);
			lrThreeAxleSideRod.transform.localEulerAngles = new Vector3(0, 180, 0);
			lrThreeAxleSideRod.localScale = new Vector3(1, 1, -1);
			rrThreeAxleSideRod.transform.localPosition = new Vector3(0, 0, 0);
			rrThreeAxleSideRod.transform.localEulerAngles = new Vector3(0, 180, 0);
			rrThreeAxleSideRod.localScale = new Vector3(1, 1, -1);

			//hide siderod LODs
			lfThreeAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			rfThreeAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			lrThreeAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			rrThreeAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);

			//create side rods for 4-coupled locomotives
			leftTwoAxleSideRod = Instantiate(leftSideRod.gameObject, leftSideRod).transform;
			rightTwoAxleSideRod = Instantiate(rightSideRod.gameObject, rightSideRod).transform;
			leftTwoAxleSideRod.gameObject.SetActive(false);
			rightTwoAxleSideRod.gameObject.SetActive(false);
			leftTwoAxleSideRod.localPosition = new Vector3(0, 0, -1.38f);
			leftTwoAxleSideRod.localEulerAngles = new Vector3(0, 0, 180);
			rightTwoAxleSideRod.localPosition = new Vector3(0, 0, -1.38f);
			rightTwoAxleSideRod.localEulerAngles = new Vector3(0, 0, 180);

			//hide siderod LODs
			leftTwoAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			rightTwoAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);

			//get rid of all the extra valve gear that we accidentally copied when cloning siderods
			ClearSideRodChildren(leftTwoAxleSideRod);
			ClearSideRodChildren(rightTwoAxleSideRod);

			//create side rods for 4-coupled locomotives
			leftTwoAxleDuplexSideRod = Instantiate(rightTwoAxleSideRod.gameObject, leftSideRod).transform;
			rightTwoAxleDuplexSideRod = Instantiate(rightTwoAxleSideRod.gameObject, rightSideRod).transform;
			leftTwoAxleDuplexSideRod.GetComponent<MeshFilter>().sharedMesh = MeshFinder.Instance.TwoAxleDuplexSideRodMesh;
			rightTwoAxleDuplexSideRod.GetComponent<MeshFilter>().sharedMesh = MeshFinder.Instance.TwoAxleDuplexSideRodMesh;
			leftTwoAxleDuplexSideRod.gameObject.SetActive(false);
			rightTwoAxleDuplexSideRod.gameObject.SetActive(false);
			leftTwoAxleDuplexSideRod.localPosition = Vector3.zero;
			leftTwoAxleDuplexSideRod.localEulerAngles = new Vector3(0, 0, 180);
			rightTwoAxleDuplexSideRod.localPosition = Vector3.zero;
			rightTwoAxleDuplexSideRod.localEulerAngles = new Vector3(0, 0, 180);

			//hide siderod LODs
			leftTwoAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);
			rightTwoAxleSideRod.transform.Find("s282_mech_wheels_connect_LOD1").gameObject.SetActive(false);

			//get rid of all the extra valve gear that we accidentally copied when cloning siderods
			ClearSideRodChildren(lfThreeAxleSideRod);
			ClearSideRodChildren(rfThreeAxleSideRod);
			ClearSideRodChildren(lrThreeAxleSideRod);
			ClearSideRodChildren(rrThreeAxleSideRod);

			ClearSideRodChildren(leftFiveAxleSideRod);
			ClearSideRodChildren(rightFiveAxleSideRod);

			ClearSideRodChildren(leftSixAxleSideRod);
			ClearSideRodChildren(rightSixAxleSideRod);

			//alter siderod mesh to fix gap between last and second-to-last axle on 10-coupled and 12-coupled engines
			MeshFilter leftTwoSideRodMeshfilter = leftTwoAxleSideRod.GetComponent<MeshFilter>();
			MeshFilter rightTwoSideRodMeshfilter = rightTwoAxleSideRod.GetComponent<MeshFilter>();

			MeshFilter lfThreeSideRodMeshfilter = lfThreeAxleSideRod.GetComponent<MeshFilter>();
			MeshFilter rfThreeSideRodMeshfilter = rfThreeAxleSideRod.GetComponent<MeshFilter>();
			MeshFilter lrThreeSideRodMeshfilter = lrThreeAxleSideRod.GetComponent<MeshFilter>();
			MeshFilter rrThreeSideRodMeshfilter = rrThreeAxleSideRod.GetComponent<MeshFilter>();

			MeshFilter leftFiveSideRodMeshfilter = leftFiveAxleSideRod.GetComponent<MeshFilter>();
			MeshFilter rightFiveSideRodMeshfilter = rightFiveAxleSideRod.GetComponent<MeshFilter>();

			MeshFilter leftSixSideRodMeshfilter = leftSixAxleSideRod.GetComponent<MeshFilter>();
			MeshFilter rightSixSideRodMeshfilter = rightSixAxleSideRod.GetComponent<MeshFilter>();


			leftTwoSideRodMeshfilter.sharedMesh = MeshFinder.Instance.TwoAxleSideRodMesh;
			rightTwoSideRodMeshfilter.sharedMesh = MeshFinder.Instance.TwoAxleSideRodMesh;

			lfThreeSideRodMeshfilter.sharedMesh = MeshFinder.Instance.ThreeAxleSideRodMesh;
			rfThreeSideRodMeshfilter.sharedMesh = MeshFinder.Instance.ThreeAxleSideRodMesh;
			lrThreeSideRodMeshfilter.sharedMesh = MeshFinder.Instance.ThreeAxleSideRodMesh;
			rrThreeSideRodMeshfilter.sharedMesh = MeshFinder.Instance.ThreeAxleSideRodMesh;

			leftFiveSideRodMeshfilter.sharedMesh = MeshFinder.Instance.FiveSixAxleSideRodMesh; 
			rightFiveSideRodMeshfilter.sharedMesh = MeshFinder.Instance.FiveSixAxleSideRodMesh;

			leftSixSideRodMeshfilter.sharedMesh = MeshFinder.Instance.FiveSixAxleSideRodMesh;
			rightSixSideRodMeshfilter.sharedMesh = MeshFinder.Instance.FiveSixAxleSideRodMesh;

			Transform leftRearSideRod = driveL2.Find("s282_mech_wheels_connect");
			Transform rightRearSideRod = driveR2.Find("s282_mech_wheels_connect");
			Transform newLRSideRod = Instantiate(leftTwoAxleDuplexSideRod, leftRearSideRod).transform;
			Transform newRRSideRod = Instantiate(rightTwoAxleDuplexSideRod, rightRearSideRod).transform;
			newLRSideRod.gameObject.SetActive(true);
			newRRSideRod.gameObject.SetActive(true);
			leftRearSideRod.GetComponent<MeshRenderer>().enabled = false;
			rightRearSideRod.GetComponent<MeshRenderer>().enabled = false;

			//newSideRodMesh.UploadMeshData(true);

			leftMainRod = leftSideRod.Find("s282_mech_push_rod_to_connect");
			rightMainRod = rightSideRod.Find("s282_mech_push_rod_to_connect");
			mainRod = leftMainRod.GetComponent<MeshFilter>().sharedMesh;
			shortMainRod = MeshFinder.Instance.DuplexMainRodMesh;
			leftRearSideRod.Find("s282_mech_push_rod_to_connect").GetComponent<MeshFilter>().mesh = shortMainRod;
			rightRearSideRod.Find("s282_mech_push_rod_to_connect").GetComponent<MeshFilter>().mesh = shortMainRod;

			leftCrosshead = driveL.Find("s282_mech_push_rod");
			rightCrosshead = driveR.Find("s282_mech_push_rod");

			//Setup the brake calipers
			splitLoco = transform.Find("LocoS282A_Body/Static_LOD0/s282a_split_body(Clone)");
			rightFirstBrakeCaliper = splitLoco.Find("s282a_caliper_r_1");
			
			rightSecondBrakeCaliper = splitLoco.Find("s282a_caliper_r_2");

			rightThirdBrakeCaliper = splitLoco.Find("s282a_caliper_r_3");

			rightFourthBrakeCaliper = splitLoco.Find("s282a_caliper_r_4");
			rightFifthBrakeCaliper = Instantiate(rightThirdBrakeCaliper.gameObject, splitLoco).transform;
			rightSixthBrakeCaliper = Instantiate(rightThirdBrakeCaliper.gameObject, splitLoco).transform;
			leftFirstBrakeCaliper = splitLoco.Find("s282a_caliper_l_1");
			leftSecondBrakeCaliper = splitLoco.Find("s282a_caliper_l_2");
			leftThirdBrakeCaliper = splitLoco.Find("s282a_caliper_l_3");
			leftFourthBrakeCaliper = splitLoco.Find("s282a_caliper_l_4");
			leftFifthBrakeCaliper = Instantiate(leftThirdBrakeCaliper.gameObject, splitLoco).transform;
			leftSixthBrakeCaliper = Instantiate(leftThirdBrakeCaliper.gameObject, splitLoco).transform;

			rightFifthBrakeCaliper.gameObject.name = "s282a_caliper_r_5";
			leftFifthBrakeCaliper.gameObject.name = "s282a_caliper_l_5";
			rightSixthBrakeCaliper.gameObject.name = "s282a_caliper_r_6";
			leftSixthBrakeCaliper.gameObject.name = "s282a_caliper_l_6";

			rightBrakeShoes = driveR.Find("s282a_brake_shoes(Clone)");
			leftBrakeShoes = driveL.Find("s282a_brake_shoes(Clone)");

			rightFirstBrakeShoe = rightBrakeShoes.Find("s282a_brake_shoe_1");
			leftFirstBrakeShoe = leftBrakeShoes.Find("s282a_brake_shoe_1");
			rightSecondBrakeShoe = rightBrakeShoes.Find("s282a_brake_shoe_2");
			leftSecondBrakeShoe = leftBrakeShoes.Find("s282a_brake_shoe_2");
			rightThirdBrakeShoe = rightBrakeShoes.Find("s282a_brake_shoe_3");
			leftThirdBrakeShoe = leftBrakeShoes.Find("s282a_brake_shoe_3");
			rightFourthBrakeShoe = rightBrakeShoes.Find("s282a_brake_shoe_4");
			leftFourthBrakeShoe = leftBrakeShoes.Find("s282a_brake_shoe_4");
			
			if (rightFirstBrakeShoe is null)
			{
				Main.Logger.Error("First right brake shoe is null");
			}

			rightFifthBrakeShoe = Instantiate(rightFirstBrakeShoe.gameObject, rightBrakeShoes).transform;
			leftFifthBrakeShoe = Instantiate(rightFirstBrakeShoe.gameObject, leftBrakeShoes).transform;
			rightSixthBrakeShoe = Instantiate(rightFirstBrakeShoe.gameObject, rightBrakeShoes).transform;
			leftSixthBrakeShoe = Instantiate(rightFirstBrakeShoe.gameObject, leftBrakeShoes).transform;


			rightFifthBrakeShoe.gameObject.name = "s282a_brake_shoe_5";
			leftFifthBrakeShoe.gameObject.name = "s282a_brake_shoe_5";
			rightSixthBrakeShoe.gameObject.name = "s282a_brake_shoe_6";
			leftSixthBrakeShoe.gameObject.name = "s282a_brake_shoe_6";

			//Add brake shoes to BrakesOverheatingController to make them glow
			BrakesOverheatingController brakesOverheatingController = GetComponent<BrakesOverheatingController>();
			var brakeRenderers = brakesOverheatingController.brakeRenderers.ToList();
			brakeRenderers.Add(rightFifthBrakeShoe.GetComponent<MeshRenderer>());
			brakeRenderers.Add(leftFifthBrakeShoe.GetComponent<MeshRenderer>());
			brakeRenderers.Add(rightSixthBrakeShoe.GetComponent<MeshRenderer>());
			brakeRenderers.Add(leftSixthBrakeShoe.GetComponent<MeshRenderer>());
			brakesOverheatingController.brakeRenderers = brakeRenderers.ToArray();

			//Generate sparks for 5th and 6th drive axles.
			//In locomotives with less drive axles, we hide the uneeded spark transforms.
			Transform wheelSparks = loco.transform.Find("LocoS282A_Particles/[wheel sparks]");
			if (wheelSparks is null)
			{
				Main.Logger.Log("[wheel sparks] is null");
				return;
			}

			//In vanilla DV, sparks spawn too low. This patches that.
			//The "z=0.64" is in vanilla DV, this just changes the y coordinate
			wheelSparks.localPosition = new Vector3(0, 0.05f, 0.64f);
			WheelslipSparksController wheelslipSparks = wheelSparks.GetComponent<WheelslipSparksController>();
			WheelSlideSparksController wheelSlideSparks = wheelSparks.GetComponent<WheelSlideSparksController>();

			//Altfuture's naming for wheel sparks sucks. I refuse to use their names for these.
			spark1L = wheelSparks.Find("spark7");
			spark2L = wheelSparks.Find("spark5");
			spark3L = wheelSparks.Find("spark3");
			spark4L = wheelSparks.Find("spark1");
			spark1R = wheelSparks.Find("spark8");
			spark2R = wheelSparks.Find("spark6");
			spark3R = wheelSparks.Find("spark4");
			spark4R= wheelSparks.Find("spark2");

			//we don't bother setting positions here; that will be done in Remove5thAnd6thDriveAxle()
			spark5L = new GameObject("spark9").transform;
			spark5L.SetParent(wheelSparks);
			spark5L.localEulerAngles = Vector3.zero;
			spark5L.gameObject.SetActive(false);

			spark5R = new GameObject("spark10").transform;
			spark5R.gameObject.SetActive(false);
			spark5R.SetParent(wheelSparks);
			spark5R.localEulerAngles = Vector3.zero;

			spark6L = new GameObject("spark11").transform;
			spark6L.gameObject.SetActive(false);
			spark6L.SetParent(wheelSparks);
			spark6L.localEulerAngles = Vector3.zero;

			spark6R = new GameObject("spark12").transform;
			spark6R.gameObject.SetActive(false);
			spark6R.SetParent(wheelSparks);
			spark6R.localEulerAngles = Vector3.zero;

			WheelslipSparksController.WheelSparksDefinition axle5Sparks = new()
			{
				poweredWheel = driveAxle5.GetComponent<PoweredWheel>(),
				sparksLeftAnchor = spark5L,
				sparksRightAnchor = spark5R
			};
			axle5Sparks.Init();
			WheelslipSparksController.WheelSparksDefinition axle6Sparks = new()
			{
				poweredWheel = driveAxle5.GetComponent<PoweredWheel>(),
				sparksLeftAnchor = spark6L,
				sparksRightAnchor = spark6R
			};
			axle6Sparks.Init();

			if (wheelslipSparks is not null)
			{
				wheelslipSparks.wheelSparks = wheelslipSparks.wheelSparks
					.Append(axle5Sparks)
					.Append(axle6Sparks)
					.ToArray();
			}
			if (wheelSlideSparks is not null)
			{
				//Main.Logger.Log("Adding wheel slide sparks to 10-coupled S282");
				wheelSlideSparks.sparkAnchors = wheelSlideSparks.sparkAnchors
					.Append(spark5L)
					.Append(spark5R)
					.Append(spark6L)
					.Append(spark6R)
					.ToArray();
				wheelSlideSparks.sparks = null;
			}

			//Simulation changes for duplex
			oldSimCtrlr = transform.GetComponent<SimController>();
			duplexSimCtrlr = gameObject.AddComponent<DuplexSimController>();
			oldSim = transform.Find("[sim]");
			duplexSim = transform.Find("[sim duplex]");
			duplexSimCtrlr.connectionsDefinition = oldSimCtrlr.connectionsDefinition;
			duplexSimCtrlr.simFlow = oldSimCtrlr.simFlow;

			//make second set of traction simulation
			duplexTraction = transform.Find("[sim]/duplexTraction");
			var duplexTractionDef = duplexTraction.gameObject.GetComponent<DuplexTractionDefinition>();

			var duplexTractionPortsFeeder = duplexSim.gameObject.GetComponent<DuplexTractionPortsFeeder>();
			var duplexDrivingForce = duplexSim.gameObject.GetComponent<DrivingForce>();
			var duplexWheelslipCtrlr = duplexSim.gameObject.GetComponent<WheelslipController>();

			duplexSimCtrlr.tractionPortsFeeder = duplexTractionPortsFeeder;
			duplexSimCtrlr.drivingForce = duplexDrivingForce;
			duplexSimCtrlr.wheelslipController = duplexWheelslipCtrlr;
			duplexSimCtrlr.poweredWheels = oldSimCtrlr.poweredWheels;
			duplexSimCtrlr.OnValidate();
			duplexSimCtrlr.Initialize(loco, transform.GetComponent<DamageController>());
			duplexTractionPortsFeeder.adhesionController = duplexSimCtrlr.adhesionController;

			//We need to connect the rear WheelslipSparksController and WheelSlideSparksController
			//with the rear WheelslipController and AdhesionController

			var particles = transform.Find("LocoS282A_Particles");
			var duplexWheelSparks = particles.Find("[duplex wheel sparks]");
			var duplexWheelslipSparksCtrlr = duplexWheelSparks.gameObject.GetComponent<WheelslipSparksController>();
			var duplexWheelSlideSparksCtrlr = duplexWheelSparks.gameObject.GetComponent<WheelSlideSparksController>();
			duplexWheelslipSparksCtrlr.wheelslipController = duplexWheelslipCtrlr;
			loco.adhesionController.wheelslipController.WheelslipStateChanged
				-= duplexWheelslipSparksCtrlr.OnWheelslipStateChanged;
			duplexWheelslipCtrlr.WheelslipStateChanged
				+= duplexWheelslipSparksCtrlr.OnWheelslipStateChanged;

			sparkD1L = duplexWheelSparks.Find("spark7");
			sparkD2L = duplexWheelSparks.Find("spark5");
			sparkD3L = duplexWheelSparks.Find("spark3");
			sparkD4L = duplexWheelSparks.Find("spark1");
			sparkD1R = duplexWheelSparks.Find("spark8");
			sparkD2R = duplexWheelSparks.Find("spark6");
			sparkD3R = duplexWheelSparks.Find("spark4");
			sparkD4R = duplexWheelSparks.Find("spark2");

			//TODO: figure out how to patch WheelSlideSparksController

			oldCylCockSteam = particles.Find("CylCockSteam");
			oldCylCockWaterDrip = particles.Find("CylCockWaterDrip");
			rearCylCockSteam = particles.Find("DuplexCylCockSteam");
			rearCylCockWaterDrip = particles.Find("DuplexCylCockWaterDrip");

			//A few items need a SimController to work, like the PoweredWheelRotaters.
			//So we create one that doesn't do anything, but just stores the right variables.
			SimController duplexFakeSimCtrlr = duplexSim.gameObject.AddComponent<SimController>();
			duplexFakeSimCtrlr.enabled = false;

			AnimatorStartTimeOffsetPair driveL2AnimationPair = new()
			{
				startTimeOffset = 0,
				animator = lrAnimator,
			};
			AnimatorStartTimeOffsetPair driveR2AnimationPair = new()
			{
				startTimeOffset = 0.75f,
				animator = rrAnimator,
			};
			Transform wheelRotation = transform.Find("[wheel rotation]");
			//set active so that the PoweredWheelRotationViaAnimation classes
			//don't Awake() until we've initialized everything.
			wheelRotation.gameObject.SetActive(false);
			oldDrivetrainRotater = wheelRotation.GetComponent<PoweredWheelRotationViaAnimation>();
			//duplexDrivetrainRotaterF = wheelRotation.gameObject.AddComponent<PoweredWheelRotationViaAnimation>();
			duplexDrivetrainRotaterR = wheelRotation.gameObject.AddComponent<PoweredWheelRotationViaAnimation>();
			//duplexDrivetrainRotaterF.wheelRadius = oldDrivetrainRotater.wheelRadius;
			duplexDrivetrainRotaterR.wheelRadius = oldDrivetrainRotater.wheelRadius;
			//duplexDrivetrainRotaterF.animatorSetups = oldDrivetrainRotater.animatorSetups;
			duplexDrivetrainRotaterR.animatorSetups = new AnimatorStartTimeOffsetPair[]
			{
				driveR2AnimationPair, driveL2AnimationPair
			};

			duplexFakeSimCtrlr.wheelslipController = duplexSimCtrlr.wheelslipController;
			duplexFakeSimCtrlr.poweredWheels = oldSimCtrlr.poweredWheels;
			duplexFakeSimCtrlr.tractionPortsFeeder = duplexTractionPortsFeeder;
			wheelRotation.gameObject.SetActive(true);
			duplexDrivetrainRotaterR.simController = duplexFakeSimCtrlr;
			duplexDrivetrainRotaterR.poweredWheelsManager = duplexFakeSimCtrlr.poweredWheels;
			//duplexDrivetrainRotaterF.enabled = false;
			duplexDrivetrainRotaterR.enabled = false;
		}

		//Generates animation curves that are 45 degrees off for the rear cylinder cocks.
		//We're offsetting the rear cylinders by 45 degrees so that the engine sounds cool.
		private static AnimationCurve ShiftAnimation45Deg(AnimationCurve anim)
		{
			//Because we're shifting all the keys to the right, we need a new key at time=0
			Keyframe firstFrame = anim.keys[0];
			firstFrame.time = 1f - firstFrame.value;
			AnimationCurve newAnim = new AnimationCurve(anim.keys);

			//shift all but the last key by 0.125
			for (int i = 0; i < newAnim.length - 1; i++) {
				anim.keys[i].time += 0.125f;
			}

			anim.AddKey(firstFrame);
			return anim;
		}

		/* Clears all the given side rod's children except the mesh we need
		 * 
		 * When we clone the new side rods, we clone a bunch of extra valve gear.
		 * This helper function gets rid of the extra valve gear we don't need to clone.
		 */
		private static void ClearSideRodChildren(Transform sideRod)
		{
			for (int i = 0; i < sideRod.childCount; i++)
			{
				GameObject child = sideRod.GetChild(i).gameObject;
				//kill all the children except our favorite
				if (!child.name.Equals("s282_mech_wheels_connect_LOD1"))
				{
					UnityEngine.Object.DestroyImmediate(child);
					i--; //gotta offset i because we deleted an object
				}
			}
		}

		private void SetupRearBogie()
		{
			//generate the rear axle supports

			rearBogie = loco.transform.Find("Axle_R/bogie_car");

			Transform driveAxleTransform;
			int driveAxleCounter = 0;

			//rename rear axles to something reasonable
			for (int i = 0; i < rearBogie.childCount; i++)
			{
				if (rearBogie.GetChild(i).localPosition.z < -2.4f)
				{//if the ith rear axle is the rear front axle, grab it
					firstRearAxle = rearBogie.GetChild(i);
					firstRearAxle.gameObject.name = "[axle] 1";
				}
				else
				{//while we're here, grab the extra drive axles and rename them so we can easily access them later
					driveAxleTransform = rearBogie.GetChild(i);
					driveAxleTransform.gameObject.name = $"[axle] drive {driveAxleCounter}";
					driveAxleCounter++;
				}
			}

			driveAxle3 = rearBogie.Find("[axle] drive 0");
			driveAxle4 = rearBogie.Find("[axle] drive 1");

			firstRearAxle.localPosition = new Vector3(firstRearAxle.localPosition.x, 0.53f, firstRearAxle.localPosition.z);
			firstRearAxle.localScale = new Vector3(1, 0.92f, 0.92f);

			//generate second and third axle
			secondRearAxle = Instantiate(firstFrontAxle, rearBogie).transform;
			secondRearAxle.gameObject.SetActive(false);
			secondRearAxle.gameObject.name = "[axle] 2";
			secondRearAxle.localPosition = new Vector3(0, 0.36f, -1.32f);
			secondRearAxle.transform.Find("axleF_modelLOD1").gameObject.SetActive(false);
			thirdRearAxle = Instantiate(secondRearAxle, rearBogie).transform;
			thirdRearAxle.gameObject.SetActive(false);
			thirdRearAxle.gameObject.name = "[axle] 3";
			thirdRearAxle.localPosition = new Vector3(0, 0.36f, -2.52f);
			thirdRearAxle.transform.Find("axleF_modelLOD1").gameObject.SetActive(false);

			//also generate the second and third axle supports
			secondRearAxleSupport = Instantiate(firstFrontAxleSupport.gameObject, firstFrontAxleSupport.parent).transform;
			secondRearAxleSupport.gameObject.name = "s282_wheels_rear_support_1";
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
					wheelRotater.wheelRadius = 0.529f;
					/*wheelRotater.transformsToRotate = wheelRotater.transformsToRotate
						.Append(firstRearAxle)
						.ToArray();*/
				}
			}
		}

		public void SwitchWheelArrangementRandom()
		{
			int wa = rand.Next(Main.settings.RandomWAs.Count);
			SwitchWheelArrangement((int) Main.settings.RandomWAs[wa]);
		}

		public void SwitchWheelArrangement(int wa)
		{
			//Was having some issues with multiple requests coming in at once for the same
			//loco. This will hopefully stop that from happening.
			if (skipWheelArrangementChange)
				return;
			skipWheelArrangementChange = true;

			//When reloading a save, a bunch of the transforms go null for some reason.
			//This reloads everything if that happens.
			if (firstLeftDriveWheel is null)
			{
				Start();
			}

			S282AWheelArrangementType oldWA = currentWA;

			S282AWheelArrangementType waType = (S282AWheelArrangementType)wa;
			currentWA = waType;

			Main.Logger.Log($"Switching an S282 from {oldWA} to {waType}");

			derailModifier = (WheelArrangement.S282ANumOfNondrivenWheels[(int)currentWA]
				- WheelArrangement.S282ANumOfNondrivenWheels[(int)S282AWheelArrangementType.s282])
				/ 10.0f;
			Main.Logger.Log("Derail Modifier: " + derailModifier);

			HideLeadingWheels();
			ResetBody();
			ResetToFourCoupled(); 
			HideTrailingWheels();

			FixExplodedModel();
			
			//if we're currently on a locomotive with no leading axles, that means
			//we've moved the pilot back already. We need to move it forward before
			//we forget.
			switch (oldWA)
			{
				case S282AWheelArrangementType.s080:
				case S282AWheelArrangementType.s082:
				case S282AWheelArrangementType.s084:
				case S282AWheelArrangementType.s0100:
				case S282AWheelArrangementType.s0102:
				case S282AWheelArrangementType.s0104:
				case S282AWheelArrangementType.s0120:
					splitS282A.MovePilot(new Vector2(0, 0.47f));
					frontHandrail.gameObject.SetActive(true);
					break;
			}

			//select leading axle configuration
			switch (waType)
			{
				case S282AWheelArrangementType.s080:
				case S282AWheelArrangementType.s082:
				case S282AWheelArrangementType.s084:
				case S282AWheelArrangementType.s0100:
				case S282AWheelArrangementType.s0102:
				case S282AWheelArrangementType.s0104:
				case S282AWheelArrangementType.s0120:
					splitS282A.MovePilot(new Vector2(0, -0.47f));
					frontHandrail.gameObject.SetActive(false);
					break;
				case S282AWheelArrangementType.s280:
				case S282AWheelArrangementType.s282:
				case S282AWheelArrangementType.s284:
				case S282AWheelArrangementType.s280Big:
				case S282AWheelArrangementType.s282Big:
				case S282AWheelArrangementType.s284Big:
				case S282AWheelArrangementType.s2100:
				case S282AWheelArrangementType.s2102:
				case S282AWheelArrangementType.s2104:
				case S282AWheelArrangementType.s2120:
				case S282AWheelArrangementType.s2122:
				case S282AWheelArrangementType.s2442:
					ShowTwoLeadingWheels();
					break;
				case S282AWheelArrangementType.s4100:
				case S282AWheelArrangementType.s4102:
				case S282AWheelArrangementType.s4104:
				case S282AWheelArrangementType.s4122:
				//case WheelArrangementType.s4664:
					ShowFourLeadingWheels();
					break;
				case S282AWheelArrangementType.s4444:
					ShowFourLeadingWheels4444();
					break;
				case S282AWheelArrangementType.s404:
				case S282AWheelArrangementType.s440:
				case S282AWheelArrangementType.s442:
				case S282AWheelArrangementType.s444:
				case S282AWheelArrangementType.s460:
				case S282AWheelArrangementType.s462:
				case S282AWheelArrangementType.s464:
				case S282AWheelArrangementType.s480:
				case S282AWheelArrangementType.s482:
				case S282AWheelArrangementType.s484:
					ShowProper4LeadingWheels();
					break;
				default:
					break;
			};

			//select drive axle configuration
			switch (waType)
			{
				case S282AWheelArrangementType.s404:
					ShowNoDrivers();
					break;
				case S282AWheelArrangementType.s440:
				case S282AWheelArrangementType.s442:
				case S282AWheelArrangementType.s444:
					Show4Drivers();
					break;
				case S282AWheelArrangementType.s460:
				case S282AWheelArrangementType.s462:
				case S282AWheelArrangementType.s464:
					Show6Drivers();
					break;
				case S282AWheelArrangementType.s280Big:
				case S282AWheelArrangementType.s282Big:
				case S282AWheelArrangementType.s284Big:
					Show8BigDrivers();
					break;
				case S282AWheelArrangementType.s080:
				case S282AWheelArrangementType.s082:
				case S282AWheelArrangementType.s084:
				case S282AWheelArrangementType.s280:
				case S282AWheelArrangementType.s282:
				case S282AWheelArrangementType.s284:
					if (!Main.settings.show8CoupledValveGear)
					{
						HideValveGear();
					}
					break;
				case S282AWheelArrangementType.s480:
				case S282AWheelArrangementType.s482:
				case S282AWheelArrangementType.s484:
					Show8RearDrivers();
					break;
				case S282AWheelArrangementType.s0100:
				case S282AWheelArrangementType.s0102:
				case S282AWheelArrangementType.s0104:
				case S282AWheelArrangementType.s2100:
				case S282AWheelArrangementType.s2102:
				case S282AWheelArrangementType.s2104:
				case S282AWheelArrangementType.s4100:
				case S282AWheelArrangementType.s4102:
				case S282AWheelArrangementType.s4104:
					Show10Drivers();
					break;
				case S282AWheelArrangementType.s0120:
				case S282AWheelArrangementType.s2120:
				case S282AWheelArrangementType.s2122:
				case S282AWheelArrangementType.s4122:
					Show12SmallDrivers();
					break;
				case S282AWheelArrangementType.s2442:
					ShowDuplex();
					Show2442Duplex();
					break;
				case S282AWheelArrangementType.s4444:
					ShowDuplex();
					Show4444Duplex();
					break;
				default:
					break;
			};

			//select trailing axle configuration
			switch (waType)
			{
			//two trailing wheels:
				case S282AWheelArrangementType.s442:
					switch (Main.settings.x42Options)
					{
						case Settings.X82Options.vanilla:
							ShowTwoTrailingWheelsFor4CoupledVanilla();
							break;
						case Settings.X82Options.small_wheels:
							ShowTwoTrailingWheelsFor4Coupled();
							break;
						case Settings.X82Options.small_wheels_alternate:
							ShowTwoTrailingWheelsFor4CoupledAlternate();
							break;
					}
					break;
				case S282AWheelArrangementType.s462:
					switch (Main.settings.x62Options)
					{
						case Settings.X102Options.vanilla:
							ShowTwoTrailingWheelsFor6CoupledVanilla();
							break;
						case Settings.X102Options.small_wheels:
							ShowTwoTrailingWheelsFor6Coupled();
							break;
					}
					break;
				case S282AWheelArrangementType.s082:
				case S282AWheelArrangementType.s282:
					switch (Main.settings.x82Options)
					{
						case Settings.X82Options.vanilla:
							ShowTwoTrailingWheelsVanilla();
							break;
						case Settings.X82Options.small_wheels:
							ShowTwoTrailingWheels();
							break;
						case Settings.X82Options.small_wheels_alternate:
							ShowTwoTrailingWheelsAlternate();
							break;
					}
					break;
				case S282AWheelArrangementType.s282Big: //don't want the alternate position for S282Big
					switch (Main.settings.x82Options)
					{
						case Settings.X82Options.vanilla:
							ShowTwoTrailingWheelsVanilla();
							break;
						default:
							ShowTwoTrailingWheels();
							break;
					}
					break;
				case S282AWheelArrangementType.s482:
					switch (Main.settings.x82Options)
					{
						case Settings.X82Options.vanilla:
							ShowTwoTrailingWheelsFor10CoupledVanilla();
							break;
						case Settings.X82Options.small_wheels:
							ShowTwoTrailingWheelsFor482();
							break;
					}
					break;
				case S282AWheelArrangementType.s0102:
				case S282AWheelArrangementType.s2102:
				case S282AWheelArrangementType.s4102:
					switch (Main.settings.x102Options)
					{
						case Settings.X102Options.vanilla:
							ShowTwoTrailingWheelsFor10CoupledVanilla();
							break;
						case Settings.X102Options.small_wheels:
							ShowTwoTrailingWheelsFor10Coupled();
							break;
					}
					break;
				case S282AWheelArrangementType.s2122:
				case S282AWheelArrangementType.s4122:
				case S282AWheelArrangementType.s2442:
					ShowTwoTrailingWheelsFor12Coupled();
					break;

				//four trailing wheels:
				case S282AWheelArrangementType.s444:
					ShowFourTrailingWheelsFor4Coupled();
					break;
				case S282AWheelArrangementType.s404:
				case S282AWheelArrangementType.s084:
				case S282AWheelArrangementType.s284:
					ShowFourTrailingWheels();
					break;
				case S282AWheelArrangementType.s284Big:
					ShowFourTrailingWheelsForBig8Coupled();
					break;
				case S282AWheelArrangementType.s464:
				case S282AWheelArrangementType.s484:
				case S282AWheelArrangementType.s0104:
				case S282AWheelArrangementType.s2104:
				case S282AWheelArrangementType.s4104:
				case S282AWheelArrangementType.s4444:
					ShowFourTrailingWheelsFor10Coupled();
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

			valveGearMechanism = interior.Find("LocoS282A_Audio(Clone)/[sim] Engine/ValveGearMechanism");

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

		//The idea is to only fix the exploded model if we haven't done it already.
		//To do this we need to know how many gameObjectSwaps the loco usually uses
		//so we know if we've added any or not.
		private static int stockNumOfMaterialSwaps = -1;

		// Exploded locos are handled separately from regular locos, so we need to alter
		// the model to look right when it gets exploded
		private void FixExplodedModel()
		{
			ExplosionModelHandler explosionModelHandler = GetComponent<ExplosionModelHandler>();
			if (explosionModelHandler is null)
			{
				Main.Logger.Error("ExplosionModelHandler is null in FixExplodedModel()");
			}

			//if we've already added the new axles to this loco, don't do it again
			if (stockNumOfMaterialSwaps == -1)
			{
				stockNumOfMaterialSwaps = explosionModelHandler.materialSwaps.Length;
			}
			else if (explosionModelHandler.materialSwaps.Length > stockNumOfMaterialSwaps)
			{
				return;
			}
			GameObject frontAxle2 = secondFrontAxle.gameObject;
			GameObject rearAxle2 = secondRearAxle.gameObject;
			GameObject rearAxle3 = thirdRearAxle.gameObject;

			//This incantation adds the axle to the explosionModelHandler.
			//By doing this, the axle textures (for all LODs) will be automatically
			//swapped out when the loco is exploded (or "exploaded" as Altfuture says, lol)
			explosionModelHandler.materialSwaps[1].affectedGameObjects
				= explosionModelHandler.materialSwaps[1].affectedGameObjects
				.Append(frontAxle2)
				.Append(rearAxle2)
				.Append(rearAxle3).ToArray();

			//Main.Logger.Log("Finished fixing exploded axles");
		}

		//------ HIDE WHEELS ------

		//This hides any leading wheels.
		private void HideLeadingWheels()
		{
			firstFrontAxle.gameObject.SetActive(false);
			secondFrontAxle.gameObject.SetActive(false);
			firstFrontAxleSupport.gameObject.SetActive(false);
			firstFrontAxleSupportLOD.gameObject.SetActive(false);
			secondFrontAxleSupport.gameObject.SetActive(false);
			secondFrontAxleSupportLOD.gameObject.SetActive(false);

			firstFrontAxle.localScale = Vector3.one;
			firstFrontAxle.localPosition = new Vector3(0, 0.36f, 2.55f);
			secondFrontAxle.localScale = Vector3.one;
			secondFrontAxle.localPosition = new Vector3(0, 0.36f, 1.5f);
		}

		//When testing, I ran into a bug where the locomotive would "hop" forward or
		//backward slightly when moving the bogies. This method turns off physics
		//updates during that transition so that the "hop" doesn't happen.
		private static async void setBogieKinematicLater(Rigidbody bogie)
		{
			bool isKinematic = bogie.isKinematic;
			bogie.isKinematic = true;
			Main.Logger.Log("Setting bogie kinematic true...");
			await System.Threading.Tasks.Task.Delay(100);//delay 1 frame @ 10fps
			bogie.isKinematic = isKinematic;
			Main.Logger.Log("Setting bogie kinematic back to its original value");
			await System.Threading.Tasks.Task.Delay(100);//delay 1 frame @ 10fps
			bogie.isKinematic = isKinematic;
			Main.Logger.Log("Setting bogie kinematic again");
		}

		private void ResetBody()
		{
			//lubrication
			lubricator.localPosition = Vector3.zero;
			lubricatorSupport.localPosition = Vector3.zero;
			lubricatorSupport.gameObject.SetActive(true);
			lubricatorSightGlass.localPosition = new Vector3(0, 0, -1.37f);
			lubricatorHandle.localPosition = new Vector3(1.22f, 1.86f, 8.81f);
			rOilLines.localPosition = Vector3.zero;
			fOilLines.localPosition = Vector3.zero;

			if (loco.AreExternalInteractablesLoaded)
			{
				lubricatorLabel.localPosition = new Vector3(1.17f, 1.95f, 8.86f);
				lubricatorOilLevel.localPosition = Vector3.zero;
				lubricatorHandValveJoint.connectedAnchor = new Vector3(1.1232f, 1.9384f, 8.9388f);

				airValveLabel.localPosition += new Vector3(0.97f, 2.32f, 9.91f);
				airValveJoint.connectedAnchor = new Vector3(0.9056f, 2.4552f, 9.9186f);
			}

			//air
			lAirTank.localPosition = Vector3.zero;
			rAirTank.localPosition = Vector3.zero;
			airPump.localPosition = Vector3.zero;
			airPumpInputPipe.localPosition = Vector3.zero;
			airPumpOutputPipe.localPosition = Vector3.zero;
			airPumpOutputPipe.localScale = Vector3.one;

			//crosshead guide and lifting arm support
			lfCrossheadBracket.localPosition = Vector3.zero;
			lfCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			lfCrossheadBracket.gameObject.SetActive(true);
			rfCrossheadBracket.localPosition = Vector3.zero;
			rfCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			rfCrossheadBracket.gameObject.SetActive(true);

			lfCrossheadBracket.GetComponent<MeshFilter>().mesh = lCrossheadGuide;
			rfCrossheadBracket.GetComponent<MeshFilter>().mesh = rCrossheadGuide;

			lrCrossheadBracket.gameObject.SetActive(false);
			rrCrossheadBracket.gameObject.SetActive(false);

			lLiftingArmSupport.gameObject.SetActive(true);
			rLiftingArmSupport.gameObject.SetActive(true);

			splitS282A.MoveCylinders(Vector3.zero);
			lfCylinder.gameObject.SetActive(true);
			rfCylinder.gameObject.SetActive(true);
			cylinderCocks.gameObject.SetActive(true);

			lDryPipe.localPosition = Vector3.zero;
			rDryPipe.localPosition = Vector3.zero;
			lDryPipe.localScale = new Vector3(-1, 1, 1);
			rDryPipe.localScale = new Vector3(-1, 1, 1);
			lDryPipe.gameObject.SetActive(true);
			rDryPipe.gameObject.SetActive(true);

			lCrosshead.localScale = Vector3.one;
			rCrosshead.localScale = Vector3.one;

			loco.massController.CarMass = 125000;
			loco.massController.UpdateTrainCarMass();
			SetMassOnTrainPlate(125000);
		}

		private void ResetToFourCoupled()
		{
			steamEngine.numCylinders = 2;
			steamEngine.cylinderBore = 0.55f * 1;
			steamEngine.pistonStroke = 0.711f * 1;
			steamEngine.minCutoff = 0.05f;
			steamEngine.maxCutoff = 0.9f;

			//disable rear engine
			newSteamEngine.maxCutoff = 0;

			driveL.gameObject.SetActive(true);
			driveR.gameObject.SetActive(true);
			driveL2.gameObject.SetActive(false);
			driveR2.gameObject.SetActive(false);
			lfAnimator.runtimeAnimatorController = lfDefaultAnimatorCtrl;
			rfAnimator.runtimeAnimatorController = rfDefaultAnimatorCtrl;
			duplexDrivetrainRotaterR.enabled = false;
			duplexSim.gameObject.SetActive(false);
			duplexSimCtrlr.enabled = false;

			loco.adhesionController.wheelSlideFrictionCoef = loco.carLivery.parentType.WheelSlideFrictionCoef;
			loco.adhesionController.wheelslipFrictionCoef = loco.carLivery.parentType.WheelslipFrictionCoef;

			lfFranklinValveGear.gameObject.SetActive(false);
			rfFranklinValveGear.gameObject.SetActive(false);
			lfFranklinValveGear4444.gameObject.SetActive(false);
			rfFranklinValveGear4444.gameObject.SetActive(false);

			leftTwoAxleSideRod.gameObject.SetActive(false);
			rightTwoAxleSideRod.gameObject.SetActive(false);
			lfThreeAxleSideRod.gameObject.SetActive(false);
			rfThreeAxleSideRod.gameObject.SetActive(false);
			lrThreeAxleSideRod.gameObject.SetActive(false);
			rrThreeAxleSideRod.gameObject.SetActive(false);
			leftFiveAxleSideRod.gameObject.SetActive(false);
			rightFiveAxleSideRod.gameObject.SetActive(false);
			leftSixAxleSideRod.gameObject.SetActive(false);
			rightSixAxleSideRod.gameObject.SetActive(false);
			leftTwoAxleDuplexSideRod.gameObject.SetActive(false);
			rightTwoAxleDuplexSideRod.gameObject.SetActive(false);

			//Main.Logger.Log("Hid siderods");

			firstLeftDriveWheel.gameObject.SetActive(true);
			firstRightDriveWheel.gameObject.SetActive(true);
			thirdLeftDriveWheel.gameObject.SetActive(true);
			thirdRightDriveWheel.gameObject.SetActive(true);
			fourthLeftDriveWheel.gameObject.SetActive(true);
			fourthRightDriveWheel.gameObject.SetActive(true);
			fifthLeftDriveWheel.gameObject.SetActive(false);
			fifthRightDriveWheel.gameObject.SetActive(false);
			sixthLeftDriveWheel.gameObject.SetActive(false);
			sixthRightDriveWheel.gameObject.SetActive(false);

			//Main.Logger.Log("showed standard wheels, hid extra wheels");

			//first and fourth drivers are moved for 4-coupled locomotives, so we move it back
			firstLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 8f);
			firstRightDriveWheel.localPosition = new Vector3(0, 0.71f, 8f);
			secondLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 6.3f);
			secondRightDriveWheel.localPosition = new Vector3(0, 0.71f, 6.3f);
			fourthLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 3.2f);
			fourthRightDriveWheel.localPosition = new Vector3(0, 0.71f, 3.2f);

			//Main.Logger.Log("moved first and fourth drive wheels to where they should be");

			driveL.localPosition = Vector3.zero;
			driveR.localPosition = Vector3.zero;
			driveL.localScale = new Vector3(-1, 1, 1);
			driveR.localScale = Vector3.one;

			//Main.Logger.Log("reset driver position and scale");

			driveAxle1.gameObject.SetActive(true);
			driveAxle2.gameObject.SetActive(true);
			driveAxle3.gameObject.SetActive(true);
			driveAxle4.gameObject.SetActive(true);
			driveAxle5.gameObject.SetActive(false);
			driveAxle6.gameObject.SetActive(false);

			//Main.Logger.Log("reset driveAxle show/hide");

			leftReachRod.gameObject.SetActive(true);
			rightReachRod.gameObject.SetActive(true);
			leftReachRod.localScale = Vector3.one;
			rightReachRod.localScale = Vector3.one;

			//Main.Logger.Log("reset reach rods");

			MeshFilter secondLeftMeshFilter = secondLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter secondRightMeshFilter = secondRightDriveWheel.GetComponent<MeshFilter>();
			secondLeftMeshFilter.mesh = blindDriver;
			secondRightMeshFilter.mesh = blindDriver;

			MeshFilter thirdLeftMeshFilter = thirdLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter thirdRightMeshFilter = thirdRightDriveWheel.GetComponent<MeshFilter>();
			thirdLeftMeshFilter.mesh = blindDriverBigWeight;
			thirdRightMeshFilter.mesh = blindDriverBigWeight;

			//Main.Logger.Log("reset blind drivers");

			MeshFilter fourthLeftMeshFilter = fourthLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter fourthRightMeshFilter = fourthRightDriveWheel.GetComponent<MeshFilter>();
			fourthLeftMeshFilter.mesh = flangedDriver;
			fourthRightMeshFilter.mesh = flangedDriver;

			//Main.Logger.Log("Hid fifth and sixth drive wheels, side rods");

			//Set wheel radius to stock (56 inch diameter)
			setWheelRadius(0.712f);

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 4;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 4;

			//Main.Logger.Log("Set poweredAxles to 4");

			//show stock siderods
			leftSideRod.GetComponent<MeshRenderer>().enabled = true;
			rightSideRod.GetComponent<MeshRenderer>().enabled = true;

			//show mechanical lubricator linkage
			driveR.Find("s282_mech_cutoff_rail/s282_mech_lubricator_rod").gameObject.SetActive(true);
			driveR.Find("LubricatorParts").gameObject.SetActive(true);

			float time = lfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			lfAnimator.runtimeAnimatorController = lfDefaultAnimatorCtrl;
			rfAnimator.runtimeAnimatorController = rfDefaultAnimatorCtrl;
			lfAnimator.Play(lfAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, time              );
			rfAnimator.Play(rfAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, (time + 0.75f) % 1);

			ShowValveGear();
			ShowReachRod();
			driveL.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, -1.55f);
			driveR.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, -1.55f);
			driveL2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, -1.55f);
			driveR2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, -1.55f);

			leftMainRod.GetComponent<MeshFilter>().mesh = mainRod;
			rightMainRod.GetComponent<MeshFilter>().mesh = mainRod;

			leftCrosshead.localScale = Vector3.one;

			//show stock brake calipers
			leftFirstBrakeCaliper.gameObject.SetActive(true);
			leftSecondBrakeCaliper.gameObject.SetActive(true);
			leftThirdBrakeCaliper.gameObject.SetActive(true);
			leftFourthBrakeCaliper.gameObject.SetActive(true);
			leftFifthBrakeCaliper.gameObject.SetActive(false);
			leftSixthBrakeCaliper.gameObject.SetActive(false);
			rightFirstBrakeCaliper.gameObject.SetActive(true);
			rightSecondBrakeCaliper.gameObject.SetActive(true);
			rightThirdBrakeCaliper.gameObject.SetActive(true);
			rightFourthBrakeCaliper.gameObject.SetActive(true);
			rightFifthBrakeCaliper.gameObject.SetActive(false);
			rightSixthBrakeCaliper.gameObject.SetActive(false);

			leftFirstBrakeShoe.gameObject.SetActive(true);
			leftSecondBrakeShoe.gameObject.SetActive(true);
			leftThirdBrakeShoe.gameObject.SetActive(true);
			leftFourthBrakeShoe.gameObject.SetActive(true);
			leftFifthBrakeShoe.gameObject.SetActive(false);
			leftSixthBrakeShoe.gameObject.SetActive(false);
			rightFirstBrakeShoe.gameObject.SetActive(true);
			rightSecondBrakeShoe.gameObject.SetActive(true);
			rightThirdBrakeShoe.gameObject.SetActive(true);
			rightFourthBrakeShoe.gameObject.SetActive(true);
			rightFifthBrakeShoe.gameObject.SetActive(false);
			rightSixthBrakeShoe.gameObject.SetActive(false);

			rightFirstBrakeCaliper.localPosition = new Vector3(0, 0, 0.04f);
			leftFirstBrakeCaliper.localPosition = new Vector3(0, 0, 0.04f);
			rightSecondBrakeCaliper.localPosition = new Vector3(0, 0, -1.67f);
			leftSecondBrakeCaliper.localPosition = new Vector3(0, 0, -1.67f);
			rightThirdBrakeCaliper.localPosition = new Vector3(0, 0, 0.03f);
			leftThirdBrakeCaliper.localPosition = new Vector3(0, 0, 0.03f);
			rightFourthBrakeCaliper.localPosition = new Vector3(0, 0, -1.52f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0, -1.52f);
			rightFifthBrakeCaliper.localPosition = new Vector3(0, -0.045f, -2.36f);
			leftFifthBrakeCaliper.localPosition = new Vector3(0, -0.045f, -2.36f);
			rightSixthBrakeCaliper.localPosition = new Vector3(0, -0.045f, -3.67f);
			leftSixthBrakeCaliper.localPosition = new Vector3(0, -0.045f, -3.67f);

			rightFirstBrakeShoe.localPosition = new Vector3(0, 0, 8.175f);
			leftFirstBrakeShoe.localPosition = new Vector3(0, 0, 8.175f);
			rightSecondBrakeShoe.localPosition = new Vector3(0, 0, 6.465f);
			leftSecondBrakeShoe.localPosition = new Vector3(0, 0, 6.465f);
			rightThirdBrakeShoe.localPosition = new Vector3(0, 0, 4.91f);
			leftThirdBrakeShoe.localPosition = new Vector3(0, 0, 4.91f);
			rightFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.36f);
			leftFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.36f);
			rightFifthBrakeShoe.localPosition = new Vector3(0, 0, 1.8f);
			leftFifthBrakeShoe.localPosition = new Vector3(0, 0, 1.8f);
			rightSixthBrakeShoe.localPosition = new Vector3(0, 0, 0.25f);
			leftSixthBrakeShoe.localPosition = new Vector3(0, 0, 0.25f);

			//reset particle effects
			spark1L.localPosition = new Vector3(-.75f, 0, 7.36f);
			spark1R.localPosition = new Vector3(.75f, 0, 7.36f);
			spark2L.localPosition = new Vector3(-.75f, 0, 5.67f);
			spark2R.localPosition = new Vector3(.75f, 0, 5.67f);
			spark3L.localPosition = new Vector3(-.75f, 0, 4.11f);
			spark3R.localPosition = new Vector3(.75f, 0, 4.11f);
			spark4L.localPosition = new Vector3(-.75f, 0, 2.56f);
			spark4R.localPosition = new Vector3(.75f, 0, 2.56f);
			spark5L.localPosition = new Vector3(-.75f, 0, 1.01f);
			spark5R.localPosition = new Vector3(.75f, 0, 1.01f);
			spark6L.localPosition = new Vector3(-.75f, 0, 0.55f);
			spark6R.localPosition = new Vector3(.75f, 0, 0.55f);

			spark1L.gameObject.SetActive(true);
			spark1R.gameObject.SetActive(true);
			spark2L.gameObject.SetActive(true);
			spark2R.gameObject.SetActive(true);
			spark3L.gameObject.SetActive(true);
			spark3R.gameObject.SetActive(true);
			spark4L.gameObject.SetActive(true);
			spark4R.gameObject.SetActive(true);
			spark5L.gameObject.SetActive(false);
			spark5R.gameObject.SetActive(false);
			spark6L.gameObject.SetActive(false);
			spark6R.gameObject.SetActive(false);

			sparkD1L.gameObject.SetActive(false);
			sparkD1R.gameObject.SetActive(false);
			sparkD2L.gameObject.SetActive(false);
			sparkD2R.gameObject.SetActive(false);
			sparkD3L.gameObject.SetActive(false);
			sparkD3R.gameObject.SetActive(false);
			sparkD4L.gameObject.SetActive(false);
			sparkD4R.gameObject.SetActive(false);

			rearCylCockWaterDrip.gameObject.SetActive(false);
			rearCylCockSteam.gameObject.SetActive(false);

			if (valveGearMechanism is not null)
			{
				valveGearMechanism.gameObject.SetActive(true);
			}
		}

		//This hides any trailing wheels.
		private void HideTrailingWheels()
		{
			firstRearAxle.gameObject.SetActive(false);
			secondRearAxle.gameObject.SetActive(false);
			thirdRearAxle.gameObject.SetActive(false);
			secondRearAxleSupport.gameObject.SetActive(false);

			//reset trailing wheels to normal size if necessary
			secondRearAxle.localScale = Vector3.one;
			thirdRearAxle.localScale = Vector3.one;
			WheelRotationViaCode[] wheelRotaters = loco.transform.Find("[wheel rotation]").GetComponents<WheelRotationViaCode>();
			foreach (WheelRotationViaCode wheelRotater in wheelRotaters)
			{
				if (wheelRotater.wheelRadius == 0.44375f)
				{
					wheelRotater.wheelRadius = 0.355f;
				}
			}
		}

//------ SHOW LEADING WHEELS ------

		private void ShowTwoLeadingWheels()
		{
			//splitS282A.MovePilot(Vector2.zero);

			firstFrontAxle.gameObject.SetActive(true);

			firstFrontAxleSupport.localPosition = new Vector3(
				firstFrontAxleSupport.localPosition.x,
				firstFrontAxleSupport.localPosition.y,
				10.54f);
			firstFrontAxleSupportLOD.localPosition = new Vector3(
				secondFrontAxleSupportLOD.localPosition.x,
				secondFrontAxleSupportLOD.localPosition.y,
				10.54f);

			firstFrontAxleSupport.gameObject.SetActive(true);
			firstFrontAxleSupportLOD.gameObject.SetActive(true);
		}

		//Show four leading wheels, with the second axle under the cylinder saddle
		private void ShowFourLeadingWheels()
		{
			ShowTwoLeadingWheels();
			firstFrontAxle.localPosition = new Vector3(0, 0.36f, 2.75f);
			secondFrontAxle.localPosition = new Vector3(secondFrontAxle.localPosition.x, secondFrontAxle.localPosition.y, 1.45f);
			secondFrontAxle.gameObject.SetActive(true);
			secondFrontAxleSupport.localPosition = new Vector3(
				secondFrontAxleSupport.localPosition.x,
				secondFrontAxleSupport.localPosition.y,
				9.46f);
			secondFrontAxleSupportLOD.localPosition = new Vector3(
				secondFrontAxleSupportLOD.localPosition.x,
				secondFrontAxleSupportLOD.localPosition.y,
				9.46f);
			secondFrontAxleSupport.gameObject.SetActive(true);
			secondFrontAxleSupportLOD.gameObject.SetActive(true);
		}

		//Show four leading wheels, with the second axle under the cylinder saddle
		private void ShowFourLeadingWheels4444()
		{
			ShowTwoLeadingWheels();
			firstFrontAxle.localPosition = new Vector3(0, 0.36f, 2.75f);
			secondFrontAxle.localPosition = new Vector3(secondFrontAxle.localPosition.x, secondFrontAxle.localPosition.y, 1.05f);
			secondFrontAxle.gameObject.SetActive(true);

			firstFrontAxleSupport.localPosition = new Vector3(
				firstFrontAxleSupport.localPosition.x,
				firstFrontAxleSupport.localPosition.y,
				10.75f);
			firstFrontAxleSupportLOD.localPosition = new Vector3(
				secondFrontAxleSupportLOD.localPosition.x,
				secondFrontAxleSupportLOD.localPosition.y,
				10.75f);

			secondFrontAxleSupport.localPosition = new Vector3(
				secondFrontAxleSupport.localPosition.x,
				secondFrontAxleSupport.localPosition.y,
				9.06f);
			secondFrontAxleSupportLOD.localPosition = new Vector3(
				secondFrontAxleSupportLOD.localPosition.x,
				secondFrontAxleSupportLOD.localPosition.y,
				9.06f);
			secondFrontAxleSupport.gameObject.SetActive(true);
			secondFrontAxleSupportLOD.gameObject.SetActive(true);
		}

		//Show four leading wheels, with the second axle where it should be (behind the cylinder saddle)
		//Also scale up the small trailing wheels to match
		private void ShowProper4LeadingWheels()
		{
			ShowTwoLeadingWheels();
			secondFrontAxle.localPosition = new Vector3(secondFrontAxle.localPosition.x, secondFrontAxle.localPosition.y, 0.44f);
			secondFrontAxle.gameObject.SetActive(true);

			firstFrontAxleSupport.localPosition = new Vector3(
				firstFrontAxleSupport.localPosition.x,
				firstFrontAxleSupport.localPosition.y,
				10.74f);
			firstFrontAxleSupportLOD.localPosition = new Vector3(
				secondFrontAxleSupportLOD.localPosition.x,
				secondFrontAxleSupportLOD.localPosition.y,
				10.74f);

			secondFrontAxleSupport.localPosition = new Vector3(
				secondFrontAxleSupport.localPosition.x,
				secondFrontAxleSupport.localPosition.y,
				8.3f);
			secondFrontAxleSupportLOD.localPosition = new Vector3(
				secondFrontAxleSupportLOD.localPosition.x,
				secondFrontAxleSupportLOD.localPosition.y,
				8.3f);
			secondFrontAxleSupport.gameObject.SetActive(true);
			secondFrontAxleSupportLOD.gameObject.SetActive(true);
			/*loco.transform.Find("LocoS282A_Body/Static_LOD0/s282_wheels_front_support_2").gameObject.SetActive(true);
			loco.transform.Find("LocoS282A_Body/Static_LOD1/s282_wheels_front_support_2_LOD1").gameObject.SetActive(true);*/

			firstFrontAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			firstFrontAxle.localPosition = new Vector3(0, 0.44f, 2.6f);
			secondFrontAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			secondFrontAxle.localPosition = new Vector3(0, 0.44f, 0.3f);

			WheelRotationViaCode[] wheelRotaters = loco.transform.Find("[wheel rotation]").GetComponents<WheelRotationViaCode>();
			foreach (WheelRotationViaCode wheelRotater in wheelRotaters)
			{
				if (wheelRotater.wheelRadius == 0.355f)
				{
					wheelRotater.wheelRadius = 0.44375f;
				}
			}
		}

		//------ HIDE/SHOW VALVE GEAR ------
		//These functions do not show or hide the reach rods or the mechanical lubricator linkage.
		//Those are wheel-arrangement dependent (they are always hidden on the 4-coupled and 6-coupled
		//locomotives, for example), so they are controlled by the ShowXDrivers() functions.

		private void HideValveGear()
		{
			driveR.Find("s282_mech_cutoff_boomerang_high").gameObject.SetActive(false);
			driveL.Find("s282_mech_cutoff_boomerang_high").gameObject.SetActive(false);
			driveR.Find("s282_mech_cutoff_rail").gameObject.SetActive(false);
			driveL.Find("s282_mech_cutoff_rail").gameObject.SetActive(false);
			driveR.Find("s282_mech_cutoff_rail").gameObject.SetActive(false);
			driveL.Find("s282_mech_cutoff_rail").gameObject.SetActive(false);
			driveR.Find("s282_mech_cutoff_rod_hinge").gameObject.SetActive(false);
			driveL.Find("s282_mech_cutoff_rod_hinge").gameObject.SetActive(false);
			driveR.Find("s282_mech_cylinder_rod").gameObject.SetActive(false);
			driveL.Find("s282_mech_cylinder_rod").gameObject.SetActive(false);
			driveR.Find("s282_mech_push_rod/s282_mech_push_rod_to_cutoff_base").gameObject.SetActive(false);
			driveL.Find("s282_mech_push_rod/s282_mech_push_rod_to_cutoff_base").gameObject.SetActive(false);
			rightSideRod.Find("s282_mech_connect_to_cutoff_base").gameObject.SetActive(false);
			leftSideRod.Find("s282_mech_connect_to_cutoff_base").gameObject.SetActive(false);
			reachRodSupport.gameObject.SetActive(false);
		}

		private void HideReachRod()
		{
			driveR.Find("s282_mech_cutoff_rod").gameObject.SetActive(false);
			driveL.Find("s282_mech_cutoff_rod").gameObject.SetActive(false);
		}

		private void ShowValveGear()
		{
			driveR.Find("s282_mech_cutoff_boomerang_high").gameObject.SetActive(true);
			driveL.Find("s282_mech_cutoff_boomerang_high").gameObject.SetActive(true);
			driveR.Find("s282_mech_cutoff_rail").gameObject.SetActive(true);
			driveL.Find("s282_mech_cutoff_rail").gameObject.SetActive(true);
			driveR.Find("s282_mech_cutoff_rail").gameObject.SetActive(true);
			driveL.Find("s282_mech_cutoff_rail").gameObject.SetActive(true);
			driveR.Find("s282_mech_cutoff_rod_hinge").gameObject.SetActive(true);
			driveL.Find("s282_mech_cutoff_rod_hinge").gameObject.SetActive(true);
			driveR.Find("s282_mech_cylinder_rod").gameObject.SetActive(true);
			driveL.Find("s282_mech_cylinder_rod").gameObject.SetActive(true);
			driveR.Find("s282_mech_push_rod/s282_mech_push_rod_to_cutoff_base").gameObject.SetActive(true);
			driveL.Find("s282_mech_push_rod/s282_mech_push_rod_to_cutoff_base").gameObject.SetActive(true);
			driveL.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_rod").gameObject.SetActive(true);
			driveR.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_rod").gameObject.SetActive(true);
			rightSideRod.Find("s282_mech_connect_to_cutoff_base").gameObject.SetActive(true);
			leftSideRod.Find("s282_mech_connect_to_cutoff_base").gameObject.SetActive(true);
			reachRodSupport.gameObject.SetActive(true);
		}

		private void ShowReachRod()
		{
			driveR.Find("s282_mech_cutoff_rod").gameObject.SetActive(true);
			driveL.Find("s282_mech_cutoff_rod").gameObject.SetActive(true);
		}

		//------ SHOW DRIVERS ------

		/*private void MoveRearBogieTo(float z, bool setKinematic)
		{
			//move rear bogie so that it lines up with the rearmost driver
			Transform axleR = loco.transform.Find("Axle_R");
			ConfigurableJoint axleRJoint = axleR.GetComponent<ConfigurableJoint>();
			if (axleRJoint is null)
			{
				Main.Logger.Log($"Couldn't find ConfigurableJoint on ten-coupled Axle_R.");
			}
			else
			{
				axleRJoint.autoConfigureConnectedAnchor = false;
				axleRJoint.connectedAnchor = new Vector3(axleRJoint.connectedAnchor.x, axleRJoint.connectedAnchor.y, z);
			}
			Rigidbody axleRRigidbody = axleR.GetComponent<Rigidbody>();
			if (axleRRigidbody is not null && setKinematic)
				setBogieKinematicLater(axleRRigidbody);
			axleR.localPosition = new Vector3(axleR.localPosition.x, axleR.localPosition.y, z);
			//axleRJoint.autoConfigureConnectedAnchor = true;
		}*/

		private void Show4Drivers()
		{
			//MoveRearBogieTo(2.86f, false);

			driveL.localPosition = new Vector3(0, 0, -3.935f);
			driveR.localPosition = new Vector3(0, 0, -3.935f);
			driveL.localScale = new Vector3(-1, 1.42f, 1.42f);
			driveR.localScale = new Vector3(1, 1.42f, 1.42f);

			firstLeftDriveWheel.gameObject.SetActive(false);
			firstRightDriveWheel.gameObject.SetActive(false);
			fourthLeftDriveWheel.gameObject.SetActive(false);
			fourthRightDriveWheel.gameObject.SetActive(false);
			secondLeftDriveWheel.GetComponent<MeshFilter>().mesh = flangedDriver;
			secondRightDriveWheel.GetComponent<MeshFilter>().mesh = flangedDriver;
			secondLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 6.62f);
			secondRightDriveWheel.localPosition = new Vector3(0, 0.71f, 6.62f);
			thirdLeftDriveWheel.GetComponent<MeshFilter>().mesh = flangedDriver;
			thirdRightDriveWheel.GetComponent<MeshFilter>().mesh = flangedDriver;

			driveAxle1.gameObject.SetActive(false);
			driveAxle4.gameObject.SetActive(false);
			setWheelRadius(1.03376f); // == 1.42*0.728 = 40.75in*2 = 81.5in

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 2;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 2;

			leftReachRod.gameObject.SetActive(false);
			rightReachRod.gameObject.SetActive(false);

			leftSideRod.GetComponent<MeshRenderer>().enabled = false;
			rightSideRod.GetComponent<MeshRenderer>().enabled = false;
			leftTwoAxleSideRod.gameObject.SetActive(true);
			rightTwoAxleSideRod.gameObject.SetActive(true);

			//hide mechanical lubricator linkage
			driveR.Find("s282_mech_cutoff_rail/s282_mech_lubricator_rod").gameObject.SetActive(false);
			driveR.Find("LubricatorParts").gameObject.SetActive(false);

			//hide or show valve gear
			if (!Main.settings.show4CoupledValveGear)
				HideValveGear();

			Transform leftConnectingRod = leftSideRod.Find("s282_mech_push_rod_to_connect");
			Transform rightConnectingRod = rightSideRod.Find("s282_mech_push_rod_to_connect");
			leftConnectingRod.localPosition = new Vector3(0.1f, 0, -1.55f);
			rightConnectingRod.localPosition = new Vector3(0.1f, 0, -1.55f);
			leftConnectingRod.localScale = Vector3.one;
			rightConnectingRod.localScale = Vector3.one;

			//For 4-4-x's, we use the first and third brake caliper. This is because we
			//want the sand pipe on the first drive axle, but not the second.
			leftFirstBrakeCaliper.localPosition = new Vector3(0, 0.13f, -2.19f);
			rightFirstBrakeCaliper.localPosition = new Vector3(0, 0.13f, -2.19f);
			leftThirdBrakeCaliper.localPosition = new Vector3(0, 0.13f, -1.59f);
			rightThirdBrakeCaliper.localPosition = new Vector3(0, 0.13f, -1.59f);
			leftSecondBrakeCaliper.gameObject.SetActive(false);
			rightSecondBrakeCaliper.gameObject.SetActive(false);
			leftFourthBrakeCaliper.gameObject.SetActive(false);
			rightFourthBrakeCaliper.gameObject.SetActive(false);

			leftFirstBrakeShoe.localPosition = new Vector3(0, 0, 6.79f);
			rightFirstBrakeShoe.localPosition = new Vector3(0, 0, 6.79f);
			leftThirdBrakeShoe.localPosition = new Vector3(0, 0, 4.92f);
			rightThirdBrakeShoe.localPosition = new Vector3(0, 0, 4.92f);
			leftSecondBrakeShoe.gameObject.SetActive(false);
			rightSecondBrakeShoe.gameObject.SetActive(false);
			leftFourthBrakeShoe.gameObject.SetActive(false);
			rightFourthBrakeShoe.gameObject.SetActive(false);

			spark1L.localPosition = new Vector3(-.75f, 0, 4.83f);
			spark1R.localPosition = new Vector3(.75f, 0, 4.83f);
			spark2L.localPosition = new Vector3(-.75f, 0, 2.2f);
			spark2R.localPosition = new Vector3(.75f, 0, 2.2f);

			spark3L.gameObject.SetActive(false);
			spark3R.gameObject.SetActive(false);
			spark4L.gameObject.SetActive(false);
			spark4R.gameObject.SetActive(false);

			splitS282A.MoveCylinders(new Vector3(0, 0.3f, 0));

			lCrosshead.localScale = new Vector3(1, 0.7f, 1);
			rCrosshead.localScale = new Vector3(1, 0.7f, 1);

			//lubrication
			lubricator.localPosition = new Vector3(-0.01f, 0.42f, 0.1f);
			lubricatorSupport.gameObject.SetActive(false);
			lubricatorSightGlass.localPosition += lubricator.localPosition;
			lubricatorHandle.localPosition += lubricator.localPosition;
			rOilLines.localPosition = lubricator.localPosition;
			fOilLines.localPosition = lubricator.localPosition;

			//air
			lAirTank.localPosition = new Vector3(0, 0, -0.1f);
			rAirTank.localPosition = lAirTank.localPosition;
			airPump.localPosition = new Vector3(0, 0, 0.07f);
			airPumpInputPipe.localPosition = airPump.localPosition;
			airPumpOutputPipe.localPosition = new Vector3(0, 0, -0.58f);
			airPumpOutputPipe.localScale = new Vector3(1, 1, 1.12f);

			//move lubrication and air stuff in interior
			if (loco.AreExternalInteractablesLoaded)
			{
				lubricatorLabel.localPosition += lubricator.localPosition;
				lubricatorOilLevel.localPosition = lubricator.localPosition;
				lubricatorHandValveJoint.connectedAnchor += lubricator.localPosition;

				airValveLabel.localPosition += airPumpInputPipe.localPosition;
				airValveJoint.connectedAnchor += airPumpInputPipe.localPosition;
			}

			//crosshead guide and lifting arm support
			lfCrossheadBracket.localPosition = new Vector3(0, 0.3f, -1);
			lfCrossheadBracket.localScale = new Vector3(-1, 1, 1.25f);
			rfCrossheadBracket.localPosition = lfCrossheadBracket.localPosition;
			rfCrossheadBracket.localScale = lfCrossheadBracket.localScale;
			lLiftingArmSupport.gameObject.SetActive(false);
			rLiftingArmSupport.gameObject.SetActive(false);
		}

		private void Show6Drivers()
		{
			//MoveRearBogieTo(2.03f, false);

			driveL.localPosition = new Vector3(0, 0, -2.1f);
			driveR.localPosition = new Vector3(0, 0, -2.1f);
			driveL.localScale = new Vector3(-1, 1.2879f, 1.2879f);
			driveR.localScale = new Vector3(1, 1.2879f, 1.2879f);
			firstLeftDriveWheel.gameObject.SetActive(false);
			firstRightDriveWheel.gameObject.SetActive(false);
			driveAxle1.gameObject.SetActive(false);
			setWheelRadius(0.9144f); // == 1.3058*0.71 = 36.5in = 73in/2

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 3;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 3;

			secondLeftDriveWheel.GetComponent<MeshFilter>().mesh = flangedDriver;
			secondRightDriveWheel.GetComponent<MeshFilter>().mesh = flangedDriver;

			leftReachRod.gameObject.SetActive(false);
			rightReachRod.gameObject.SetActive(false);

			leftSideRod.GetComponent<MeshRenderer>().enabled = false;
			rightSideRod.GetComponent<MeshRenderer>().enabled = false;
			lfThreeAxleSideRod.gameObject.SetActive(true);
			rfThreeAxleSideRod.gameObject.SetActive(true);
			lrThreeAxleSideRod.gameObject.SetActive(true);
			rrThreeAxleSideRod.gameObject.SetActive(true);

			//hide mechanical lubricator linkage
			driveR.Find("s282_mech_cutoff_rail/s282_mech_lubricator_rod").gameObject.SetActive(false);
			driveR.Find("LubricatorParts").gameObject.SetActive(false);

			//hide or show valve gear
			if (!Main.settings.show6CoupledValveGear)
				HideValveGear();

			leftFirstBrakeCaliper.gameObject.SetActive(false);
			rightFirstBrakeCaliper.gameObject.SetActive(false);
			leftSecondBrakeCaliper.localPosition = new Vector3(0, 0.09f, -1.75f);
			rightSecondBrakeCaliper.localPosition = new Vector3(0, 0.09f, -1.75f);
			leftThirdBrakeCaliper.localPosition = new Vector3(0, 0.09f, -0.49f);
			rightThirdBrakeCaliper.localPosition = new Vector3(0, 0.09f, -0.49f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0.09f, -2.49f);
			rightFourthBrakeCaliper.localPosition = new Vector3(0, 0.09f, -2.49f);

			leftFirstBrakeShoe.gameObject.SetActive(false);
			rightFirstBrakeShoe.gameObject.SetActive(false);

			spark1L.localPosition = new Vector3(-.75f, 0, 5.37f);
			spark1R.localPosition = new Vector3(.75f, 0, 5.37f);
			spark2L.localPosition = new Vector3(-.75f, 0, 3.37f);
			spark2R.localPosition = new Vector3(.75f, 0, 3.37f);
			spark3L.localPosition = new Vector3(-.75f, 0, 1.37f);
			spark3R.localPosition = new Vector3(.75f, 0, 1.37f);

			spark4L.gameObject.SetActive(false);
			spark4R.gameObject.SetActive(false);

			splitS282A.MoveCylinders(new Vector3(0, 0.21f, 0));

			lCrosshead.localScale = new Vector3(1, 0.78f, 1);
			rCrosshead.localScale = new Vector3(1, 0.78f, 1);

			//lubrication
			lubricator.localPosition = new Vector3(-0.01f, 0.42f, 0.1f);
			lubricatorSupport.gameObject.SetActive(false);
			lubricatorSightGlass.localPosition += lubricator.localPosition;
			lubricatorHandle.localPosition += lubricator.localPosition;
			rOilLines.localPosition = lubricator.localPosition;
			fOilLines.localPosition = lubricator.localPosition;

			//air
			lAirTank.localPosition = new Vector3(0, 0, -0.1f);
			rAirTank.localPosition = lAirTank.localPosition;
			airPump.localPosition = new Vector3(0, 0, 0.07f);
			airPumpInputPipe.localPosition = airPump.localPosition;
			airPumpOutputPipe.localPosition = new Vector3(0, 0, -0.58f);
			airPumpOutputPipe.localScale = new Vector3(1, 1, 1.12f);

			//move lubrication and air stuff in interior
			if (loco.AreExternalInteractablesLoaded)
			{
				lubricatorLabel.localPosition += lubricator.localPosition;
				lubricatorOilLevel.localPosition = lubricator.localPosition;
				lubricatorHandValveJoint.connectedAnchor += lubricator.localPosition;

				airValveLabel.localPosition += airPumpInputPipe.localPosition;
				airValveJoint.connectedAnchor += airPumpInputPipe.localPosition;
			}

			//crosshead guide and lifting arm support
			lfCrossheadBracket.localPosition = new Vector3(0, 0.21f, 0);
			rfCrossheadBracket.localPosition = lfCrossheadBracket.localPosition;
			lLiftingArmSupport.gameObject.SetActive(false);
			rLiftingArmSupport.gameObject.SetActive(false);
		}

		//moves drivers back for the 4-8-0 and 4-8-2.
		//Actually, it does this by hiding the first drivers and showing the fifth drivers.
		private void Show8RearDrivers()
		{
			//MoveRearBogieTo(1.5f, false);
			firstLeftDriveWheel.gameObject.SetActive(false);
			firstRightDriveWheel.gameObject.SetActive(false);

			fifthLeftDriveWheel.gameObject.SetActive(true);
			fifthRightDriveWheel.gameObject.SetActive(true);

			MeshFilter secondLeftMeshFilter = secondLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter secondRightMeshFilter = secondRightDriveWheel.GetComponent<MeshFilter>();
			secondLeftMeshFilter.mesh = flangedDriver;
			secondRightMeshFilter.mesh = flangedDriver;

			/*MeshFilter thirdLeftMeshFilter = thirdLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter thirdRightMeshFilter = thirdRightDriveWheel.GetComponent<MeshFilter>();
			thirdLeftMeshFilter.mesh = blindDriver;
			thirdRightMeshFilter.mesh = blindDriver;*/

			//show added side rods, and hide the vanilla ones
			leftFiveAxleSideRod.gameObject.SetActive(true);
			rightFiveAxleSideRod.gameObject.SetActive(true);
			leftSideRod.GetComponent<MeshRenderer>().enabled = false;
			rightSideRod.GetComponent<MeshRenderer>().enabled = false;

			rightFirstBrakeCaliper.localPosition = new Vector3(0, 0, -1.67f);
			leftFirstBrakeCaliper.localPosition = new Vector3(0, 0, -1.67f);
			rightSecondBrakeCaliper.localPosition = new Vector3(0, 0, -3.23f);
			leftSecondBrakeCaliper.localPosition = new Vector3(0, 0, -3.23f);
			rightThirdBrakeCaliper.localPosition = new Vector3(0, 0, -1.52f);
			leftThirdBrakeCaliper.localPosition = new Vector3(0, 0, -1.52f);
			rightFourthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);

			leftFirstBrakeShoe.gameObject.SetActive(false);
			rightFirstBrakeShoe.gameObject.SetActive(false);
			leftFifthBrakeShoe.gameObject.SetActive(true);
			rightFifthBrakeShoe.gameObject.SetActive(true);

			//show correct sparks
			spark1L.gameObject.SetActive(false);
			spark1R.gameObject.SetActive(false);
			spark5L.gameObject.SetActive(true);
			spark5R.gameObject.SetActive(true);

			//hide or show valve gear
			if (!Main.settings.show8CoupledValveGear)
				HideValveGear();
		}

		//embiggens drivers for the high-speed 2-8-2
		private void Show8BigDrivers()
		{
			driveL.localPosition = new Vector3(0, 0, -1.5f);
			driveR.localPosition = new Vector3(0, 0, -1.5f);
			driveL.localScale = new Vector3(-1, 1.198f, 1.198f);
			driveR.localScale = new Vector3(1, 1.198f, 1.198f);
			setWheelRadius(0.8509f); // == 1.198*0.71 = 42in = 73in/2

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 4;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 4;

			leftReachRod.gameObject.SetActive(false);
			rightReachRod.gameObject.SetActive(false);

			//hide mechanical lubricator linkage
			driveR.Find("s282_mech_cutoff_rail/s282_mech_lubricator_rod").gameObject.SetActive(false);
			driveR.Find("LubricatorParts").gameObject.SetActive(false);

			splitS282A.MoveCylinders(new Vector3(0, 0.145f, 0));

			//crosshead guide and lifting arm support
			lfCrossheadBracket.localPosition = new Vector3(0, 0.145f, 0);
			rfCrossheadBracket.localPosition = lfCrossheadBracket.localPosition;
			lLiftingArmSupport.gameObject.SetActive(false);
			rLiftingArmSupport.gameObject.SetActive(false);

			//hide or show valve gear
			if (!Main.settings.show282BigValveGear)
				HideValveGear();

			rightFirstBrakeCaliper.localPosition = new Vector3(0, 0.06f, 0.235f);
			leftFirstBrakeCaliper.localPosition = new Vector3(0, 0.06f, 0.235f);
			rightSecondBrakeCaliper.localPosition = new Vector3(0, 0.06f, -1.78f);
			leftSecondBrakeCaliper.localPosition = new Vector3(0, 0.06f, -1.78f);
			rightThirdBrakeCaliper.localPosition = new Vector3(0, 0.06f, -0.38f);
			leftThirdBrakeCaliper.localPosition = new Vector3(0, 0.06f, -0.38f);
			rightFourthBrakeCaliper.localPosition = new Vector3(0, 0.06f, -2.24f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0.06f, -2.24f);

			rightFirstBrakeShoe.localPosition = new Vector3(0, 0, 8.15f);
			leftFirstBrakeShoe.localPosition = new Vector3(0, 0, 8.15f);

			spark1L.localPosition = new Vector3(-.75f, 0, 7.45f);
			spark1R.localPosition = new Vector3(.75f, 0, 7.45f);
			spark2L.localPosition = new Vector3(-.75f, 0, 5.41f);
			spark2R.localPosition = new Vector3(.75f, 0, 5.41f);
			spark3L.localPosition = new Vector3(-.75f, 0, 3.55f);
			spark3R.localPosition = new Vector3(.75f, 0, 3.55f);
			spark4L.localPosition = new Vector3(-.75f, 0, 1.69f);
			spark4R.localPosition = new Vector3(.75f, 0, 1.69f);

			//lubrication
			lubricator.localPosition = new Vector3(-0.01f, 0.42f, 0.1f);
			lubricatorSupport.gameObject.SetActive(false);
			lubricatorSightGlass.localPosition += lubricator.localPosition;
			lubricatorHandle.localPosition += lubricator.localPosition;
			rOilLines.localPosition = lubricator.localPosition;
			fOilLines.localPosition = lubricator.localPosition;

			//move lubrication stuff in interior
			if (loco.AreExternalInteractablesLoaded)
			{
				lubricatorLabel.localPosition += lubricator.localPosition;
				lubricatorOilLevel.localPosition = lubricator.localPosition;
				lubricatorHandValveJoint.connectedAnchor += lubricator.localPosition;
			}
		}

		private void Show10Drivers()
		{
			/*//move bogie so that it lines up with the rearmost driver
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
					1.5f);*/

			//add fifth axle to ten-coupled rear bogie
			driveAxle5.gameObject.SetActive(true);

			fifthLeftDriveWheel.gameObject.SetActive(true);
			fifthRightDriveWheel.gameObject.SetActive(true);

			//show added side rods
			leftFiveAxleSideRod.gameObject.SetActive(true);
			rightFiveAxleSideRod.gameObject.SetActive(true);

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 5;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 5;

			MeshFilter fourthLeftMeshFilter = fourthLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter fourthRightMeshFilter = fourthRightDriveWheel.GetComponent<MeshFilter>();
			fourthLeftMeshFilter.mesh = blindDriver;
			fourthRightMeshFilter.mesh = blindDriver;

			//show sparks at fifth drive axle
			spark5L.gameObject.SetActive(true);
			spark5R.gameObject.SetActive(true);

			//hide or show valve gear
			if (!Main.settings.show10CoupledValveGear)
				HideValveGear();

			//show brake calipers
			leftFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			rightFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFifthBrakeCaliper.gameObject.SetActive(true);
			rightFifthBrakeCaliper.gameObject.SetActive(true);

			leftFifthBrakeShoe.gameObject.SetActive(true);
			rightFifthBrakeShoe.gameObject.SetActive(true);
		}

		private void Show12SmallDrivers()
		{
			/*//move bogie so that it lines up with the rearmost driver
			GameObject axleR = loco.transform.Find("Axle_R").gameObject;
			ConfigurableJoint axleRJoint = axleR.GetComponent<ConfigurableJoint>();
			if (axleRJoint is null)
			{
				Main.Logger.Log($"Couldn't find ConfigurableJoint on ten-coupled Axle_R.");
			}
			else
			{
				axleRJoint.autoConfigureConnectedAnchor = false;
				axleRJoint.connectedAnchor = new Vector3(axleRJoint.connectedAnchor.x, axleRJoint.connectedAnchor.y, 1.12f);
			}
			axleR.transform.localPosition =
				new Vector3(
					axleR.transform.localPosition.x,
					axleR.transform.localPosition.y,
					1.12f);*/

			driveL.localScale = new Vector3(-1, 0.84f, 0.84f);
			driveR.localScale = new Vector3(1, 0.84f, 0.84f);
			driveL.localPosition = new Vector3(0, 0, 1.1f);
			driveR.localPosition = new Vector3(0, 0, 1.1f);

			//add fifth and sixth axle to ten-coupled rear bogie
			driveAxle5.gameObject.SetActive(true);
			driveAxle6.gameObject.SetActive(true);

			fifthLeftDriveWheel.gameObject.SetActive(true);
			fifthRightDriveWheel.gameObject.SetActive(true);
			sixthLeftDriveWheel.gameObject.SetActive(true);
			sixthRightDriveWheel.gameObject.SetActive(true);

			//show added side rods
			leftSixAxleSideRod.gameObject.SetActive(true);
			rightSixAxleSideRod.gameObject.SetActive(true);

			setWheelRadius(0.6096f); // == 0.8586*0.71 = 24in = 48in/2

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 6;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 6;

			MeshFilter secondLeftMeshFilter = secondLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter secondRightMeshFilter = secondRightDriveWheel.GetComponent<MeshFilter>();
			secondLeftMeshFilter.mesh = flangedDriver;
			secondRightMeshFilter.mesh = flangedDriver;

			MeshFilter fourthLeftMeshFilter = fourthLeftDriveWheel.GetComponent<MeshFilter>();
			MeshFilter fourthRightMeshFilter = fourthRightDriveWheel.GetComponent<MeshFilter>();
			fourthLeftMeshFilter.mesh = blindDriver;
			fourthRightMeshFilter.mesh = blindDriver;

			//show sparks at fifth and sixth drive axle
			spark1L.localPosition = new Vector3(-.75f, 0, 7.2f);
			spark1R.localPosition = new Vector3(.75f, 0, 7.2f);
			spark2L.localPosition = new Vector3(-.75f, 0, 5.75f);
			spark2R.localPosition = new Vector3(.75f, 0, 5.75f);
			spark3L.localPosition = new Vector3(-.75f, 0, 4.45f);
			spark3R.localPosition = new Vector3(.75f, 0, 4.45f);
			spark4L.localPosition = new Vector3(-.75f, 0, 3.15f);
			spark4R.localPosition = new Vector3(.75f, 0, 3.15f);
			spark5L.localPosition = new Vector3(-.75f, 0, 1.85f);
			spark5R.localPosition = new Vector3(.75f, 0, 1.85f);
			spark6L.localPosition = new Vector3(-.75f, 0, 0.55f);
			spark6R.localPosition = new Vector3(.75f, 0, 0.55f);

			spark5L.gameObject.SetActive(true);
			spark5R.gameObject.SetActive(true);
			spark6L.gameObject.SetActive(true);
			spark6R.gameObject.SetActive(true);

			leftReachRod.localScale = new Vector3(1, 1, 1.2f);
			rightReachRod.localScale = new Vector3(1, 1, 1.2f);

			//hide mechanical lubricator linkage
			driveR.Find("s282_mech_cutoff_rail/s282_mech_lubricator_rod").gameObject.SetActive(false);
			driveR.Find("LubricatorParts").gameObject.SetActive(false);

			splitS282A.MoveCylinders(new Vector3(0, -0.11f, 0));

			lDryPipe.localPosition = new Vector3(0, -0.5f, 0);
			rDryPipe.localPosition = new Vector3(0, -0.5f, 0);
			lDryPipe.localScale = new Vector3(-1, 1.2f, 1);
			rDryPipe.localScale = new Vector3(-1, 1.2f, 1);

			//crosshead guide and lifting arm support
			lfCrossheadBracket.localPosition = new Vector3(0, -0.1f, 0);
			rfCrossheadBracket.localPosition = lfCrossheadBracket.localPosition;
			lLiftingArmSupport.gameObject.SetActive(false);
			rLiftingArmSupport.gameObject.SetActive(false);

			//hide or show valve gear
			if (!Main.settings.show12CoupledValveGear)
				HideValveGear();

			//show brake calipers
			rightFirstBrakeCaliper.localPosition = new Vector3(0, -0.04f, -0.26f);
			leftFirstBrakeCaliper.localPosition = new Vector3(0, -0.04f, -0.26f);
			rightSecondBrakeCaliper.localPosition = new Vector3(0, -0.04f, -1.7f);
			leftSecondBrakeCaliper.localPosition = new Vector3(0, -0.04f, -1.7f);
			rightThirdBrakeCaliper.localPosition = new Vector3(0, -0.045f, 0.25f);
			leftThirdBrakeCaliper.localPosition = new Vector3(0, -0.045f, 0.25f);
			rightFourthBrakeCaliper.localPosition = new Vector3(0, -0.045f, -1.05f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, -0.045f, -1.05f);

			leftFifthBrakeCaliper.gameObject.SetActive(true);
			rightFifthBrakeCaliper.gameObject.SetActive(true);
			leftSixthBrakeCaliper.gameObject.SetActive(true);
			rightSixthBrakeCaliper.gameObject.SetActive(true);

			leftFifthBrakeShoe.gameObject.SetActive(true);
			rightFifthBrakeShoe.gameObject.SetActive(true);
			leftSixthBrakeShoe.gameObject.SetActive(true);
			rightSixthBrakeShoe.gameObject.SetActive(true);

			//lubrication
			lubricator.localPosition = new Vector3(0f, -0.03f, 0.15f);
			lubricatorSupport.localPosition += lubricator.localPosition;
			lubricatorSightGlass.localPosition += lubricator.localPosition;
			lubricatorHandle.localPosition += lubricator.localPosition;
			rOilLines.localPosition = lubricator.localPosition;
			fOilLines.localPosition = lubricator.localPosition;

			//move lubrication and air stuff in interior
			if (loco.AreExternalInteractablesLoaded)
			{
				lubricatorLabel.localPosition += lubricator.localPosition;
				lubricatorOilLevel.localPosition = lubricator.localPosition;
				lubricatorHandValveJoint.connectedAnchor += lubricator.localPosition;
			}
		}

		private void ShowDuplex()
		{
			steamEngine.cylinderBore = 0.55f * 0.97f;
			steamEngine.pistonStroke = 0.711f * 0.97f;
			steamEngine.minCutoff = 0.01f;
			steamEngine.maxCutoff = 0.75f;

			//See DuplexAdhesionController for the reason why this exists
			loco.adhesionController.wheelSlideFrictionCoef = loco.carLivery.parentType.WheelSlideFrictionCoef * 0.5f;
			loco.adhesionController.wheelslipFrictionCoef = loco.carLivery.parentType.WheelslipFrictionCoef * 0.5f;

			//Re-enable rear engine
			newSteamEngine.maxCutoff = 0.75f;

			//Make locomotive heavier to offset increase in tractive effort
			loco.massController.CarMass = 175000;
			loco.massController.UpdateTrainCarMass();
			SetMassOnTrainPlate(175000);

			lfCylinder.gameObject.SetActive(false);
			rfCylinder.gameObject.SetActive(false);
			cylinderCocks.gameObject.SetActive(false);
			lDryPipe.gameObject.SetActive(false);
			rDryPipe.gameObject.SetActive(false);

			lfCrossheadBracket.gameObject.SetActive(true);
			rfCrossheadBracket.gameObject.SetActive(true);
			lrCrossheadBracket.gameObject.SetActive(true);
			rrCrossheadBracket.gameObject.SetActive(true);
			lLiftingArmSupport.gameObject.SetActive(false);
			rLiftingArmSupport.gameObject.SetActive(false);

			lfCrossheadBracket.GetComponent<MeshFilter>().mesh = lDuplexCrossheadGuide;
			rfCrossheadBracket.GetComponent<MeshFilter>().mesh = rDuplexCrossheadGuide;

			//hide mechanical lubricator linkage
			driveR.Find("s282_mech_cutoff_rail/s282_mech_lubricator_rod").gameObject.SetActive(false);
			driveR.Find("LubricatorParts").gameObject.SetActive(false);

			HideValveGear();
			HideReachRod();
			driveL.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").gameObject.SetActive(true);
			driveR.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").gameObject.SetActive(true);
			driveL.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, 0);
			driveR.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, 0);
			driveL2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, 0);
			driveR2.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base").localPosition = new Vector3(0.15f, 0, 0);

			driveL2.Find("s282_mech_push_rod/s282_mech_push_rod_to_cutoff_base").gameObject.SetActive(false);
			driveR2.Find("s282_mech_push_rod/s282_mech_push_rod_to_cutoff_base").gameObject.SetActive(false);

			driveL.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_rod").gameObject.SetActive(false);
			driveR.Find("s282_mech_wheels_connect/s282_mech_connect_to_cutoff_base/s282_mech_connect_to_cutoff_rod").gameObject.SetActive(false);

			leftMainRod.GetComponent<MeshFilter>().mesh = shortMainRod;
			rightMainRod.GetComponent<MeshFilter>().mesh = shortMainRod;

			//cylinder cock particle effects
			rearCylCockSteam.gameObject.SetActive(true);
			rearCylCockWaterDrip.gameObject.SetActive(true);

			//simulation changes
			duplexSim.gameObject.SetActive(true);
			oldSimCtrlr.enabled = true;
			duplexSimCtrlr.enabled = true;
			duplexDrivetrainRotaterR.enabled = true;

			spark3L.gameObject.SetActive(false);
			spark3R.gameObject.SetActive(false);
			spark4L.gameObject.SetActive(false);
			spark4R.gameObject.SetActive(false);

			sparkD1L.gameObject.SetActive(true);
			sparkD1R.gameObject.SetActive(true);
			sparkD2L.gameObject.SetActive(true);
			sparkD2R.gameObject.SetActive(true);

			if (valveGearMechanism is not null)
			{
				//unfortunately this blows up the log. I should probably fix this at some point
				//valveGearMechanism.gameObject.SetActive(false);
			}
		}

		private void Show2442Duplex()
		{
			thirdLeftDriveWheel.gameObject.SetActive(false);
			thirdRightDriveWheel.gameObject.SetActive(false);
			fourthLeftDriveWheel.gameObject.SetActive(false);
			fourthRightDriveWheel.gameObject.SetActive(false);
			leftThirdBrakeCaliper.gameObject.SetActive(false);
			rightThirdBrakeCaliper.gameObject.SetActive(false);
			leftFourthBrakeCaliper.gameObject.SetActive(true);
			rightFourthBrakeCaliper.gameObject.SetActive(true);
			leftFifthBrakeCaliper.gameObject.SetActive(true);
			rightFifthBrakeCaliper.gameObject.SetActive(true);
			leftThirdBrakeShoe.gameObject.SetActive(false);
			rightThirdBrakeShoe.gameObject.SetActive(false);
			leftFourthBrakeShoe.gameObject.SetActive(true);
			rightFourthBrakeShoe.gameObject.SetActive(true);
			leftFifthBrakeShoe.gameObject.SetActive(true);
			rightFifthBrakeShoe.gameObject.SetActive(true);

			rightSecondBrakeCaliper.localPosition = new Vector3(0, 0, -4.63f);
			leftSecondBrakeCaliper.localPosition = new Vector3(0, 0, -4.63f);
			rightFourthBrakeCaliper.localPosition = new Vector3(0, 0, 1.59f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0, 1.59f);
			rightFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);

			leftFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.51f);
			rightFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.51f);

			/*rightFourthBrakeCaliper.localPosition = new Vector3(0, 0, -1.37f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0, -1.37f);
			rightFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);*/

			/*rightFourthBrakeCaliper.localPosition = new Vector3(0, 0, -1.37f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0, -1.37f);
			rightFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.51f);
			rightFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.51f);*/

			driveAxle3.gameObject.SetActive(false);
			driveAxle5.gameObject.SetActive(true);

			leftSideRod.GetComponent<MeshRenderer>().enabled = false;
			rightSideRod.GetComponent<MeshRenderer>().enabled = false;
			leftTwoAxleDuplexSideRod.gameObject.SetActive(true);
			rightTwoAxleDuplexSideRod.gameObject.SetActive(true);

			driveL2.Find("s282_wheels_driving_1").GetComponent<MeshFilter>().mesh = blindDriver;
			driveR2.Find("s282_wheels_driving_1").GetComponent<MeshFilter>().mesh = blindDriver;
			driveL2.Find("s282_wheels_driving_2").GetComponent<MeshFilter>().mesh = flangedDriver;
			driveR2.Find("s282_wheels_driving_2").GetComponent<MeshFilter>().mesh = flangedDriver;

			driveL.localPosition = new Vector3(0, 0, 0);
			driveR.localPosition = new Vector3(0, 0, 0);
			driveL2.localPosition = new Vector3(0, 0, -4.65f);
			driveR2.localPosition = new Vector3(0, 0, -4.65f);
			driveL.localScale = new Vector3(-1, 1, 1);
			driveR.localScale = new Vector3(1, 1, 1);
			driveL2.localScale = new Vector3(-1, 1, 1);
			driveR2.localScale = new Vector3(1, 1, 1);
			driveL2.Find("s282_wheels_driving_3").gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_3").gameObject.SetActive(false);
			driveL2.Find("s282_wheels_driving_4").gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_4").gameObject.SetActive(false);

			lfCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			lfCrossheadBracket.localPosition = new Vector3(0, 0, 0.065f);
			rfCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			rfCrossheadBracket.localPosition = new Vector3(0, 0, 0.065f);
			lrCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			lrCrossheadBracket.localPosition = new Vector3(0, 0f, -4.58f);
			rrCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			rrCrossheadBracket.localPosition = new Vector3(0, 0f, -4.58f);

			//lubrication
			lubricator.localPosition = new Vector3(-0.01f, 0.42f, 0.1f);
			lubricatorSupport.gameObject.SetActive(false);
			lubricatorSightGlass.localPosition += lubricator.localPosition;
			lubricatorHandle.localPosition += lubricator.localPosition;
			rOilLines.localPosition = lubricator.localPosition;
			fOilLines.localPosition = lubricator.localPosition;

			//move lubrication stuff in interior
			if (loco.AreExternalInteractablesLoaded)
			{
				lubricatorLabel.localPosition += lubricator.localPosition;
				lubricatorOilLevel.localPosition = lubricator.localPosition;
				lubricatorHandValveJoint.connectedAnchor += lubricator.localPosition;
			}

			splitS282A.MoveCylinders(new Vector3(0, 0.09f, 0));

			lfFranklinValveGear.gameObject.SetActive(true);
			rfFranklinValveGear.gameObject.SetActive(true);
			lBranchPipe.gameObject.SetActive(true);
			rBranchPipe.gameObject.SetActive(true);
			lBranchPipe4444.gameObject.SetActive(false);
			rBranchPipe4444.gameObject.SetActive(false);
			driveL2.gameObject.SetActive(true);
			driveR2.gameObject.SetActive(true);

			float time = lfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			lfAnimator.runtimeAnimatorController = lfFranklinAnimatorCtrl;
			rfAnimator.runtimeAnimatorController = rfFranklinAnimatorCtrl;
			lfAnimator.Play(lfAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, time);
			rfAnimator.Play(rfAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, (time + 0.75f) % 1);
			lrAnimator.Play(lrAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, (time + 0.0f) % 1);
			rrAnimator.Play(rrAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, (time + 0.75f) % 1);

			lFranklinBReverseGear.gameObject.SetActive(true);
			rFranklinBReverseGear.gameObject.SetActive(true);
			lFranklinBReverseGear4444.gameObject.SetActive(false);
			rFranklinBReverseGear4444.gameObject.SetActive(false);
		}

		private void Show4444Duplex()
		{
			thirdLeftDriveWheel.gameObject.SetActive(false);
			thirdRightDriveWheel.gameObject.SetActive(false);
			fourthLeftDriveWheel.gameObject.SetActive(false);
			fourthRightDriveWheel.gameObject.SetActive(false);
			leftThirdBrakeCaliper.gameObject.SetActive(false);
			rightThirdBrakeCaliper.gameObject.SetActive(false);
			leftFourthBrakeCaliper.gameObject.SetActive(true);
			rightFourthBrakeCaliper.gameObject.SetActive(true);
			leftFifthBrakeCaliper.gameObject.SetActive(true);
			rightFifthBrakeCaliper.gameObject.SetActive(true);
			leftThirdBrakeShoe.gameObject.SetActive(false);
			rightThirdBrakeShoe.gameObject.SetActive(false);
			leftFourthBrakeShoe.gameObject.SetActive(true);
			rightFourthBrakeShoe.gameObject.SetActive(true);
			leftFifthBrakeShoe.gameObject.SetActive(true);
			rightFifthBrakeShoe.gameObject.SetActive(true);

			leftFirstBrakeCaliper.localPosition = new Vector3(0, 0, -0.21f);
			rightFirstBrakeCaliper.localPosition = new Vector3(0, 0, -0.21f);
			leftSecondBrakeCaliper.localPosition = new Vector3(0, 0, -4.6325f);
			rightSecondBrakeCaliper.localPosition = new Vector3(0, 0, -4.6325f);
			rightFourthBrakeCaliper.localPosition = new Vector3(0, 0, 1.33f);
			leftFourthBrakeCaliper.localPosition = new Vector3(0, 0, 1.33f);
			rightFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFifthBrakeCaliper.localPosition = new Vector3(0, 0, -3.08f);
			leftFirstBrakeShoe.localPosition = new Vector3(0, 0, 7.92f);
			rightFirstBrakeShoe.localPosition = new Vector3(0, 0, 7.92f);
			leftSecondBrakeShoe.localPosition = new Vector3(0, 0, 6.21f);
			rightSecondBrakeShoe.localPosition = new Vector3(0, 0, 6.21f);
			leftFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.51f);
			rightFourthBrakeShoe.localPosition = new Vector3(0, 0, 3.51f);

			driveAxle3.gameObject.SetActive(false);
			driveAxle5.gameObject.SetActive(true);

			leftSideRod.GetComponent<MeshRenderer>().enabled = false;
			rightSideRod.GetComponent<MeshRenderer>().enabled = false;
			leftTwoAxleDuplexSideRod.gameObject.SetActive(true);
			rightTwoAxleDuplexSideRod.gameObject.SetActive(true);

			driveL2.Find("s282_wheels_driving_1").GetComponent<MeshFilter>().mesh = blindDriver;
			driveR2.Find("s282_wheels_driving_1").GetComponent<MeshFilter>().mesh = blindDriver;
			driveL2.Find("s282_wheels_driving_2").GetComponent<MeshFilter>().mesh = flangedDriver;
			driveR2.Find("s282_wheels_driving_2").GetComponent<MeshFilter>().mesh = flangedDriver;

			driveL.localPosition = new Vector3(0, 0, 0);
			driveR.localPosition = new Vector3(0, 0, 0);
			driveL2.localPosition = new Vector3(0, 0, -4.65f);
			driveR2.localPosition = new Vector3(0, 0, -4.65f);
			driveL.localScale = new Vector3(-1, 1, 1);
			driveR.localScale = new Vector3(1, 1, 1);
			driveL2.localScale = new Vector3(-1, 1, 1);
			driveR2.localScale = new Vector3(1, 1, 1);
			driveL2.Find("s282_wheels_driving_3").gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_3").gameObject.SetActive(false);
			driveL2.Find("s282_wheels_driving_4").gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_4").gameObject.SetActive(false);

			firstLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 7.76f);
			firstRightDriveWheel.localPosition = new Vector3(0, 0.71f, 7.76f);
			secondLeftDriveWheel.localPosition = new Vector3(0, 0.71f, 6.06f);
			secondRightDriveWheel.localPosition = new Vector3(0, 0.71f, 6.06f);

			lfCrossheadBracket.localScale = new Vector3(-1, 1, 1.33f);
			lfCrossheadBracket.localPosition = new Vector3(0, 0, -0.92f);
			rfCrossheadBracket.localScale = new Vector3(-1, 1, 1.33f);
			rfCrossheadBracket.localPosition = new Vector3(0, 0, -0.92f);
			lrCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			lrCrossheadBracket.localPosition = new Vector3(0, 0f, -4.58f);
			rrCrossheadBracket.localScale = new Vector3(-1, 1, 1);
			rrCrossheadBracket.localPosition = new Vector3(0, 0f, -4.58f);

			leftCrosshead.localScale = new Vector3(1, 1, 1.35f);
			rightCrosshead.localScale = leftCrosshead.localScale;

			//lubrication
			lubricator.localPosition = new Vector3(-0.01f, 0.42f, 0.33f);
			lubricatorSupport.gameObject.SetActive(false);
			lubricatorSightGlass.localPosition += lubricator.localPosition;
			lubricatorHandle.localPosition += lubricator.localPosition;
			rOilLines.localPosition = lubricator.localPosition;
			fOilLines.localPosition = lubricator.localPosition;

			//move lubrication stuff in interior
			if (loco.AreExternalInteractablesLoaded)
			{
				lubricatorLabel.localPosition += lubricator.localPosition;
				lubricatorOilLevel.localPosition = lubricator.localPosition;
				lubricatorHandValveJoint.connectedAnchor += lubricator.localPosition;
			}

			splitS282A.MoveCylinders(new Vector3(0, 0.09f, 0.35f));

			lfFranklinValveGear4444.gameObject.SetActive(true);
			rfFranklinValveGear4444.gameObject.SetActive(true);
			lBranchPipe.gameObject.SetActive(false);
			rBranchPipe.gameObject.SetActive(false);
			lBranchPipe4444.gameObject.SetActive(true);
			rBranchPipe4444.gameObject.SetActive(true);
			driveL2.gameObject.SetActive(true);
			driveR2.gameObject.SetActive(true);

			float time = lfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			lfAnimator.runtimeAnimatorController = lfFranklinAnimatorCtrl4444;
			rfAnimator.runtimeAnimatorController = rfFranklinAnimatorCtrl4444;
			lfAnimator.Play(lfAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, time);
			rfAnimator.Play(rfAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, (time + 0.75f) % 1);
			lrAnimator.Play(lrAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, (time + 0.0f) % 1);
			rrAnimator.Play(rrAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, (time + 0.75f) % 1);

			lFranklinBReverseGear.gameObject.SetActive(false);
			rFranklinBReverseGear.gameObject.SetActive(false);
			lFranklinBReverseGear4444.gameObject.SetActive(true);
			rFranklinBReverseGear4444.gameObject.SetActive(true);
		}


		private void Showx66xDuplex()
		{
			steamEngine.numCylinders = 4;
			steamEngine.cylinderBore = 0.55f * 0.85f;
			steamEngine.pistonStroke = 0.711f * 0.85f;

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 6;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 6;

			fourthLeftDriveWheel.gameObject.SetActive(false);
			fourthRightDriveWheel.gameObject.SetActive(false);
			leftFourthBrakeCaliper.gameObject.SetActive(false);
			rightFourthBrakeCaliper.gameObject.SetActive(false);
			leftFourthBrakeShoe.gameObject.SetActive(false);
			rightFourthBrakeShoe.gameObject.SetActive(false);

			driveL.localPosition = new Vector3(0, 0, 2.2f);
			driveR.localPosition = new Vector3(0, 0, 2.2f);
			driveL2.localPosition = new Vector3(0, 0, -2.75f);
			driveR2.localPosition = new Vector3(0, 0, -2.75f);
			driveL.localScale = new Vector3(-1, 0.8f, 0.8f);
			driveR.localScale = new Vector3(1, 0.8f, 0.8f);
			driveL2.localScale = new Vector3(-1, 0.8f, 0.8f);
			driveR2.localScale = new Vector3(1, 0.8f, 0.8f);
			driveL2.Find("s282_wheels_driving_3").gameObject.SetActive(true);
			driveR2.Find("s282_wheels_driving_3").gameObject.SetActive(true);
			driveL2.Find("s282_wheels_driving_4").gameObject.SetActive(false);
			driveR2.Find("s282_wheels_driving_4").gameObject.SetActive(false);

			driveL2.Find("s282_wheels_driving_2").GetComponent<MeshFilter>().mesh = blindDriver;
			driveR2.Find("s282_wheels_driving_2").GetComponent<MeshFilter>().mesh = blindDriver;
			driveL2.Find("s282_wheels_driving_3").GetComponent<MeshFilter>().mesh = flangedDriver;
			driveR2.Find("s282_wheels_driving_3").GetComponent<MeshFilter>().mesh = flangedDriver;

			lfCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			lfCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);
			rfCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			rfCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);
			lrCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			lrCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);
			rrCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			rrCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);

			lfCrossheadBracket.gameObject.SetActive(true);
			rfCrossheadBracket.gameObject.SetActive(true);
			driveL2.gameObject.SetActive(true);
			driveR2.gameObject.SetActive(true);

			driveAxle5.gameObject.SetActive(true);
			driveAxle6.gameObject.SetActive(true);
		}

		private void Showx68xDuplex()
		{
			steamEngine.numCylinders = 4;

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 7;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 7;

			fourthLeftDriveWheel.gameObject.SetActive(false);
			fourthRightDriveWheel.gameObject.SetActive(false);
			leftFourthBrakeCaliper.gameObject.SetActive(false);
			rightFourthBrakeCaliper.gameObject.SetActive(false);
			leftFourthBrakeShoe.gameObject.SetActive(false);	
			rightFourthBrakeShoe.gameObject.SetActive(false);

			driveL.localPosition = new Vector3(0, 0, 2.2f);
			driveR.localPosition = new Vector3(0, 0, 2.2f);
			driveL2.localPosition = new Vector3(0, 0, -2.75f);
			driveR2.localPosition = new Vector3(0, 0, -2.75f);
			driveL.localScale = new Vector3(-1, 0.8f, 0.8f);
			driveR.localScale = new Vector3(1, 0.8f, 0.8f);
			driveL2.localScale = new Vector3(-1, 0.8f, 0.8f);
			driveR2.localScale = new Vector3(1, 0.8f, 0.8f);

			lfCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			lfCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);
			rfCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			rfCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);
			lrCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			lrCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);
			rrCrossheadBracket.localScale = new Vector3(-0.8f, 1, 1);
			rrCrossheadBracket.localPosition = new Vector3(0.215f, 0, 4.88f);

			lfCrossheadBracket.gameObject.SetActive(true);
			rfCrossheadBracket.gameObject.SetActive(true);

			driveL2.gameObject.SetActive(true);
			driveR2.gameObject.SetActive(true);
			driveL2.Find("s282_wheels_driving_4").gameObject.SetActive(true);
			driveR2.Find("s282_wheels_driving_4").gameObject.SetActive(true);
		}

		//used for the 4-0-4
		private void ShowNoDrivers()
		{
			driveL.gameObject.SetActive(false);
			driveR.gameObject.SetActive(false);

			leftFirstBrakeCaliper.gameObject.SetActive(false);
			leftSecondBrakeCaliper.gameObject.SetActive(false);
			leftThirdBrakeCaliper.gameObject.SetActive(false);
			leftFourthBrakeCaliper.gameObject.SetActive(false);
			leftFifthBrakeCaliper.gameObject.SetActive(false);
			leftSixthBrakeCaliper.gameObject.SetActive(false);
			rightFirstBrakeCaliper.gameObject.SetActive(false);
			rightSecondBrakeCaliper.gameObject.SetActive(false);
			rightThirdBrakeCaliper.gameObject.SetActive(false);
			rightFourthBrakeCaliper.gameObject.SetActive(false);
			rightFifthBrakeCaliper.gameObject.SetActive(false);
			rightSixthBrakeCaliper.gameObject.SetActive(false);

			leftFirstBrakeShoe.gameObject.SetActive(false);
			leftSecondBrakeShoe.gameObject.SetActive(false);
			leftThirdBrakeShoe.gameObject.SetActive(false);
			leftFourthBrakeShoe.gameObject.SetActive(false);
			leftFifthBrakeShoe.gameObject.SetActive(false);
			leftSixthBrakeShoe.gameObject.SetActive(false);
			rightFirstBrakeShoe.gameObject.SetActive(false);
			rightSecondBrakeShoe.gameObject.SetActive(false);
			rightThirdBrakeShoe.gameObject.SetActive(false);
			rightFourthBrakeShoe.gameObject.SetActive(false);
			rightFifthBrakeShoe.gameObject.SetActive(false);
			rightSixthBrakeShoe.gameObject.SetActive(false);

			driveAxle1.gameObject.SetActive(false);
			driveAxle2.gameObject.SetActive(false);
			driveAxle3.gameObject.SetActive(false);
			driveAxle4.gameObject.SetActive(false);

			steamEngine.maxCutoff = 0f;

			loco.transform.Find("[sim]/poweredAxles").GetComponent<ConfigurablePortDefinition>().value = 0;
			loco.transform.Find("[sim]/traction").GetComponent<WheelslipController>().numberOfPoweredAxlesPort.Value = 0;
		}

		//------ SHOW TRAILING WHEELS ------

		//Show the vanilla big trailing axle
		private void ShowTwoTrailingWheelsVanilla()
		{
			firstRearAxle.localPosition = new Vector3(0, 0.53f, -2.53f);
			firstRearAxle.gameObject.SetActive(true);
		}

		//Show the small trailing axle where the vanilla trailing axle is
		private void ShowTwoTrailingWheels()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.36f, -2.53f);
			secondRearAxle.gameObject.SetActive(true);
		}

		//Show the small trailing axle at the position suggested by discord
		private void ShowTwoTrailingWheelsAlternate()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.36f, -2.25f);
			secondRearAxle.gameObject.SetActive(true);
		}

		private void ShowTwoTrailingWheelsFor4Coupled()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.44f, -2.53f);
			secondRearAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			secondRearAxle.gameObject.SetActive(true);
		}

		private void ShowTwoTrailingWheelsFor4CoupledAlternate()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.44f, -2.25f);
			secondRearAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			secondRearAxle.gameObject.SetActive(true);
		}

		//show the vanilla training wheels in the position needed for the x-6-2 engines
		private void ShowTwoTrailingWheelsFor4CoupledVanilla()
		{
			firstRearAxle.transform.localPosition = new Vector3(0, 0.53f, -2.53f);
			firstRearAxle.gameObject.SetActive(true);
		}

		private void ShowTwoTrailingWheelsFor6Coupled()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.44f, -3.1f);
			secondRearAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			secondRearAxle.gameObject.SetActive(true);
		}

		//show the vanilla training wheels in the position needed for the 4-6-2 engines
		private void ShowTwoTrailingWheelsFor6CoupledVanilla()
		{
			firstRearAxle.transform.localPosition = new Vector3(0, 0.53f, -3.1f);
			secondRearAxleSupport.transform.localPosition = new Vector3(0, 0.57f, 0.1f);
			secondRearAxleSupport.transform.localScale = new Vector3(1, 1.4f, 1.4f);
			firstRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		//Show the small trailing axle where the vanilla trailing axle is and scale it up
		private void ShowTwoTrailingWheelsFor482()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.44f, -3.1f);
			secondRearAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			secondRearAxleSupport.localPosition = new Vector3(0, 0.44f, 0.1f);
			secondRearAxleSupport.localScale = new Vector3(1, 1.7f, 1.3f);
			secondRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		//the ten-coupled engines (x-10-2) need a trailing axle further back than the
		//trailing axle for the eight coupled engines (x-8-2).
		private void ShowTwoTrailingWheelsFor10Coupled()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.36f, -3.1f);
			secondRearAxleSupport.localPosition = new Vector3(0, 0.44f, 0.1f);
			secondRearAxleSupport.localScale = new Vector3(1, 1.7f, 1.3f);
			secondRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		//show the vanilla training wheels in the position needed for the x-10-2 engines
		private void ShowTwoTrailingWheelsFor10CoupledVanilla()
		{
			firstRearAxle.transform.localPosition = new Vector3(0, 0.53f, -3.1f);
			secondRearAxleSupport.transform.localPosition = new Vector3(0, 0.6f, 0.1f);
			secondRearAxleSupport.transform.localScale = new Vector3(1, 1.34f, 1.34f);
			firstRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		//show the small training wheels in the position needed for the x-12-2 engines
		private void ShowTwoTrailingWheelsFor12Coupled()
		{
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -3.1f);
			secondRearAxleSupport.transform.localPosition = new Vector3(0, 0.57f, 0.1f);
			secondRearAxleSupport.transform.localScale = new Vector3(1, 1.4f, 1.4f);
			secondRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		private void ShowFourTrailingWheelsFor4Coupled()
		{
			secondRearAxle.transform.localPosition = new Vector3(0, 0.44f, -2.05f);
			secondRearAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			thirdRearAxle.transform.localPosition = new Vector3(0, 0.44f, -3.5f);
			thirdRearAxle.localScale = new Vector3(1, 1.25f, 1.25f);
			secondRearAxleSupport.transform.localPosition = new Vector3(0, 0.4f, -0.3f);
			secondRearAxleSupport.transform.localScale = new Vector3(1, 1.7f, 1.3f);
			secondRearAxle.gameObject.SetActive(true);
			thirdRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		//Show four trailing wheels for 8-coupled engines
		private void ShowFourTrailingWheels()
		{
			secondRearAxle.localPosition = new Vector3(0, 0.36f, -2.52f);
			thirdRearAxle.localPosition = new Vector3(0, 0.36f, -1.32f);
			secondRearAxle.gameObject.SetActive(true);
			thirdRearAxle.gameObject.SetActive(true);
		}

		private void ShowFourTrailingWheelsForBig8Coupled()
		{
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -2.52f);
			thirdRearAxle.transform.localPosition = new Vector3(0, 0.36f, -3.7f);
			secondRearAxleSupport.transform.localPosition = new Vector3(0, 0.4f, -0.5f);
			secondRearAxleSupport.transform.localScale = new Vector3(1, 1.7f, 1.3f);
			secondRearAxle.gameObject.SetActive(true);
			thirdRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		private void ShowFourTrailingWheelsFor10Coupled()
		{
			secondRearAxle.transform.localPosition = new Vector3(0, 0.36f, -2.7f);
			thirdRearAxle.transform.localPosition = new Vector3(0, 0.36f, -3.7f);
			secondRearAxleSupport.transform.localPosition = new Vector3(0, 0.4f, -0.5f);
			secondRearAxleSupport.transform.localScale = new Vector3(1, 1.7f, 1.3f);
			secondRearAxle.gameObject.SetActive(true);
			thirdRearAxle.gameObject.SetActive(true);
			secondRearAxleSupport.gameObject.SetActive(true);
		}

		private void SetMassOnTrainPlate(int mass)
		{
			string massText = Mathf.RoundToInt(mass) + "kg";
			string lengthText = $"{loco.InterCouplerDistance:0.#}m";
			loco.trainPlatesCtrl.carMassLengthText = massText + lengthText.PadLeft(23 - massText.Length);
			foreach (TrainCarPlate trainCarPlate in loco.trainPlatesCtrl.trainCarPlates)
			{
				trainCarPlate.carMassLength.text = loco.trainPlatesCtrl.carMassLengthText;
			}
		}
	}
}
