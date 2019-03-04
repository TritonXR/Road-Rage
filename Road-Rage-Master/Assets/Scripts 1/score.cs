using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class score : MonoBehaviour {
    public GameObject carScript;
    int times = 0;
	// Use this for initialization
	void Start () {
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        if(times == 0)
        {
            carScript.GetComponent<Dot_Truck_Controller>().points++;
            times++;
        }
        Debug.Log("POINTS: " + carScript.GetComponent<Dot_Truck_Controller>().points);
        
    }
    // Update is called once per frame
    void Update () {
		
	}
}
