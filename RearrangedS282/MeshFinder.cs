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

		public Mesh S282Mesh
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
			new Range(5733, 5735),
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

		public MeshFinder()
		{
			const String assetStudioPath = @".\Mods\RearrangedS282\AssetStudioModCLI_net472_win32_64\AssetStudioModCLI.exe";
			const String importPath = @"DerailValley_Data\resources.assets";
			const String exportPath = @"mods\RearrangedS282\assets";
			String importPathFull = System.IO.Path.GetFullPath(importPath);
			String exportPathFull = System.IO.Path.GetFullPath(exportPath);
			String assetStudioCmd = $"{assetStudioPath} \"{importPathFull}\" -o \"{exportPathFull}\" -t mesh --filter-by-name \"s282_locomotive_body\" --log-output file";

			//https://stackoverflow.com/questions/1469764/run-command-prompt-commands
			var process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.Start();
			process.StandardInput.WriteLine(assetStudioCmd);
			process.StandardInput.Flush();
			process.StandardInput.Close();
			process.WaitForExit();
			Main.Logger.Log(process.StandardOutput.ReadToEnd());

			//Load the new mesh into the game
			S282Mesh = new OBJLoader().Load(exportPathFull + "\\s282_locomotive_body.obj");
			if (S282Mesh is null)
			{
				Main.Logger.Error($"MeshFinder can't find the mesh at {exportPathFull + "\\s282_locomotive_body.obj"}");
				return;
			}

			//Alter mesh so that the firebox doesn't clip through the drivers
			Vector3[] vertices = S282Mesh.vertices;
			foreach (Range range in verticesToMoveRight)
			{
				for (int i = range.start; i < range.end; i++)
				{
					vertices[i].x -= 0.2f;
				}
			}
			foreach (Range range in verticesToMoveLeft)
			{
				for (int i = range.start; i < range.end; i++)
				{
					vertices[i].x += 0.2f;
				}
			}
			/*for (int i = 0; i < vertices.Length; i++) 
			{
				float deltaX = vertices[i].x - 0.816768f;
				if (deltaX < 0.0005f && deltaX > -0.0005f)
				{
					Main.Logger.Log($"Might be vertex {i} at location {vertices[i]}.");
				}
			}*/
			S282Mesh.vertices = vertices;
			S282Mesh.RecalculateNormals();
			S282Mesh.RecalculateTangents();
			S282Mesh.RecalculateBounds();
        }
	}
}
