using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipsMotion : MonoBehaviour {
public GameObject panel;

	// Use this for initialization
	void Start () {
		StartCoroutine (RemoveAfterSeconds (5, panel));
	}
	IEnumerator RemoveAfterSeconds(int seconds, GameObject obj){
			yield return new WaitForSeconds(5);
			obj.SetActive(false);
		}

	
	// Update is called once per frame
	void Update () {
		
	}
}
