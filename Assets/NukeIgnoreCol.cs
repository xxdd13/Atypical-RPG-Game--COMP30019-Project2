using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeIgnoreCol : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Physics.IgnoreLayerCollision(13, 12);
        Physics.IgnoreLayerCollision(10, 12);
        Physics.IgnoreLayerCollision(11, 12);
        Physics.IgnoreLayerCollision(12, 12);
    }
	
	// Update is called once per frame
	void Update () {
		
    }
}
