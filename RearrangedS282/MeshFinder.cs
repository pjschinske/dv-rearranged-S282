using Dummiesman;
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
		
		public Mesh S282Mesh
		{ get; private set; }
		public Mesh BrakeCaliperMesh
		{ get; private set; }
		public Mesh BrakeShoeMesh
		{ get; private set; }
		public Mesh BrakeCaliperSandMesh
		{ get; private set; }
		public Mesh TwoAxleSideRodMesh
		{ get; private set; }
		public Mesh ThreeAxleSideRodMesh
		{ get; private set; }
		public Mesh FiveSixAxleSideRodMesh
		{ get; private set; }

		//public GameObject S282Obj
		//{ get; private set; }

		struct Range
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
		}

		struct RangeFloat
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
		}

		private static readonly Range[] verticesToMoveRight =
		{
			//Left side:
			//side
			new Range(1861, 1881),
			//top chamfer
			new Range(1935, 1955),
			//bottom chamfer
			new Range(1957, 1977),
			//bottom
			new Range(1834, 1843),
			new Range(1844, 1845),
			//top
			new Range(2274, 2281),
			//small triangles on top
			new Range(2282, 2285),
		};

		private static readonly Range[] verticesToMoveLeft =
		{
			//Right side:
			//side
			new Range(1915, 1935),
			//top chamfer
			//new Range(5733, 5735) // these points are already being shifted, don't need to shift twice
			new Range(5739, 5759),
			//bottom chamfer
			new Range(1892, 1893),
			new Range(1894, 1913),
			//bottom
			new Range(1845, 1848),
			new Range(1849, 1854),
			new Range(1858, 1859),
			//top
			new Range(5728, 5735),
			//small triangles on top
			new Range(5736, 5739),
		};

		private static readonly int[] trisToHideOnBody =
		{
			11463,
			12106, 12107, 12108, 12109,
			12110, 12111, 
			12113, 12114, 12115, 12116, 12117, 12118, 12119,
			12120, 12121, 12122, 12123, 12124, 12125, 12126, 12127, 12128, 12129,
			12130, 12131, 12132, 12133, 12134, 12135, 12136, 12137, 12138, 12139,
			12140, 12141, 12142, 12143, 12144, 12145, 12146, 12147, 12148, 12149,
			12150, 12151, 12152, 12153, 12154, 12155,
			//right side #2 brake caliper
			31637,
			31353, 31354, 31355, 31356, 31357, 31358, 31359, 31360, 31361, 31362, 31363, 31364,
			31719, 31720, 31721, 31722, 31723, 31724, 31725, 31726, 31727, 31728,
			31729, 31730, 31731, 31732,
			44266, 44267,
			44270, 44271, 44272, 44273,
			//left side #2 brake caliper
			33593, 33594, 33595, 33596, 33597, 33598, 33599, 33600,
			33959, 33960, 33961, 33962, 33963, 33964, 33965, 33966, 33967, 33968,
			45528, 45529,
			45538, 45539,  45540, 45541,
		};

		//brake caliper limits:
		private static readonly RangeFloat brakeCaliperLimitX = new RangeFloat(-.81f, -.31f);
		private static readonly RangeFloat brakeCaliperLimitY = new RangeFloat(0, .66f);
		private static readonly RangeFloat brakeCaliperLimitZ = new RangeFloat(.31f, .74f);
		private static readonly RangeFloat brakeCaliperSandLimitX = new RangeFloat(-.81f, -.31f);
		private static readonly RangeFloat brakeCaliperSandLimitY = new RangeFloat(0, .66f);
		private static readonly RangeFloat brakeCaliperSandLimitZ = new RangeFloat(3.56f, 4);

		//other brake caliper limits, this is just to know what to hide
		private static readonly RangeFloat brakeCaliperLimitXLeft = new RangeFloat(.31f, .81f);
		private static readonly RangeFloat brakeCaliperLimitZ4 = new RangeFloat(-1.24f, -.81f);
		private static readonly RangeFloat brakeCaliperSandLimitZ2 = new RangeFloat(1.87f, 2.22f);
		private static readonly RangeFloat brakeCaliperSandLimitY2a = new RangeFloat(0.26f, .66f);
		private static readonly RangeFloat brakeCaliperSandLimitZ2a = new RangeFloat(1.87f, 2.3f);


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
			String assetStudioCmd = $"{assetStudioPath} \"{importPathFull}\" -o \"{exportPathFull}\" -t mesh --filter-by-name \"s282_locomotive_body\" --log-output file";
			process.StandardInput.WriteLine(assetStudioCmd);
			process.StandardInput.Flush();
			assetStudioCmd = $"{assetStudioPath} \"{importPathFull}\" -o \"{exportPathFull}\" -t mesh --filter-by-name \"s282_mech_wheels_connect\" --log-output file";
			process.StandardInput.WriteLine(assetStudioCmd);
			process.StandardInput.Flush();
			assetStudioCmd = $"{assetStudioPath} \"{importPathFull}\" -o \"{exportPathFull}\" -t mesh --filter-by-name \"s282_brake_shoes\" --log-output file";
			process.StandardInput.WriteLine(assetStudioCmd);
			process.StandardInput.Flush();
			process.StandardInput.Close();
			process.WaitForExit();
			Main.Logger.Log(process.StandardOutput.ReadToEnd());

			getBodyMesh();
			getCaliperMesh();
			getCaliperSandMesh();
			getBrakeShoeMesh();
			getSideRodMeshes();
		}

		//hide the part of a mesh enclosed in three ranges
		private void hidePartOfMesh(Mesh mesh, RangeFloat xRange, RangeFloat yRange, RangeFloat zRange)
		{
			Vector3[] vertices = mesh.vertices;
			int[] triangles = mesh.triangles;
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
			vertices = vertices.Where((source, index) => source != Vector3.zero).ToArray();
			mesh.vertices = vertices;
			triangles = triangles.Where((source, index) => source != -1).ToArray();
			mesh.triangles = triangles;
		}

		private void getBodyMesh()
		{
			//Load the new S282 mesh into the game
			S282Mesh = new OBJLoader().Load(exportPathFull + "\\s282_locomotive_body.obj");
			if (S282Mesh is null)
			{
				Main.Logger.Error($"MeshFinder can't find the mesh at {exportPathFull + "\\s282_locomotive_body.obj"}");
				return;
			}

			//Alter mesh so that the firebox doesn't clip through the drivers
			Vector3[] s282Vertices = S282Mesh.vertices;
			int[] s282Tris = S282Mesh.triangles;
			foreach (Range range in verticesToMoveRight)
			{
				for (int i = range.start; i < range.end; i++)
				{
					s282Vertices[i].x -= 0.2f;
				}
			}
			foreach (Range range in verticesToMoveLeft)
			{
				for (int i = range.start; i < range.end; i++)
				{
					s282Vertices[i].x += 0.2f;
				}
			}
			foreach (int tri in trisToHideOnBody)
			{
				s282Tris[tri * 3] = -1;
				s282Tris[tri * 3 + 1] = -1;
				s282Tris[tri * 3 + 2] = -1;
			}
			s282Tris = s282Tris.Where((source, index) => source != -1).ToArray();
			S282Mesh.vertices = s282Vertices;
			S282Mesh.triangles = s282Tris;

			hidePartOfMesh(S282Mesh, brakeCaliperLimitX, brakeCaliperLimitY, brakeCaliperLimitZ);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitX, brakeCaliperLimitY, brakeCaliperLimitZ4);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitX, brakeCaliperLimitY, brakeCaliperSandLimitZ);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitX, brakeCaliperLimitY, brakeCaliperSandLimitZ2);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitX, brakeCaliperSandLimitY2a, brakeCaliperSandLimitZ2a);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitXLeft, brakeCaliperLimitY, brakeCaliperLimitZ);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitXLeft, brakeCaliperLimitY, brakeCaliperLimitZ4);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitXLeft, brakeCaliperLimitY, brakeCaliperSandLimitZ);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitXLeft, brakeCaliperLimitY, brakeCaliperSandLimitZ2);
			hidePartOfMesh(S282Mesh, brakeCaliperLimitXLeft, brakeCaliperSandLimitY2a, brakeCaliperSandLimitZ2a);

			S282Mesh.RecalculateNormals();
			S282Mesh.RecalculateTangents();
			S282Mesh.RecalculateBounds();
		}

		private void getCaliperMesh()
		{
			BrakeCaliperMesh = new OBJLoader().Load(exportPathFull + "\\s282_locomotive_body.obj");
			Vector3[] caliperVertices = BrakeCaliperMesh.vertices;
			int[] caliperTriangles = BrakeCaliperMesh.triangles;
			//if the vertex is out of range, set it to zero
			for (int i = 0; i < caliperVertices.Length; i++)
			{
				if (!brakeCaliperLimitX.contains(caliperVertices[i].x)
					|| !brakeCaliperLimitY.contains(caliperVertices[i].y)
					|| !brakeCaliperLimitZ.contains(caliperVertices[i].z))
				{
					caliperVertices[i] = Vector3.zero;
				}
			}
			//triangles that contain out-of-range vertices get removed
			for (int i = 0; i < caliperTriangles.Length; i += 3)
			{
				if (caliperVertices[caliperTriangles[i]].Equals(Vector3.zero)
					|| caliperVertices[caliperTriangles[i + 1]].Equals(Vector3.zero)
					|| caliperVertices[caliperTriangles[i + 2]].Equals(Vector3.zero))
				{
					caliperTriangles[i] = -1;
					caliperTriangles[i + 1] = -1;
					caliperTriangles[i + 2] = -1;
				}
			}
			caliperVertices = caliperVertices.Where((source, index) => source != Vector3.zero).ToArray();
			BrakeCaliperMesh.vertices = caliperVertices;
			caliperTriangles = caliperTriangles.Where((source, index) => source != -1).ToArray();
			BrakeCaliperMesh.triangles = caliperTriangles;
			BrakeCaliperMesh.RecalculateNormals();
			BrakeCaliperMesh.RecalculateTangents();
			BrakeCaliperMesh.RecalculateBounds();
		}

		private void getCaliperSandMesh()
		{
			BrakeCaliperSandMesh = new OBJLoader().Load(exportPathFull + "\\s282_locomotive_body.obj");
			Vector3[] caliperVertices = BrakeCaliperSandMesh.vertices;
			int[] caliperTriangles = BrakeCaliperSandMesh.triangles;
			//if the vertex is out of range, set it to zero
			for (int i = 0; i < caliperVertices.Length; i++)
			{
				if (!brakeCaliperLimitX.contains(caliperVertices[i].x)
					|| !brakeCaliperLimitY.contains(caliperVertices[i].y)
					|| !brakeCaliperSandLimitZ.contains(caliperVertices[i].z))
				{
					caliperVertices[i] = Vector3.zero;
				}
			}
			//triangles that contain out-of-range vertices get removed
			for (int i = 0; i < caliperTriangles.Length; i += 3)
			{
				if (caliperVertices[caliperTriangles[i]].Equals(Vector3.zero)
					|| caliperVertices[caliperTriangles[i + 1]].Equals(Vector3.zero)
					|| caliperVertices[caliperTriangles[i + 2]].Equals(Vector3.zero))
				{
					caliperTriangles[i] = -1;
					caliperTriangles[i + 1] = -1;
					caliperTriangles[i + 2] = -1;
				}
			}
			caliperVertices = caliperVertices.Where((source, index) => source != Vector3.zero).ToArray();
			BrakeCaliperSandMesh.vertices = caliperVertices;
			caliperTriangles = caliperTriangles.Where((source, index) => source != -1).ToArray();
			BrakeCaliperSandMesh.triangles = caliperTriangles;
			BrakeCaliperSandMesh.RecalculateNormals();
			BrakeCaliperSandMesh.RecalculateTangents();
			BrakeCaliperSandMesh.RecalculateBounds();
		}

		private void getBrakeShoeMesh()
		{
			BrakeShoeMesh = new OBJLoader().Load(exportPathFull + "\\s282_brake_shoes.obj");
			Vector3[] caliperVertices = BrakeShoeMesh.vertices;
			int[] caliperTriangles = BrakeShoeMesh.triangles;
			//if the vertex is out of range, set it to zero
			for (int i = 0; i < caliperVertices.Length; i++)
			{
				if (!brakeCaliperLimitX.contains(caliperVertices[i].x)
					|| !brakeCaliperLimitY.contains(caliperVertices[i].y)
					|| !brakeCaliperLimitZ.contains(caliperVertices[i].z))
				{
					caliperVertices[i] = Vector3.zero;
				}
			}
			//triangles that contain out-of-range vertices get removed
			for (int i = 0; i < caliperTriangles.Length; i += 3)
			{
				if (caliperVertices[caliperTriangles[i]].Equals(Vector3.zero)
					|| caliperVertices[caliperTriangles[i + 1]].Equals(Vector3.zero)
					|| caliperVertices[caliperTriangles[i + 2]].Equals(Vector3.zero))
				{
					caliperTriangles[i] = -1;
					caliperTriangles[i + 1] = -1;
					caliperTriangles[i + 2] = -1;
				}
			}
			caliperVertices = caliperVertices.Where((source, index) => source != Vector3.zero).ToArray();
			BrakeShoeMesh.vertices = caliperVertices;
			caliperTriangles = caliperTriangles.Where((source, index) => source != -1).ToArray();
			BrakeShoeMesh.triangles = caliperTriangles;
			BrakeShoeMesh.RecalculateNormals();
			BrakeShoeMesh.RecalculateTangents();
			BrakeShoeMesh.RecalculateBounds();
		}

		private void getSideRodMeshes()
		{
			Mesh sideRodMesh = new OBJLoader().Load(exportPathFull + "\\s282_mech_wheels_connect.obj");

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
			TwoAxleSideRodMesh.RecalculateNormals();
			TwoAxleSideRodMesh.RecalculateTangents();
			TwoAxleSideRodMesh.RecalculateBounds();

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
			if (S282Mesh is null)
			{
				Main.Logger.Error($"MeshFinder can't find the mesh at {exportPathFull + "\\s282_mech_wheels_connect.obj"}");
				return;
			}

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
