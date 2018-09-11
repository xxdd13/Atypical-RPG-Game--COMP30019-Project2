using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheonixController : MonoBehaviour {

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
    void Start () {
        m_Animator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("v"))
        {
            //m_Animator.SetTrigger(m_Attack13End);
            m_Animator.Play("Attack13End");
            print("space key was pressed");
        }

        


        float angle = 30.0f;
        float anglePlayer = Vector3.Angle(this.transform.right, player.transform.position - this.transform.position);
        if (anglePlayer > angle)
        {
            float angleLeft = Vector3.Angle(this.transform.forward, player.transform.position - this.transform.position);
            float angleRight = Vector3.Angle(-this.transform.forward, player.transform.position - this.transform.position);

            if (angleLeft < angleRight)
            {
                turnRight = false;
                turnLeft = true;
                if (anglePlayer > 0f)
                {
                    AnimatorStateInfo animationState = m_Animator.GetCurrentAnimatorStateInfo(0);
                    if (animationState.IsName("Turn Left"))
                {
                    AnimatorClipInfo[] myAnimatorClip = m_Animator.GetCurrentAnimatorClipInfo(0);
                    float turnTime = myAnimatorClip[0].clip.length * animationState.normalizedTime;
                    if (turnTime > 0.5f)
                    {

                            this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * -30f, Vector3.up);

                        }

                }

                }
                
                print("Pheonix turning Left");
            }
            else
            {


                turnRight = true;
                turnLeft = false;

                if (anglePlayer > 0f) {
                    AnimatorStateInfo animationState = m_Animator.GetCurrentAnimatorStateInfo(0);
                   

                    if (animationState.IsName("Turn Right")) {
                        AnimatorClipInfo[] myAnimatorClip = m_Animator.GetCurrentAnimatorClipInfo(0);
                        float turnTime = myAnimatorClip[0].clip.length * animationState.normalizedTime;
                        if (turnTime > 0.7f) {
                            this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * 30f, Vector3.up);
                            
                        }

                    }
                    

                }
                    

                print("Pheonix turning Right");


            }
            turning = true;

        }
        else {
            turning = turnLeft = turnRight = false ;
        }
        
        

        m_Animator.SetBool(m_turning, turning);
        m_Animator.SetBool(m_turnLeft, turnLeft);
        m_Animator.SetBool(m_turnRight, turnRight);



    }
}
