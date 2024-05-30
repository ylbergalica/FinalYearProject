using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

public class GameManagement : MonoBehaviour
{
	[SerializeField] GameObject mainCamera;
	[SerializeField] GameObject UI;
	[SerializeField] Transform chatBox;
	[SerializeField] GameObject player;
	[SerializeField] GameObject playerObject;
	[SerializeField] GameObject convai;
	[SerializeField] GameObject note;
	[SerializeField] GameObject playBtn;
	[SerializeField] GameObject sideMenu;
	[SerializeField] GameObject eventSystem;

	GameObject ameliaNPC;

	private Vector3 startPos;
	private Quaternion startRot;

	private bool playMode = false;

	void Start() {
		startPos = player.transform.position;

		ameliaNPC = convai.transform.Find("Convai NPC Amelia").gameObject;
	}

    public void Play() {
		playBtn.SetActive(false);
		sideMenu.SetActive(false);
		note.SetActive(true);

		mainCamera.SetActive(false);
		playerObject.SetActive(true);
		eventSystem.SetActive(false);
		convai.SetActive(true);

		playMode = true;
	}

	public void StopGame() {
		ExampleCharacterController characterController = player.GetComponent<ExampleCharacterController>();
		KinematicCharacterMotor motor = player.GetComponent<KinematicCharacterMotor>();
		characterController.enabled = false;
		motor.SetPosition(startPos);
		characterController.enabled = true;

		playerObject.SetActive(false);
		mainCamera.SetActive(true);

		if (ameliaNPC.GetComponent<CharacterController>() != null) {
			CharacterController ameliaCharacterController = ameliaNPC.GetComponent<CharacterController>();
			ameliaCharacterController.enabled = false;
			ameliaNPC.transform.position = new Vector3(998f, 5f, 2.5f);
			ameliaCharacterController.enabled = true;
		}
		else ameliaNPC.transform.position = new Vector3(-2.9f, 5f, 2.5f);

		convai.SetActive(false);

		playMode = false;
		note.SetActive(false);
		playBtn.SetActive(true);
		sideMenu.SetActive(true);

		eventSystem.SetActive(true);

		Cursor.lockState = CursorLockMode.None;

		for (int i = 1; i < chatBox.childCount; i++) {
			Destroy(chatBox.GetChild(i).gameObject);
		}
	}

	void Update() {
		// press g to quit
		if (playMode && Input.GetKeyDown(KeyCode.G)) {
			StopGame();
		}
	}
}
