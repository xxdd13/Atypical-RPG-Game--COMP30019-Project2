using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToPlayer : MonoBehaviour {
    public GameObject player;
    public float moveSpeed = 10f;
    public GoldDragonController gdc;

    readonly int m_Attack13End = Animator.StringToHash("Attack13End");

    protected Animator m_Animator;
    readonly int m_stand = Animator.StringToHash("stand");
    readonly int m_turnLeft = Animator.StringToHash("turnLeft");
    readonly int m_turnRight = Animator.StringToHash("turnRight");
    readonly int m_moveFront = Animator.StringToHash("MoveFront");
    


    protected bool turning = false;
    protected bool turnLeft = false;
    protected bool turnRight = false;
    protected bool movingFront = false;

    private bool acti = false;

    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
       
        float distance = Vector3.Distance(player.transform.position, this.gameObject.transform.position);
        if (!acti)
        {
            if (distance < 20f) {
                acti = true;
                gdc.enabled = true;
            }
            return;
        }

        float angle = 20.0f;
        float anglePlayer = Vector3.Angle(this.transform.forward, player.transform.position - this.transform.position);
        if (!movingFront&&anglePlayer > angle)
        {
            float angleLeft = Vector3.Angle(-this.transform.right, player.transform.position - this.transform.position);
            float angleRight = Vector3.Angle(this.transform.right, player.transform.position - this.transform.position);


            Vector3 dir = player.transform.position-this.gameObject.transform.position;    
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 33.0f * Time.deltaTime);


            if (angleLeft < angleRight)
            {

                m_Animator.ResetTrigger(m_turnRight);
                m_Animator.SetTrigger(m_turnLeft);
                
                
                turnRight = false;
                turnLeft = true;
                if (anglePlayer > 0f)
                {
                    AnimatorStateInfo animationState = m_Animator.GetCurrentAnimatorStateInfo(0);
                    if (animationState.IsName("turnLeft"))
                    {
                        //this.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * -30f, Vector3.up);
                        // slerp to the desired rotation over time
                        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 1.0f * Time.deltaTime);

                    }

                }
            }
            else
            {


                turnRight = true;
                turnLeft = false;

                if (anglePlayer > 0f)
                {
                    m_Animator.ResetTrigger(m_turnLeft);
                    m_Animator.SetTrigger(m_turnRight);
                    AnimatorStateInfo animationState = m_Animator.GetCurrentAnimatorStateInfo(0);


                    if (animationState.IsName("turnRight"))
                    {
                        //this.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * 30f, Vector3.up);
                        // slerp to the desired rotation over time
                        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 1.0f * Time.deltaTime);

                    }


                }



            }
            turning = true;
            m_Animator.ResetTrigger(m_stand);

        }
        else
        {
            turning = turnLeft = turnRight = false;
            
            m_Animator.ResetTrigger(m_turnRight);
            m_Animator.ResetTrigger(m_turnLeft);

            

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Stand")) {
                m_Animator.ResetTrigger(m_stand);
            } else if(!movingFront)
            {
                m_Animator.SetTrigger(m_stand);
            }

            //check for distance
   
            
            if (distance > 35f) {
                m_Animator.SetTrigger(m_moveFront);
                movingFront = true;
            }
            //moving -> standing
            if (distance < 5f)
            {
                movingFront = false;
                m_Animator.ResetTrigger(m_moveFront);
                m_Animator.SetTrigger(m_stand);
            }

            //moving forward
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("MoveFront"))
            {
                turning = turnLeft = turnRight = false;
                transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
                facePlayer();
            }

        }
        if (distance < 9f&&movingFront)
        {
            movingFront = false;
            m_Animator.ResetTrigger(m_moveFront);
            m_Animator.SetTrigger(m_stand);
        }


    }

    public void facePlayer() {
        Vector3 dir = player.transform.position - this.gameObject.transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 2.0f * Time.deltaTime);
    }
}
