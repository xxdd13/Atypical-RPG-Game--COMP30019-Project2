using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour {
    /// xxxxxxxxxxxxxxx
    public GameObject rbSkill;
    public float speed = 1000;
    public Transform spawnPosition;
    /// xxxxxxxxxxxxxx
    /// 

    // Use this for initialization
    void Start () {
        Physics.IgnoreLayerCollision(0, 8);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void rb() {
        //ignore player and magic spell collision

        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        position = Camera.main.ScreenToWorldPoint(position);


        Physics.IgnoreLayerCollision(10, 11);

        Physics.IgnoreLayerCollision(1, 11);

        Vector3 forwardPos = spawnPosition.position + spawnPosition.forward * 5.0f;
        Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y, forwardPos.z);

        GameObject projectile = Instantiate(rbSkill, newPos, Quaternion.identity) as GameObject;
        projectile.transform.LookAt(position);


        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
        //projectile.GetComponent<PlayerProjectile>().impactNormal = position.normal;


    }
}
