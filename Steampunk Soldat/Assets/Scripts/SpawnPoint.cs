using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    //this script just forces the z to 0

	// Use this for initialization
	void Start () {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
	}

    public static GameObject FindNearest(Vector3 from)
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        float shortestDistance = float.PositiveInfinity;
        GameObject chosenSpawnPoint = null;
        foreach(GameObject sp in spawnPoints)
        {
            Vector3 spPos = sp.transform.position;
            float distance = Vector3.Distance(from, spPos);
            if(distance < shortestDistance)
            {
                shortestDistance = distance;
                chosenSpawnPoint = sp;
            }
        }
        if(chosenSpawnPoint == null)
        {
            Debug.LogError("No spawn points placed on the map!!!");
        }
        return chosenSpawnPoint;
    }
	
}
