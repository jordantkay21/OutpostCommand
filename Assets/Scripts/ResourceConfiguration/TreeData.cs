using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeData : MonoBehaviour
{
    public int minWoodDrops = 1;       // Minimum wood resources dropped
    public int maxWoodDrops = 5;      // Maximum wood resources dropped
    public float saplingDropChance = 0.25f; // Chance to drop a sapling (25%)
    public int minSaplingDrops = 1;   // Minimum saplings dropped
    public int maxSaplingDrops = 2;   // Maximum saplings dropped
}
