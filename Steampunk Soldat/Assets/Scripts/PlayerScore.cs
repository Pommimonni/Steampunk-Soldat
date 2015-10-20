using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerScore : NetworkBehaviour {

    [SyncVar (hook = "Killed")]
    public int kills = 0;
    [SyncVar]
    public int deaths = 0;
    [SyncVar (hook = "NameChanged")]
    public short playerID = -1;
    [SyncVar]
    public float playerPing = -2;

    static short IDCount = 1; //which id to give to next player

    public NetworkPlayer[] connectionsHere;
    
    float sendTime;

    public int winningKillAmount = 20;

	// Use this for initialization
	void Start () {
        LocalData.gameEnded = false;
        if (isLocalPlayer)
        {
            Debug.Log("Will start invoking ping refresh");
            InvokeRepeating("RefreshPing", 1f, 5f);
        }
        if (isServer)
            SetPlayerID(); //the local player will get an ID from server that is synced to all
        
    }

    void NameChanged(short newNameID)
    {
        this.playerID = newNameID;
        if (isLocalPlayer)
        {
            NameSetter.setName = "Player " + newNameID;
        }
    }

    void RefreshPing() //called only on local player
    {
        //Debug.Log("Refreshing pin");
        if (!isServer)
        {
            StartBouncingPing();
        } else
        {
            playerPing = 0;
        }
    }

    void StartBouncingPing()
    {
        sendTime = Time.time;
        CmdServerBounce();
    }

    [Command]
    void CmdServerBounce()
    {
        RpcBounceBack();
    }

    [ClientRpc]
    void RpcBounceBack()
    {
        if (isLocalPlayer)
        {
            float ping = (Time.time - sendTime)*1000;
            ping = Mathf.Floor(ping);
            CmdUpdatePing(ping);
        }
    }

    void Killed(int newKills)
    {
        kills = newKills;
        if (isServer)
        {
            if (newKills >= winningKillAmount)
            {
                RpcGameOver();
                Invoke("RestartLevel", 4f);
            }
        }
        
    }

    [ClientRpc]
    void RpcGameOver()
    {
        LocalData.gameEnded = true;
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("loaded level playscor " + level);
        LocalData.gameEnded = false;
    }

    void RestartLevel()
    {
        NetworkManager.singleton.ServerChangeScene("Map1");
    }
    
    [Server]
    void SetPlayerID()
    {
        if(GetComponent<EnemyAI>() != null)
        {
            playerID = 41;
            return;
        }
        playerID = IDCount++;
        Debug.Log("Server will now set new player to id " + playerID);
        
    }
	
	// Update is called once per frame
	void Update () {
	}

    [Command]
    void CmdUpdatePing(float newPing)
    {
        //Debug.Log("Server Received new ping for player "+playerID+ ": " + newPing);
        playerPing = newPing;
    }

}
