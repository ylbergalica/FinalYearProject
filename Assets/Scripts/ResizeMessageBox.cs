using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResizeMessageBox : MonoBehaviour
{
	public RectTransform messagePanel;
	RectTransform messageTransform;
	TMP_Text messageText;

	RectTransform rt;

	void Awake()
	{
		// Get the Text component of the message box
		messageTransform = transform.GetChild(0).GetComponent<RectTransform>();
		messageText = GetComponentInChildren<TMP_Text>();
		rt = GetComponent<RectTransform>();
	}

	public void Resize()
	{
		float preferredWidth = messagePanel.rect.width - 30;

		// If the text is wider than the preferred width
		if (messageText.preferredWidth > preferredWidth)
		{
			// Wrap the text, set width to not preferred width, and set it to be exactly preferred width
			messageText.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			messageText.GetComponent<TMP_Text>().enableWordWrapping = true;
			messageText.GetComponent<RectTransform>().sizeDelta = new Vector2(preferredWidth, messageTransform.sizeDelta.y);
			rt.sizeDelta = new Vector2(preferredWidth + 10, messageText.preferredHeight + 3);
		}
		else
		{
			rt.sizeDelta = new Vector2(messageText.preferredWidth + 20, messageText.preferredHeight + 3); // add some for the padding
		}
	}
}