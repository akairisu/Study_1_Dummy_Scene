using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject user;
    public GameObject target;
    public bool SPAWN = false; // switch for spawning the target

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SPAWN) {
            SPAWN = false;
            SpawnTarget();
        }
    }

    public void SpawnTarget() {
        // Calculate a random position in front of the user
        Vector3 spawnPosition = user.transform.position + user.transform.forward * 10 + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        spawnPosition.y += 2; // Increase the y-coordinate to make the spawn position higher than the user

        // Instantiate the target at the calculated position
        GameObject newTarget = Instantiate(target, spawnPosition, Quaternion.identity);

        // Calculate the direction from the spawn position to the user
        Vector3 directionToUser = (user.transform.position - spawnPosition).normalized;

        // Apply force to the target in the direction of the user
        newTarget.GetComponent<Rigidbody>().AddForce(directionToUser * Random.Range(100, 250));
    }
}
