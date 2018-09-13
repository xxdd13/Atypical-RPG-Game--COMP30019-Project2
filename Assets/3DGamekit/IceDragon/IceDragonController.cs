using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDragonController : MonoBehaviour {

    public GameObject player;
    public GameObject pheonixHead;

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
            m_Animator.Play("iceage-start");
            print("iceage key was pressed");
        }
        if (Input.GetKeyDown("c"))
        {
            //m_Animator.SetTrigger(m_Attack13End);
            m_Animator.Play("ice-final-start");
            print("ice final key was pressed");
        }

        //m_Animator.SetBool(m_turning, turning);
        //m_Animator.SetBool(m_turnLeft, turnLeft);
        //m_Animator.SetBool(m_turnRight, turnRight);

        if (Input.GetKeyDown("x"))
            transform.position += transform.forward * 1f; 

    }
}
