using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RearrangedS282.Sim.Booster
{
	internal class BoosterReciprocatingSteamEngineDefinition
		: ReciprocatingSteamEngineDefinition
	{
		BoosterReciprocatingSteamEngineDefinition()
		{
			numCylinders = 2;
			cylinderBore = 10 * 0.0254f;
			pistonStroke = 12 * 0.0254f;
			minCutoff = 0.75f;
			maxCutoff = minCutoff;
		}
	}
}
