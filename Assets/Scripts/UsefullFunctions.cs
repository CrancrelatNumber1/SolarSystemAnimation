using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefullFunctions : MonoBehaviour
{
    // Returns the distance (size of the vector) between two bodies
    public float GetDistance(GameObject body, GameObject otherBody) {
        return (body.transform.position - otherBody.transform.position).magnitude;
    }

    // Return the array of massive objects (as in objects with the massive tag)
    public GameObject[] GetBodyArray() {
        return GameObject.FindGameObjectsWithTag("Massive");
        // return GameObject.FindObjectsByType(Body);
    }

    // Return the array of all the masses of the bodies
    public float[] GetMasses(GameObject[] bodies) {
        float[] masses = new float[bodies.Length];
        for (int i = 0; i < bodies.Length; i++) {
            masses[i] = bodies[i].GetComponent<Body>().mass;
        }
        return masses;
    }

    // Return the array of all the positions of the bodies
    public Vector3[] GetPositions(GameObject[] bodies) {
        Vector3[] positions = new Vector3[bodies.Length];
        for (int i=0; i < bodies.Length; i++) {
            positions[i] = bodies[i].transform.position;
        } 
        return positions;
    }

    // Return the center of mass of a system (an array of bodies and the array of their mass)
    public Vector3 GetBarycenter(Vector3[] bodies, float[] masses) {
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

    // Return the initial velocity needed for body to have a circular orbit around the center of mass of the bodies
    public float CircularSpeed(GameObject body, GameObject[] bodies, float Gconstant) {
        float[] masses = GetMasses(bodies);
        Vector3[] positions = GetPositions(bodies);
        Vector3 barycenter = GetBarycenter(positions, masses);
        float distanceToBarycenter = (body.transform.position - barycenter).magnitude;
        float totalMass = 0;
        foreach (float mass in masses) {
            totalMass += mass;
        }
        float circularSpeed = Mathf.Sqrt(Gconstant * totalMass / distanceToBarycenter);
        return circularSpeed;
    }

    // Initialize the trail renderer for each body
    public void TrailInit(GameObject body) {
        // Initializing a trail renderer for each body
        TrailRenderer trail = body.AddComponent<TrailRenderer>();
        Body bodyScript = body.GetComponent<Body> ();
        trail.time = bodyScript.trailPersistance;
        trail.material.color = bodyScript.trailColor;
        trail.startWidth = bodyScript.trailSize;
        trail.endWidth = bodyScript.trailSize;
        trail.enabled = bodyScript.trailIsOn;
    }
}
