using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDragonController : MonoBehaviour {

    public GameObject player;

    public GameObject iceFlame;
    private bool flameDone = false;


    readonly int m_Attack13End = Animator.StringToHash("Attack13End");


    readonly int m_turning = Animator.StringToHash("turning");
    readonly int m_turnLeft = Animator.StringToHash("turnLeft");
    readonly int m_turnRight = Animator.StringToHash("turnRight");

    protected Animator m_Animator;

    protected bool turning = false;
    protected bool turnLeft = false;
    protected bool turnRight = false;


    // Use this for initialization
    void Start()
    {
        m_Animator = GetComponent<Animator>();

    }

    


    // Update is called once per frame
    void Update()
    {

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
        if (!flameDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("icedragon-4-end1"))
        {
            print("final cast");
            iceFlame.SetActive(true);
            iceFlame.GetComponent<FlameHidden>().startFlame();
            flameDone = true;

        }


        if (!flameDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ice-final-end"))
        {
            print("final cast");
            iceFlame.SetActive(true);
            iceFlame.GetComponent<FlameHidden>().startFlame();
            flameDone = true;

        }

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("icedragon-final-end2"))
        {
            //iceFlame.SetActive(false);
            FlameHidden fh = iceFlame.GetComponent<FlameHidden>();
            fh.stopFlame();
            flameDone = false;
            
        }
        if (!flameDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("iceage-loop-end"))
        {
            print("iceage cast");
            iceFlame.SetActive(true);
            iceFlame.GetComponent<FlameHidden>().startFlame();
            flameDone = true;

        }

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
        {
            FlameHidden fh = iceFlame.GetComponent<FlameHidden>();
            fh.stopFlame();
            flameDone = false;

        }

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("iceage-start"))
        {
            //this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y + Time.deltaTime * 2.0f, this.transform.position.z);

        }


    }
}
