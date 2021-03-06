﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Proj2
{
    public class PlayerSkill : MonoBehaviour
    {
        private FindTarget ft;
        public GameObject rbSkill;

        public GameObject rbSkill2;
        public GameObject nukeSkill;
        public GameObject guidedSkill;
        public GameObject smallIceSwordSkill;
        public GameObject bigIceSwordSkill;
        public GameObject ballistaSkill;
        public GameObject crossSkill;

        public float speed = 1000;
        public Transform spawnPosition;

        public GameObject goldDragon;

        private Animator m_Animator;

        readonly int m_HashGunEnd = Animator.StringToHash("GunEnd");
        readonly int m_HashGunAuto = Animator.StringToHash("GunAuto");


        //for guided arrow graduate generation
        public int guidedSpellProjectileNumber = 15;
        int currentProjectileNumber = 0;
        float guidedDelayTimer = 0.0f;
        public float guidedDelay = 0.5f;

        internal bool guidedTrigger = false;

        public int iceSwordNumber = 20;
        int currentIceSwordNumber = 0;
        internal bool iceSwordTrigger = false;





        // Use this for initialization
        void Start()
        {
            Physics.IgnoreLayerCollision(10, 11);
            Physics.IgnoreLayerCollision(11, 11);
            ft = this.gameObject.GetComponent<FindTarget>();
        }

        private void Awake()
        {
            m_Animator = this.gameObject.GetComponent<PlayerController>().m_Animator;
        }


        // Update is called once per frame
        void Update()
        {
            if (guidedTrigger)
            {
                if (currentProjectileNumber < guidedSpellProjectileNumber)
                {
                    if (guidedDelayTimer >= guidedDelay)
                    {
                        guided();
                        currentProjectileNumber += 1;
                        guidedDelayTimer = 0f;
                    }
                    else
                    {
                        guidedDelayTimer += Time.deltaTime;
                    }

                }
                if (currentProjectileNumber >= guidedSpellProjectileNumber)
                {
                    guidedTrigger = false;
                    currentProjectileNumber = 0;
                    //put gun away
                    m_Animator = this.gameObject.GetComponent<PlayerController>().m_Animator;

                    m_Animator.SetTrigger(m_HashGunEnd);
                    m_Animator.ResetTrigger(m_HashGunAuto);

                }
            }


            //icesword
            if (iceSwordTrigger)
            {
                if (currentIceSwordNumber < iceSwordNumber)
                {
                    iceSword();
                    currentIceSwordNumber++;

                }
                if (currentIceSwordNumber >= iceSwordNumber)
                {
                    iceSwordTrigger = false;
                    currentIceSwordNumber = 0;

                }
            }

        }
        public void rb()
        {
            //ignore player and magic spell collision

            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
            position = Camera.main.ScreenToWorldPoint(position);

            

            Vector3 forwardPos = spawnPosition.position + spawnPosition.forward;
            Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y, forwardPos.z);

            GameObject projectile = Instantiate(rbSkill, newPos, Quaternion.identity) as GameObject;
            projectile.transform.LookAt(position);
            rbwait(1f);



            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
            //projectile.GetComponent<PlayerProjectile>().impactNormal = position.normal;


        }
        public void rb2()
        {
            //ignore player and magic spell collision

            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
            position = Camera.main.ScreenToWorldPoint(position);



            Vector3 forwardPos = spawnPosition.position + spawnPosition.forward;
            Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y, forwardPos.z);

            GameObject projectile = Instantiate(rbSkill2, newPos, Quaternion.identity) as GameObject;
            projectile.transform.LookAt(position);
            



            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);


        }

        IEnumerator rbwait(float waitTime)
        {
            print(1111111);
            yield return new WaitForSeconds(waitTime);
            Vector3 forwardPos = spawnPosition.position + spawnPosition.forward;
            Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y, forwardPos.z);
            GameObject projectile = Instantiate(rbSkill, newPos, Quaternion.identity) as GameObject;
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
            position = Camera.main.ScreenToWorldPoint(position);
            projectile.transform.LookAt(position);

            rbwait(1f);

        }

        public void nuke()
        {

            Vector3 forwardPos = spawnPosition.position + spawnPosition.forward * 10;
            Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y + 10f, forwardPos.z);
            Vector3 newPos2 = new Vector3(forwardPos.x, forwardPos.y - 15f, forwardPos.z);

            GameObject projectile = Instantiate(nukeSkill, newPos, Quaternion.identity) as GameObject;
            projectile.transform.LookAt(newPos2);

            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
            //projectile.GetComponent<PlayerProjectile>().impactNormal = position.normal;


        }

        public void guided()
        {

            if (ft.target == null) {
                return;
            }
            float rnd1 = Random.Range(-5.0f, 5.0f);
            float rnd2 = Random.Range(-5.0f, 5.0f);
            Vector3 position = new Vector3(this.transform.position.x + rnd1, this.transform.position.y + 10f, this.transform.position.z + rnd2);

            GameObject projectile = Instantiate(guidedSkill, spawnPosition.position, Quaternion.identity) as GameObject;
            projectile.transform.LookAt(position);

            GuidedArrow missile = projectile.GetComponent<GuidedArrow>();
            missile.target = ft.target.transform;

        }

        public void iceSword()
        {

            //for (int i = 0; i <= 20; i++) {iceSwordNumber
            int i = currentIceSwordNumber;


            float rnd4 = Random.Range(-2.0f, 2.0f);
            float rnd = Random.Range(-15.0f, 15.0f);



            Vector3 forwardPos = spawnPosition.position + spawnPosition.forward * ((float)i / 2f);
            Vector3 swordPosition = forwardPos + spawnPosition.right * (rnd4);
            GameObject projectile;

            if (i >= 19)
            {
                projectile = Instantiate(bigIceSwordSkill, forwardPos, this.transform.rotation) as GameObject;
                projectile.GetComponent<AudioSource>().Play();

            }
            else
            {
                projectile = Instantiate(smallIceSwordSkill, swordPosition, this.transform.rotation) as GameObject;
                projectile.transform.rotation *= Quaternion.Euler(rnd, rnd4, rnd);
                if (i % 3 == 0)
                {
                    projectile.GetComponents<AudioSource>()[2].Play();
                }
                else if (i % 2 == 0)
                {
                    projectile.GetComponents<AudioSource>()[1].Play();
                }
                else if (i == 1)
                {
                    projectile.GetComponents<AudioSource>()[0].Play();
                }

            }
            projectile.GetComponent<IceSwordMove>().goalHeight = this.transform.position.y + 0.6f;
            projectile.GetComponent<IceSwordMove>().goalSet = true;
            projectile.transform.position = new Vector3(projectile.transform.position.x, this.transform.position.y - i, projectile.transform.position.z);


            //}





        }

        public void ballista()
        {

            Vector3 forwardPos = spawnPosition.position + spawnPosition.forward * 3f;
            Vector3 pos = new Vector3(forwardPos.x, forwardPos.y - 1.33f, forwardPos.z);

            GameObject projectile = Instantiate(ballistaSkill, pos, spawnPosition.transform.localRotation) as GameObject;
            projectile.transform.LookAt(pos + spawnPosition.forward * 3f);

            //projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);

        }

        public void cross()
        {
            Vector3 forwardPos = spawnPosition.position + spawnPosition.forward * 10;
            Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y + 20f, forwardPos.z);


            GameObject projectile = Instantiate(crossSkill, newPos, spawnPosition.transform.rotation) as GameObject;
            

        }

        public void teleport()
        {
            RaycastHit hit;
            Vector3 forwardPos = spawnPosition.transform.position; ;// + transform.forward;
            if (Physics.Raycast(forwardPos, spawnPosition.transform.forward, out hit, 12f, 11)) {
                if (hit.collider != null) //need to stop right before collision
                {
                    print(hit.collider.gameObject.name);
                    Vector3 teleportPos = hit.point - (spawnPosition.transform.forward); //don't wanna stuck, so step back a few units
                                                                                         //this.transform.position = teleportPos;
                    this.gameObject.transform.position = teleportPos;
                    //GameObject projectile = Instantiate(rbSkill, teleportPos, spawnPosition.transform.rotation) as GameObject;
                }
                else
                { // hit nothing, just eleport
                    this.gameObject.transform.position += this.gameObject.transform.forward * 12f;
                }
            }
            else
            { // hit nothing, just eleport
                this.gameObject.transform.position += this.gameObject.transform.forward * 12f;
            }






        }


    }
}