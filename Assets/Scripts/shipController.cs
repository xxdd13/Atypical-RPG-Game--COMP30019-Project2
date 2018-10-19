using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipController : MonoBehaviour {

  
    public Transform battleGround;
    public float speed;


    // Use this for initialization
    void Start () {


	}
	
	// Update is called once per frame
	void Update () {

        transform.RotateAround(battleGround.position, Vector3.up, speed * Time.deltaTime);


     
	}


}
