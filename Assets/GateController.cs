using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour {

    public bool active=false;
	// Use this for initialization
	void Start () {
        this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (this.gameObject.transform.position.y < 105) {
            Destroy(this.gameObject);
        }
        this.gameObject.transform.position -= this.gameObject.transform.up * Time.deltaTime*0.5f;

    }
}
