using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColName : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter(Collision hit)
    {
        Debug.Log("Collider: " + hit.contacts[0].thisCollider.name);
    }
}
