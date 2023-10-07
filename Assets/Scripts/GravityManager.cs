using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [SerializeField] float G = 1;
    // [SerializeField] float minAttractionDistance = .1f; // Must be > 0 to not count infinite self attraction in CalculateForces ?
    [SerializeField] bool mergingEnabled = true;
    bool mergingJustOccured = false;

    GameObject[] bodies;
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

            TrailRenderer trail = body.AddComponent<TrailRenderer>();
            Body bodyScript = body.GetComponent<Body> ();
            trail.time = bodyScript.trailPersistance;
            trail.material.color = bodyScript.trailColor;
            trail.startWidth = bodyScript.trailSize;
            trail.endWidth = bodyScript.trailSize;
            trail.enabled = bodyScript.trailIsOn;

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
        if (mergingJustOccured) {
            bodies = GetBodyArray();
            masses = GetMasses(bodies);
            mergingJustOccured = false;
        }

        bodies = GetBodyArray();
        masses = GetMasses(bodies);

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
        if (mergingEnabled) 
        {
        for (int i=0; i < bodies.Length; i++) {
            GameObject body = bodies[i];
            for (int j=i+1; j < bodies.Length; j++) {
                GameObject otherBody = bodies[j];
                if (GetDistance(body, otherBody) < body.transform.localScale.x) {
                    Merge(body, otherBody);
                }
            }
        }
        }
    }

    void Merge(GameObject body1, GameObject body2) {
        Vector3 body1Velocity = body1.GetComponent<Body>().currentVelocity;
        Vector3 body2Velocity = body2.GetComponent<Body>().currentVelocity;

        float body1Mass = body1.GetComponent<Body>().mass;
        float body2Mass = body2.GetComponent<Body>().mass;
        
        float body1Size = body1.transform.localScale.x;
        float body2Size = body2.transform.localScale.x;

        Destroy(body1);
        Destroy(body2);
        
        GameObject mergedObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mergedObject.transform.position = body1.transform.position;
        mergedObject.transform.localScale = Vector3.one * Mathf.Pow(body1Size*body1Size*body1Size + body2Size*body2Size*body2Size, 1f/3f);
        mergedObject.AddComponent<Body>();
        mergedObject.GetComponent<Body>().currentVelocity = body1Mass/(body1Mass + body2Mass)*body1Velocity + body2Mass/(body1Mass + body2Mass)*body2Velocity;
        mergedObject.GetComponent<Body>().mass = body1Mass + body2Mass;
        mergedObject.tag = "Massive";

        mergingJustOccured = true;
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
                    Vector3 vectToAttractor = attractor.transform.position - body.transform.position;
                    Vector3 directionToAttractor = vectToAttractor.normalized;
                    float distanceToAttractor = vectToAttractor.magnitude;
                    acceleration += G * masses[j] / (distanceToAttractor * distanceToAttractor) * directionToAttractor;
                }
                // if (i != j) 
                // {
                //     GameObject attractor = bodies[j];

                //     if (GetDistance(attractor, body) > minAttractionDistance) {
                //         Vector3 vectToAttractor = attractor.transform.position - body.transform.position;
                //         Vector3 directionToAttractor = vectToAttractor.normalized;
                //         float distanceToAttractor = vectToAttractor.magnitude;
                //         acceleration += (G * masses[i] / distanceToAttractor) * directionToAttractor; 
                //     }
                // }
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
            masses[i] = bodies[i].GetComponent<Body>().mass;
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

}
