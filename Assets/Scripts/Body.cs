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
    [HideInInspector]
    public Vector3 currentVelocity;
    public Vector3 currentAcceleration;

    // Trail parameters
    public float trailSize = .1f;
    public float trailPersistance = 2;
    public Color trailColor = Color.white;
    public bool trailIsOn = true;

    void Start() {
        gameObject.AddComponent<TrailRenderer>();
    }

}
    