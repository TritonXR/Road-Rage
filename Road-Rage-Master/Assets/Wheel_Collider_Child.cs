using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Collider_Child : MonoBehaviour {
    /*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    */
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered child");
        Wheel parentScript;
        //if (parentScript = this.transform.parent.GetComponent<Wheel>()) parentScript.OnTriggerEnter(other);
        //else Debug.Log("could not get parent script");
    }

}
