using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendering : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] bodies = GetBodyArray();

        Body[] bodyScipts = new Body[bodies.Length];
        foreach (GameObject body in bodies) {
            Body bodyScript = body.GetComponent<Body> ();
            TrailRenderer trail = body.GetComponent<TrailRenderer> ();
            trail.time = bodyScript.trailPersistance;
            trail.material.color = bodyScript.trailColor;
            trail.startWidth = bodyScript.trailSize;
            trail.endWidth = bodyScript.trailSize;
            trail.enabled = bodyScript.trailIsOn;
        }

          
    }

    GameObject[] GetBodyArray() {
        return GameObject.FindGameObjectsWithTag("Massive");
    }
}
