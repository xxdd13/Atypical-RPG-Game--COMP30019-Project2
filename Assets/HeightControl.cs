using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightControl : MonoBehaviour {

    public float height = 111.83f;
    protected Animator m_Animator;

    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Stand") ||
            m_Animator.GetCurrentAnimatorStateInfo(0).IsName("turnLeft")||
            m_Animator.GetCurrentAnimatorStateInfo(0).IsName("turnRight")||
            m_Animator.GetCurrentAnimatorStateInfo(0).IsName("MoveFront") )
        {
            float diff = this.transform.position.y - height;
            if (Mathf.Abs(diff) > 0.1f) {

                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y- diff*Time.deltaTime*5f, this.transform.position.z);

            }

                

        }
    }
}
