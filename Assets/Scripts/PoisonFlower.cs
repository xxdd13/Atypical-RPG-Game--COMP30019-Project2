using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Proj2
{
    public class PoisonFlower : MonoBehaviour
    {


        public int amount;
        public LayerMask damagedLayers;
        public float delay = 0.1f;
        private float timer = 0f;
        private void OnTriggerStay(Collider other)
        {
            if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
                return;

            Damageable d = other.GetComponentInChildren<Damageable>();

            if (d != null && timer > delay)
            {

                timer = 0f;
                d.currentHitPoints -= amount;
                if (d.currentHitPoints <= 3) {
                    
                    Damageable.DamageMessage message = new Damageable.DamageMessage
                    {
                        damageSource = transform.position,
                        damager = this,
                        amount = 100,
                        direction = (other.transform.position - transform.position).normalized,
                        throwing = false
                    };

                    d.ApplyDamage(message);
                }
            }
            else { timer += Time.deltaTime; }
            
        }
    }
}
