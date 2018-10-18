using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimateLoop : MonoBehaviour
{
    private float timer;
    private Renderer renderer;
    private int index;
    public Texture[] textures;
    public bool increasing;
    public float delay = 0.1f;
    // Use this for initialization
    void Start()
    {
        timer = 0.0f;
        renderer = this.gameObject.GetComponent<Renderer>();
        index = 0;
        increasing = true;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            timer = 0f;
            renderer.material.mainTexture = textures[index];
            if (index >= textures.Length - 1)
            {
                increasing = false; ;
            }
            if (index == 0)
            {
                increasing = true;
            }
            if (increasing)
            {
                index += 1;
            }
            else
            {
                index -= 1;
            }

        }
    }
}
