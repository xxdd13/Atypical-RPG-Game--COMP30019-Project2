using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Proj2
{

    public class IceDragonController : MonoBehaviour
    {

        public GameObject player;

        public GameObject iceFlame;
        public TurnToPlayer ttp;
        private bool flameDone = false;


        //hurtboxes
        public GameObject biteHurtBox;
        public GameObject stompHurtBox;
        public GameObject landHurtBox;

        //eff
        public GameObject boom;


        readonly int m_Attack13End = Animator.StringToHash("Attack13End");


        readonly int m_turning = Animator.StringToHash("turning");
        readonly int m_turnLeft = Animator.StringToHash("turnLeft");
        readonly int m_turnRight = Animator.StringToHash("turnRight");

        readonly int m_iceDouble = Animator.StringToHash("AttackDouble");
        private bool biteDone = false;

        readonly int m_iceBreath = Animator.StringToHash("AttackIceBreath");
        readonly int m_stompL = Animator.StringToHash("AttackStompL");

        readonly int m_iceFly = Animator.StringToHash("AttackIceFly");
        readonly int m_iceFlyEnd = Animator.StringToHash("AttackIceFlyEnd");

        readonly int m_death = Animator.StringToHash("death");

        private Damageable damageable = null;



        protected Animator m_Animator;

        protected bool turning = false;
        protected bool turnLeft = false;
        protected bool turnRight = false;

        private bool doing = false;
        private bool boomDone = false;


        // Use this for initialization
        void Start()
        {
            m_Animator = GetComponent<Animator>();
            ttp = GetComponent<TurnToPlayer>();
            damageable = GetComponent<Damageable>();

        }




        // Update is called once per frame
        void Update()
        {
            //ice dragon died
            AnimatorStateInfo animationState = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (damageable.currentHitPoints <=0){
                if (!animationState.IsName("ice-die"))
                {
                    m_Animator.SetTrigger(m_death);
                    this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, 112.5f, this.transform.position.z);
                    return;
                }
                else {

                    
                    return;
                }
            }


            if (!ttp.turning && !ttp.movingFront && ttp.inRange0 && !doing)
            {

                ttp.attacking = true;
                if (ttp.attacking)
                {

                    int att = Random.Range(3, 4);

                    print(att);


                    if (att == 0)
                    {
                        attack(m_iceDouble);
                    }
                    else if (att == 1)
                    {
                        attack(m_iceBreath);
                    }
                    else if (att == 2)
                    {
                        attack(m_stompL);
                    }
                    else if (att == 3)
                    {
                        attack(m_iceFly);
                    }

                }



            }


            
            AnimatorClipInfo[] clip = m_Animator.GetCurrentAnimatorClipInfo(0);

            if (Input.GetKeyDown("x"))
            {
                //m_Animator.SetTrigger(m_Attack13End);
                m_Animator.Play("icedragon-4-start");
            }
            if (Input.GetKeyDown("c"))
            {
                //m_Animator.SetTrigger(m_Attack13End);
                m_Animator.Play("ice-final-start");

            }


            if (animationState.IsName("ice-left-stomp"))
            {


                float stateTime = clip[0].clip.length * animationState.normalizedTime;
                if (stateTime > 4.5f && stateTime < 5.5f)
                {
                    HurtBox(stompHurtBox);

                    if (!boomDone)
                    {
                        boomDone = true;
                        GameObject groundBoom = Instantiate(boom, stompHurtBox.transform.position, stompHurtBox.transform.localRotation) as GameObject;
                    }


                }
                else
                {
                    HurtBoxDone(stompHurtBox);
                    boomDone = false;
                }
                ttp.facePlayer();
            }
            else if (animationState.IsName("ice-fly-start"))
            {


                float stateTime = clip[0].clip.length * animationState.normalizedTime;
                if (stateTime > 5.5f)
                {

                    this.gameObject.transform.position += this.gameObject.transform.up * Time.deltaTime * 5f;


                }
                else
                {

                }
                ttp.facePlayer();
            }
            else if (animationState.IsName("ice-flip-breath"))
            {


                float stateTime = clip[0].clip.length * animationState.normalizedTime;
                if (stateTime > 5.0f && stateTime < 10.0f)
                {
                    fire();
                }
                else if (stateTime > 10.1f)
                {
                    halt();
                }
                ttp.facePlayer();


            }
            else if (animationState.IsName("ice-fly-loop"))
            {
                ttp.facePlayer();

            }
            else if (animationState.IsName("ice-land-a"))
            {


                float stateTime = clip[0].clip.length * animationState.normalizedTime;
                if (stateTime > 6.0f && this.gameObject.transform.position.y > 111.83f)
                {
                    print(this.gameObject.transform.position.y);
                    this.gameObject.transform.position -= this.gameObject.transform.up * Time.deltaTime * 40f;


                }
                if (stateTime > 6.9f && stateTime < 7.5f)
                {
                    if (!boomDone)
                    {
                        boomDone = true;
                        HurtBox(landHurtBox);
                        Vector3 pos = landHurtBox.transform.position + (landHurtBox.transform.up * 4f) + (landHurtBox.transform.forward * 6f);
                        GameObject groundBoom = Instantiate(boom, pos, landHurtBox.transform.localRotation) as GameObject;
                        groundBoom.transform.localScale *= 2.5f;
                    }
                }
                else if (stateTime > 7.6f)
                {
                    HurtBoxDone(landHurtBox);
                    boomDone = false;
                }
                ttp.facePlayer();
            }


            //04-end1

            if (animationState.IsName("icedragon-4-end1"))
            {


                float stateTime = clip[0].clip.length * animationState.normalizedTime;
                if (stateTime < 5.1f)
                {
                    fire();
                }
                else
                {
                    halt();
                }
                ttp.facePlayer();
            }
            else if (animationState.IsName("icedragon-4-start"))
            {
                ttp.facePlayer();
            }



            if (!flameDone && animationState.IsName("ice-final-end"))
            {
                print("final cast");
                iceFlame.SetActive(true);
                iceFlame.GetComponent<FlameHidden>().startFlame();
                flameDone = true;

            }

            if (animationState.IsName("icedragon-final-end2"))
            {
                //iceFlame.SetActive(false);
                FlameHidden fh = iceFlame.GetComponent<FlameHidden>();
                fh.stopFlame();
                flameDone = false;

            }
            if (!flameDone && animationState.IsName("iceage-loop-end"))
            {
                print("iceage cast");
                iceFlame.SetActive(true);
                iceFlame.GetComponent<FlameHidden>().startFlame();
                flameDone = true;

            }

            if (animationState.IsName("Stand"))
            {
                FlameHidden fh = iceFlame.GetComponent<FlameHidden>();
                fh.stopFlame();
                flameDone = false;
                ttp.attacking = false;
                this.doing = false;

            }

            else if (animationState.IsName("iceage-start"))
            {
                //this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y + Time.deltaTime * 2.0f, this.transform.position.z);

            }
            if (animationState.IsName("ice-double"))
            {


                float stateTime = clip[0].clip.length * animationState.normalizedTime;
                print(stateTime);
                if (stateTime > 2f && stateTime < 6.1f)
                {
                    biteHurtBox.SetActive(true);


                }
                else
                {

                    biteHurtBox.SetActive(false);
                }
            }



        }
        public void fire()
        {
            iceFlame.SetActive(true);
            iceFlame.GetComponent<FlameHidden>().startFlame();
        }
        public void halt()
        {
            FlameHidden fh = iceFlame.GetComponent<FlameHidden>();
            fh.stopFlame();

        }

        public void attack(int trigger)
        {
            foreach (AnimatorControllerParameter p in m_Animator.parameters)
            {
                m_Animator.ResetTrigger(p.name);
            }
            m_Animator.SetTrigger(trigger);
            this.doing = true;
            ttp.attacking = true;
        }

        public void HurtBox(GameObject obj)
        {
            obj.SetActive(true);
        }

        public void HurtBoxDone(GameObject obj)
        {
            obj.SetActive(false);
        }

    }
}
