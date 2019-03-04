using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


/*
 * tutorial used for unity vive controls:https://www.youtube.com/watch?v=U-L0COB3lys
 * tutorial used for rotations: https://www.youtube.com/watch?v=nJiFitClnKo
 * 
 */
public class wheel2 : MonoBehaviour
{
    const int CONTR_IND_DIFF = 3;
    const int LEFT_IND = 1;
    const int RIGHT_IND = 0;
    const int NONE_IND = 2;
    private Collider ourCollider = null;
    public float rotationAngle = 0.0f;
    public float rotSpeed = 1f;

    //currController
    public GameObject carRel;

    public SteamVR_TrackedObject controller_left;
    public SteamVR_TrackedObject controller_right;
    public GameObject grabPoint;
    public GameObject toPoint;

    private SteamVR_Controller.Device device, device_l, device_r;
    private Vector3 pos_l, pos_r;
    private bool[] triggerEntered = { false, false, false };
    //private bool[] proximity = { false, false, false };
    private bool[] primed = { false, false, false };

    //whatever IND this variable corresponds to is what controller is controlling the wheel
    private int controlling = NONE_IND;


    
    public float exitDistance;

    // Use this for initialization
    void Start()
    {
        device = null;
        ourCollider = GetComponentInChildren<Collider>();
        if (!ourCollider) Debug.Log("collider is null");

        // set our controller device indices
        controller_left.SetDeviceIndex(LEFT_IND);
        controller_right.SetDeviceIndex(RIGHT_IND);
        Debug.Log("Left device index: " + (int)controller_left.index);
        Debug.Log("Right device index: " + (int)controller_right.index);

    }

    /**
     * Update called once per frame
     * 
     * Three states to controller controls
     *  1. no controllers with trigger pressed in proximity 
     *      - the wheel is not being controlled;do nothing
     *      
     *  2. one controller is in proximity and trigger pressed
     *      - the wheel is being controlled by that controller
     *      
     *  3. two controllers are in proximity with trigger pressed
     *      - the wheel is being controlled by the controller that most recently entered the
     *          state of being in proximity AND having trigger pressed
     *          
     * Clarifications
     *  a) being in proximity means having initially triggered the collider and from then onwards staying
     *      withing exitDistance from the collider
     *  b) having a controller's trigger pressed means exactly what it implies as the front trigger being
     *      pressed/held down
     *  c) when a controller is both in proximity and with trigger pressed it is primed
     *  
     *  Note: controller index is changed to right:3 and left:4 afterwards but it is fine as long as we dont use LEFT_IND and RIGHT_IND in it's place
     *  Update: being in proximity is going through OnTriggerEnter without going through OnTriggerExit
     * 
     */

