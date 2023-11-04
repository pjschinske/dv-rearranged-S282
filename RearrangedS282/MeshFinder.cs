using Dummiesman;
using MeshXtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Component = UnityEngine.Component;

namespace RearrangedS282
{
	internal class MeshFinder
	{

		private static MeshFinder _instance;
		public static MeshFinder Instance
		{
			get { return _instance ??= new MeshFinder(); }
		}

		private static readonly String assetStudioPath = @".\Mods\RearrangedS282\AssetStudioModCLI_net472_win32_64\AssetStudioModCLI.exe";
		private static readonly String importPath = @"DerailValley_Data\resources.assets";
		private static readonly String exportPath = @"mods\RearrangedS282\assets";
		private static readonly String importPathFull = System.IO.Path.GetFullPath(importPath);
		private static readonly String exportPathFull = System.IO.Path.GetFullPath(exportPath);
		
		public Mesh TwoAxleSideRodMesh
		{ get; private set; }
		public Mesh ThreeAxleSideRodMesh
		{ get; private set; }
		public Mesh FiveSixAxleSideRodMesh
		{ get; private set; }
		public Mesh TwoAxleDuplexSideRodMesh
		{ get; private set; }

		readonly struct Range
		{
			public readonly int start;//inclusive
			public readonly int end;//exclusive

			public Range(int start, int end)
			{
				this.start = start;
				this.end = end;
			}
			public bool contains(int x)
			{
				return x >= start && x < end;
			}

			public override string ToString()
			{
				return $"{base.ToString()}: start={start}, end={end}";
			}
		}

		readonly struct RangeFloat
		{
			public readonly float start;//inclusive
			public readonly float end;//exclusive

			public RangeFloat(float start, float end)
			{
				this.start = start;
				this.end = end;
			}
			public bool contains(float x)
			{
				return x >= start && x <= end;
			}

			public override string ToString()
			{
				return $"{base.ToString()}: start={start}, end={end}";
			}
		}

		//moving some siderod verticies to make wheel spacing even on 10-coupled and 12-coupled engines
		private static readonly Range[] verticesToMoveBackward =
		{
			new Range(102, 136),
			new Range(317, 349),
			new Range(476, 508),
		};

		//Verticies that need to be moved forward 3.25m to make the 4-coupled siderod
		private static readonly Range[] verticesToMoveForward325 =
		{
			new Range(34, 68),
			new Range(269, 270),
			new Range(349, 380),
			new Range(413, 445),
		};

		//Verticies that need to be moved forward 0.15m to make the 4-coupled siderod
		private static readonly Range[] verticesToMoveForward015 =
		{
			new Range(471, 473),
			new Range(475, 476),
			new Range(312, 314),
			new Range(316, 317),
		};

		//faces that need to be hidden to make the 4-coupled siderod
		private static readonly Range[] facesToHideFor4Coupled =
		{
			new Range(453, 488),
			new Range(384, 421),
			new Range(349, 352),
			new Range(230, 313),
			new Range(128, 146),
			//202, 204, 206, 208, 210, 212, 214, 216, 218, 220, 222, 224, 226, 228
			//147, 149, 151, 153, 155, 157, 159, 161, 163, 165
			new Range(64, 96),
			new Range(0, 32),
		};

		private static readonly Range[] facesToHideFor6Coupled =
		{
			//faces to keep
			/*new Range(32, 80),
			244, 245, 247, 249, 253, 255, 257, 258, 259,
			new Range(262, 270),
			new Range(349, 383),
			386, 388, 390, 391, 392, 394, 396, 400, 401,
			new Range(410, 453),*/

			//faces to hide
			new Range(0, 32),
			new Range(80, 130),
			new Range(217, 244),
			new Range(260, 262),
			new Range(270, 349),
			new Range(384, 386),
			new Range(402, 410),
			new Range(453, 524),
		};

