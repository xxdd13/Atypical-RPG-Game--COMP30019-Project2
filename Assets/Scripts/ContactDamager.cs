using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Proj2
{
    public class ContactDamager : MonoBehaviour
    {

        public int amount;
        public LayerMask damagedLayers;

        private void OnTriggerStay(Collider other)
        {
            if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
                return;

            Damageable d = other.GetComponentInChildren<Damageable>();

            if (d != null && !d.isInvulnerable)
            {
                Damageable.DamageMessage message = new Damageable.DamageMessage
                {
                    damageSource = transform.position,
                    damager = this,
                    amount = amount,
                    direction = (other.transform.position - transform.position).normalized,
                    throwing = false
                };

                d.ApplyDamage(message);
            }
        }
    }



}