    //private Vector3 db_v1 = new Vector3(0, 1, 0);
    //private Vector3 db_v2 = new Vector3(1, 0, 0);
    void Update()
    {

        this.transform.forward = -carRel.transform.forward;
        //RotateFromTo(db_v1, db_v2);
        Debug.Log("grabpoint: " + grabPoint.transform.position);
        Debug.Log("topoint: " + toPoint.transform.position);
        device_l = SteamVR_Controller.Input((int)controller_left.index);
        device_r = SteamVR_Controller.Input((int)controller_right.index);


        // get the position of each controller; set to pos_l,pos_r
        pos_l = controller_left.gameObject.transform.position;
        pos_r = controller_right.gameObject.transform.position;

        
        //place pos_l,pos_r as a vector relative to transform.position on a plane with the axis of rotation
        // as the normal; they are the closest position to their previous world space position to the plane
        // assumes transform.forward lies on axis of rotation
        pos_l = pos_l - transform.position;
        pos_l = pos_l - Vector3.Project(pos_l, transform.forward);
        pos_r = pos_r - transform.position;
        pos_r = pos_r - Vector3.Project(pos_r, transform.forward);
        print("left" + pos_l);
        print("right" + pos_r);


        

        /* 
         * New way of determining which device is controlling 
         * 1. Device is activated on trigger press and being within range
         * 2. If current device gets released, set the current device to the appropriate controller
         */
        if (device_l.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && triggerEntered[LEFT_IND]) {
            device = device_l;
            primed[LEFT_IND] = true;
            controlling = LEFT_IND;
            grabPoint.transform.localPosition = pos_l;
        }
        if (device_r.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && triggerEntered[RIGHT_IND])
        {
            device = device_r;
            primed[RIGHT_IND] = true;
            controlling = RIGHT_IND;
            grabPoint.transform.localPosition = pos_r;
        }

        /*
         * If either not within the collision trigger or the trigger isn't pressed
         * 1. Device shouldn't be controlling the wheel, set primed to false
         * 2. if the other device can control the wheel, let it control the wheel. Otherwise nothing is 
         *      controlling the wheel
         *
         */
        if (!device_l.GetPress(SteamVR_Controller.ButtonMask.Trigger) || !triggerEntered[LEFT_IND]) 
        {
            primed[LEFT_IND] = false;
            if (controlling == LEFT_IND)
            {
                if (primed[RIGHT_IND])
                {
                    device = device_r;
                    controlling = RIGHT_IND;
                }
                else
                {
                    device = null;
                    controlling = NONE_IND;
                }
            }
        }
        if (!device_r.GetPress(SteamVR_Controller.ButtonMask.Trigger) || !triggerEntered[RIGHT_IND])
        {
            primed[RIGHT_IND] = false;
            if (controlling == RIGHT_IND)
            {
                if (primed[LEFT_IND])
                {
                    device = device_l;
                    controlling = LEFT_IND;
                }
                else
                {
                    device = null;
                    controlling = NONE_IND;
                }
            } 
        }

        
        // device != null
        // if there is a current controller
        if (controlling != NONE_IND)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAA device identified");
            // newPos is the relative vector of the anchor of the wheel pointing to the closest point on the collider
            Vector3 newPosRelative = (controlling == LEFT_IND ? pos_l : pos_r);

            //rotates wheel and changes grabpoint value in the direction of smallest rotation from grabPoint
            // to newPosRelative
            //sets localposition to be position with 0 rotation
            //grabPoint.transform.localPosition = grabPoint.transform.position - this.transform.position;
            RotateTo(newPosRelative);
        }
        //Debug.Log("end update loop");
   
    }

    /**
     * called when another collider enters this object(the wheel) collider
     * if other collider is of a controller, identify that it has activated a triggerEnter
     */
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger entered");
        //check if the collider entering is a steam controller
        SteamVR_TrackedObject controller;
        if (controller = other.GetComponent<SteamVR_TrackedObject>())
        {
            Debug.Log("Trigger Entered: " + (int)controller.index);
            //sets current controller to the gameobject that holds Collider other
            int ind = 2;
            if (controller.index == controller_left.index)
            {
                Debug.Log("LEFT TRIGGER ENTERED");
                ind = LEFT_IND;
            }
            else if (controller.index == controller_right.index)
            {
                Debug.Log("RIGHT TRIGGER ENTERED");
                ind = RIGHT_IND;
            }
            else Debug.Log("no matching controller index");


            triggerEntered[ind] = true;
        }
    }

    /*
     * Called when a collider exits the trigger
     * 
     */
    void OnTriggerExit(Collider other)
    {
        Debug.Log("trigger exited");
        //check if the collider entering is a steam controller
        SteamVR_TrackedObject controller;
        if (controller = other.GetComponent<SteamVR_TrackedObject>())
        {
            Debug.Log("Trigger Exited: " + (int)controller.index);
            //sets current controller to the gameobject that holds Collider other
            int ind = 2;
            if (controller.index == controller_left.index)
            {
                Debug.Log("LEFT TRIGGER EXITED");
                ind = LEFT_IND;
            }
            else if (controller.index == controller_right.index)
            {
                Debug.Log("RIGHT TRIGGER EXITED");
                ind = RIGHT_IND;
            }
            else Debug.Log("no matching controller index");


            triggerEntered[ind] = false;
        }
    }


    /**
     * rotates the wheel such that relative vector "from" would be in the same position as 
     * relative vector "to" after the rotation with a limited rotation speed
     * 
     * Assumes vectors are on a plane orthogonal to this.transform.forward
     * 
     * Note: due to the rotation speed, the wheel will only rotate in the direction of the rotation
     * from "from" to "to" and won't do the full rotation(see Quaternion.Lerp)
     */
    private void RotateTo( Vector3 to)
    {
        
        /*float rot = Vector3.SignedAngle(grabPoint.transform.position, to, this.transform.forward);
        rot = Mathf.Lerp(0.0f, rot, rotSpeed * Time.deltaTime);
        
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotationAngle);*/
        toPoint.transform.position = to + this.transform.position;
        //to =  to;
        //rotationAngle -= rot;
        //print("to" + to);
        
        Quaternion rot = Quaternion.FromToRotation(grabPoint.transform.localPosition, to);
        rot = Quaternion.Lerp(Quaternion.identity, rot, rotSpeed * Time.deltaTime);
        transform.localRotation = rot * transform.localRotation;
        // transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 1);
        //return rot;
    }
}