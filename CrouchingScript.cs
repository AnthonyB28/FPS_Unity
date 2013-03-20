using UnityEngine;
using System.Collections;

public class CrouchingScript : MonoBehaviour {
	
	private CharacterMotor motorScript;
	private float crounchScale = 0.2f;
	private float standingScale = 0.4f;
	private float crouchSpeed = 2;
	private float normalSpeed = 5;
	
	private float crouchBaseHeight = 0.5f;
	private float normalBaseHeight = 1;
	private float crouchExtraHeight = 0.5f;
	private float normalExtraHeight = 1;
	
	public bool croundEngaged = true;
	
	// Use this for initialization
	void Start () {
	
		if(networkView.isMine == true)
		{
			
			//Access playerStats
			GameObject gameManager = GameObject.Find ("GameManager");
			PlayerStats statScript = gameManager.GetComponent<PlayerStats>();
			normalSpeed = statScript.maxSpeed;
			
			//Set the player's normal speed on spawn
			motorScript = gameObject.GetComponent<CharacterMotor>();
			motorScript.movement.maxForwardSpeed = normalSpeed;
			motorScript.movement.maxBackwardsSpeed = normalSpeed;
			motorScript.movement.maxSidewaysSpeed = normalSpeed;
			
			//Set player's normal jump height
			motorScript.jumping.baseHeight = normalBaseHeight;
			motorScript.jumping.extraHeight = normalExtraHeight;
		}
		else{
			enabled = false;}
	}
	
	// Update is called once per frame
	void Update () {
	if(Input.GetButtonDown("Crouch") && croundEngaged == false && Screen.lockCursor == true)
		{
			croundEngaged = true;
			//Camera shifts down so that it remains in player
			GameObject mainCam = GameObject.Find ("Main Camera");
			Vector3 cameraPos = new Vector3(transform.position.x,mainCam.transform.position.y - 0.14f, transform.position.z);
			mainCam.transform.position = cameraPos;
			transform.localScale = new Vector3(transform.lossyScale.x, crounchScale, transform.lossyScale.z);
			
			//Change character's motorscript and change speeds while crouched
			motorScript.movement.maxForwardSpeed = crouchSpeed;
			motorScript.movement.maxBackwardsSpeed = crouchSpeed;
			motorScript.movement.maxSidewaysSpeed = crouchSpeed;
			motorScript.jumping.baseHeight = crouchBaseHeight;
			motorScript.jumping.extraHeight = crouchExtraHeight;
		}
	//Disengage crouch
		if(Input.GetButtonUp("Crouch") && croundEngaged == true && Screen.lockCursor == true)
		{
			croundEngaged = false;
			//Boost the player up so that they can't fall through the floor
			transform.position = new Vector3(transform.position.x, transform.position.y + crounchScale, transform.position.z);
			transform.localScale = new Vector3(transform.lossyScale.x, standingScale, transform.lossyScale.z);
			motorScript.movement.maxForwardSpeed = normalSpeed;
			motorScript.movement.maxBackwardsSpeed = normalSpeed;
			motorScript.movement.maxSidewaysSpeed = normalSpeed;
			
			//Set player's normal jump height
			motorScript.jumping.baseHeight = normalBaseHeight;
			motorScript.jumping.extraHeight = normalExtraHeight;
		}
	}
}
