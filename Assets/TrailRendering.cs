using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendering : MonoBehaviour
{
    TrailRenderer trail;

    GameObject[] GetBodyArray() {
        return GameObject.FindGameObjectsWithTag("Massive");
    }

    IEnumerator TrailUpdate() {
        GameObject[] bodies = GetBodyArray();

        foreach (GameObject body in bodies) {
            Body bodyScript = body.GetComponent<Body> ();
            
            if (body.gameObject.GetComponent<TrailRenderer>() == null) {
               trail = body.gameObject.AddComponent<TrailRenderer> ();
            }

            // trail.time = bodyScript.trailPersistance;
            // trail.material.color = bodyScript.trailColor;
            // trail.startWidth = bodyScript.trailSize;
            // trail.endWidth = bodyScript.trailSize;
            // trail.enabled = bodyScript.trailIsOn;
        }

        yield return null;
    }

    void Start()
    {
        GameObject[] bodies = GetBodyArray();

        foreach (GameObject body in bodies) {

            Body bodyScript = body.GetComponent<Body> ();

            if (trail == null) {
            trail = body.gameObject.AddComponent<TrailRenderer> ();
            }

            trail.time = bodyScript.trailPersistance;
            trail.material.color = bodyScript.trailColor;
            trail.startWidth = bodyScript.trailSize;
            trail.endWidth = bodyScript.trailSize;
            trail.enabled = bodyScript.trailIsOn;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // StartCoroutine(TrailUpdate());  
    }
}
