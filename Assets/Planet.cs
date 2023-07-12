using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass = 1; // La masse n'a pas d'importance pour le moment
    public float initSpeed; // Vitesse initiale
    public Vector3 initDir; // Direction du vecteur vitees initial
    public float rotSpeed = 0; // Vitesse de rotation en deg/sec
    public Vector3 rotDir = new Vector3(0,1,0); // Axe de rotation

    Vector3 velocity;
    GameObject attractor;
        float attractorMass;
    float attractorRadius;
    GameObject cte;
    float G;

    // Start is called before the first frame update
    void Start()
    {
        attractor = GameObject.FindGameObjectWithTag("Attracteur");
        cte = GameObject.FindGameObjectWithTag("Constants");

        attractorMass = attractor.GetComponent<Soleil> ().mass;
        attractorRadius = attractor.transform.localScale.x;
        G = cte.GetComponent<Constants> ().G;

        velocity = initDir.normalized * initSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectToAttractor = attractor.transform.position - transform.position;
        Vector3 dirToAttractor = vectToAttractor.normalized;
        float dstToAttractor = (attractor.transform.position - transform.position).magnitude;
        
        if (dstToAttractor > attractorRadius) {
            Vector3 acceleration = G * attractorMass / Mathf.Pow(dstToAttractor, 2) * dirToAttractor;
            velocity += acceleration * Time.deltaTime;
        }

        transform.Translate(velocity * Time.deltaTime, Space.World);

        if (rotSpeed != 0) {
            transform.Rotate(rotDir.normalized * rotSpeed * Time.deltaTime);
        }
    }
}
