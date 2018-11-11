using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Speedometer : MonoBehaviour {
    private const float MIN_FILL_AMOUNT = 0.09802f;
    private const float MAX_FILL_AMOUNT = 0.90198f;
    private const float TOTAL_FILL_AMOUNT = MAX_FILL_AMOUNT - MIN_FILL_AMOUNT;
    private const float MIN_SPEED = 0.0f;
    private const float MAX_SPEED = 100.0f;
    private const float TOTAL_SPEED = MAX_SPEED - MIN_SPEED;
    private float motorspeed;

    public Text speedText;
    public Image bar;
    public Dot_Truck_Controller truck_controller;
    public float speed = 0;
	
	// Update is called once per frame
	void Update () {
        speed++;
        ChangeSpeed(speed);
	}
    
    void ChangeSpeed(float speed) {
        motorspeed = Mathf.Abs(truck_controller.motor1 /15);
        this.speed = motorspeed;
        float fill_amount = MIN_FILL_AMOUNT + (speed / TOTAL_SPEED) * TOTAL_FILL_AMOUNT;
        bar.fillAmount = fill_amount;
        speedText.text = (int)speed+"";
    }
    /*float LimitedSpeed(float speed) {
        if (speed > MAX_SPEED) return MAX_SPEED;
        if (speed < MIN_SPEED) return MIN_SPEED;
        return speed;
    }*/
}
