using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public float G = 1;
    [SerializeField] bool mergingEnabled = true;
    bool mergingJustOccured = false;

    GameObject[] bodies;
    float[] masses;
    Vector3[] accelerations;
    Vector3[] velocities;
    Vector3[] positions;

    UsefullFunctions usefullFunctions;

    // Start is called before the first frame update
    void Start()
    {
        usefullFunctions = gameObject.AddComponent<UsefullFunctions>();
        
        bodies = usefullFunctions.GetBodyArray();
        masses = usefullFunctions.GetMasses(bodies);
        velocities = new Vector3[bodies.Length];
        accelerations = new Vector3[bodies.Length];
        positions = new Vector3[bodies.Length];

        for (int i=0; i < bodies.Length; i++) {

            // Initializing a trail renderer for each body
            usefullFunctions.TrailInit(bodies[i]);
    
            // Initializing the position of each body
            positions[i] = bodies[i].transform.position;

            // Initializing the speed of each body
            Body bodyScript = bodies[i].GetComponent<Body> ();
            if (bodyScript.initCircOrbit) {
                float circSpeed = usefullFunctions.CircularSpeed(bodies[i], bodies, G);
                Vector3[] pos = usefullFunctions.GetPositions(bodies);
                Vector3 barycenter = usefullFunctions.GetBarycenter(pos, masses);
                Vector3 direction = Vector3.Cross((bodies[i].transform.position - barycenter).normalized, Vector3.back);
                velocities[i] = circSpeed * direction;
            }
            else {
                velocities[i] = bodyScript.initSpeed * bodyScript.initDirection;
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        bodies = usefullFunctions.GetBodyArray();
        masses = usefullFunctions.GetMasses(bodies);

        if (mergingJustOccured) {
            for (int i=0; i < bodies.Length; i++) {
                velocities[i] = bodies[i].GetComponent<Body>().currentVelocity;
            }
            mergingJustOccured = false;
        }

        UpdateVelocities();
        UpdatePositions();
        mergeCheck();
    }

    // Calculates the sum of the forces applied on the body 
    void CalculateAccelerations(GameObject[] bodies, float[] masses) {

        accelerations = new Vector3[bodies.Length];

        for (int i = 0; i < bodies.Length; i++) {
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
            }
            accelerations[i] = acceleration;
        }
    }

    void UpdateVelocities() {
        CalculateAccelerations(bodies, masses);
        for (int i=0; i < bodies.Length; i++){
            velocities[i] += accelerations[i] * Time.deltaTime;
            bodies[i].GetComponent<Body>().currentVelocity = velocities[i];
        }
    }

    void UpdatePositions() {
        for (int i=0; i < bodies.Length; i++){
            bodies[i].transform.position += velocities[i] * Time.deltaTime;
        }
    }

    void mergeCheck() {
        if (mergingEnabled) {
            for (int i=0; i < bodies.Length; i++) {
                GameObject body = bodies[i];
                for (int j=i+1; j < bodies.Length; j++) {
                    GameObject otherBody = bodies[j];
                    if (usefullFunctions.GetDistance(body, otherBody) < Mathf.Max(body.transform.localScale.x, otherBody.transform.localScale.x)) {
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
        mergedObject.AddComponent<Body>();
        mergedObject.tag = "Massive";
        mergedObject.transform.position = body1.transform.position;
        mergedObject.transform.localScale = Vector3.one * Mathf.Pow(body1Size*body1Size*body1Size + body2Size*body2Size*body2Size, 1f/3f);
        mergedObject.GetComponent<Body>().mass = body1Mass + body2Mass;
        mergedObject.GetComponent<Body>().currentVelocity = body1Mass/(body1Mass + body2Mass)*body1Velocity + body2Mass/(body1Mass + body2Mass)*body2Velocity;

        mergingJustOccured = true;
    }
}
