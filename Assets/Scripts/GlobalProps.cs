using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalProps : MonoBehaviour
{
	private static GlobalProps _instance;
	public static GlobalProps Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("GlobalProps").AddComponent<GlobalProps>();
			}
			return _instance;
		}
	}
	public static string worldName;
	public static string pathToFolder;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
}