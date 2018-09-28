using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallistaPoint : MonoBehaviour {
    public GameObject arrow;
    public GameObject point;
    public GameObject target;
    public GameObject smoke;


    public float shootDelay = 0.05f;
    private float timer = 0f;
    private float destroyTimer = 0.3f;
    private int currentArrow = 0;
    private bool smokeReleased = false;

	// Use this for initialization
	void Start () {
        GameObject smokeObjStart = Instantiate(smoke, transform.position, transform.rotation);
        smokeObjStart.transform.parent = transform;
    }
	
	// Update is called once per frame
	void Update () {
        if (target == null) {
            target = GetComponent<FindTarget>().target;
        }
        //turn to target
        if (target != null) {
            FaceTarget();
        }

        //for (int i = 0; i <= 20; i++) {iceSwordNumber
        if (currentArrow > 120) {
            //kill obj
            SlowDestroy();
            return;
        }
        if (timer < shootDelay) { timer += Time.deltaTime; return; } else { timer = 0f; }

        float rnd4 = Random.Range(-2.0f, 2.0f);
        float rnd = Random.Range(-10.0f, 10.0f);



        Vector3 forwardPos = point.transform.position + point.transform.forward;
        
        GameObject projectile;

        projectile = Instantiate(arrow, forwardPos, point.transform.rotation) as GameObject;
        projectile.transform.rotation *= Quaternion.Euler(rnd, rnd, rnd);
        if (currentArrow % 3 == 0)
        {
            projectile.GetComponents<AudioSource>()[2].Play();
        }
        else if (currentArrow % 2 == 0)
        {
            projectile.GetComponents<AudioSource>()[1].Play();
        }
        else if (currentArrow == 1)
        {
            projectile.GetComponents<AudioSource>()[0].Play();
        }
        currentArrow += 1;



        //}
    }

    public void FaceTarget()
    {
        Vector3 dir = target.transform.position - this.gameObject.transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 2.0f * Time.deltaTime);
    }

    public void SlowDestroy() {
        if (destroyTimer < 0f)
        {
            //kill obj
            Destroy(this.gameObject);
        }
        else {
            if (!smokeReleased) {
                smokeReleased =true;
                GameObject smokeObj = Instantiate(smoke, transform.position, transform.rotation);
                smokeObj.transform.parent = transform;
            }
            destroyTimer -= Time.deltaTime;
        }
        
    }
}
