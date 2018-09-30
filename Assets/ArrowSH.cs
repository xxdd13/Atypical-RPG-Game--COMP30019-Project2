using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSH : MonoBehaviour {

    Vector3 dir;
    Rigidbody rb;
    private bool hit=false;
    private float timer = 1f;
	// Use this for initialization
	void Start () {
        dir = transform.forward;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward* 1000f);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("n"))
        {
            //this.transform.position = this.transform.position+transform.forward;


        }
        if (!hit)
        {
            transform.position = this.transform.position;
            transform.LookAt(transform.position + rb.velocity);
        }
        else {
            if (timer < 0f)
            {
                Destroy(this.gameObject);
            }else{
                timer -= Time.deltaTime;
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
