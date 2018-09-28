using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraGravity : MonoBehaviour {
    public float extraGravity;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * extraGravity * rb.mass);
    }
}
