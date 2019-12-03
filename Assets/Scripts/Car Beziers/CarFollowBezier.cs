﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollowBezier : MonoBehaviour
{

    Transform currentRoute;
    float bezLength; //Length of the actual bezier line
    private Vector3 playerPos;
    [HideInInspector] public Rigidbody rb;

    public bool enRoute; //Whether mid path or not

    [Header("Car Speed")]
    [Range(0, 15)]
    public float speed; //Speed at which the player is supposed to move along the bezier.
    [Range(1, 10)]
    public float changePathSpeed; //Keeps the player from jumping rapidly between beziers when one ends.

    [Header("Object Sensors")]
    public float frontSensorOffset;
    public float sensorWidth;
    public float sensorDistance;

    [HideInInspector] public bool canRotate;
    LayerMask threatLayerMask;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {

        if (!enRoute) //If not in a bezier path, look for a new one and follow it
        {
            canRotate = false;
            currentRoute = NearestBezier(transform.position);
            StartCoroutine(FollowRoute(currentRoute));
        }
       
        

    }


    bool SenseObjects() //Raycasts for cars and people in front;
    {
        Vector3 sensorPos = transform.position + (transform.forward * frontSensorOffset);
        Vector3 sensorBounds = new Vector3(sensorWidth, 10, sensorDistance);


        Collider[] potentialThreats = Physics.OverlapBox(sensorPos, sensorBounds, Quaternion.identity);
        foreach (Collider potentialThreat in potentialThreats)
        {
            if (potentialThreat.gameObject != this.gameObject)
            {
                if (potentialThreat.gameObject.layer == LayerMask.NameToLayer("Player") || potentialThreat.gameObject.layer == LayerMask.NameToLayer("Car") || potentialThreat.gameObject.layer == LayerMask.NameToLayer("Civilian"))
                {
                    return true;
                }
            }
        }
        return false;
    }


    IEnumerator FollowRoute(Transform route)
    {
        //Starts Route, creates the points p0 (Start Pos) p1 (Bezier Pos) and p2 (End Pos)
        enRoute = true;

        QuadraticBezier currentBezier = currentRoute.gameObject.GetComponent<QuadraticBezier>();
        Vector3 initialPos = currentBezier.points[0];
        bezLength = currentBezier.bezierLength;

        if (Mathf.Abs((transform.position - initialPos).magnitude) > 2)
        {
            while (Mathf.Abs((transform.position - initialPos).magnitude) > .1f)
            {
                rb.transform.position = Vector3.Lerp(transform.position, initialPos, changePathSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

        }


        float t = 0;
        while (t < 1)
        {
            
            if (!SenseObjects())
            {
                canRotate = true;   

                t += Time.deltaTime / bezLength * speed; //Move along any length of bezier at consistent speed
            }
            else
            {
                canRotate = false;
            }
            playerPos = currentBezier.GetPositionFromCompletionPercentage(t);
            rb.transform.position = playerPos;
            // //Absolute max speed 
            yield return new WaitForEndOfFrame();

        }
        enRoute = false;
        yield return null;
    }

    //Finds a close bezier start position to the current car's location (upon finishing the movement of a full bezier)
    Transform NearestBezier(Vector3 position)
    {
        Collider[] nearColliderCheck = Physics.OverlapSphere(transform.position, 35);
        List<GameObject> beziers = new List<GameObject>();
        foreach (Collider collider in nearColliderCheck)
        {
            if (collider.transform.tag == "Bezier")
            {
                beziers.Add(collider.gameObject);
            }
        }


        GameObject closest = null;
        float distance = 100;
        if (beziers.Count == 0)
        {
            Debug.LogError("No beziers to track to!");
        }

        //Pass 1: Checks for as many bezier start points that are super close to it as it can find, and randomizes which one it decides to go on
        List<GameObject> bestPossibilities = new List<GameObject>();
        foreach (GameObject bezier in beziers)
        {
            Vector3 bezStartPos = bezier.GetComponent<QuadraticBezier>().points[0];
            Vector3 diff = bezStartPos - position;
            float currDistance = diff.magnitude;
            if (currDistance < 4)
            {
                bestPossibilities.Add(bezier);
            }
        }



        if (bestPossibilities.Count > 0) //If two path starts are really close to each other it will add both to the list and randomize between them to pick which route it goes next.
        {
            int chooseNextPath = Random.Range(0, bestPossibilities.Count);
            return bestPossibilities[chooseNextPath].transform;
        }
        // If there aren't extremely close possible routes to follow then...
        //Pass 2: Checks for the nearest bezier script, regardless of how far it is
        else
        {
            foreach (GameObject bezier in beziers)
            {
                Vector3 bezStartPos = bezier.GetComponent<QuadraticBezier>().points[0];
                Vector3 diff = bezStartPos - position;
                float currDistance = diff.magnitude;
                if (currDistance < distance)
                {
                    closest = bezier;
                    distance = currDistance;
                }
            }

            if (closest == null) //If there is literally only one path, then it just goes back to the starting point of the current route.
            {
                return currentRoute;
            }
            else
            {
                return closest.transform;
            }
        }

    }


   
}