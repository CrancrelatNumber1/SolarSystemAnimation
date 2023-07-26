using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public float G = 1;
    public bool mergingEnabled = true;
    public float minAttractionDistance = .1f; // Must be > 0 to not count infinite self attraction in CalculateForces
}
