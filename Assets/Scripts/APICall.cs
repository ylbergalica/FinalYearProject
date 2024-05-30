using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class APICall : MonoBehaviour
{
	public class Response {
		public string fact;
		public int length;
	}

    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(GetRequest("https://catfact.ninja/fact"));
    }

	IEnumerator GetRequest(string uri) {
		using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			switch (webRequest.result) {
				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.DataProcessingError:
					Debug.Log(String.Format("Something went wrong: {0}", webRequest.error));
					break;
				case UnityWebRequest.Result.Success:
					Response data = JsonUtility.FromJson<Response>(webRequest.downloadHandler.text);
					Debug.Log(String.Format("Response: {0}", data.fact));
					break;
			}
		}
	}
}
