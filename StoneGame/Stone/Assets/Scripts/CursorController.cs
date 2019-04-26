using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    public Texture2D m_CursorTexture;

    void OnGUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (m_CursorTexture != null)
        {
            Cursor.SetCursor(m_CursorTexture, Vector2.zero, CursorMode.Auto);
        }        
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
    }
}
