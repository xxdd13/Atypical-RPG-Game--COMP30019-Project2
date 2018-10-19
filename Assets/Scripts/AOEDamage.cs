using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Proj2 {
    public class AOEDamage : MonoBehaviour
    {
        public float damage = 10f, radius = 16f;

        // Use this for initialization
        void Start()
        {

        }
        private void Awake()
        {
            AreaDamageEnemies(this.transform.position, radius, damage);
        }

        // Update is called once per frame
        void Update()
        {

        }
        void AreaDamageEnemies(Vector3 location, float radius, float damage)
        {
            Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
            foreach (Collider col in objectsInRange)
            {
                if (col.gameObject.layer == 13)
                {
                    Damageable enemy = col.GetComponent<Damageable>();
                    if (enemy != null)
                    {
                        // linear falloff of effect
                        float proximity = (location - enemy.transform.position).magnitude;
                        float effect = 1 - (proximity / radius);


                        Damageable.DamageMessage message = new Damageable.DamageMessage
                        {
                            damageSource = transform.position,
                            damager = this,
                            amount = (int)(damage),
                            direction = (col.gameObject.transform.position - transform.position).normalized,
                            throwing = false,
                        };

                        enemy.ApplyDamage(message);
                    }
                }

            }

        }

    }
}
