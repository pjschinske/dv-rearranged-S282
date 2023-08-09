using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RearrangedS282
{
	public enum WheelArrangementType
	{
		a080 = 0,
		a082,
		a084,
		a280,
		a282,
		a284,
		a480,
		a482,
		a484,
		a0100,
		a0102,
		a0104,
		a2100,
		a2102,
		a2104,
		a4100,
		a4102,
		a4104,
	}

	public static class WheelArrangement
	{
		//The idea here is that the values of the enum WheelArrangementType can be
		//used as indices for the array WheelArrangementNames.
		//Ideally I would have used an enum with a string value (like in Rust), or
		//a union (like in C or C++), but C# doesn't support either.

		public static readonly string[] WheelArrangementNames =
		{
			"0-8-0 Eight–wheel switcher",
			"0-8-2",
			"0-8-4",
			"2-8-0 Consolidation",
			"2-8-2 Mikado",
			"2-8-4 Berkshire",
			"4-8-0 Twelve wheeler",
			"4-8-2 Mountain",
			"4-8-4 Northern",
			"0-10-0 Ten-wheel switcher",
			"0-10-2 Union",
			"0-10-4",
			"2-10-0 Decapod",
			"2-10-2 Santa Fe",
			"2-10-4 Texas",
			"4-10-0 Mastodon",
			"4-10-2 Southern Pacific",
			"4-10-4"
		};
	}
}
