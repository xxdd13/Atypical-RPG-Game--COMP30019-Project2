using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCam : MonoBehaviour {
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera cam4;
    public Camera cam5;
    private float timer;
	// Use this for initialization
	void Start () {
        cam3.enabled= false;
        cam4.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {



        if (Input.GetKeyDown("y"))
        {
            
            cam2.enabled = !cam2.enabled;
        }

        if (timer < 7f)
        {
            
        }
        else {
            cam2.enabled = false;
        }

        if (timer > 7f && timer < 10f)
        {
            cam4.enabled = true;
        }
        else {
            cam4.enabled = false;
        }
        if (timer > 10f && timer < 13f)
        {
            cam5.enabled = true;
        }
        else
        {
            cam5.enabled = false;
        }
        if (timer > 13f && timer < 18f)
        {
            cam3.enabled = true;
        }
        else
        {
            cam3.enabled = false;
        }

        timer += Time.deltaTime;



    }
}
