using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Proj2
{
    public class PlayerSetBossUIActive : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            print("colider");
            if (other.gameObject.CompareTag("IceDragon"))
            {
                print("colider11111");
                other.gameObject.GetComponent<BossHealthDisplay>().active = true;
            }

        }

    }
}
