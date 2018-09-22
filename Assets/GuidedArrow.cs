using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedArrow : MonoBehaviour
{
    public enum MissileType
    {
        Unguided,
        Guided,
        Predictive
    }

    public MissileType missileType;
    public Transform target;
    public LayerMask layerMask;

    public GameObject impactParticle;

    public float detonationDistance;

    public float lifeTime = 5f; // Missile life time
    public float despawnDelay; // Delay despawn in ms
    public float velocity = 300f; // Missile velocity
    public float alignSpeed = 1f;
    public float RaycastAdvance = 2f; // Raycast advance multiplier

    public bool DelayDespawn = false; // Missile despawn flag

    public ParticleSystem[] delayedParticles; // Array of delayed particles
    ParticleSystem[] particles; // Array of Missile particles

    new Transform transform; // Cached transform

    bool isHit = false; // Missile hit flag
    bool isFXSpawned = false; // Hit FX prefab spawned flag

    float timer = 0f; // Missile timer

    Vector3 targetLastPos;
    Vector3 step;

    MeshRenderer meshRenderer;


    void Awake()
    {
        // Cache transform and get all particle systems attached
        transform = GetComponent<Transform>();
        particles = GetComponentsInChildren<ParticleSystem>();
        //meshRenderer = GetComponent<MeshRenderer>();
    }

    // OnSpawned called by pool manager 
    public void OnSpawned()
    {
        isHit = false;
        isFXSpawned = false;
        timer = 0f;
        targetLastPos = Vector3.zero;
        step = Vector3.zero;
        //meshRenderer.enabled = true;
    }

    // OnDespawned called by pool manager 
    public void OnDespawned()
    {
        Destroy(this.gameObject);
    }

    // Stop attached particle systems emission and allow them to fade out before despawning
    void Delay()
    {
        if (particles.Length > 0 && delayedParticles.Length > 0)
        {
            bool delayed;

            for (int i = 0; i < particles.Length; i++)
            {
                delayed = false;

                for (int y = 0; y < delayedParticles.Length; y++)
                    if (particles[i] == delayedParticles[y])
                    {
                        delayed = true;
                        break;
                    }

                particles[i].Stop(false);

                if (!delayed)
                    particles[i].Clear(false);
            }
        }
    }

    // Despaw routine
    void OnMissileDestroy()
    {
        Destroy(impactParticle);
        Destroy(this.gameObject);

    }

    void OnHit()
    {
        //meshRenderer.enabled = false;
        isHit = true;

        
        // Invoke delay routine if required
        if (DelayDespawn)
        {
            // Reset missile timer and let particles systems stop emitting and fade out correctly
            timer = 0f;
            Delay();
        }
    }

    void Update()
    {
        // If something was hit
        if (isHit)
        {
            // Execute once
            if (!isFXSpawned)
            {
                // Put your calls to effect manager that spawns explosion on hit
                impactParticle = Instantiate(impactParticle, transform.position, Quaternion.identity) as GameObject;

                isFXSpawned = true;
            }

            // Despawn current missile 
            if (!DelayDespawn || (DelayDespawn && (timer >= despawnDelay)))
                OnMissileDestroy();
        }
        // No collision occurred yet
        else
        {
            // Navigate
            if (target != null)
            {
                if (missileType == MissileType.Predictive)
                {
                    Vector3 hitPos = Predict(transform.position, target.position, targetLastPos,
                        velocity);
                    targetLastPos = target.position;

                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(hitPos - transform.position), Time.deltaTime * alignSpeed);
                }
                else if (missileType == MissileType.Guided)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(target.position - transform.position), Time.deltaTime * alignSpeed);
                }
            }

            // Missile step per frame based on velocity and time
            step = transform.forward * Time.deltaTime * velocity;

            if (target != null && missileType != MissileType.Unguided &&
                Vector3.SqrMagnitude(transform.position - target.position) <= detonationDistance)
            {
                OnHit();
            }
         
            // Nothing hit
            else
            {
                // Despawn missile at the end of life cycle
                if (timer >= lifeTime)
                {
                    // Do not detonate
                    isFXSpawned = true;
                    OnHit();
                }
            }

            // Advances missile forward
            transform.position += step;
        }

        // Updates missile timer
        timer += Time.deltaTime;
    }



    /////////algorithm////////////////
    public static Vector3 Predict(Vector3 sPos, Vector3 tPos, Vector3 tLastPos, float pSpeed)
    {
        // Target velocity
        Vector3 tVel = (tPos - tLastPos) / Time.deltaTime;

        // Time to reach the target
        float flyTime = GetProjFlightTime(tPos - sPos, tVel, pSpeed);

        if (flyTime > 0)
            return tPos + flyTime * tVel;
        return tPos;
    }

    static float GetProjFlightTime(Vector3 dist, Vector3 tVel, float pSpeed)
    {
        float a = Vector3.Dot(tVel, tVel) - pSpeed * pSpeed;
        float b = 2.0f * Vector3.Dot(tVel, dist);
        float c = Vector3.Dot(dist, dist);

        float det = b * b - 4 * a * c;

        if (det > 0)
            return 2 * c / (Mathf.Sqrt(det) - b);
        return -1;
    }

}
