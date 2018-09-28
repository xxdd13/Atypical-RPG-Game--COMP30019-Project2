using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballista : MonoBehaviour {
    public GameObject point;
    public GameObject target;
	// Use this for initialization
	void Start () {
        this.GetComponent<AudioSource>().Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
