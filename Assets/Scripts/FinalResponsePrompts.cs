using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using BlockadeLabsSDK;

public class FinalResponsePrompts : MonoBehaviour
{
	// Blockade Labs Skybox
	[SerializeField] private BlockadeLabsSkyboxGenerator generator;
	private IReadOnlyList<SkyboxStyleFamily> styleFamilies;

	public void HandleResponse(string response)
	{
		Debug.Log(response);

		int startIndex = response.IndexOf("{GameName}:") + "{GameName}:s".Length;
		int endIndex = response.IndexOf('{', startIndex) != -1 ? response.IndexOf('{', startIndex) : response.Length - 1;
		string name = response[startIndex..endIndex]; // creates a range from startIndex to endIndex
		name = name.Replace("\n", "");
		name = name.Replace(" ", "");

		GlobalProps.worldName = name;

		generator.ApiKey = "s5uqA5b0D1PajdcNQfQCdRtXo9w0ho4xeA6ixiCaeIFBISJOTV6s8pfcXwqJ";
		styleFamilies = generator.StyleFamilies;
		generator.SelectedStyle = styleFamilies[2].items[0];

		// get string after {Environment} in response until another '{' character
		startIndex = response.IndexOf("{Environment}:") + "{Environment}:s".Length;
		endIndex = response.IndexOf('{', startIndex) != -1 ? response.IndexOf('{', startIndex) : response.Length - 1;
		string environmentPrompt = response[startIndex..endIndex]; // creates a range from startIndex to endIndex
		environmentPrompt = environmentPrompt.Replace("\n", "");
		environmentPrompt = environmentPrompt.Replace(" ", "");

		generator.Prompt = environmentPrompt;

		if (generator.CurrentState == BlockadeLabsSkyboxGenerator.State.Generating)
		{
			generator.Cancel();
			return;
		}

		if (environmentPrompt.Length == 0)
		{
			// _promptCharacterWarning.SetActive(true);
			// _promptCharacterWarning.GetComponentInChildren<TMP_Text>().text = "Prompt cannot be empty";
			Debug.Log("Prompt cannot be empty");
			return;
		}

		if (environmentPrompt.Length > generator.SelectedStyle.maxChar)
		{
			// _promptCharacterWarning.SetActive(true);
			// _promptCharacterWarning.GetComponentInChildren<TMP_Text>().text = $"Prompt should be {generator.SelectedStyle.maxChar} characters or less";
			Debug.Log($"Prompt should be {generator.SelectedStyle.maxChar} characters or less");
			return;
		}

		if (generator.NegativeText.Length > generator.SelectedStyle.negativeTextMaxChar)
		{
			// _negativeTextCharacterWarning.SetActive(true);
			// _negativeTextCharacterWarning.GetComponentInChildren<TMP_Text>().text = $"Negative text should be {generator.SelectedStyle.negativeTextMaxChar} characters or less";
			Debug.Log($"Negative text should be {generator.SelectedStyle.negativeTextMaxChar} characters or less");
			return;
		}

		// make a folder in Assets/MyGames folder with the name of the world
		GlobalProps.pathToFolder = "Assets/MyGames/" + GlobalProps.worldName;
		Debug.Log("PATHING: " + GlobalProps.pathToFolder);
		Debug.Log("NAME: " + GlobalProps.worldName);
		// if (!System.IO.Directory.Exists(GlobalProps.pathToFolder))
		// {
		// 	System.IO.Directory.CreateDirectory(GlobalProps.pathToFolder);
		// }

		generator.GenerateSkyboxAsync(GlobalProps.worldName, GlobalProps.pathToFolder);
	}


}
