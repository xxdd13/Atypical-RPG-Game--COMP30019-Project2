using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroudTypeSpell : MonoBehaviour {

    public int layerSelf =12;
    public int layerOther =13;
	// Use this for initialization
	void Start () {
        Physics.IgnoreLayerCollision(layerSelf, layerOther);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
