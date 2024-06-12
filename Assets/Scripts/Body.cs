using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    // Initial state of the body
    public float mass = 1;
    public float initSpeed; // Vitesse initiale
    public Vector3 initDirection; // Direction du vecteur vitees initial
    public float rotSpeed; // Vitesse de rotation en deg/sec
    public Vector3 rotDirection = new Vector3(0,0,1); // Axe de rotation

    // Variables of the body
    [HideInInspector] public Vector3 currentAcceleration;
    [HideInInspector] public Vector3 currentVelocity;

    // Visual aspects of the body
    public Color bodyColor = Color.white;
    public float bodySize = 1;
    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public MeshRenderer meshRenderer;
    [HideInInspector] public Material material;

    // Trail parameters
    TrailRenderer trail;
    public float trailSize = .1f;
    public float trailPersistance = 2;
    public Color trailColor = Color.white;
    public bool trailIsOn = true;

    // Initialisation of the orbit
    public bool initCircOrbit = false;
    public GameObject BodyOfReference;
    public float divisionFactor = 1;

    void OnValidate()
    {
        // Updating the size of the body
        transform.localScale = Vector3.one * bodySize;

        // Get the gravityManager object and the simulation speed
        GravityManager gravityManager = GameObject.FindObjectOfType<GravityManager>();
        float simulationSpeed = 1;
        if (gravityManager!= null) {
            simulationSpeed = gravityManager.simulationSpeed;
            
        }

        // We get the trail renderer of the body and we set its parameters
        trail = GetComponent<TrailRenderer>();
        if (trail != null) {
            trail.time = trailPersistance / simulationSpeed;
            trail.material.color = trailColor;
            trail.startWidth = trailSize;
            trail.endWidth = trailSize;
            trail.enabled = trailIsOn;
        }

        // We get the mesh renderer of the body and we set its parameters
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) {
            material = meshRenderer.sharedMaterial;
            material.color = bodyColor;            
        }
    }
}
    