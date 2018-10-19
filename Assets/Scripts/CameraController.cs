using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    public Transform lookAt; // Look at

    // Min and Max distance between camera and lookAt Obj
    private float distanceOffset = 4.5f;
    private float distanceOffsetMin = 0.5f;
    private float distanceOffsetMax = 4.5f;

    // Camera Speed for both x & y translation
    private float xCameraSpeed = 10.0f;
    private float yCameraSpeed = 10.0f;

    // Minimum Angles for the camera
    private float yMinAngle = -15f;
    private float yMaxAngle = 45f;
    private float xMinAngle = -360f;
    private float xMaxAngle = 360f;

    // Current Parameters
    private Quaternion rotation;
    private Vector3 position;
    private float x = 0.0f;
    private float y = 0.0f;

    // Detect Camera Collisions (Radius of thin/thick SphereCast)
    private float thinRadius = 0.15f;
    private float thickRadius = 0.3f;
    private LayerMask layerMask;


    void Start() {
        // Initialise the initial angles 
        Vector3 angles = this.transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Since update is used in player controls, late update is used for camera controls
    void LateUpdate() {

        // Check if there's an object attached
        if (lookAt) {

            // Move the camera accordinly
            x += Input.GetAxis("Mouse X") * xCameraSpeed;
            y -= Input.GetAxis("Mouse Y") * yCameraSpeed;
            x = clampAngle(x, xMinAngle, xMaxAngle);
            y = clampAngle(y, yMinAngle, yMaxAngle);

            // check for distance offset
            if (distanceOffset < distanceOffsetMax) {
                distanceOffset = Mathf.Lerp(distanceOffset, distanceOffsetMax, Time.deltaTime * 2f);
            }

            // Update camera rotation and position
            rotation = Quaternion.Euler(y, x, 0);
            transform.rotation = rotation;
            transform.position = rotation * new Vector3(0.0f, 0.0f, -distanceOffset) + lookAt.position;
            checkForCameraCollision();
        }

    }

    public float clampAngle(float angle, float minAngle, float maxAngle) {
        if (angle < -360F) {
            angle += 360F;
        }

        if (angle > 360F) {
            angle -= 360F;
        }

        return Mathf.Clamp(angle, minAngle, maxAngle);

    }


    // Checking for Camera Collision attributed from https://github.com/Datedsandwich/third-person-camera
    void checkForCameraCollision() {
        Vector3 normal, thickNormal;
        Vector3 ray = transform.position - lookAt.position;

        Vector3 collisionPoint = getDoubleSphereCastCollision(transform.position, thinRadius, out normal, true);
        Vector3 collisionPointThick = getDoubleSphereCastCollision(transform.position, thickRadius, out thickNormal, false);
        Vector3 collisionPointRay = getRayCollisionPoint(transform.position);

        Vector3 collisionPointProjectedOnRay = Vector3.Project(collisionPointThick - lookAt.position, ray.normalized) + lookAt.position;
        Vector3 vectorToProject = (collisionPointProjectedOnRay - collisionPointThick).normalized;
        Vector3 collisionPointThickProjectedOnThin = collisionPointProjectedOnRay - vectorToProject * thinRadius;
        float thinToThickDistance = Vector3.Distance(collisionPointThickProjectedOnThin, collisionPointThick);
        float thinToThickDistanceNormal = thinToThickDistance / (thickRadius - thinRadius);

        float collisionDistanceThin = Vector3.Distance(lookAt.position, collisionPoint);
        float collisionDistanceThick = Vector3.Distance(lookAt.position, collisionPointProjectedOnRay);

        float collisionDistance = Mathf.Lerp(collisionDistanceThick, collisionDistanceThin, thinToThickDistanceNormal);

        bool isThickPointIncorrect = transform.InverseTransformDirection(collisionPointThick - lookAt.position).z > 0;
        isThickPointIncorrect = isThickPointIncorrect || (collisionDistanceThin < collisionDistanceThick);
        if (isThickPointIncorrect) {
            collisionDistance = collisionDistanceThin;
        }

        if (collisionDistance < distanceOffset) {
            distanceOffset = collisionDistance;
        } else {
            distanceOffset = Mathf.SmoothStep(distanceOffset, collisionDistance, Time.deltaTime * 100 * Mathf.Max(distanceOffset * 0.1f, 0.1f));
        }

        distanceOffset = Mathf.Clamp(distanceOffset, distanceOffsetMin, distanceOffsetMax);
        transform.position = lookAt.position + ray.normalized * distanceOffset;

        if (Vector3.Distance(lookAt.position, collisionPoint) > Vector3.Distance(lookAt.position, collisionPointRay)) {
            transform.position = collisionPointRay;
        }
    }


    Vector3 getDoubleSphereCastCollision(Vector3 camPos, float rad, out Vector3 norm, bool pushAlongNorm) {

        float rayLength = 1;

        RaycastHit hit;
        Vector3 origin = lookAt.position;
        Vector3 ray = origin - camPos;

        float dot = Vector3.Dot(transform.forward, ray);
        if (dot < 0) {
            ray *= -1;
        }

        // Project the sphere in an opposite direction of the desired character->camera vector to get some space for the real spherecast
        if (Physics.SphereCast(origin, rad, ray.normalized, out hit, rayLength, layerMask)) {
            origin = origin + ray.normalized * hit.distance;
        } else {
            origin += ray.normalized * rayLength;
        }

        // Do final spherecast with offset origin
        ray = origin - camPos;
        if (Physics.SphereCast(origin, rad, -ray.normalized, out hit, ray.magnitude, layerMask)) {
            norm = hit.normal;

            if (pushAlongNorm) {
                return hit.point + hit.normal * rad;
            } else {
                return hit.point;
            }
        } else {
            norm = Vector3.zero;
            return camPos;
        }
    }

    Vector3 getRayCollisionPoint(Vector3 camPos) {
        Vector3 origin = lookAt.position;
        Vector3 ray = camPos - origin;

        RaycastHit hit;
        if (Physics.Raycast(origin, ray.normalized, out hit, ray.magnitude, layerMask)) {
            return hit.point + hit.normal * 0.15f;
        }

        return camPos;

    }

}
