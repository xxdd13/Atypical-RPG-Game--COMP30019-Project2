using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
    Camera cam;

	// Use this for initialization
	void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(cam.transform.position, Vector3.up);	
	}
}
