using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipController : MonoBehaviour {

    // The target marker.
    public Transform target_home_1;
    public Transform target_home_2;
    public Transform target_home_3;

    public Transform target_buffer;

    public Transform target_dest_1;
    public Transform target_dest_2;
    public Transform target_dest_3;

    Boolean isTurning = false;
    Boolean isAtDest = false;

    Vector3 currentTargetPos;

    // Speed in units per sec.
    public float speed;
    private Vector3 startPos;
    private Quaternion startRotation;

    Boolean moveToDest = false;
    Boolean moveToHome = false;
    Boolean isPlayerOnBoard = false;

    public Transform battleGround;


    // Use this for initialization
    void Start () {
        startPos = transform.position;
        startRotation = transform.rotation;
        currentTargetPos = target_home_1.position;

	}
	
	// Update is called once per frame
	void Update () {

        transform.RotateAround(battleGround.position, Vector3.up, speed * Time.deltaTime);


        /*
        float step = speed * Time.deltaTime;

        if (Input.GetKeyDown("o")) {
            Debug.Log("Ship moving to Rock");
            moveToDest = true;
            moveToHome = false;
        }else if (Input.GetKeyDown("p")){
            Debug.Log("Ship moving to Castle");
            moveToDest = false;
            moveToHome = true;
        }


        if (!isAtDest && moveToDest) {
            moveShipFromHomeToDest(step);

        }else if(isAtDest && moveToHome){
            moveShipFromDestToHome(step);
        }

        */


	}




    private void OnTriggerStay(Collider other) {

        if(other.gameObject.CompareTag("Player")){
            isPlayerOnBoard = true;
            Debug.Log("PLAYER ON BOARD");
        }

    }



    void moveShipFromDestToHome(float step){
        
        moveToPos(currentTargetPos, step);
        Debug.Log("MOVING BACK HOME");


        if (transform.position == target_buffer.position){
            currentTargetPos = startPos;
        }


        if(currentTargetPos == startPos){
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startRotation, step);

        }

        if (transform.position == startPos) {
            isAtDest = false;
            currentTargetPos = target_home_1.position;
            Debug.Log("ARRIVED HOME");
            arriveAtHome();

        }
        
    }

    void arriveAtHome() {
        moveToDest = false;
        moveToHome = false;

    }

    void arriveAtDest(){
        moveToDest = false;
        moveToHome = false;
        
    }

    void moveShipFromHomeToDest(float step){

        if (isTurning) {
            rotateCurrentObject(step);
        }

     
        moveToPos(currentTargetPos, step);
        
        if(transform.position == target_home_1.position){
            currentTargetPos = target_home_2.position;
            isTurning = true;

        }

        if (transform.position == target_home_2.position) {
            currentTargetPos = target_home_3.position;
            isTurning = true;

        }

        if(transform.position == target_home_3.position) {
            currentTargetPos = target_dest_1.position;
            isTurning = false;

        }

        if (transform.position == target_dest_1.position) {
            currentTargetPos = target_dest_2.position;
            isTurning = true;

        }

        if (transform.position == target_dest_2.position) {
            currentTargetPos = target_dest_3.position;
            isTurning = true;

        }

        if (transform.position == target_dest_3.position) {
            isTurning = false;
            isAtDest = true;
            currentTargetPos = target_buffer.position;
            arriveAtHome();
        }

    }




    private void rotateCurrentObject(float step) {
        float offset = 1.2f;
        transform.Rotate(0, (step * offset), 0);
    }

    void moveToPos(Vector3 position, float step){
        
        if(currentTargetPos == target_home_1.position){
            Debug.Log("MOVE TO TARGET HOME 1");
            
        }else if (currentTargetPos == target_home_2.position){
            Debug.Log("MOVE TO TARGET HOME 2");
            
        }else if(currentTargetPos == target_home_3.position){
            Debug.Log("MOVE TO TARGET HOME 3");

        }else if (currentTargetPos == target_dest_1.position) {
            Debug.Log("MOVE TO TARGET DEST 1");

        }else if (currentTargetPos == target_dest_3.position) {
            Debug.Log("MOVE TO TARGET DEST 2");

        }else if (currentTargetPos == target_dest_3.position) {
            Debug.Log("MOVE TO TARGET DEST 3");

        }

        transform.position = Vector3.MoveTowards(transform.position, position, step);
    }



}
