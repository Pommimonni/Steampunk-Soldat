using UnityEngine;
using System.Collections;

public class CharacterModelControl : MonoBehaviour {

    public float armDirection;
    public bool localPlay = false;
    public float roll = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        TurnCharacter();
        SetXSpeed(transform.parent.GetComponent<Rigidbody>().velocity.x);
    }

    void TurnCharacter()
    {
        
    }
    public void SetArmDirection(Quaternion q)
    {
        
        Vector3 norm = q * Vector3.forward;
        armDirection = norm.y;
        if (armDirection > 0)
        {
            //float animSet = animControl ;
            //animControl = animSet*animSet;
            armDirection *= armDirection;
            armDirection *= 0.5f;
        }
        this.GetComponent<Animator>().SetFloat("aimDirection", armDirection);
    }
    public void SetArmDirection(float direction)
    {
        this.GetComponent<Animator>().SetFloat("aimDirection", direction);
    }
    public void SetXSpeed(float speed)
    {
        //Debug.Log("x speed: " + speed);
        float a = Mathf.Abs(speed/3);
        this.GetComponent<Animator>().SetFloat("rolling", a);
    }
    public void TriggerShoot()
    {
        this.GetComponent<Animator>().SetTrigger("shooting");
    }
}
