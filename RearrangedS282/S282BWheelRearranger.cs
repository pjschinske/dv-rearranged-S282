using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Ports;
using DV.ThingTypes;
using DV.Wheels;
using LocoMeshSplitter.MeshLoaders;
using LocoSim.Definitions;
using LocoSim.Implementations.Wheels;
using RearrangedS282.Sim.Booster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace RearrangedS282
{
	internal class S282BWheelRearranger : MonoBehaviour
	{
		private static readonly Random rand = new();

		private TrainCar loco;
		private SimController simCtrlr;

		void Start()
		{
			loco = GetComponent<TrainCar>();

			if (loco == null)
			{
				Main.Logger.Error("S282BWheelRearranger initialized on something that isn't a train car");
				return;
			}
			if (loco.carType != TrainCarType.LocoSteamHeavy)
			{
				Main.Logger.Error("S282BWheelRearranger initialized on something that isn't an S282B");
				return;
			}

			simCtrlr = GetComponent<SimController>();

			SetupBoosterSimulation();

			//set locomotive to initial wheel arrangement
			if (Main.settings.spawnRandomWA)
			{
				SwitchWheelArrangementRandom();
			}
			else
			{
				SwitchWheelArrangement((int)S282BWheelArrangementType.vanilla);
			}
		}

		private void SwitchWheelArrangementRandom()
		{
			int wa = rand.Next(2);
			SwitchWheelArrangement(wa);
		}

		private void SwitchWheelArrangement(int wa)
		{

		}

		private void SetupBoosterSimulation()
		{
			Transform sim = transform.Find("[sim]");
			GameObject steamEngine = new GameObject("steamEngine");
			GameObject exhaust = new GameObject("exhaust");
			GameObject traction = new GameObject("traction");
			steamEngine.transform.parent = sim;
			exhaust.transform.parent = sim;
			traction.transform.parent = sim;
			var steamEngineDef = steamEngine.AddComponent<BoosterReciprocatingSteamEngineDefinition>();
			var exhaustDef = exhaust.AddComponent<BoosterSteamExhaustDefinition>();
			var tractionDef = traction.AddComponent<TractionDefinition>();
			var tractionPortsFeeder = traction.AddComponent<TractionPortsFeeder>();
			var drivingForce = traction.AddComponent<DrivingForce>();
			var wheelslipCtrlr = traction.AddComponent<WheelslipController>();

			simCtrlr.tractionPortsFeeder = tractionPortsFeeder;
			simCtrlr.drivingForce = drivingForce;
			simCtrlr.wheelslipController = wheelslipCtrlr;

			//Setup powered axles on front bogie
			Transform bogieF = transform.Find("BogieF");
			Bogie bogie = bogieF.GetComponent<Bogie>();
			Transform bogieCar = bogieF.Find("bogie_car");
			GameObject axle1 = new GameObject("[axle] drive 1");
			GameObject axle2 = new GameObject("[axle] drive 2");
			axle1.transform.parent = bogieCar;
			axle2.transform.parent = bogieCar;
			var axle1PoweredWheel = axle1.AddComponent<PoweredWheel>();
			var axle2PoweredWheel = axle2.AddComponent<PoweredWheel>();
			axle1PoweredWheel.wheelTransform = bogie.axles[1].transform;
			axle2PoweredWheel.wheelTransform = bogie.axles[0].transform;

			GameObject poweredWheels = new GameObject("[poweredWheels]");
			poweredWheels.transform.parent = transform;
			var poweredWheelsManager = poweredWheels.AddComponent<PoweredWheelsManager>();
			poweredWheelsManager.poweredWheels = new PoweredWheel[] { axle1PoweredWheel, axle2PoweredWheel };

			//By default, there's just one wheel rotater. We need to add the PoweredWheelRotation
			//for the front axles, and remove the front axles from the old WheelRotation code.
			var rearWheelRotation = GetComponent<WheelRotationViaCode>();
			var frontWheelRotation = transform.gameObject.AddComponent<PoweredWheelRotationViaCode>();
			rearWheelRotation.transformsToRotate = new Transform[]
			{
				rearWheelRotation.transformsToRotate[0],
				rearWheelRotation.transformsToRotate[1]
			};

			var broadcastPortCtrlr = simCtrlr.broadcastPortController;
			BroadcastPortValueConsumer steamPressureConsumer = new BroadcastPortValueConsumer();
			steamPressureConsumer.consumerPortId = "throttleCalculator.STEAM_CHEST_PRESSURE";
			steamPressureConsumer.connectionTag = "RearrangedS282_THROTTLE_STEAM_CHEST_PRESSURE";
			steamPressureConsumer.disconnectedValue = 1f; //atmospheric pressure
			steamPressureConsumer.propagateConsumerValueChangeBackToProvider = true;
			BroadcastPortValueConsumer steamTempConsumer = new BroadcastPortValueConsumer();
			steamTempConsumer.consumerPortId = "firebox.TEMPERATURE";
			steamTempConsumer.connectionTag = "RearrangedS282_STEAM_TEMPERATURE";
			//for some reason this is the temp that the S282A spawns in at
			steamTempConsumer.disconnectedValue = 101.6894f;
			steamTempConsumer.propagateConsumerValueChangeBackToProvider = false;
			
		}
	}
}
