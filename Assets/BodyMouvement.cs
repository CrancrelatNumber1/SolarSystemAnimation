using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMouvement : MonoBehaviour
{
    // Initial state of the body
    public float selfMass = 1;
    public float initSpeed; // Vitesse initiale
    public Vector3 initDir; // Direction du vecteur vitees initial
    public float rotSpeed = 0; // Vitesse de rotation en deg/sec
    public Vector3 rotDir = new Vector3(0,1,0); // Axe de rotation

    // Variables of the body
    Vector3 velocity;

    // Relation to other objects
    GameObject[] bodies;
    GameObject constants;
    float[] masses;

    // Constants of the sim
    public float minAttractionDistance = .1f; // Must be > 0 to not count infinite self attraction in CalculateForces
    float G;

    // Trail parameters
    TrailRenderer trailRenderer;
    public float trailSize = .1f;
    public float trailPersistance = 2;
    public Color trailColor = Color.white;
    public bool trailIsOn = true;

    // Calculates the sum of the forces applied on the body 
    Vector3 CalculateAcceleration(GameObject[] bodies, float[] masses) {

        Vector3[] forces = new Vector3[bodies.Length];
        Vector3 sumForce = Vector3.zero;

        for (int i = 0; i < bodies.Length; i++) {
            GameObject attractor = bodies[i];
            
            if (GetDistanceTo(attractor) > minAttractionDistance && attractor != this.gameObject) {
                Vector3 vectToAttractor = attractor.transform.position - transform.position;
                Vector3 directionToAttractor = vectToAttractor.normalized;
                float distanceToAttractor = vectToAttractor.magnitude;
                forces[i] = (G * masses[i] / distanceToAttractor) * directionToAttractor; 
            }

            foreach (Vector3 force in forces) {
                sumForce += force;
            }
        }
        return sumForce;
    }

    float GetDistanceTo(GameObject body) {
        return (body.transform.position - transform.position).magnitude;
    }

    // Start is called before the first frame update
    void Start()
    {
        bodies = GetBodyArray();
        constants = GetConstants();

        G = constants.GetComponent<Constants> ().G;

        masses = new float[bodies.Length];
        for (int i = 0; i < bodies.Length; i++) {
            masses[i] = bodies[i].GetComponent<BodyMouvement> ().selfMass;
        }
        
        velocity = initDir.normalized * initSpeed;

        trailRenderer = this.gameObject.AddComponent<TrailRenderer> ();
        TrailSettingsUpdate();        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 acceleration = CalculateAcceleration(bodies, masses);
        velocity += acceleration * Time.deltaTime;
        
        transform.Translate(velocity * Time.deltaTime, Space.World);

        if (rotSpeed != 0) {
            transform.Rotate(rotDir.normalized * rotSpeed * Time.deltaTime);
        }

        TrailSettingsUpdate();        
    }

    void TrailSettingsUpdate () {
        trailRenderer.time = trailPersistance;
        trailRenderer.material.color = trailColor;
        trailRenderer.startWidth = trailSize;
        trailRenderer.endWidth = trailSize;
        trailRenderer.enabled = trailIsOn;
    }

    GameObject[] GetBodyArray() {
        return GameObject.FindGameObjectsWithTag("Massive");
    }

    GameObject GetConstants() {
        return GameObject.FindGameObjectWithTag("Constants");
    }
}
