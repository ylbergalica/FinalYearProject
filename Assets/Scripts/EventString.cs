using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// using text mesh pro 
using TMPro;
using UnityEngine.EventSystems;

public class EventString : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputField; // the input field
	[SerializeField] private Button button; // the button to submit

	[Serializable]
	public class UnityStringEvent : UnityEvent<string> { }

	// Create an instance of UnityStringEvent, this means that the parameter is automatically taken as a string
    public UnityStringEvent onSomeEvent;

    private void Awake()
    {
        // Initialize the event
        onSomeEvent ??= new UnityStringEvent();

		button.onClick.AddListener(TriggerEvent);
		// onSomeEvent.AddListener(PrintMessage);
    }

	void Update() {
		// if the user presses enter and the input field is active and not empty, trigger the event, otherwise, select input field
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (inputField.gameObject == EventSystem.current.currentSelectedGameObject && inputField.text != "") TriggerEvent();
			else if (!inputField.isFocused) inputField.Select();
			else Debug.Log("You cannot send an empty message.");
		}
	}

    // Method to trigger the event
    void TriggerEvent()
    {
		string input = inputField.text;
        onSomeEvent.Invoke("Me> " + input);
		inputField.text = "";
    }
}
