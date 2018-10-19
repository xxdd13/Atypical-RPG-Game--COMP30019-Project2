using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj2
{
    public class MeleeWeapon : MonoBehaviour
    {
        public int damage = 1;

        [System.Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform attackRoot;

#if UNITY_EDITOR
            //editor only as it's only used in editor to display the path of the attack that is used by the raycast
            [NonSerialized] public List<Vector3> previousPositions = new List<Vector3>();
#endif

        }

        public ParticleSystem hitParticlePrefab;
        public LayerMask targetLayers;

        public AttackPoint[] attackPoints = new AttackPoint[0];

        public TimeEffect[] effects;


        public bool throwingHit
        {
            get { return m_IsThrowingHit; }
            set { m_IsThrowingHit = value; }
        }

        protected GameObject m_Owner;

        protected Vector3[] m_PreviousPos = null;
        protected Vector3 m_Direction;

        protected bool m_IsThrowingHit = false;
        protected bool m_InAttack = false;

        const int PARTICLE_COUNT = 10;
        protected ParticleSystem[] m_ParticlesPool = new ParticleSystem[PARTICLE_COUNT];
        protected int m_CurrentParticle = 0;

        protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
        protected static Collider[] s_ColliderCache = new Collider[32];

        private void Awake()
        {
            if (hitParticlePrefab != null)
            {
                for (int i = 0; i < PARTICLE_COUNT; ++i)
                {
                    m_ParticlesPool[i] = Instantiate(hitParticlePrefab);
                    m_ParticlesPool[i].Stop();
                }
            }
        }

        private void OnEnable()
        {

        }

        
        public void SetOwner(GameObject owner)
        {
            m_Owner = owner;
        }

        public void BeginAttack(bool thowingAttack)
        {
            
            throwingHit = thowingAttack;

            m_InAttack = true;

            m_PreviousPos = new Vector3[attackPoints.Length];

            for (int i = 0; i < attackPoints.Length; ++i)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                                   attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                m_PreviousPos[i] = worldPos;

            }
        }

        public void EndAttack()
        {
            m_InAttack = false;

        }

        private void FixedUpdate()
        {
            if (m_InAttack)
            {
                for (int i = 0; i < attackPoints.Length; ++i)
                {
                    AttackPoint pts = attackPoints[i];

                    Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
                    Vector3 attackVector = worldPos - m_PreviousPos[i];

                    if (attackVector.magnitude < 0.001f)
                    {
                        
                        attackVector = Vector3.forward * 0.0001f;
                    }


                    Ray r = new Ray(worldPos, attackVector.normalized);

                    int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache, attackVector.magnitude,
                        ~0,
                        QueryTriggerInteraction.Ignore);

                    for (int k = 0; k < contacts; ++k)
                    {
                        Collider col = s_RaycastHitCache[k].collider;

                        if (col != null)
                            CheckDamage(col, pts);
                    }

                    m_PreviousPos[i] = worldPos;

                }
            }
        }

        private bool CheckDamage(Collider other, AttackPoint pts)
        {
            Damageable d = other.GetComponent<Damageable>();
            if (d == null)
            {
                return false;
            }

            if (d.gameObject == m_Owner)
                return true; 

            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            {
                
                return false;
            }

           

            Damageable.DamageMessage data;

            data.amount = damage;
            data.damager = this;
            data.direction = m_Direction.normalized;
            data.damageSource = m_Owner.transform.position;
            data.throwing = m_IsThrowingHit;
            data.stopCamera = false;

            d.ApplyDamage(data);

            if (hitParticlePrefab != null)
            {
                m_ParticlesPool[m_CurrentParticle].transform.position = pts.attackRoot.transform.position;
                m_ParticlesPool[m_CurrentParticle].time = 0;
                m_ParticlesPool[m_CurrentParticle].Play();
                m_CurrentParticle = (m_CurrentParticle + 1) % PARTICLE_COUNT;
            }

            return true;
        }

    }
}