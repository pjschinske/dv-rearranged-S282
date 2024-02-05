using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RearrangedS282.Sim.Booster
{
	internal class BoosterSteamExhaustDefinition
		: SteamExhaustDefinition
	{
		BoosterSteamExhaustDefinition()
		{
			passiveExhaust = 0.6f;
			entrainmentRatio = 1.65f;
			maxBlowerFlow = 0f;
			pressureForMaxBlowerFlow = 1f;
			maxWhistleFlow = 0f;
		}
	}
}
