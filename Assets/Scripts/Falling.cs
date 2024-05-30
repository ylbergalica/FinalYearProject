using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MonoBehaviour
{
    // make the npc fall down when not colliding with ground
	void Update()
	{
		if (!GetComponent<CharacterController>().isGrounded && gameObject.activeSelf)
		{
			GetComponent<CharacterController>().Move(Vector3.down * Time.deltaTime * 9.8f);
		}
	}
}
