using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDragonController : MonoBehaviour {

    public GameObject player;

    public GameObject iceFlame;
    public TurnToPlayer ttp;
    private bool flameDone = false;


    //hurtboxes
    public GameObject biteHurtBox;


    readonly int m_Attack13End = Animator.StringToHash("Attack13End");


    readonly int m_turning = Animator.StringToHash("turning");
    readonly int m_turnLeft = Animator.StringToHash("turnLeft");
    readonly int m_turnRight = Animator.StringToHash("turnRight");

    readonly int m_iceDouble = Animator.StringToHash("AttackDouble");
    private bool biteDone = false;





    protected Animator m_Animator;

    protected bool turning = false;
    protected bool turnLeft = false;
    protected bool turnRight = false;


    // Use this for initialization
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        ttp = GetComponent<TurnToPlayer>();

    }

    


    // Update is called once per frame
    void Update()
    {


        if (!ttp.turning &&!ttp.movingFront && ttp.inRange0) {

            ttp.attacking = true;
            if(ttp.attacking)
                m_Animator.SetTrigger(m_iceDouble);


        }


        AnimatorStateInfo animationState = m_Animator.GetCurrentAnimatorStateInfo(0);
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
            print("ice final key was pressed");
        }


        //04-s
        if (!flameDone && animationState.IsName("icedragon-4-end1"))
        {
            print("final cast");
            iceFlame.SetActive(true);
            iceFlame.GetComponent<FlameHidden>().startFlame();
            flameDone = true;

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

        }

        else if (animationState.IsName("iceage-start"))
        {
            //this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y + Time.deltaTime * 2.0f, this.transform.position.z);

        }
        if (animationState.IsName("ice-double"))
        {
  

            float stateTime = clip[0].clip.length * animationState.normalizedTime;
            print(stateTime);
            if (stateTime > 2f && stateTime < 6.1f) {
                biteHurtBox.SetActive(true);


            }
            else
            {
                

                biteHurtBox.SetActive(false);
            }
        }



    }
}
