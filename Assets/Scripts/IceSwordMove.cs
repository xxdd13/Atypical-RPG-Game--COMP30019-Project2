using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSwordMove : MonoBehaviour {
    private float swordHeight;
    private float timer =1f;
    public float goalHeight;
    public bool goalSet;
    private bool canKill = false;

	// Use this for initialization
	void Start () {
        swordHeight = this.transform.position.y;

    }
	
	// Update is called once per frame
	void Update () {
        if (goalSet) {
            
            if (timer <= 0f)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - Time.deltaTime * 30f, this.transform.position.z);
                if (this.transform.position.y <= swordHeight) { canKill = true; }
            }
            else {
                if (this.transform.position.y < goalHeight)
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + Time.deltaTime * 50f, this.transform.position.z);
                }
                //sword fully poped out
                if (this.transform.position.y >= goalHeight)
                {
                    timer -= Time.deltaTime;
                }
            }
            if (canKill) {
                Destroy(this.gameObject);
            }
        }
        
    }
}
