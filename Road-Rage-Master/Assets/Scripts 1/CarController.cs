﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    public class CarController : MonoBehaviour
    {

        public LinearMapping linearMapping;

        public float idealRPM = 500f;
        public float maxRPM = 1000f;

        public Transform centerOfGravity;

        public WheelCollider wheelFR;
        public WheelCollider wheelFL;
        public WheelCollider wheelRR;
        public WheelCollider wheelRL;

        public float turnRadius = 6f;
        public float torque = 25f;
        public float brakeTorque = 100f;

        public float AntiRoll = 20000.0f;

        public enum DriveMode { Front, Rear, All };
        public DriveMode driveMode = DriveMode.Rear;

        public Text speedText;

        void Start()
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfGravity.localPosition;
        }

        public float Speed()
        {
            return wheelRR.radius * Mathf.PI * wheelRR.rpm * 60f / 1000f;
        }

        public float Rpm()
        {
            return wheelRL.rpm;
        }

        void FixedUpdate()
        {

            if (speedText != null)
                speedText.text = "Speed: " + Speed().ToString("f0") + " km/h";

            //Debug.Log ("Speed: " + (wheelRR.radius * Mathf.PI * wheelRR.rpm * 60f / 1000f) + "km/h    RPM: " + wheelRL.rpm);

            float scaledTorque = Input.GetAxis("Vertical") * torque;

           // Debug.Log(Input.GetAxis("Vertical"));

            if (wheelRL.rpm < idealRPM)
                scaledTorque = Mathf.Lerp(scaledTorque / 10f, scaledTorque, wheelRL.rpm / idealRPM);
            else
                scaledTorque = Mathf.Lerp(scaledTorque, 0, (wheelRL.rpm - idealRPM) / (maxRPM - idealRPM));

            DoRollBar(wheelFR, wheelFL);
            DoRollBar(wheelRR, wheelRL);

            //wheelFR.steerAngle = Input.GetAxis("Horizontal") * turnRadius;
            //wheelFL.steerAngle = Input.GetAxis("Horizontal") * turnRadius;

            wheelFR.steerAngle = -(2 * linearMapping.value - 1) * turnRadius;
            wheelFL.steerAngle = -(2 * linearMapping.value - 1) * turnRadius;
            

            wheelFR.motorTorque = driveMode == DriveMode.Rear ? 0 : scaledTorque;
            wheelFL.motorTorque = driveMode == DriveMode.Rear ? 0 : scaledTorque;
            wheelRR.motorTorque = driveMode == DriveMode.Front ? 0 : scaledTorque;
            wheelRL.motorTorque = driveMode == DriveMode.Front ? 0 : scaledTorque;
            if (Input.GetTouch(14).phase == TouchPhase.Moved)
            {
                Debug.Log("Left Controller Trigger");
            }
            if (Input.GetTouch(15).phase == TouchPhase.Moved)
            {
                Debug.Log("Right Controller Trigger");
          
            }
            if (Input.GetButton("Touch"))
            {
                Debug.Log("Left/Right Controller Trigger");
            }
            
        

                if (Input.GetButton("Touch"))
            {
                
                wheelFR.brakeTorque = brakeTorque;
                wheelFL.brakeTorque = brakeTorque;
                wheelRR.brakeTorque = brakeTorque;
                wheelRL.brakeTorque = brakeTorque;
            }
            else
            {
                wheelFR.brakeTorque = 0;
                wheelFL.brakeTorque = 0;
                wheelRR.brakeTorque = 0;
                wheelRL.brakeTorque = 0;
            }
        }

        
        void DoRollBar(WheelCollider WheelL, WheelCollider WheelR)
        {
            WheelHit hit;
            float travelL = 1.0f;
            float travelR = 1.0f;

            bool groundedL = WheelL.GetGroundHit(out hit);
            if (groundedL)
                travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

            bool groundedR = WheelR.GetGroundHit(out hit);
            if (groundedR)
                travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

            float antiRollForce = (travelL - travelR) * AntiRoll;

            if (groundedL)
                GetComponent<Rigidbody>().AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                                             WheelL.transform.position);
            if (groundedR)
                GetComponent<Rigidbody>().AddForceAtPosition(WheelR.transform.up * antiRollForce,
                                             WheelR.transform.position);
        }

    }

}

