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
    private bool over = false;

    public Text speedText;
    public Text scoreText;
    public Text timeText;
    public Text gameOver;
    public Image bar;
    public Dot_Truck_Controller truck_controller;
    public float speed = 0;
    public float timeLeft = 30;
   

    void Start() {
        gameOver.text = "";
    }

	// Update is called once per frame
	void Update () {
        speed++;
        ChangeSpeed(speed);
        changeTime();
	}

    void changeTime() {
        timeLeft -= Time.deltaTime;
        timeText.text = "Time Left: " + timeLeft;
        if (timeLeft < 0 && over != true) {
            GameOver(truck_controller.points); 
        }
    }

    void GameOver(int score) {
        gameOver.text = "Times Up!\n Score: " + score;
        Debug.Log("Game Over!");
        over = true;
    }

    void ChangeSpeed(float speed) {
        motorspeed = truck_controller.carVelocity.magnitude;
        this.speed = motorspeed;
        float fill_amount = MIN_FILL_AMOUNT + (speed / TOTAL_SPEED) * TOTAL_FILL_AMOUNT;
        bar.fillAmount = fill_amount;

        if( this.speed <= 2)
        {
            speedText.text = "Speed: " + 0;
        }
        else
        {
            speedText.text = "Speed: " + (int)speed + "";
        }
        //speedText.text = (int)speed+"";
        scoreText.text = "Score: " + truck_controller.points;
    }
    /*float LimitedSpeed(float speed) {
        if (speed > MAX_SPEED) return MAX_SPEED;
        if (speed < MIN_SPEED) return MIN_SPEED;
        return speed;
    }*/
}
