using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToPlayer : MonoBehaviour {
    public GameObject player;
    public float moveSpeed = 10f;


    public float angleTolerance = 20.0f;
    public float distanceTolerance = 30.0f;
    public float detectDistance = 20.0f;
    public float stopDistance = 9f;
    public float tooFarDistance = 100f;

    public bool attacking = false;
    public bool inRange0 = false;
    public float targetDistance = 0.0f;

    protected Animator m_Animator;
    readonly int m_stand = Animator.StringToHash("stand");
    readonly int m_turnLeft = Animator.StringToHash("turnLeft");
    readonly int m_turnRight = Animator.StringToHash("turnRight");
    readonly int m_moveFront = Animator.StringToHash("MoveFront");



    public bool turning = false;
    public bool turnLeft = false;
    public bool turnRight = false;
    public bool movingFront = false;

    public float angleDiff;

    private bool acti = false;

    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (attacking)
            return;

        float distance = Vector3.Distance(player.transform.position, this.gameObject.transform.position);
        targetDistance = distance;
        if (!acti)
        {
            if (distance < detectDistance)
            {
                acti = true;
            }
            return;
        }
        else if(distance >= tooFarDistance) {
            acti = false;
            return;
        }

        float angle = angleTolerance;
        
        float anglePlayer = Vector3.Angle(this.transform.forward, player.transform.position - this.transform.position);

        angleDiff = anglePlayer;

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

                if (movingFront&& anglePlayer > angle) {
                    movingFront = false;
                }
            } else if(!movingFront)
            {
                m_Animator.SetTrigger(m_stand);
            }



            //check for distance
   
            
            if (distance > distanceTolerance) {
                m_Animator.SetTrigger(m_moveFront);
                movingFront = true;
                inRange0 = false;
            }
            //moving -> standing
            if (distance < stopDistance)
            {
                movingFront = false;
                m_Animator.ResetTrigger(m_moveFront);
                m_Animator.SetTrigger(m_stand);
                inRange0 = true;
            }

            //moving forward
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("MoveFront"))
            {
                turning = turnLeft = turnRight = false;
                transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
                facePlayer();
            }

        }
        if (distance < stopDistance && movingFront)
        {
            movingFront = false;
            m_Animator.ResetTrigger(m_moveFront);
            m_Animator.SetTrigger(m_stand);
            inRange0 = true;
        }

        


    }

    public void facePlayer() {
        Vector3 dir = player.transform.position - this.gameObject.transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 3.0f * Time.deltaTime);
    }
}