		public MeshFinder()
		{
			//https://stackoverflow.com/questions/1469764/run-command-prompt-commands
			var process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.Start();
			string assetStudioCmd = $"{assetStudioPath} \"{importPathFull}\" -o \"{exportPathFull}\" -t mesh --filter-by-name \"s282_mech_wheels_connect\" --log-output file";
			process.StandardInput.WriteLine(assetStudioCmd);
			process.StandardInput.Flush();
			process.StandardInput.Close();
			process.WaitForExit();
			Main.Logger.Log(process.StandardOutput.ReadToEnd());

			getSideRodMeshes();
		}

		//mark part of mesh, to be either kept or destroyed with deleteMarkedPartOfMesh or
		//deleteUnmarkedPartOfMesh.
		private void markPartOfMesh(Vector3[] vertices, int[] triangles, RangeFloat xRange, RangeFloat yRange, RangeFloat zRange)
		{
			//Main.Logger.Log(xRange.ToString());
			//Main.Logger.Log(yRange.ToString());
			//Main.Logger.Log(zRange.ToString());

			vertices = (Vector3[])vertices.Clone();
			//if the vertex is out of range, set it to zero
			for (int i = 0; i < vertices.Length; i++)
			{
				if (xRange.contains(vertices[i].x)
					&& yRange.contains(vertices[i].y)
					&& zRange.contains(vertices[i].z))
				{
					vertices[i] = Vector3.zero;
				}
			}
			//triangles that contain out-of-range vertices get marked
			for (int i = 0; i < triangles.Length - 2; i += 3)
			{
				//Only set to negative 1 if it hasn't been already and all it's vertices have been set to zero
				if (triangles[i] != -1
					&& vertices[triangles[i]].Equals(Vector3.zero)
					&& vertices[triangles[i + 1]].Equals(Vector3.zero)
					&& vertices[triangles[i + 2]].Equals(Vector3.zero))
				{
					triangles[i] = -1;
					triangles[i + 1] = -1;
					triangles[i + 2] = -1;
				}
			}
		}

		private void deleteMarkedPartOfMesh(Mesh mesh, int[] markedTriangles)
		{
			mesh.triangles = markedTriangles.Where((source, index) => source != -1).ToArray();
		}

		private void deleteUnmarkedPartOfMesh(Mesh mesh, int[] markedTriangles)
		{
			mesh.triangles = mesh.triangles.Where((source, index) => markedTriangles[index] == -1).ToArray();
		}

		//hide the part of a mesh enclosed in three ranges
		private void hidePartOfMesh(Mesh mesh, RangeFloat xRange, RangeFloat yRange, RangeFloat zRange)
		{
			Vector3[] vertices = (Vector3[])mesh.vertices.Clone();
			int[] triangles = (int[])mesh.triangles.Clone();
			//if the vertex is out of range, set it to zero
			for (int i = 0; i < vertices.Length; i++)
			{
				if (xRange.contains(vertices[i].x)
					&& yRange.contains(vertices[i].y)
					&& zRange.contains(vertices[i].z))
				{
					vertices[i] = Vector3.zero;
				}
			}
			//triangles that contain out-of-range vertices get removed
			for (int i = 0; i < triangles.Length; i += 3)
			{
				if (vertices[triangles[i]].Equals(Vector3.zero)
					&& vertices[triangles[i + 1]].Equals(Vector3.zero)
					&& vertices[triangles[i + 2]].Equals(Vector3.zero))
				{
					triangles[i] = -1;
					triangles[i + 1] = -1;
					triangles[i + 2] = -1;
				}
			}
			triangles = triangles.Where((source, index) => source != -1).ToArray();
			mesh.triangles = triangles;
		}

