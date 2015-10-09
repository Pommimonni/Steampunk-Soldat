using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreBoard : MonoBehaviour {

    public Text score;
    List<PlayerScore> allScores;
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {

        //bg.SetActive(Input.GetKey(KeyCode.Tab));
        score.enabled = Input.GetKey(KeyCode.Tab);
        if (Input.GetKey(KeyCode.Tab))
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            allScores = new List<PlayerScore>();
            foreach(GameObject player in allPlayers)
            {
                allScores.Add(player.GetComponent<PlayerScore>());
            }
            score.text = "";
            foreach(PlayerScore nScore in allScores)
            {
                score.text += "Player "+nScore.playerID+": Kills: "+nScore.kills+" Deaths: "+nScore.deaths+"\n";
            }
            score.text += "End of score board text";
        }
    }
}
