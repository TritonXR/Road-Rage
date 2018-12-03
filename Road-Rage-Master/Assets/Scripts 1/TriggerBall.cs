using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBall : MonoBehaviour
{
    public Transform otherobj;
    private GameObject[] buildings;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("oncollision");
        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("collision is car");
            //Vector3 vec = new Vector3(100F, 100F, 100F);
            //otherobj.localScale = 1.01F*(otherobj.localScale);
            buildings = GameObject.FindGameObjectsWithTag("Buildings");
            foreach (GameObject b in buildings)
                b.transform.localScale = 1.5F * (b.transform.localScale);
        }
    }
}