using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouvement : MonoBehaviour
{
    public GameObject BodyOfReference;

    GameObject[] bodies;
    float[] masses;

    public float distanceToBody = 10;

    void PositionToBodyOfReference() {
        transform.position = BodyOfReference.transform.position + BodyOfReference.transform.up * distanceToBody;
    }

    Vector3 GetBarycenter(GameObject[] bodies, float[] masses) {
        Vector3 barycenter = Vector3.zero;

        float totalMass = 0;
        foreach (float mass in masses) {
            totalMass += mass;
        }

        for (int i=0; i < bodies.Length; i++) {
            barycenter += bodies[i].transform.position * (masses[i] / totalMass);
        }
        return barycenter;
    }

    void Start()
    {
        bodies = GameObject.FindGameObjectsWithTag("Massive");

        masses = new float[bodies.Length];
        for (int i=0; i < bodies.Length; i++) {
            masses[i] = bodies[i].GetComponent<BodyMouvement>().selfMass;
        }

        // Debug.Log("Barycenter : " + GetBarycenter(bodies, masses));

        // transform.position = GetBarycenter(bodies, masses);
    }

    // Update is called once per frame
    void Update()
    {
        // PositionToBodyOfReference();
        transform.position = GetBarycenter(bodies, masses) + (Vector3.up * distanceToBody);
    }
}
