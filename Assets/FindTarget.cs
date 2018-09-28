using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTarget : MonoBehaviour {

    public GameObject target = null;
    public float radius = 20f;
    public int layerMask;

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
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius);
        
        if (enemies == null)
            return null;
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Collider targetCol in enemies)
        {
            if (targetCol.gameObject.layer == layerMask) {
                return targetCol.gameObject;
            }
            /*
            Vector3 directionToTarget = targetCol.gameObject.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = targetCol.gameObject;
            }
            */

        }
        
        return bestTarget;
    }
}
