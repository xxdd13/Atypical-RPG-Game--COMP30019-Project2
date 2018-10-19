using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaBumpLevel : MonoBehaviour {
    private Renderer renderer;
    private float timer;
    public float low = 0.5f;
    public float high = 1f;

    // Use this for initialization
    void Start () {
        renderer = this.gameObject.GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > 0.1f)
        {
            timer = 0f;
            renderer.material.SetFloat("_BumpScale", Random.Range(low,high));



        }    
    }
}
