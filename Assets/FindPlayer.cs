using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayer : MonoBehaviour {

    public GameObject target;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        if (target == null) {
            target = GetClosestEnemy();
        }

        

    }
    GameObject GetClosestEnemy()
    {
        return target;
    }
}
