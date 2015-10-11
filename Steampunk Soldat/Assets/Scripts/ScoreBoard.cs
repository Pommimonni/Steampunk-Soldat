using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreBoard : MonoBehaviour {

    List<PlayerScore> allScores;
    public Image bg;
    public GameObject rowBg;
    public GameObject scoreRowPrefab;
    public GameObject scoreBoardUI;

    public List<GameObject> scoreRows;
    bool scoresVisible = false;
    // Use this for initialization
    void Start () {
        scoreRows = new List<GameObject>();
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
                Destroy(go);
            }
            scoreRows = new List<GameObject>();
            float yPos = 1;
            foreach (PlayerScore nScore in allScores)
            {
                GameObject newScoreRow = (GameObject)Instantiate(scoreRowPrefab, new Vector3(0, -(yPos++)*35 - 120, 0), Quaternion.identity);
                scoreRows.Add(newScoreRow);
                newScoreRow.GetComponentInChildren<Text>().text += "Player "+nScore.playerID+": Kills: "+nScore.kills+" Deaths: "+nScore.deaths+"\n";
                newScoreRow.transform.SetParent(scoreBoardUI.transform, false);
            }
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoardUI.SetActive(false);
            scoresVisible = false;
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
