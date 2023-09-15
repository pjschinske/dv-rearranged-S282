using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace RearrangedS282
{
	[Serializable]
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		public enum X82Options
		{
			[Description("Use vanilla-style big trailing wheels")]
			vanilla,

			[Description("Use smaller trailing wheels")]
			small_wheels,

			[Description("Use smaller wheels, but slightly further forward")]
			small_wheels_alternate
		}

		public enum X102Options
		{
			[Description("Use vanilla-style big trailing wheels")]
			vanilla,

			[Description("Use smaller trailing wheels")]
			small_wheels,
		}

		private const int HEIGHT = 12;

		/*[Draw("Allow the S282's wheels to turn when destroyed:")] public bool explodedDrivetrainRotate = true;*/

		[Header("Reapply wheel arrangement to update")]
		[Space(5)]
		[Header("Trailing truck style")]
		[Draw("4-4-2 trailing truck style:", DrawType.ToggleGroup)] public X82Options x42Options = X82Options.vanilla;
		[Draw("4-6-2 trailing truck style:", DrawType.ToggleGroup)] public X102Options x62Options = X102Options.vanilla;
		[Draw("x-8-2 trailing truck style:", DrawType.ToggleGroup)] public X82Options x82Options = X82Options.small_wheels;
		[Draw("x-10-2 trailing truck style:", DrawType.ToggleGroup)] public X102Options x102Options = X102Options.small_wheels;

		[Header("Valve gear")]
		[Draw("Show x-4-x valve gear:", Height = HEIGHT)] public bool show4CoupledValveGear = false;
		[Draw("Show x-6-x valve gear:", Height = HEIGHT)] public bool show6CoupledValveGear = false;
		[Draw("Show x-8-x valve gear:", Height = HEIGHT)] public bool show8CoupledValveGear = true;
		[Draw("Show high-speed 2-8-2 valve gear:", Height = HEIGHT)] public bool show282BigValveGear = false;
		[Draw("Show x-10-x valve gear:", Height = HEIGHT)] public bool show10CoupledValveGear = true;
		[Draw("Show x-12-x valve gear:", Height = HEIGHT)] public bool show12CoupledValveGear = true;

		[Header("Random spawns")]
		[Draw("Spawn S282 with random wheel arrangement:", DrawType.Toggle, Height = HEIGHT)] public bool spawnRandomWA = true;
		
		[Draw("Randomly spawn 4-4-0:", Height = HEIGHT)] public bool spawn440 = true;
		[Draw("Randomly spawn 4-4-2:", Height = HEIGHT)] public bool spawn442 = true;
		[Draw("Randomly spawn 4-4-4:", Height = HEIGHT)] public bool spawn444 = true;
		[Draw("Randomly spawn 4-6-0:", Height = HEIGHT)] public bool spawn460 = true;
		[Draw("Randomly spawn 4-6-2:", Height = HEIGHT)] public bool spawn462 = true;
		/*[Draw("Randomly spawn 4-6-4:", Height = HEIGHT)] public bool spawn464 = true;*/
		[Draw("Randomly spawn 0-8-0:", Height = HEIGHT)] public bool spawn080 = true;
		[Draw("Randomly spawn 0-8-2:", Height = HEIGHT)] public bool spawn082 = true;
		[Draw("Randomly spawn 0-8-4:", Height = HEIGHT)] public bool spawn084 = true;
		[Draw("Randomly spawn 2-8-0:", Height = HEIGHT)] public bool spawn280 = true;
		[Draw("Randomly spawn 2-8-2:", Height = HEIGHT)] public bool spawn282 = true;
		[Draw("Randomly spawn high-speed 2-8-2:", Height = HEIGHT)] public bool spawn282Big = true;
		[Draw("Randomly spawn 2-8-4:", Height = HEIGHT)] public bool spawn284 = true;
		[Draw("Randomly spawn 4-8-0:", Height = HEIGHT)] public bool spawn480 = true;
		[Draw("Randomly spawn 4-8-2:", Height = HEIGHT)] public bool spawn482 = true;
		[Draw("Randomly spawn 4-8-4:", Height = HEIGHT)] public bool spawn484 = true;
		[Draw("Randomly spawn 0-10-0:", Height = HEIGHT)] public bool spawn0100 = true;
		[Draw("Randomly spawn 0-10-2:", Height = HEIGHT)] public bool spawn0102 = true;
		[Draw("Randomly spawn 0-10-4:", Height = HEIGHT)] public bool spawn0104 = true;
		[Draw("Randomly spawn 2-10-0:", Height = HEIGHT)] public bool spawn2100 = true;
		[Draw("Randomly spawn 2-10-2:", Height = HEIGHT)] public bool spawn2102 = true;
		[Draw("Randomly spawn 2-10-4:", Height = HEIGHT)] public bool spawn2104 = true;
		[Draw("Randomly spawn 4-10-0:", Height = HEIGHT)] public bool spawn4100 = true;
		[Draw("Randomly spawn 4-10-2:", Height = HEIGHT)] public bool spawn4102 = true;
		[Draw("Randomly spawn 4-10-4:", Height = HEIGHT)] public bool spawn4104 = true;
		[Draw("Randomly spawn 0-12-0:", Height = HEIGHT)] public bool spawn0120 = true;
		[Draw("Randomly spawn 2-12-0:", Height = HEIGHT)] public bool spawn2120 = true;
		[Draw("Randomly spawn 2-12-2:", Height = HEIGHT)] public bool spawn2122 = true;
		[Draw("Randomly spawn 4-12-2:", Height = HEIGHT)] public bool spawn4122 = true;

		public List<WheelArrangementType> RandomWAs;

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}

		public void OnChange() {
			RandomWAs = new();
			if (spawn440)
				RandomWAs.Add(WheelArrangementType.s440);
			if (spawn442)
				RandomWAs.Add(WheelArrangementType.s442);
			if (spawn444)
				RandomWAs.Add(WheelArrangementType.s444);

			if (spawn460)
				RandomWAs.Add(WheelArrangementType.s460);
			if (spawn462)
				RandomWAs.Add(WheelArrangementType.s462);

			//EIGHT COUPLED

			if (spawn080)
				RandomWAs.Add(WheelArrangementType.s080);
			if (spawn082)
				RandomWAs.Add(WheelArrangementType.s082);
			if (spawn084)
				RandomWAs.Add(WheelArrangementType.s084);

			if (spawn280)
				RandomWAs.Add(WheelArrangementType.s280);
			if (spawn282)
				RandomWAs.Add(WheelArrangementType.s282);
			if (spawn282Big)
				RandomWAs.Add(WheelArrangementType.s282Big);
			if (spawn284)
				RandomWAs.Add(WheelArrangementType.s284);

			if (spawn480)
				RandomWAs.Add(WheelArrangementType.s480);
			if (spawn482)
				RandomWAs.Add(WheelArrangementType.s482);
			if (spawn484)
				RandomWAs.Add(WheelArrangementType.s484);

			//TEN COUPLED

			if (spawn0100)
				RandomWAs.Add(WheelArrangementType.s080);
			if (spawn0102)
				RandomWAs.Add(WheelArrangementType.s082);
			if (spawn0104)
				RandomWAs.Add(WheelArrangementType.s084);

			if (spawn2100)
				RandomWAs.Add(WheelArrangementType.s280);
			if (spawn2102)
				RandomWAs.Add(WheelArrangementType.s282);
			if (spawn2104)
				RandomWAs.Add(WheelArrangementType.s284);

			if (spawn4100)
				RandomWAs.Add(WheelArrangementType.s480);
			if (spawn4102)
				RandomWAs.Add(WheelArrangementType.s482);
			if (spawn4104)
				RandomWAs.Add(WheelArrangementType.s484);

			//TWELVE COUPLED

			if (spawn0120)
				RandomWAs.Add(WheelArrangementType.s0120);
			if (spawn2120)
				RandomWAs.Add(WheelArrangementType.s2120);
			if (spawn2122)
				RandomWAs.Add(WheelArrangementType.s2122);
			if (spawn4122)
				RandomWAs.Add(WheelArrangementType.s4122);
		}
	}
}
