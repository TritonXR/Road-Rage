using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class score : MonoBehaviour {
    public GameObject carScript;
    int times = 0;
    public int pointValue = 1;

	// Use this for initialization
	void Start () {
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        if(times == 0)
        {
            Dot_Truck_Controller car;
            car = collision.gameObject.GetComponent<Dot_Truck_Controller>();
            if (car != null) {
                car.points += pointValue;
                times++;
                Debug.Log("POINTS: " + carScript.GetComponent<Dot_Truck_Controller>().points);
                Debug.Log("position: " + collision.transform.position);
            }
            
        }
        
        
    }
    // Update is called once per frame
    void Update () {
		
	}
}
