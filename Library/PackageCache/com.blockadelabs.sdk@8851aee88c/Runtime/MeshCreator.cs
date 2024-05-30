using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using UnityEngine.Events;
using Genesis.Editor;
using Genesis;

// namespace Genesis.Editor {
	public class MeshCreator : MonoBehaviour
	{
		public void ExtractMesh(string pathToFolder)
		{
			// Mesh mesh = CreateMesh();
			// GameObject g = new GameObject();
			// g.name = $"{gameObject.name} (Mesh)";
			// var mf = g.AddComponent<MeshFilter>();
			// var mr = g.AddComponent<MeshRenderer>();
			// mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			// mr.receiveShadows = false;
			// Material mat = new Material(Shader.Find("Unlit/Texture"));
			// mat.mainTexture = (Texture2D)gameObject.GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_MainTex");
			// mr.sharedMaterial = mat;
			// mf.sharedMesh = mesh;
			Mesh mesh = CreateMesh(pathToFolder);
			GameObject g = GameObject.Find("World");
			var mf = g.GetComponent<MeshFilter>();
			var mr = g.GetComponent<MeshRenderer>();
			var mc = g.GetComponent<MeshCollider>();
			mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			mr.receiveShadows = false;
			Material mat = new Material(Shader.Find("Unlit/Texture"));
			mat.mainTexture = (Texture2D)gameObject.GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_MainTex");
			mr.sharedMaterial = mat;
			mf.sharedMesh = mesh;
			mc.sharedMesh = mesh;
		}

		private Mesh CreateMesh(string pathToFolder)
		{
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

			if (meshFilter == null || meshRenderer == null)
			{
				return null;
			}

			Texture2D depthTex = (Texture2D)meshRenderer.sharedMaterial.GetTexture("_DepthMap");
			if (!depthTex.isReadable)
			{
				throw new InvalidOperationException($"The texture must be readable. (Check if Read/Write is set in the texture's import settings)");
			}
			if (!DepthSampler.IsFormatSupported(depthTex.graphicsFormat))
			{
				throw new NotSupportedException($"The texture format {depthTex.graphicsFormat} of this depth texture is not supported.");
			}
			IDepthSampler sampler = DepthSampler.Get(depthTex);

			float scale = meshRenderer.sharedMaterial.GetFloat("_Scale");
			float max = meshRenderer.sharedMaterial.GetFloat("_Max");
			Mesh mesh = meshFilter.sharedMesh;
			Mesh extracted;
			NativeArray<Vector3> vertices = new NativeArray<Vector3>(mesh.vertexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			NativeArray<ushort> indices = new NativeArray<ushort>((int)mesh.GetIndexCount(0), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			NativeArray<Vector2> uvs = new NativeArray<Vector2>(mesh.vertexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			using (var data = Mesh.AcquireReadOnlyMeshData(mesh))
			{
				data[0].GetVertices(vertices);
				data[0].GetIndices(indices, 0);
				data[0].GetUVs(0, uvs);
			}

			for (int i = 0; i < vertices.Length; i++)
			{
				Vector2 uv = uvs[i];
				uv.x = 1 - uv.x;
				uvs[i] = uv;

				float depth = sampler.SampleBilinear(uv);
				depth = scale / depth;
				depth = Mathf.Clamp(depth, 0, max * scale);
				vertices[i] = vertices[i] * depth;
			}

			extracted = new Mesh();
			extracted.SetVertices(vertices);
			extracted.SetIndices(indices, MeshTopology.Triangles, 0);
			extracted.SetUVs(0, uvs);
			extracted.RecalculateNormals();
			extracted.UploadMeshData(false);
			// get last bit of pathToFolder for name
			extracted.name = pathToFolder.Substring(pathToFolder.LastIndexOf('/') + 1);

			vertices.Dispose();
			indices.Dispose();
			uvs.Dispose();

			// ======================== SAVE THE EXTRACTED MESH ========================
			// save the extraced var in the foler MyGames
			
			AssetDatabase.Refresh();
			AssetDatabase.CreateAsset(extracted, pathToFolder + $"/{extracted.name}.asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			
			return extracted;
		}

		private float SampleBilinear(NativeArray<float> depthData, Vector2 uv, int width, int height)
		{
			float i = Mathf.Lerp(0f, width - 1, uv.x);
			int i0 = Mathf.FloorToInt(i);

			int i1 = i0 < width - 1 ? i0 + 1 : i0;
			float j = Mathf.Lerp(0f, height - 1, uv.y);
			int j0 = Mathf.FloorToInt(j);
			int j1 = j0 < height - 1 ? j0 + 1 : j0;

			float q11 = depthData[(i0 * width) + j0];
			float q21 = depthData[(i1 * width) + j0];
			float q12 = depthData[(i0 * width) + j1];
			float q22 = depthData[(i1 * width) + j1];

			float dx = i - i0;
			float dy = j - j0;

			float v1 = q11 + dx * (q21 - q11);
			float v2 = q12 + dx * (q22 - q12);

			return v1 + dy * (v2 - v1);
		}
	}
// }