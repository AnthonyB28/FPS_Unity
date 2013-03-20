using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player characters and it allows
/// them to "teleport". They aren't acutally teleporting, we are simply 
/// making the players move at a really fast speed for a fraction of a 
/// second and we make them invisible and invulnerable for that duration.
/// This gives a convincing teleprot effect and the player can't accidentally
/// teleport through the floor, walls, etc.
/// 
/// This script accesses the CharacterMotor script to adjust the player's 
/// movement speed.
/// </summary>

public class Blink : MonoBehaviour {
	
	//Variables Start___________________________________
	
	//The inspector slot for the particle effect.
	
	public GameObject blinkEffect;
	
	
	//The speed at which the player will move when teleporting.
	
	private float teleportSpeed = 100;
	
	
	//The duration for which the player will move at the teleport
	//speed.
	
	private float activeTime = 0.07f;
		
	
	//Some quick references.
	
	private Transform myTransform;
	
	private Transform cameraHead;
	
	private Transform playerGraphics;
	
	private GameObject trigger;
	
	private CharacterMotor motorScript;
	
	private FallDamage fallDamageScript;
	
	private ChangeWeapon changeScript;
	
	
	//The start and end positions of the teleport. Used
	//in determining where the teleport particle effects
	//should be instantiated.
	
	private Vector3 startPosition = new Vector3();
	
	private Vector3 endPosition = new Vector3();
	
	private PlayerEnergy energyScript;
	private float energyCost = 40;
	
	//Variables End_____________________________________

	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			myTransform = transform;
			
			motorScript = myTransform.GetComponent<CharacterMotor>();
			
			playerGraphics = myTransform.FindChild("Graphics");
			
			trigger = myTransform.FindChild("Trigger").gameObject;
			
			cameraHead = myTransform.FindChild("CameraHead");
			
			fallDamageScript = myTransform.GetComponent<FallDamage>();
			
			changeScript = gameObject.GetComponent<ChangeWeapon>();
			energyScript = gameObject.GetComponent<PlayerEnergy>();
			
		}
		
		else
		{
			enabled = false;	
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		//Forwards teleporting.
		
		if(Input.GetButtonDown("Blink Forward") && Screen.lockCursor == true
			&& energyScript.energy >= energyCost && changeScript.selectedWeapon != ChangeWeapon.State.blockEraser)
			
		{
			energyScript.energy = energyScript.energy - energyCost;
			//Capture the blick starting position.
			
			startPosition = myTransform.position;
			
			//Don't allow player to take damage while falling
			
			fallDamageScript.cannotTakeFallDamage = true;
			
			//The blink will happen in the direction the cameraHead
			//is facing. Though when the player is on the ground (grounded) in the CharacterMotor
			//script they not travel throught the air and will only move along the ground. They
			//must be in the air in order to move upwards or downwards.
			
			motorScript.movement.velocity = cameraHead.forward * teleportSpeed;
			
			//Make the player invisible and invulnerable for a moment and then stop the teleport
			//action and make them visible and vulnerable again.
			
			DisableAndStartTimer();
			
		}
		
		
		//Backwards teleporting.
		
		if(Input.GetButtonDown("Blink Back") && Screen.lockCursor == true 
			&& energyScript.energy >= energyCost && changeScript.selectedWeapon != ChangeWeapon.State.blockEraser)
		{
			energyScript.energy = energyScript.energy - energyCost;
			//Capture the blick starting position.
			
			startPosition = myTransform.position;
			
			
			//Dont take fall damage
			fallDamageScript.cannotTakeFallDamage = true;
			
			//The blink will happen in the direction opposite to what the cameraHead
			//is facing. Though when the player is on the ground (grounded) in the CharacterMotor
			//script they not travel throught the air and will only move along the ground. They
			//must be in the air in order to move upwards or downwards.
			
			motorScript.movement.velocity = -cameraHead.forward * teleportSpeed;
			
			//Make the player invisible and invulnerable for a moment and then stop the teleport
			//action and make them visible and vulnerable again.
			
			DisableAndStartTimer();
			
		}
	}
	
	
	void DisableAndStartTimer()
	{
		//Disable the player's graphics so that they dissappear for a moment.
		
		playerGraphics.renderer.enabled = false;
		
		
		//Disable the palyer's trigger so that they can't be hit while blinking.
		
		trigger.active = false;
		
		
		//Run a Coroutine that will limit the player's high speed movement to a fraction
		//of a second.
		
		StartCoroutine(RunForAMoment());
		
	}
	
	
	IEnumerator RunForAMoment ()
	{
		yield return new WaitForSeconds(activeTime);
		
		//Enable fall damage again
		fallDamageScript.cannotTakeFallDamage = false;
		
		
		//Access the CharacterMotor script and bring the player to a halt.
		
		motorScript.movement.velocity = myTransform.forward * 0;
		
		
		//Enable the graphics so that the player can be seen again.
		
		playerGraphics.renderer.enabled = true;
		
		
		//Enable the palyer's trigger so that they can be hit.
		
		trigger.active = true;
		
		
		//Capture the end position of blinking.
		
		endPosition = myTransform.position;
		
		
		//Send out an RPC across the network so that everyone sees the blink effect.
		
		networkView.RPC("BlinkEffect", RPCMode.All, startPosition, endPosition);
	}
	
	
	[RPC]
	void BlinkEffect (Vector3 startPos, Vector3 endPos)
	{
		Instantiate(blinkEffect, startPos, Quaternion.identity);
		
		Instantiate(blinkEffect, endPos, Quaternion.identity);
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
