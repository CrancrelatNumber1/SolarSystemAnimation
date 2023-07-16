using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    // Initial state of the body
    public float selfMass = 1;
    public float initSpeed; // Vitesse initiale
    public Vector3 initDir; // Direction du vecteur vitees initial
    public float rotSpeed = 0; // Vitesse de rotation en deg/sec
    public Vector3 rotDir = new Vector3(0,1,0); // Axe de rotation

    // Variables of the body
    Vector3 velocity;
    Vector3 sumAcceleration;
    // bool ShouldMerge = false;

    // Relation to other objects
    GameObject[] bodies;
    GameObject constants;
    float[] masses;

    // Constants of the sim
    public float minAttractionDistance = .1f; // Must be > 0 to not count infinite self attraction in CalculateForces
    float G;

    // Trail parameters
    TrailRenderer trail;
    public float trailSize = .1f;
    public float trailPersistance = 2;
    public Color trailColor = Color.white;
    public bool trailIsOn = true;

    // Calculates the sum of the forces applied on the body 
    IEnumerator CalculateAcceleration() {

        bodies = GetBodyArray();
        masses = GetMasses(bodies);

        Vector3[] accelerations = new Vector3[bodies.Length];
        sumAcceleration = Vector3.zero;

        for (int i = 0; i < bodies.Length; i++) {
            GameObject attractor = bodies[i];

            if (GetDistanceTo(attractor) > Mathf.Max(transform.localScale.x, attractor.transform.localScale.x)/2 && attractor != this.gameObject) {
                Vector3 vectToAttractor = attractor.transform.position - transform.position;
                Vector3 directionToAttractor = vectToAttractor.normalized;
                float distanceToAttractor = vectToAttractor.magnitude;
                accelerations[i] = (G * masses[i] / distanceToAttractor) * directionToAttractor; 
            }
            else {
                if (GetDistanceTo(attractor) < Mathf.Max(transform.localScale.x, attractor.transform.localScale.x)/2
                && attractor != this.gameObject) {
                    // ShouldMerge = true;
                    yield return StartCoroutine(Merge(attractor, this.gameObject));
                }
            }

            foreach (Vector3 acceleration in accelerations) {
                sumAcceleration += acceleration;
            }
        }
        yield return null;
    }

    float GetDistanceTo(GameObject body) {
        return (body.transform.position - transform.position).magnitude;
    }

    // Start is called before the first frame update
    void Start()
    {
        constants = GetConstants();
        G = constants.GetComponent<Constants> ().G;

        if (Time.time == 0) {
            velocity = initDir.normalized * initSpeed;
        }

        trail = gameObject.AddComponent<TrailRenderer> ();
        trail.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(CalculateAcceleration());

        velocity += sumAcceleration * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime, Space.World);

        if (rotSpeed != 0) {
            transform.Rotate(rotDir.normalized * rotSpeed * Time.deltaTime);
        }
    }

    GameObject[] GetBodyArray() {
        return GameObject.FindGameObjectsWithTag("Massive");
    }

    float[] GetMasses(GameObject[] bodies) {
        float[] masses = new float[bodies.Length];
        for (int i = 0; i < bodies.Length; i++) {
            masses[i] = bodies[i].GetComponent<Body> ().selfMass;
        }
        return masses;
    }

    GameObject GetConstants() {
        return GameObject.FindGameObjectWithTag("Constants");
    }

    IEnumerator Merge(GameObject body1, GameObject body2) {

        Vector3 body1Velocity = body1.GetComponent<Body>().velocity;
        Vector3 body2Velocity = body2.GetComponent<Body>().velocity;

        float body1Mass = body1.GetComponent<Body>().selfMass;
        float body2Mass = body2.GetComponent<Body>().selfMass;
        
        float body1Size = body1.transform.localScale.x;
        float body2Size = body2.transform.localScale.x;

        Destroy(body1);
        Destroy(body2);
        
        GameObject mergedObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mergedObject.transform.position = GetBarycenter(new Vector3[2] {body1.transform.position,body2.transform.position}, new float[2] {.5f, .5f});
        mergedObject.transform.localScale = Vector3.one * (body1Size + body2Size);
        mergedObject.AddComponent<Body> ();
        mergedObject.GetComponent<Body>().velocity = GetBarycenter(new Vector3[2] {body1Velocity,body2Velocity}, new float[2] {body1Mass, body2Mass});
        mergedObject.GetComponent<Body>().selfMass = body1Mass + body2Mass;
        mergedObject.tag = "Massive";
        // mergedObject.AddComponent<TrailRenderer> ();

        yield return null;
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
