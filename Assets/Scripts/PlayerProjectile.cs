using UnityEngine;
using System.Collections;
 
public class PlayerProjectile : MonoBehaviour
{
    public GameObject impactParticle;
    public GameObject projectileParticle;
    public GameObject muzzleParticle;
    public GameObject[] trailParticles;
    [HideInInspector]
 
    private bool hasCollided = false;

    private float lifeTimer;
    public float maxDuration = 10f;
 
    void Start()
    {
        projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;
		if (muzzleParticle){
        muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
        Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
		}
    }
    void Update()
    {
        this.lifeTimer += Time.deltaTime;
        if (this.lifeTimer >= maxDuration) {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision hit)
    {
        if (!hasCollided)
        {
            hasCollided = true;

            impactParticle = Instantiate(impactParticle, transform.position, Quaternion.identity) as GameObject;

 
            //yield WaitForSeconds (0.05);
            foreach (GameObject trail in trailParticles)
            {
                GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                curTrail.transform.parent = null;
                Destroy(curTrail, 3f);
            }
            Destroy(projectileParticle, 3f);
            Destroy(impactParticle, 5f);
            Destroy(this.gameObject);
			
            
        }
    }
}