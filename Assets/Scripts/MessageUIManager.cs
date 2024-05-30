using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageUIManager : MonoBehaviour
{
	public GameObject messagePrefab;
	public RectTransform messagePanel;

    public void AddMessage(string message) {
		GameObject newMessage = Instantiate(messagePrefab, transform);

		newMessage.GetComponentInChildren<TMPro.TMP_Text>().text = message;

		newMessage.GetComponent<ResizeMessageBox>().messagePanel = messagePanel;
		newMessage.GetComponent<ResizeMessageBox>().Resize();
	}
}
