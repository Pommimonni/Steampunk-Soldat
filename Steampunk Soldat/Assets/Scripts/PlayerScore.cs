using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerScore : NetworkBehaviour {

    [SyncVar]
    public int kills = 0;
    [SyncVar]
    public int deaths = 0;
    [SyncVar]
    public short playerID = -1;

    static short IDCount = 1; //which id to give to next player

	// Use this for initialization
	void Start () {
        if (!isServer)
            return;
        SetPlayerID(); //the local player will get an ID from server that is synced to all
    }
    
    [Server]
    void SetPlayerID()
    {
        playerID = IDCount++;
        Debug.Log("Server will now set new player to id " + playerID);
        
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
