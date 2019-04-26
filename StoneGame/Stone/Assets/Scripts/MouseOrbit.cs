using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbit : MonoBehaviour
{

    public Transform m_GameManager;
    
    public float xSpeed = 20.0f;
    public float ySpeed = 80.0f;

    public float yMinLimit = -80f;
    public float yMaxLimit = 80f;

    private float distance = 5f; //needs to be based off of game size (how many cubes), not random number.
    private float distanceMin = 4f;
    private float distanceMax = 15f;

    private new Rigidbody rigidbody;
    private Quaternion prevRot;
    private float prevDistance;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        distance = FindObjectOfType<MS_Main>().GetGameSize();
        distanceMax = distance + 5;

        this.transform.position = m_GameManager.transform.position; //Camera starts inside the game area. Camera is later translated out of game area.

        prevRot = Quaternion.Euler(transform.eulerAngles);
        prevDistance = distance;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;


        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }

        if(m_GameManager == null) //Make sure we have a game manager
        {
            Debug.LogError("Game Manager must be set");
        } else if (m_GameManager.GetComponent<MS_Main>() == null)
        {
            Debug.LogError("Game Manager must have MS_Main script");
        }
    }

    void LateUpdate()
    {
        if (Vector3.Distance(this.transform.position, m_GameManager.position) <= distance)
        {
            CameraTranslateOut(); //while the camera is inside the game area, move it backwards until it reaches the default distance. 
        }        

        Quaternion rotation = this.transform.rotation;

        if (m_GameManager && (Input.GetButton("Fire2")))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            rotation = Quaternion.Euler(y, x, 0);

        }
        
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

        if(distance != prevDistance || rotation != prevRot) //if distance or rotation has changed, then run set pos/rot
        {
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + m_GameManager.position;

            transform.rotation = rotation;
            transform.position = position;

            prevRot = rotation;
            prevDistance = distance;
        }
    }

    public static float ClampAngle(float angle, float min, float max) //restricts an angle based off of a minimum and maximum. prevents angle from going over 360 or under 0.
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private void CameraTranslateOut()
    {
        for (int i = 0; i < 300; i++)
        {
            this.transform.Translate((Vector3.forward * -1) * (Time.deltaTime * 0.02f)); //Slowly pans backwards
        }        
    }
}