		private void getSideRodMeshes()
		{
			Mesh sideRodMesh = new OBJLoader().Load(exportPathFull + "\\s282_mech_wheels_connect.obj");
			if (sideRodMesh is null)
			{
				Main.Logger.Error($"MeshFinder can't find the mesh at {exportPathFull + "\\s282_mech_wheels_connect.obj"}");
				return;
			}

			//Load the new two axle siderod mesh into the game
			TwoAxleSideRodMesh = UnityEngine.Object.Instantiate(sideRodMesh);
			Vector3[] twoAxleSideRodVerts = TwoAxleSideRodMesh.vertices;
			int[] twoAxleSideRodTris = TwoAxleSideRodMesh.triangles;
			foreach (Range range in verticesToMoveForward325)
			{
				for (int i = range.start; i < range.end; i++)
				{
					twoAxleSideRodVerts[i].z += 3.256f - 0.32f;//3.2553669f;
				}
			}
			for (int i = 152; i < 211; i += 2)
			{
				twoAxleSideRodVerts[i].z += 3.256f - 0.32f;//3.2553669f;
			}
			foreach (Range range in verticesToMoveForward015)
			{
				for (int i = range.start; i < range.end; i++)
				{
					twoAxleSideRodVerts[i].z += 0.15f - 0.32f;
				}
			}
			twoAxleSideRodVerts[173].z += 0.15f - 0.32f;
			twoAxleSideRodVerts[175].z += 0.15f - 0.32f;
			twoAxleSideRodVerts[237].z += 0.15f - 0.32f;
			twoAxleSideRodVerts[239].z += 0.15f - 0.32f;
			/*foreach (Range range in verticesToMoveBackward)
			{
				for (int i = range.start; i < range.end; i++)
				{
					twoAxleSideRodVerts[i].z -= 0.15f;
				}
			}*/
			/*for (int i = 177; i < 236; i += 2)
			{
				twoAxleSideRodVerts[i].z -= 0.15f;
			}*/

			//for the faces, we need a way to delete them without screwing up the array.
			//So we tag the triangles for deletion by setting them to -1, then delete them all at once.
			foreach (Range range in facesToHideFor4Coupled)
			{
				for (int i = range.start; i < range.end; i++)
				{
					twoAxleSideRodTris[i * 3] = -1;
					twoAxleSideRodTris[i * 3 + 1] = -1;
					twoAxleSideRodTris[i * 3 + 2] = -1;
				}
			}
			for (int i = 147; i < 166; i += 2)
			{
				twoAxleSideRodTris[i * 3] = -1;
				twoAxleSideRodTris[i * 3 + 1] = -1;
				twoAxleSideRodTris[i * 3 + 2] = -1;
			}
			for (int i = 202; i < 229; i += 2)
			{
				twoAxleSideRodTris[i * 3] = -1;
				twoAxleSideRodTris[i * 3 + 1] = -1;
				twoAxleSideRodTris[i * 3 + 2] = -1;
			}
			//remove all elements that we tagged as -1
			twoAxleSideRodTris = twoAxleSideRodTris.Where((source, index) => source != -1).ToArray();

			TwoAxleSideRodMesh.vertices = twoAxleSideRodVerts;
			TwoAxleSideRodMesh.triangles = twoAxleSideRodTris;

			//Create duplex side rod mesh. Same as the 4 axle side rod mesh, but the
			//spacing is a bit different.
			TwoAxleDuplexSideRodMesh = UnityEngine.Object.Instantiate(TwoAxleSideRodMesh);
			Vector3[] twoAxleDuplexSiderodVerts = TwoAxleDuplexSideRodMesh.vertices;
			foreach (Range range in verticesToMoveForward325)
			{
				for (int i = range.start; i < range.end; i++)
				{
					twoAxleDuplexSiderodVerts[i].z += 0.17f;
				}
			}
			for (int i = 152; i < 211; i += 2)
			{
				twoAxleDuplexSiderodVerts[i].z += 0.17f;
			}
			foreach (Range range in verticesToMoveForward015)
			{
				for (int i = range.start; i < range.end; i++)
				{
					twoAxleDuplexSiderodVerts[i].z += 0.17f;
				}
			}
			twoAxleDuplexSiderodVerts[173].z += 0.17f;
			twoAxleDuplexSiderodVerts[175].z += 0.17f;
			twoAxleDuplexSiderodVerts[237].z += 0.17f;
			twoAxleDuplexSiderodVerts[239].z += 0.17f;

			TwoAxleDuplexSideRodMesh.vertices = twoAxleDuplexSiderodVerts;

			TwoAxleSideRodMesh.RecalculateNormals();
			TwoAxleSideRodMesh.RecalculateTangents();
			TwoAxleSideRodMesh.RecalculateBounds();
			TwoAxleDuplexSideRodMesh.RecalculateNormals();
			TwoAxleDuplexSideRodMesh.RecalculateTangents();
			TwoAxleDuplexSideRodMesh.RecalculateBounds();

			//Load the new three axle siderod mesh into the game
			ThreeAxleSideRodMesh = UnityEngine.Object.Instantiate(sideRodMesh);
			Vector3[] threeAxleSideRodVerts = ThreeAxleSideRodMesh.vertices;
			int[] threeAxleSideRodTris = ThreeAxleSideRodMesh.triangles;

			//For the three axle siderod mesh, we're actually making a 1.5 axle siderod mesh and mirroring it
			//We store the faces we want to keep and get rid of the rest
			//Then when we spawn a 6-coupled engine, we'll just spawn two of the 1.5 axle siderod meshes
			for (int i = 131; i < 216; i += 2)
			{
				threeAxleSideRodTris[i * 3] = -1;
				threeAxleSideRodTris[i * 3 + 1] = -1;
				threeAxleSideRodTris[i * 3 + 2] = -1;
			}

			foreach (Range range in facesToHideFor6Coupled)
			{
				for (int i = range.start; i < range.end; i++)
				{
					threeAxleSideRodTris[i * 3] = -1;
					threeAxleSideRodTris[i * 3 + 1] = -1;
					threeAxleSideRodTris[i * 3 + 2] = -1;
				}
			}

			int[] moreFacesToHide = { 246, 248, 250, 251, 252, 254, 256, 387, 389, 393, 395, 397, 398, 399 };
			for (int i = 0; i < moreFacesToHide.Length; i++)
			{
				threeAxleSideRodTris[moreFacesToHide[i] * 3] = -1;
				threeAxleSideRodTris[moreFacesToHide[i] * 3 + 1] = -1;
				threeAxleSideRodTris[moreFacesToHide[i] * 3 + 2] = -1;
			}

			//remove all elements that equal -1
			threeAxleSideRodTris = threeAxleSideRodTris.Where((source, index) => source != -1).ToArray();

			ThreeAxleSideRodMesh.vertices = threeAxleSideRodVerts;
			ThreeAxleSideRodMesh.triangles = threeAxleSideRodTris;
			ThreeAxleSideRodMesh.RecalculateNormals();
			ThreeAxleSideRodMesh.RecalculateTangents();
			ThreeAxleSideRodMesh.RecalculateBounds();

			//Load the new five/six axle siderod mesh into the game
			FiveSixAxleSideRodMesh = UnityEngine.Object.Instantiate(sideRodMesh);
			Vector3[] fiveSixAxleSideRodVerts = FiveSixAxleSideRodMesh.vertices;

			foreach (Range range in verticesToMoveBackward)
			{
				for (int i = range.start; i < range.end; i++)
				{
					fiveSixAxleSideRodVerts[i].z -= 0.15f;
				}
			}
			for (int i = 177; i < 236; i += 2)
			{
				fiveSixAxleSideRodVerts[i].z -= 0.15f;
			}
			FiveSixAxleSideRodMesh.vertices = fiveSixAxleSideRodVerts;
			FiveSixAxleSideRodMesh.RecalculateNormals();
			FiveSixAxleSideRodMesh.RecalculateTangents();
			FiveSixAxleSideRodMesh.RecalculateBounds();
		}
	}
}
