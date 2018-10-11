using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

    public GameObject boat;
    public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("1")) {
            if (Vector3.Distance(boat.transform.position, player.transform.position) < 2f &&boat.transform.position.x < 295f) {
                Vector3 dispalce = boat.transform.forward * Time.deltaTime * 10f;
                boat.transform.position -= dispalce;
                player.transform.position -= dispalce;
            }
            
        }
        if (Input.GetKey("2"))
        {
            if (Vector3.Distance(boat.transform.position, player.transform.position) < 2f && boat.transform.position.x > 240f)
            {
                Vector3 dispalce = boat.transform.forward * Time.deltaTime * 10f;
                boat.transform.position += dispalce;
                player.transform.position += dispalce;
            }

        }
    }
}
