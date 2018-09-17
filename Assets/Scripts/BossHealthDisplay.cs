using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthDisplay : MonoBehaviour
{
    public GameObject player;
    public bool active = true;
    public Gamekit3D.Damageable damageable;
    public Slider healthSlider;
    public GameObject healthSliderObject;

    // Use this for initialization
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        damageable = gameObject.GetComponent<Gamekit3D.Damageable>();

    }

    // Update is called once per frame
    void Update()
    {
        if (active && damageable != null)
        {
            //player is close to this boss
            if (Vector3.Distance(player.transform.position, this.transform.position) < 5f)
            {
                this.healthSliderObject.SetActive(true);
                damageable.healthSlider = this.healthSlider;
                damageable.healthSlider.enabled = true; 
            }
            else
            {
                //hide healbar
                if(damageable.healthSlider!=null&&damageable.healthSlider.enabled==true){
                    this.healthSliderObject.SetActive(false);
                   
                }
                    
                damageable.healthSlider = null;
                print("hide health bar");


            }
        }

    }
    void OnTriggerEnter(Collider other)
    {
        // If the entering collider is the iceDragon...
        this.active = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        this.active = true;
    }
}
