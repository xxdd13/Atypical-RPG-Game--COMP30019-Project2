using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameHidden : MonoBehaviour {
    private ParticleSystem system;
    private ParticleSystem[] systems;
    public GameObject hurtBox;
    // Use this for initialization
    void Start () {
        system = GetComponent<ParticleSystem>();
        systems = GetComponentsInChildren<ParticleSystem>();
    }


    void Awake(){
        system = GetComponent<ParticleSystem>();
        systems = GetComponentsInChildren<ParticleSystem>();
    }
    // Update is called once per frame
    void Update () {
		
	}
    public void stopFlame() {
        if (system == null || systems == null){ return; }
        system.Stop(true);

        hurtBox.SetActive(false);

        for (byte i = 0; i < systems.Length; i++)
        {
            systems[i].enableEmission = false;
        }
    }

    public void startFlame()
    {
        system.Stop(false);
        system.Play(true);

        hurtBox.SetActive(true);

        for (byte i = 0; i < systems.Length; i++)
        {
            systems[i].enableEmission = true;
        }
    }
}
