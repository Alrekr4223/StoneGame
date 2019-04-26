using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBox : MonoBehaviour {
    
    public AudioController.BoxLocation m_Location = AudioController.BoxLocation.empty;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MainCamera")
        {
            if (m_Location == AudioController.BoxLocation.empty)
            {
                //error, please enter a location
                Debug.Log("Please Enter a location for: " + this.gameObject.name);
            }
            else
            {
                this.transform.parent.GetComponent<AudioController>().CameraEnteredBox(m_Location);
            }
        }
    }
}
