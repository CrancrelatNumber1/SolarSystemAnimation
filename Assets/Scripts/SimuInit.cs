using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimuInit : MonoBehaviour
{
    public GameObject[] bodies;
    public float[] masses;
    public Vector3[] initPositions;
    public Vector3[] initVelocities;
    public Vector3[] initAccelerations;

    void SpawnDebris(int number, float minDistance, float maxDistance) {
        for(int i = 0; i < number; i++) {
            float distance = Random.Range(minDistance, maxDistance);
            float angle = Random.Range(0, 2*Mathf.PI);
            // float x = distance * Mathf.Cos(angle);
            // float y = distance * Mathf.Sin(angle);
            float x = distance;
            float y = 0;
            Vector3 position = new Vector3(x, y, 0);

            // GameObject debris = Instantiate(Resources.Load("Prefabs/Debris") as GameObject, position, Quaternion.identity);

            GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Body debrisScript = debris.AddComponent<Body>();
            debris.tag = "Massive";
            debris.transform.position = position;
            debris.transform.localScale = Vector3.one * .2f;
            debrisScript.mass = 0;
            debrisScript.currentVelocity = Vector3.zero;
            debrisScript.initCircOrbit = true;

            debris.transform.parent = transform;    
        }
    }
}
