using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Proj2
{
    public class BossHealthDisplay : MonoBehaviour
    {
        public GameObject player;
        public bool active = false;
        public Damageable damageable;
        public Slider healthSlider;
        public GameObject bossUI;
        public Text canvasBossText;
        public string bossName;
        public float distance = 30f;

        // Use this for initialization
        void Start()
        {
            if (player == null)
                player = GameObject.FindWithTag("Player");
            damageable = gameObject.GetComponent<Damageable>();

        }

        // Update is called once per frame
        void Update()
        {
            if (damageable != null)
            {
                //player is close to this boss
                if (Vector3.Distance(player.transform.position, this.transform.position) < distance)
                {
                    this.bossUI.SetActive(true);
                    damageable.healthSlider = this.healthSlider;
                    damageable.healthSlider.enabled = true;
                    canvasBossText.text = this.bossName;
                }
                else
                {
                    //hide healbar
                    if (damageable.healthSlider != null && damageable.healthSlider.enabled == true)
                    {
                        this.bossUI.SetActive(false);
                        canvasBossText.text = " ";
                        damageable.healthSlider = null;

                    }





                }
            }

        }

    }
}
