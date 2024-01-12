using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RearrangedS282
{
	public class MeshGrabber
	{
		private static HashSet<string> s_names = new HashSet<string>()
		{
			"s282_wheels_driving_1",
			"s282_wheels_driving_1_LOD1",
			"s282_wheels_driving_2",
			"s282_wheels_driving_2_LOD1",
			"s282_wheels_driving_3",
			"s282_wheels_driving_3_LOD1",
			"s282_wheels_driving_4",
			"s282_wheels_driving_4_LOD1",
			"s282_wheels_front",
			"s282_wheels_front_LOD1",
			"s282_wheels_rear",
			"s282_wheels_rear_LOD1",
			"s060_Wheels_01",
			"s060_Wheels_01_LOD1",
			"s060_Wheels_02",
			"s060_Wheels_02_LOD1",
			"s060_Wheels_03",
			"s060_Wheels_03_LOD1",
			"dm3_wheel_01",
			"dm3_wheel_01_LOD1",
			"dm3_wheel_02",
			"dm3_wheel_02_LOD1",
			"dm3_wheel_03",
			"dm3_wheel_03_LOD1",
		};

		private static bool s_builtCache = false;
		private static Dictionary<string, Mesh> s_meshCache = new Dictionary<string, Mesh>();

		public static Dictionary<string, Mesh> MeshCache
		{
			get
			{
				if (!s_builtCache)
				{
					BuildCache();
				}

				return s_meshCache;
			}
		}

		private static void BuildCache()
		{
			// AssetStudio is gone
			var all = Resources.FindObjectsOfTypeAll<Mesh>();

			s_meshCache.Clear();
			s_meshCache = all.Where(x => s_names.Contains(x.name)).ToDictionary(k => k.name, v => v);

			s_builtCache = true;
		}
	}
}
