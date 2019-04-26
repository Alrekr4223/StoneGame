using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public enum BoxLocation {empty, top, bottom, forward, backward, left, right}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void CameraEnteredBox(BoxLocation type)
    {
        Debug.Log("Camera entered box with type: " + type);

        switch (type)
        {
            case BoxLocation.empty:
                break;
            case BoxLocation.top:
                break;
            case BoxLocation.bottom:
                break;
            case BoxLocation.forward:
                break;
            case BoxLocation.backward:
                break;
            case BoxLocation.left:
                break;
            case BoxLocation.right:
                break;
            default:
                break;
        }
    }
}
