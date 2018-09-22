using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour {
    /// xxxxxxxxxxxxxxx
    public GameObject rbSkill;
    public GameObject nukeSkill;
    public GameObject guidedSkill;

    public float speed = 1000;
    public Transform spawnPosition;

    public GameObject goldDragon;



    //for guided arrow graduate generation
    public int guidedSpellProjectileNumber = 15;
    int currentProjectileNumber = 0;
    float guidedDelayTimer= 0.0f;
    public float guidedDelay = 0.5f;

    internal bool guidedTrigger = false;


    /// xxxxxxxxxxxxxx
    /// 

    // Use this for initialization
    void Start () {
        Physics.IgnoreLayerCollision(10, 11);
        Physics.IgnoreLayerCollision(11, 11);
    }
	
	// Update is called once per frame
	void Update () {
        if (guidedTrigger) {
            if (currentProjectileNumber < guidedSpellProjectileNumber)
            {
                if (guidedDelayTimer >= guidedDelay)
                {
                    guided();
                    currentProjectileNumber += 1;
                    guidedDelayTimer =0f;
                }
                else {
                    guidedDelayTimer += Time.deltaTime;
                }
                
            }
            if (currentProjectileNumber >= guidedSpellProjectileNumber)
            {
                guidedTrigger = false;
                currentProjectileNumber = 0;
            }
        }
	}
    public void rb() {
        //ignore player and magic spell collision

        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        position = Camera.main.ScreenToWorldPoint(position);

;

        Vector3 forwardPos = spawnPosition.position + spawnPosition.forward;
        Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y, forwardPos.z);

        GameObject projectile = Instantiate(rbSkill, newPos, Quaternion.identity) as GameObject;
        projectile.transform.LookAt(position);
        


        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
        //projectile.GetComponent<PlayerProjectile>().impactNormal = position.normal;


    }

    public void nuke()
    {

        Vector3 forwardPos = spawnPosition.position + spawnPosition.forward*10;
        Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y+10f, forwardPos.z);
        Vector3 newPos2 = new Vector3(forwardPos.x, forwardPos.y - 15f, forwardPos.z);

        GameObject projectile = Instantiate(nukeSkill, newPos, Quaternion.identity) as GameObject;
        projectile.transform.LookAt(newPos2);

        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
        //projectile.GetComponent<PlayerProjectile>().impactNormal = position.normal;


    }

    public void guided()
    {

       
        float rnd1 = Random.Range(-5.0f, 5.0f);
        float rnd2 = Random.Range(-5.0f, 5.0f);
        Vector3 position = new Vector3(this.transform.position.x + rnd1, this.transform.position.y + 10f, this.transform.position.z + rnd2);

        GameObject projectile = Instantiate(guidedSkill, spawnPosition.position, Quaternion.identity) as GameObject;
        projectile.transform.LookAt(position);

        GuidedArrow missile = projectile.GetComponent<GuidedArrow>();
        missile.target = goldDragon.transform;



    }


}
