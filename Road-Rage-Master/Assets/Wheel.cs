using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * tutorial used for unity vive controls:https://www.youtube.com/watch?v=U-L0COB3lys
 * tutorial used for rotations: https://www.youtube.com/watch?v=nJiFitClnKo
 * 
 */
public class Wheel : MonoBehaviour {
    const int NONE_IND  = 0;
    const int LEFT_IND  = 1;
    const int RIGHT_IND = 2;
    private Collider ourCollider = null;

    public float rotSpeed = 1f;

    //currController
    public SteamVR_TrackedObject controller_left;
    public SteamVR_TrackedObject controller_right;
    private int current_input_index = NONE_IND;

    private SteamVR_Controller.Device device, device_l, device_r;
    private Vector3 pos_l, pos_r;
    private bool[] triggerEntered = { false, false, false };
    private bool[] proximity = { false, false, false };
    private bool[] primed = { false, false, false };


    private Vector3 grabPoint;

    public float exitDistance;

    // Use this for initialization
    void Start () {
        ourCollider = GetComponent<Collider>();
        if (!ourCollider) Debug.Log("collider is null");

        // set our controller device indices
        controller_left.SetDeviceIndex(LEFT_IND);
        controller_right.SetDeviceIndex(RIGHT_IND);
        Debug.Log("Left device index: " + controller_left.index);
        Debug.Log("Right device index: " + controller_right.index);

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
     */

    //private Vector3 db_v1 = new Vector3(0, 1, 0);
    //private Vector3 db_v2 = new Vector3(0, -1, 0);
    void Update () {
        //db_v1 = RotateFromTo(db_v1, db_v2) * db_v1;
        
        device_l = SteamVR_Controller.Input((int)controller_left.index);
        device_r = SteamVR_Controller.Input((int)controller_right.index);


        // get the position of each controller; set to pos_l,pos_r
        pos_l = controller_left.gameObject.transform.position;
        pos_r = controller_right.gameObject.transform.position;

        // update/check if each controller is within proximity of the wheel; set closest points to collider 
        //  into pos_l,pos_r
        // Currently assuming Collider.ClosestPoint() returns a Vector3 in world space
        proximity[LEFT_IND]  = triggerEntered[LEFT_IND]  &&
                                Vector3.Distance(pos_l = ourCollider.ClosestPoint(pos_l), pos_l) < exitDistance;
        proximity[RIGHT_IND] = triggerEntered[RIGHT_IND] && 
                               Vector3.Distance(pos_r = ourCollider.ClosestPoint(pos_r), pos_r) < exitDistance;

        // if we aren't in proximity then the trigger has exited the collider
        if (!proximity[LEFT_IND]) triggerEntered[LEFT_IND] = false;
        if (!proximity[RIGHT_IND]) triggerEntered[RIGHT_IND] = false;



        //place pos_l,pos_r as a vector relative to transform.position on a plane with the axis of rotation
        // as the normal; they are the closest position to their previous world space position to the plane
        // assumes transform.forward lies on axis of rotation
        pos_l = pos_l - transform.position;
        pos_l = pos_l - Vector3.Project(pos_l, transform.forward);
        pos_r = pos_r - transform.position;
        pos_r = pos_r - Vector3.Project(pos_r, transform.forward);



        /* 
         * ESTABLISHING WHICH CONTROLLER IS CONTROLLING THE WHEEL
         * the current controller is "device"
         * for each controller check if it is primed
         *      check if it is in proximity
         *      check if its trigger is pressed
         * if it is primed and it previously wasn't primed, it is the most recently primed controller
         *      current controller is now this controller
         * if it is primed and it previously was primed, it may or may not be the most recent primed controller
         *      current controller remains as it was before
         * if it isn't primed, it is not going to control the wheel
         *      if neither controller is primed, there is no current controller
         * 
         * 
         * Note: current implementation prioritized right controller over left controller in the rare
         *      occurrence that both controllers are primed in the same frame
         */
        // check left primed
        if (proximity[LEFT_IND] && device_l.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (!primed[LEFT_IND])
            {
                primed[LEFT_IND] = true;
                device = device_l;
                ////////////////////// is there a difference between just calling 
                ///transform.position and gameobject.transform.position??? assuming no
                //results in relative vector3
                //assumes that grabPoint lies on a plane normal to the axis of rotation
                grabPoint = pos_l;
            }
        }
        else {
            primed[LEFT_IND] = false;
        }

        // check right primed
        if (proximity[RIGHT_IND] && device_r.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (!primed[RIGHT_IND])
            {
                primed[RIGHT_IND] = true;
                device = device_r;
                //results in relative vector3
                //assumes that grabPoint lies on a plane normal to the axis of rotation
                grabPoint = pos_r;//results in relative vector3
            }
        }
        else
        {
            primed[RIGHT_IND] = false;
        }

        if (!primed[LEFT_IND] && !primed[RIGHT_IND]) {
            device = null;
        }

        // if there is a current controller
        if (device != null) {
            // newPos is the relative vector of the anchor of the wheel pointing to the closest point on the collider
            Vector3 newPos = (device.index == LEFT_IND ? pos_l : pos_r);
            
            // applies rotation to grabPoint; some reason needs to be quaternion * vector3 not vector3 * quaternion but ok
            grabPoint = RotateFromTo(grabPoint, newPos) * grabPoint;

        }
	}

    /**
     * called when another collider enters this object(the wheel) collider
     * if other collider is of a controller, identify that it has activated a triggerEnter
     */
    private void OnTriggerEnter(Collider other)
    {
        //check if the collider entering is a steam controller
        SteamVR_TrackedObject controller;
        if (controller = other.GetComponent<SteamVR_TrackedObject>()) {
            Debug.Log("Is controller: "+(int)controller.index);
            //sets current controller to the gameobject that holds Collider other
            triggerEntered[(int)controller.index] = true;
        }
    }


    /**
     * rotates the wheel such that relative vector "from" would be in the same position as 
     * relative vector "to" after the rotation with a limited rotation speed
     * 
     * Note: due to the rotation speed, the wheel will only rotate in the direction of the rotation
     * from "from" to "to" and won't do the full rotation(see Quaternion.Lerp)
     */
    private Quaternion RotateFromTo(Vector3 from, Vector3 to) {
        Quaternion rot = Quaternion.FromToRotation(from, to);
        rot = Quaternion.Lerp(Quaternion.identity, rot, rotSpeed * Time.deltaTime);
        transform.rotation = rot * transform.rotation;
        return rot;
    }
}
