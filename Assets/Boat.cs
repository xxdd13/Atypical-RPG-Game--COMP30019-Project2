using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

    public GameObject boat;
    public GameObject up;
    public GameObject down;
    public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float dis = Vector3.Distance(boat.transform.position, player.transform.position);
        if (dis < 2f)
        {
            up.SetActive(true);
            down.SetActive(true);
        }
        else {
            up.SetActive(false);
            down.SetActive(false);
            return;
        }
        if (Input.GetKey("1")) {
            if (dis < 2f &&boat.transform.position.x < 295f) {
                Vector3 dispalce = boat.transform.forward * Time.deltaTime * 10f;
                boat.transform.position -= dispalce;
                player.transform.position -= dispalce;
            }
            
        }
        if (Input.GetKey("2"))
        {
            if (dis < 2f && boat.transform.position.x > 240f)
            {
                Vector3 dispalce = boat.transform.forward * Time.deltaTime * 10f;
                boat.transform.position += dispalce;
                player.transform.position += dispalce;
            }

        }
    }
}
