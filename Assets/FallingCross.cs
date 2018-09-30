using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCross : MonoBehaviour {
    private Rigidbody rb;
    private bool hit = false;
    public float duration = 3f;
    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(-transform.up * 3000f);
    }
	
	// Update is called once per frame
	void Update () {
       
        if (!hit)
        {
            rb.AddForce(-transform.up * 90.8f * rb.mass);
        }
        else
        {
            if (duration < 0f)
            {
                Destroy(this.gameObject);
            }
            else
            {
                duration -= Time.deltaTime;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        hit = true;
        rb.angularVelocity = new Vector3(0f, 0f, 0f);
        rb.isKinematic = true;
        rb.velocity = new Vector3(0f, 0f, 0f);
    }
}
