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
		//s240 = 0,
		s440 = 0,
		s442,
		s444,
		s460,
		s462,
		/*s464,*/
		s080,
		s082,
		s084,
		s280,
		s282,
		s282Big,
		s284,
		s480,
		s482,
		s484,
		s0100,
		s0102,
		s0104,
		s2100,
		s2102,
		s2104,
		s4100,
		s4102,
		s4104,
		s0120,
		s2120,
		s2122,
		s4122
	}

	public static class WheelArrangement
	{
		//The idea here is that the values of the enum WheelArrangementType can be
		//used as indices for the array WheelArrangementNames.
		//Ideally I would have used an enum with a string value (like in Rust), or
		//a union (like in C or C++), but C# doesn't support either.

		public static readonly string[] WheelArrangementNames =
		{
			//"2-4-0 Porter",
			"4-4-0 American",
			"4-4-2 Atlantic",
			"4-4-4 Reading",
			"4-6-0 Ten-wheeler",
			"4-6-2 Pacific",
			/*"4-6-4 Hudson",*/
			"0-8-0 Eight–wheel switcher",
			"0-8-2",
			"0-8-4",
			"2-8-0 Consolidation",
			"2-8-2 Mikado",
			"2-8-2 High-speed Mikado",
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
			"4-10-4",
			"0-12-0 Twelve-wheel switcher",
			"2-12-0",
			"2-12-2",
			"4-12-2 Union Pacific"
		};
	}
}
