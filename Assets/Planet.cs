using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass = 1;
    public float initSpeed;
    public Vector3 initDir;

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

        velocity = initDir * initSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectToAttractor = attractor.transform.position - transform.position;
        Vector3 dirToAttractor = vectToAttractor.normalized;
        float dstToAttractor = (attractor.transform.position - transform.position).magnitude;

        Vector3 acceleration = G * attractorMass / Mathf.Pow(dstToAttractor, 2) * dirToAttractor;
        
        if (dstToAttractor > attractorRadius) {
            velocity += acceleration * Time.deltaTime;
        }
        
        transform.position += velocity * Time.deltaTime;
    }
}
