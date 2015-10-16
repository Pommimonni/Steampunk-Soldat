using UnityEngine;
using System.Collections;

public class PointToCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public Animator anim;

    public void SetAnimator(Animator newAn)
    {
        this.anim = newAn;
    }

    // Update is called once per frame
    void Update () {
        Vector3 pPos = transform.parent.position;
        Vector3 mPos = Input.mousePosition;
        //Debug.Log("Mouse: " + mPos);
        Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));
        
        Vector3 shootDirection = (mWorldPos - pPos);
        shootDirection.z = 0; // just to be sure
        shootDirection.Normalize();
        this.transform.LookAt(mWorldPos);

        float animControl = shootDirection.y;
        
        //anim.gameObject.GetComponent<CharacterModelControl>().SetArmDirection(animControl);
    }
}
