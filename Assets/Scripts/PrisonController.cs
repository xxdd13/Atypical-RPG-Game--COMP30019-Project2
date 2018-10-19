using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonController : MonoBehaviour {
    public GameObject player;
    public GameObject wonText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(player.gameObject.transform.position, this.gameObject.transform.position) < 3.5f)
        {
            this.gameObject.GetComponent<Animator>().Play("prison-open");
            StartCoroutine(ShowWon());
        }
    }

    IEnumerator ShowWon()
    {

        yield return new WaitForSeconds(2);
        wonText.SetActive(true);
    }
}
