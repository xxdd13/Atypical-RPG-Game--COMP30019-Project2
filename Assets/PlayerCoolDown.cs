using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoolDown : MonoBehaviour {
    public float rbCoolDown = 1.5f;
    public float nukeCoolDown = 2f;
    public float guidedSpellCoolDown = 3f;
    public float IceSwordCoolDown = 3f;
    public float BallistaCoolDown = 5f;
    public float CrossCoolDown = 2f;

    public bool rb = true;
    public bool nuke = true;
    public bool guidedSpell = true;
    public bool iceSword = true;
    public bool ballista = true;
    public bool cross = true;

    public float rbCD;
    public float nukeCD;
    public float guidedSpellCD;
    public float iceSwordCD;
    public float ballistaCD;
    public float crossCD;


    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update () {
        if (!rb) {
            rbCD += Time.deltaTime;
            print(rbCD);
        }
        if (!nuke)
        {
            nukeCD += Time.deltaTime;
        }
        if (!guidedSpell)
        {
            guidedSpellCD += Time.deltaTime;
        }
        if (!iceSword)
        {
            iceSwordCD += Time.deltaTime;
        }
        if (!ballista)
        {
            ballistaCD += Time.deltaTime;
        }
        if (!cross)
        {
            crossCD += Time.deltaTime;
        }

        if (rbCD >= rbCoolDown) { rb = true; rbCD = 0f; }
        if (nukeCD >= nukeCoolDown) { nuke = true; nukeCD = 0f;  }
        if (guidedSpellCD >= guidedSpellCoolDown) { guidedSpell = true; guidedSpellCD = 0f; }
        if (iceSwordCD >= IceSwordCoolDown) { iceSword = true; iceSwordCD = 0f; }
        if (ballistaCD >= BallistaCoolDown) { ballista = true; ballistaCD = 0f; }
        if (crossCD >= CrossCoolDown) { cross = true; crossCD = 0f; }

    }
    public void rbCast() {
        rb = false;
    }
    public void nukeCast()
    {
        nuke = false;
    }
    public void guidedSpellCast()
    {
        guidedSpell = false;
    }
    public void iceSwordCast()
    {
        iceSword = false;
    }
    public void ballistaCast()
    {
        ballista = false;
    }
    public void crossCast()
    {
        cross = false;
    }
}
