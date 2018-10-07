using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour {
    public float delay=1.0f;
    public float dur = 1.0f;
    private float delayTimer = 0.0f;
    private float timer = 0f;
    public bool attack = false;

	// Use this for initialization
	void Start () {
        this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (attack)
        {
            if (delayTimer < delay)
            {
                delayTimer += Time.deltaTime;
            }
            else
            {//delay finish
                

                if (timer < dur)
                {
                    timer += Time.deltaTime;
                }
                else
                {//disable, attack done
                    this.gameObject.SetActive(false);
                    attack = false;
                    timer = 0.0f;
                    delayTimer = 0f;
                }
            }

            
        }
        else {
            timer = 0.0f;
            this.gameObject.SetActive(false);
        }
        */
	}
}
