using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [SerializeField] float G = 1;
    [SerializeField] float minAttractionDistance = .1f; // Must be > 0 to not count infinite self attraction in CalculateForces
    [SerializeField] bool mergingEnabled = true;
    bool mergingJustOccured = false;

    GameObject[] bodies;
    Constants constants;
    float[] masses;
    Vector3[] accelerations;
    Vector3[] velocities;
    Vector3[] positions;

    // Start is called before the first frame update
    void Start()
    {
        bodies = GetBodyArray();
        masses = GetMasses(bodies);
        velocities = new Vector3[bodies.Length];
        accelerations = new Vector3[bodies.Length];
        positions = new Vector3[bodies.Length];
        for (int i=0; i < bodies.Length; i++) {
            Body body = bodies[i].GetComponent<Body>();
            positions[i] = bodies[i].transform.position;
            velocities[i] = body.initSpeed * body.initDirection;
            accelerations[i] = Vector3.zero;

            // print ("Positions[" + i + "] = " + positions[i] + " Start Call");
            // print ("Velocities[" + i + "] = " + velocities[i] + " Start Call");
            // print ("accelerations[" + i + "] = " + accelerations[i] + " Start Call");
        }      
    }

    // Update is called once per frame
    void Update()
    {
        // for (int i=0; i < bodies.Length; i++){
        //     print ("Positions[" + i + "] = " + positions[i] + " Update Call");
        //     print ("Velocities[" + i + "] = " + velocities[i] + " Update Call");
        //     print ("accelerations[" + i + "] = " + accelerations[i] + " Update Call");
        // }

        if (mergingJustOccured) {
            bodies = GetBodyArray();
            masses = GetMasses(bodies);
        }

        UpdateVelocities();
        UpdatePositions();
        mergeCheck();
    }

    void UpdateVelocities() {
        CalculateAccelerations(bodies, masses);
        for (int i=0; i < bodies.Length; i++){
            velocities[i] += accelerations[i] * Time.deltaTime;
        }
    }

    void UpdatePositions() {
        for (int i=0; i < bodies.Length; i++){
            bodies[i].transform.position += velocities[i] * Time.deltaTime;
        }
    }

    void mergeCheck() {
        for (int i=0; i < bodies.Length; i++) {
            GameObject body = bodies[i];
            for (int j=i+1; j < bodies.Length; j++) {
                GameObject otherBody = bodies[j];
                if (GetDistance(body, otherBody) < body.transform.localScale.x/2 && mergingEnabled) {
                    Merge(body, otherBody);
                }
            }
        }
    }

    void Merge(GameObject body1, GameObject body2) {
        
        if (mergingEnabled) {
            Vector3 body1Velocity = body1.GetComponent<Body>().currentVelocity;
            Vector3 body2Velocity = body2.GetComponent<Body>().currentVelocity;

            float body1Mass = body1.GetComponent<Body>().mass;
            float body2Mass = body2.GetComponent<Body>().mass;
            
            float body1Size = body1.transform.localScale.x;
            float body2Size = body2.transform.localScale.x;

            Destroy(body1);
            Destroy(body2);
            
            GameObject mergedObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mergedObject.transform.position = 
            GetBarycenter(new Vector3[2] {body1.transform.position,body2.transform.position}, new float[2] {body1Mass, body2Mass});
            mergedObject.transform.localScale = Vector3.one * (body1Size + body2Size);
            mergedObject.AddComponent<Body> ();
            mergedObject.GetComponent<Body>().currentVelocity = GetBarycenter(new Vector3[2] {body1Velocity,body2Velocity}, new float[2] {body1Mass, body2Mass});
            mergedObject.GetComponent<Body>().mass = body1Mass + body2Mass;
            mergedObject.tag = "Massive";

            mergingJustOccured = true;
        }
    }

    // Calculates the sum of the forces applied on the body 
    void CalculateAccelerations(GameObject[] bodies, float[] masses) {

        accelerations = new Vector3[bodies.Length];

        for (int i = 0; i < bodies.Length; i++) 
        {
            GameObject body = bodies[i];
            Vector3 acceleration = Vector3.zero;
            for (int j=0; j < bodies.Length; j++) 
            {
                if (i != j) 
                {
                    GameObject attractor = bodies[j];

                    if (GetDistance(attractor, body) > minAttractionDistance) {
                        Vector3 vectToAttractor = attractor.transform.position - transform.position;
                        Vector3 directionToAttractor = vectToAttractor.normalized;
                        float distanceToAttractor = vectToAttractor.magnitude;
                        acceleration += (G * masses[i] / distanceToAttractor) * directionToAttractor; 
                    }
                }
            }
            accelerations[i] = acceleration;
        }
    }

    float GetDistance(GameObject body, GameObject otherBody) {
        return (body.transform.position - otherBody.transform.position).magnitude;
    }

    GameObject[] GetBodyArray() {
        return GameObject.FindGameObjectsWithTag("Massive");
        // return GameObject.FindObjectsByType(Body);
    }

    float[] GetMasses(GameObject[] bodies) {
        float[] masses = new float[bodies.Length];
        for (int i = 0; i < bodies.Length; i++) {
            masses[i] = bodies[i].GetComponent<Body> ().mass;
        }
        return masses;
    }

    Vector3[] GetPositions(GameObject[] bodies) {
        Vector3[] positions = new Vector3[bodies.Length];
        for (int i=0; i < bodies.Length; i++) {
            positions[i] = bodies[i].transform.position;
        }
        return positions;
    }

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

}
