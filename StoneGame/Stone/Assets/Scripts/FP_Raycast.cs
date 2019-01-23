using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP_Raycast : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

    }
    
    public static GameObject Update()
    {

        RaycastHit rHit;
        Vector3 direction = Camera.main.transform.forward;
        Vector3 origin = Camera.main.transform.position;

        
        if (Physics.Raycast(origin, direction, out rHit))
        {
            //print("Found GameObject: " + rHit.collider.gameObject.tag);
            return rHit.collider.gameObject;
                
        }
        else
        {
            return null;
        }
    }
}
