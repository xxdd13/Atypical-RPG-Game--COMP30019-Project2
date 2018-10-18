using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Proj2 { 
public class UnicornHeal : MonoBehaviour {


    public int amount;
    public LayerMask damagedLayers;
        public float delay=0.1f;
    private float timer = 0f;

    private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == 10) {
                other.gameObject.GetComponent<PlayerController>().respawnPos = this.gameObject.transform;
            }
    }
    private void OnTriggerStay(Collider other)
    {
        if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
            return;

        Damageable d = other.GetComponentInChildren<Damageable>();

            if (d != null && timer > delay && d.currentHitPoints < d.maxHitPoints)
            {

                timer = 0f;
                d.currentHitPoints += amount;
                print(d.currentHitPoints);
            }
            else { timer += Time.deltaTime; }
    }
}
}
