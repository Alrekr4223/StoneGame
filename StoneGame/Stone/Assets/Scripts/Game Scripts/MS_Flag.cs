using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Flag : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MS_Bomb>())
        {

            GameObject bomb = collision.gameObject.GetComponent<MS_Bomb>().gameObject;
            bomb.SetActive(false);

            GameObject.FindObjectOfType<MS_Main>().FlagFoundBomb();

            this.gameObject.transform.parent = bomb.transform.parent;
            this.gameObject.transform.localPosition = bomb.transform.localPosition;
            Destroy(this.gameObject.GetComponent<Rigidbody>());

        }
        print("Flag is colliding with: " + collision.gameObject.name);
    }
}
