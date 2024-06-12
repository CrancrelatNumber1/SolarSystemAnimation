using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouvement : MonoBehaviour
{
    public GameObject BodyOfReference;
    public bool isOnBoard;
    public bool OnBodyOfReference = false;
    public float mvtSmoothness = 10;
    public float distanceToBody = 10;
    GameObject[] bodies;
    float[] masses;

    // void PositionToBodyOfReference() {
    //     transform.position = BodyOfReference.transform.position + BodyOfReference.transform.up * distanceToBody;
    // }

    Vector3 GetBarycenter(Vector3[] bodies, float[] masses) {
        Vector3 barycenter = Vector3.zero;

        float totalMass = 0;
        foreach (float mass in masses) {
            totalMass += mass;
        }

        for (int i=0; i < bodies.Length; i++) {
            barycenter += bodies[i] * (masses[i] / totalMass);
        }
        return barycenter;
    }

    void OnValidate() 
    {
        if (OnBodyOfReference) { PositionToBodyOfReference();}
        else { MoveToBarycenter(); }
    }

    // void Start()
    // {
    //     if (OnBodyOfReference) { PositionToBodyOfReference();}
    //     else { MoveToBarycenter();}
    // }

    // Update is called once per frame
    void Update()
    {
        if (OnBodyOfReference) { PositionToBodyOfReference();}
        
        else { MoveToBarycenter(); } 
    }

    void MoveToBarycenter() {
        bodies = GameObject.FindGameObjectsWithTag("Massive");

        masses = new float[bodies.Length];
        for (int i=0; i < bodies.Length; i++) {
            masses[i] = bodies[i].GetComponent<Body>().mass;
        }

        // PositionToBodyOfReference();

        Vector3[] bodiesPosition = new Vector3[bodies.Length];
        for (int i=0; i < bodies.Length; i++) {
            bodiesPosition[i] = bodies[i].transform.position;
        }
        Vector3 barycenter = GetBarycenter(bodiesPosition, masses) + (Vector3.back * distanceToBody);
        float newX = Mathf.Lerp(transform.position.x, barycenter.x, Time.deltaTime/mvtSmoothness);
        float newY = Mathf.Lerp(transform.position.y, barycenter.y, Time.deltaTime/mvtSmoothness);
        float newZ = Mathf.Lerp(transform.position.z, barycenter.z, Time.deltaTime/mvtSmoothness);
        transform.position = new Vector3 (newX, newY, newZ);
    }

    void PositionToBodyOfReference() {
        transform.position = BodyOfReference.transform.position + Vector3.back * distanceToBody;
    }
}
