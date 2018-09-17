using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossHealUiSync : MonoBehaviour {

    public Slider slider;
    public Slider bossSlider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.slider.value = bossSlider.value;
	}
}
