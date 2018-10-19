using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Proj2 { 
public class PheonixController : MonoBehaviour {

    public GameObject player;

    public GameObject iceFlame;
    public TurnToPlayer ttp;
    private bool flameDone = false;
    public GameObject gate;

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

    readonly int m_ph4 = Animator.StringToHash("AttackPh4");
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
        if (damageable.currentHitPoints <= 0)
        {
            if (!animationState.IsName("ph-die"))
            {
                m_Animator.SetTrigger(m_death);
                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, 4.1f, this.transform.position.z);
                    FlameHidden fh = iceFlame.GetComponent<FlameHidden>();
                    fh.stopFlame();
                    flameDone = false;
                    return;
            }
            else
            {
                gate.GetComponent<GateController>().enabled = true;

                return;
            }
        }


        if (!ttp.turning && !ttp.movingFront && ttp.targetDistance<9f && !doing)
        {

            ttp.attacking = true;
            if (ttp.attacking)
            {

                int att = Random.Range(0, 3);


                if (att == 0)
                {
                    //attack(m_iceDouble);
                }
                else if (att == 1)
                {
                    attack(m_ph4);
                }
                else if (att == 2)
                {
                    //attack(m_stompL);
                }


            }



        }



        AnimatorClipInfo[] clip = m_Animator.GetCurrentAnimatorClipInfo(0);

        if (Input.GetKeyDown("x"))
        {
            //m_Animator.SetTrigger(m_Attack13End);
            m_Animator.Play("ph-4-start");
        }
        if (Input.GetKeyDown("c"))
        {
            //m_Animator.SetTrigger(m_Attack13End);
            m_Animator.Play("ice-final-start");

        }


        if (animationState.IsName("ph-4-ang"))
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
        else if (animationState.IsName("ph-4-start"))
        {
                

                float stateTime = clip[0].clip.length * animationState.normalizedTime;
                if (stateTime > 3f && stateTime < 10.0f)
                {
                    fire();
                }
                else {
                    this.gameObject.transform.position += this.gameObject.transform.up * Time.deltaTime * 2f;
                }



                ttp.facePlayer();
        }
        else if (animationState.IsName("ph-4-loop"))
        {


            
            ttp.facePlayer();


        }
        else if (animationState.IsName("ph-4-end"))
            {


                halt();
                ttp.facePlayer();


        }
            else if (animationState.IsName("ice-fly-loop"))
        {
            ttp.facePlayer();

        }
        else if (animationState.IsName("ph-land"))
        {
                

            float stateTime = clip[0].clip.length * animationState.normalizedTime;
            if (stateTime>5.0f &&stateTime < 5.6f && this.gameObject.transform.position.y > 3.6f)
            {
                this.gameObject.transform.position -= this.gameObject.transform.up * Time.deltaTime * 10f;


            }
            if (stateTime > 5.8f && stateTime < 6.1f)
            {
                if (!boomDone)
                {
                        boomDone = true;
                        
                    Vector3 pos = gameObject.transform.position - (gameObject.transform.up * 6f) + (landHurtBox.transform.forward * 5f);
                    GameObject groundBoom = Instantiate(boom, pos, this.gameObject.transform.rotation) as GameObject;
                    groundBoom.transform.localScale *= 2.5f;
                }
            }
                if (stateTime < 6.1f) {
                    HurtBox(landHurtBox);
                }
                else
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
