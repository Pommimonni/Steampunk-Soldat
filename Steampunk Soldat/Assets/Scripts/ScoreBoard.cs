using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreBoard : MonoBehaviour {

    List<PlayerScore> allScores;
    public GameObject scoreBoardUI;

    public List<GameObject> scoreRows; //SET IN EDITOR
    bool scoresVisible = false;
    // Use this for initialization
    void Start () {
        scoreBoardUI.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update () {

        //bg.SetActive(Input.GetKey(KeyCode.Tab));
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoresVisible = true;
            scoreBoardUI.SetActive(true);

            allScores = FindAllScores();
            
            foreach(GameObject go in scoreRows)
            {
                go.SetActive(true);
                go.GetComponentInChildren<Text>().text = "";
            }

            int index = 0;
            foreach (PlayerScore nScore in allScores)
            {
                scoreRows[index].SetActive(true);
                scoreRows[index++].GetComponentInChildren<Text>().text = "Player "+nScore.playerID+ ": \t\tKills: " + nScore.kills+ " \t\tDeaths: " + nScore.deaths + " \t\tPing: " + nScore.playerPing;
            }
            for(int a = index; a < scoreRows.Count; a++)
            {
                scoreRows[a].SetActive(false);
            }
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoardUI.SetActive(false);
            scoresVisible = false;
        }
        if(scoresVisible && !Input.GetKey(KeyCode.Tab))
        {
            scoreBoardUI.SetActive(false);
        }
        
    }

    List<PlayerScore> FindAllScores()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        List<PlayerScore> allScoresFound = new List<PlayerScore>();
        foreach (GameObject player in allPlayers)
        {
            allScoresFound.Add(player.GetComponent<PlayerScore>());
        }
        return allScoresFound;
    }
}
