using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutScene : MonoBehaviour {
    public RawImage cutsceneImage;
    private float timer;
    private bool skip = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey("enter") || Input.GetKey("space") || Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            skip = true;
        }

        if (skip) {
            if (timer >= 1f) {
                cutsceneImage.enabled = false;
            }
        }
        if (timer <= 3f)
        {
            timer += Time.deltaTime;
        }
        else {
            cutsceneImage.enabled = false;
        }
	}
}
