using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimateSingle : MonoBehaviour {
    private float timer;
    private Renderer renderer;
    private int index;
    public Texture[] textures;
    public bool increasing;
    public float playSpeed=0.1f;
    public GameObject blocker;
    // Use this for initialization
    void Start () {
        timer = 0.0f;
        renderer = this.gameObject.GetComponent<Renderer>();
        index = textures.Length-1;

    }
	
	// Update is called once per frame
	void Update () {
        if (index < 0) { return; }
        timer +=Time.deltaTime;
        if (timer > playSpeed) {
            timer = 0f;
            renderer.material.mainTexture = textures[index];
            index -= 1;
            
        }
        //blocker.transform.localScale *= (1.0f + Time.deltaTime*1.5f) ;
    }
}
