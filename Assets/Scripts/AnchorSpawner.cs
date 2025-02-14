using UnityEngine;
using System.Collections.Generic;

public class AnchorSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnItem
    {
        public GameObject prefab; // Object to spawn
        public Vector3 positionOffset; // Offset from the anchor point
    }

    [Header("Anchor Point")]
    public Transform anchorPoint; // The reference point for spawning

    [Header("Objects to Spawn")]
    public List<SpawnItem> spawnItems = new List<SpawnItem>(); // List of objects with offsets

    // Function to spawn all objects at their respective offsets
    public void SpawnObjects()
    {
        if (anchorPoint == null) return;

        foreach (SpawnItem item in spawnItems)
        {
            if (item.prefab != null)
            {
                // Calculate spawn position relative to the anchor point
                Vector3 spawnPosition = anchorPoint.position + item.positionOffset;

                // Instantiate the object at the calculated position
                Instantiate(item.prefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
