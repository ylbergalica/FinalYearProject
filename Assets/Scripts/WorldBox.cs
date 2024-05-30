using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldBox : MonoBehaviour
{
	[SerializeField] TMP_Text worldNameText;
	[SerializeField] GameObject worldImage;

	// private bool isLoaded;

    public void SetWorldName(string worldName)
	{
		// this.gameObject.name = worldName;
		worldNameText.text = worldName;
	}

	public void SetWorldImage(Texture2D image)
	{
		// set the image of the world box
		worldImage.GetComponent<RawImage>().texture = image;
	}

	public void SetIsLoaded(bool loaded)
	{
		GetComponent<Button>().interactable = !loaded;
	}
}
