using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Proj2
{
    public class GoldDragonController : MonoBehaviour
    {

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
        public GameObject bigThunder;
        private Damageable damageable = null;
        readonly int m_death = Animator.StringToHash("death");
        readonly int m_cast = Animator.StringToHash("cast");
        public TurnToPlayer ttp;
        private bool doing = false;

        // Use this for initialization
        void Start()
        {
            m_Animator = GetComponent<Animator>();
            circleDone = false;
            ttp = GetComponent<TurnToPlayer>();
            damageable = GetComponent<Damageable>();
        }

        // Update is called once per frame
        void Update()
        {

            
            AnimatorStateInfo animationState = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (damageable.currentHitPoints <= 0)
            {
                if (!animationState.IsName("ad-die"))
                {
                    m_Animator.SetTrigger(m_death);
                    return;
                }
                else
                {
                    this.gameObject.transform.position += (this.gameObject.transform.up+this.gameObject.transform.forward) * Time.deltaTime;
                    if (this.gameObject.transform.position.y > 5f) {
                        Instantiate(bigThunder, this.gameObject.transform.position+ this.gameObject.transform.up*2f, Quaternion.identity);
                        Destroy(this.gameObject);
                    }
                }
            }

            if (!ttp.turning && !ttp.movingFront && ttp.inRange0 && !doing)
            {

                ttp.attacking = true;
                if (ttp.attacking)
                {

                    m_Animator.SetTrigger(m_cast);
                    this.doing = true;
                    ttp.attacking = true;

                }



            }

            if (!circleDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ad-cast"))
            {

                Vector3 forwardPos = transform.position + transform.forward * 5.0f;
                Vector3 newPos = new Vector3(forwardPos.x, forwardPos.y + 12.0f, forwardPos.z);
                instantiatedMagicCircle = (GameObject)Instantiate(thunderCircle, newPos, thunderCircle.transform.rotation);
                circleDone = true;
            }
            if (!thunderDone && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ad-exec"))
            {
                print("thunder cast");
                Vector3 forwardPos = transform.position + transform.forward * 5.0f;
                Vector3 newPos2 = new Vector3(forwardPos.x, forwardPos.y + 5f, forwardPos.z);
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
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ad-end"))
            {
                thunderDone = false;
                circleDone = false;
                blockerDone = false;
                Destroy(instantiatedTunder);
                Destroy(instantiatedMagicCircle);
                Destroy(instantiatedBlocker);

            }
            else if (animationState.IsName("ad-stand")) {
                doing = false;
                ttp.attacking = false;
            }

        }
    }
}