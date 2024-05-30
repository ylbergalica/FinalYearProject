using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MainMenu : MonoBehaviour
{
	[SerializeField] GameObject worldBoxPrefab;
	[SerializeField] GameObject worldSelectorContent;

	[SerializeField] GameObject gameSelectButton;
	[SerializeField] GameObject gameSelectPanel;

	[SerializeField] GameObject chatButton;
	[SerializeField] GameObject chatPanel;

	[SerializeField] GameObject skyBox;
	[SerializeField] GameObject world;

	void OnEnable () {
		foreach (Transform child in worldSelectorContent.transform) {
			Destroy(child.gameObject);
		}
		
		// for each folder inside MyGames, create a wolrdbox from the prefab and add it to the content game object
		foreach (string folder in System.IO.Directory.GetDirectories("Assets/MyGames"))
		{
			// get only the part after the last '\' char of the folder path
			string name = folder.Substring(folder.LastIndexOf('\u005c') + 1);
		
			GameObject worldBox = Instantiate(worldBoxPrefab, worldSelectorContent.transform);
			worldBox.GetComponent<WorldBox>().SetWorldName(name);

			// there is one .png file inside the folder with a _texture suffix and save it
			string[] files = System.IO.Directory.GetFiles(folder, "*_main_texture.png");
			if (files.Length == 0)
			{
				Debug.LogError("No texture file found for world " + name);
				continue;
			}
			else {
				Texture2D texture = new Texture2D(2, 2);
				texture.LoadImage(System.IO.File.ReadAllBytes(files[0]));
				worldBox.GetComponent<WorldBox>().SetWorldImage(texture);
			}

			// give it an onclick listener
			worldBox.GetComponent<Button>().onClick.AddListener(() => {
				foreach (Transform child in worldSelectorContent.transform) {
					if (child.gameObject != worldBox) child.gameObject.GetComponent<WorldBox>().SetIsLoaded(false);
				}

				worldBox.GetComponent<WorldBox>().SetIsLoaded(true);
				LoadGame(name);
			});
		}
	}

	public void LoadGame(string gameName) {
		var mf = world.GetComponent<MeshFilter>();
		var mr = world.GetComponent<MeshRenderer>();
		var mc = world.GetComponent<MeshCollider>();

		string[] files = System.IO.Directory.GetFiles("Assets/MyGames/" + gameName, "*_material.mat");

		if (files.Length == 0) Debug.LogError("No material file found for world " + gameName);
		else {
			// parse the material file
			Material material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath(files[0], typeof(Material));
			MeshRenderer renderer = skyBox.GetComponent<MeshRenderer>();

			Material[] materials = renderer.materials;
			materials[0] = material;
			renderer.materials = materials;

			Material mat = new Material(Shader.Find("Unlit/Texture"));
			mat.mainTexture = material.GetTexture("_MainTex");
			mr.sharedMaterial = mat;
		}

		string[] meshFiles = System.IO.Directory.GetFiles("Assets/MyGames/" + gameName, gameName + ".asset");

		if (meshFiles.Length == 0) Debug.LogError("No mesh file found for world " + gameName);
		else {
			// parse the mesh file
			Mesh mesh = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath(meshFiles[0], typeof(Mesh));
			mf.sharedMesh = mesh;
			mc.sharedMesh = mesh;
		}
	}

    public void ShowGameSelectMenu()
	{
		gameSelectButton.SetActive(false);
		chatButton.SetActive(false);
		gameSelectPanel.SetActive(true);
	}

	public void ShowChatMenu()
	{
		chatButton.SetActive(false);
		gameSelectButton.SetActive(false);
		chatPanel.SetActive(true);
	}

	public void HideMenu()
	{
		chatPanel.SetActive(false);
		gameSelectPanel.SetActive(false);
	
		chatButton.SetActive(true);
		gameSelectButton.SetActive(true);
	}
}