using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDragonController : MonoBehaviour {

    public GameObject thunderCircle;
    public GameObject thunder;
    public GameObject blocker;
    protected Animator m_Animator;
    private bool circleDone;
    private bool thunderDone;
    private bool blockerDone;
    private GameObject instantiatedTunder;
    private GameObject instantiatedMagicCircle;
    private GameObject instantiatedBlocker;

    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
        circleDone = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!circleDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ad-loop"))
        {

            Vector3 forwardPos = transform.position + transform.forward * 5.0f;
            Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y+12.0f, forwardPos.z);
            instantiatedMagicCircle = (GameObject)Instantiate(thunderCircle, newPos, thunderCircle.transform.rotation);
            circleDone = true;
        }
        if (!thunderDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ad-exec"))
        {
            print("thunder cast");
            Vector3 forwardPos = transform.position + transform.forward * 5.0f;
            Vector3 newPos2 = new Vector3(forwardPos.x, forwardPos.y+5f, forwardPos.z);
            instantiatedTunder = (GameObject)Instantiate(thunder, newPos2, transform.rotation);
            thunderDone = true;
        }
        if (!blockerDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ad-cast"))
        {

            Vector3 forwardPos = transform.position + transform.forward * 5.0f;
            Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y + 15.0f, forwardPos.z);
            instantiatedBlocker = (GameObject)Instantiate(blocker, newPos, blocker.transform.rotation);
            blockerDone = true;
        }
        if ( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ad-end")) {
            thunderDone = false;
            circleDone = false;
            blockerDone = false;
            Destroy(instantiatedTunder);
            Destroy(instantiatedMagicCircle);
            Destroy(instantiatedBlocker);
        }

    }
}
