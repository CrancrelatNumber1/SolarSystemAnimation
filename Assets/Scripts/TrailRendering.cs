using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendering : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GameObject[] bodies = GetBodyArray();

        foreach (GameObject body in bodies) {
            Body bodyScript = body.GetComponent<Body> ();
            TrailRenderer trail = body.GetComponent<TrailRenderer> ();

            if (trail != null) {
                trail.time = bodyScript.trailPersistance;
                trail.material.color = bodyScript.trailColor;
                trail.startWidth = bodyScript.trailSize;
                trail.endWidth = bodyScript.trailSize;
                trail.enabled = bodyScript.trailIsOn;
            }
            new WaitForSeconds(.5f);
        }

          
    }

    GameObject[] GetBodyArray() {
        return GameObject.FindGameObjectsWithTag("Massive");
    }
}
