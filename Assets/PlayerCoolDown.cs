using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoolDown : MonoBehaviour {
    public float rbCoolDown = 1.5f;
    public float nukeCoolDown = 2f;

    public bool rb = true;
    public bool nuke = true;

    public float rbCD;
    public float nukeCD;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!rb) {
            rbCD += Time.deltaTime;
            print(rbCD);
        }
        if (!nuke)
        {
            nukeCD += Time.deltaTime;
        }

        if (rbCD >= rbCoolDown) { rb = true; rbCD = 0f; }
        if (nukeCD >= nukeCoolDown) { nuke = true; nukeCD = 0f;  }

    }
    public void rbCast() {
        rb = false;
    }
    public void nukeCast()
    {
        nuke = false;
    }
}
