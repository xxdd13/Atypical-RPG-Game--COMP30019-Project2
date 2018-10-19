using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {

    public GameObject explodeObj;
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        Vector3 pos = collision.contacts[0].point;
        pos += transform.forward * offsetX;
        pos += transform.right * offsetZ;
        pos += transform.up * offsetY;
        GameObject projectile = Instantiate(explodeObj, pos, this.transform.localRotation) as GameObject;
        
    }
}
