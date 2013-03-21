using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to players and is used when the player 
/// is under water. It governs their movement behaviour while in water.
/// 
/// This script accesses the player's CharaterMotor script to change 
/// speed and gravity values.
/// 
/// This script accesses the PlayerStats script to obtain the player's
/// normal movement speed.
/// </summary>


public class WaterBehaviour : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	private Transform myTransform;
	
	private Transform cameraHead;
	
	private CharacterMotor motorScript;
	
	
	public bool submerged = false;

	private float waterHeight = 14;
		
	
	private float swimSpeed = 2;
	
	private float normalSpeed;
		
	
	private float normalGravity = 10;
	
	private float waterGravity = 0.1f;
	
	
	private float normalMaxFallSpeed = 20;
	
	private float waterMaxFallSpeed = 2;

	//Variables End___________________________________________________________	

	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{			
			
			myTransform = transform;

			cameraHead = myTransform.FindChild("CameraHead");
			
			motorScript = myTransform.GetComponent<CharacterMotor>();

			
			//Access the PlayerStats script and get the player's max speed.
			//This is their normal movement speed outside of the water.
			
			GameObject gManager = GameObject.Find("GameManager");
			
			PlayerStats statScript = gManager.GetComponent<PlayerStats>();
			
			normalSpeed = statScript.maxSpeed;
		}
		
		else
		{
			enabled = false;	
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
	
		if(submerged == false)
		{	
			//If the player enters the water then change the 
			//motorScript properties.
			
			if(myTransform.position.y <= waterHeight)
			{
				submerged = true;
				
				motorScript.movement.gravity = waterGravity;
		
				motorScript.movement.maxFallSpeed = waterMaxFallSpeed;	
				
				motorScript.movement.maxForwardSpeed = swimSpeed;
			
				motorScript.movement.maxBackwardsSpeed = swimSpeed;
			
				motorScript.movement.maxSidewaysSpeed = swimSpeed;
			}
		
		}
		
		
		
		if(submerged == true)
		{		
			//Set grounded to false. This will allow the player to swim upwards
			//without having to jump off the lake bed.
			
			motorScript.grounded = false;
			
			
			//When the player's camera level is above the water line
			//disengage underwater behaviour.
			
			if(myTransform.position.y > waterHeight)
			{							
				submerged = false;
							
				motorScript.movement.gravity = normalGravity;
		
				motorScript.movement.maxFallSpeed = normalMaxFallSpeed;
								
				motorScript.movement.maxForwardSpeed = normalSpeed;
			
				motorScript.movement.maxBackwardsSpeed = normalSpeed;
			
				motorScript.movement.maxSidewaysSpeed = normalSpeed;
			}
			
						
			//If the player presses the forward or backward key they will move
			//in the direction of the CameraHead (reverse for backward key). This
			//means that the player can simply look up, press the forward key, and 
			//they will swim up to the water surface.
			
			if(Screen.lockCursor == true && Input.GetAxis("Vertical")!= 0)
			{	
				//This if statement isn't executed if the player's velocity is greater
				//than normalSpeed otherwise it will be possible for the player to continue accelerating.
				
				if(motorScript.movement.velocity.magnitude <= normalSpeed)
				{	
					//Note that the velocity is being added to. This makes it feel like that the player
					//has to exert some effort to change direction.
					
					motorScript.movement.velocity += cameraHead.forward * Input.GetAxis("Vertical") * normalSpeed * Time.deltaTime;
				}
			}
		}
	}
}
