using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCarAccel : MonoBehaviour {
    public static Quaternion Rotation;
    public double magnitude;
    public float value;
    // Use this for initialization
    void Start () {
        JoyconDemo joycon = this.GetComponent<JoyconDemo>();
    }
	
	// Update is called once per frame
	void Update () {
        Rotation = JoyconDemo.orientation;
        magnitude = (Rotation.x - 0.888)/1.776;
        value = (float)magnitude;
        print(value);
        //this.transform.position += this.transform.forward * magnitude;
        this.transform.position -= this.transform.up * Time.deltaTime * 5 * value;
        //print(Rotation.x);
        //print(magnitude);
    }
}
