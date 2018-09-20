using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlocker : MonoBehaviour {
    private float timer;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timer < 10f)
        {
            this.transform.localScale *= (1.0f + Time.deltaTime * 1.5f);
            timer += Time.deltaTime;
        }
            
    }
}
