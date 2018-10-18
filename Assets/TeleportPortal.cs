using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPortal : MonoBehaviour {
    public Transform portalPoint;
    public Transform outPoint;
    public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(player.gameObject.transform.position, portalPoint.position)<3.5f) {
            player.transform.position = outPoint.position;
        }
	}
}
