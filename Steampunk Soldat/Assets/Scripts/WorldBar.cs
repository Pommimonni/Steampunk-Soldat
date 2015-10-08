using UnityEngine;
using System.Collections;

public class WorldBar : MonoBehaviour {

    public GameObject parent;
    public float yPos = 0.8f;
    public CombatControl comCon;
	// Use this for initialization
	void Awake () {
        comCon = parent.GetComponent<CombatControl>();
	}
	
	// Update is called once per frame
	void Update () {
        var wantedPos = Camera.main.WorldToScreenPoint(parent.transform.position+new Vector3(0, yPos, 0));
        transform.position = wantedPos;
    }
}
