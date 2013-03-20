using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to each player character and
/// is responsible for calculating fall damage based on the
/// player's vertical speed.
/// 
/// This script accesses the CharacterMotor script and constantly
/// checks the player's vertical speed.
/// 
/// The HealthAndDamage script on this player is accessed if fall
/// damage needs to be applied.
/// 
/// The Blink script accesses this script to set cannotTakeFallDamage.
/// </summary>


public class FallDamage : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	private Transform myTransform;
	
	private bool takeFallDamage = false;
	
	public bool cannotTakeFallDamage = false;
	
	private float captureVelocity;
	
	private float hurtVelocity = -15;
	
	private float fatalVelocity;
	
	private float damageFactor;
	
	private float damageRatio;
	
	private float damageToApply;
	
	private float tolerance = 14;
	
	private CharacterMotor motorScript;
	
	//Variables End___________________________________________________________	
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			myTransform = transform;
			
			motorScript = myTransform.GetComponent<CharacterMotor>();
		}
		
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Whenever the player blinks don't allow them to take fall damage.
		//This way blinking can be used by a player to save themselves if they
		//are hurtling to the ground.
		
		if(cannotTakeFallDamage == true)
		{
			takeFallDamage = false;	
			
			captureVelocity = 0;
		}
		
		//If the player's y velocity is less than the threshold then
		//fall damage should be applied. Remember that velocity while
		//falling is negative. cannotTakeFallDamage will be true if the
		//the player is blinking.
		
		if(motorScript.movement.velocity.y < hurtVelocity && cannotTakeFallDamage == false)
		{	
			//Keep capturing the velocity that the player reached.
			//This way the greatest fall velocity reached by the player
			//is captured. Remember the signs are the other way around
			//becuase the fall velocity is negative.
			
			if(captureVelocity > motorScript.movement.velocity.y)
			{
				captureVelocity = motorScript.movement.velocity.y;	
			}
			
			takeFallDamage = true;
		}
		
		
		//Only apply fall damage once their fall velocity is greater
		//than their captureVelocity and the takeFallDamage bool is true.
		//This means that they are probably not falling anymore and have 
		//touched the ground. A tolerance is used because the y velocity
		//fluctuates a bit as the player starts to reach terminal velocity.
	
		if(motorScript.movement.velocity.y > captureVelocity + tolerance && takeFallDamage == true)
		{
			//The fatalVelocity is whatever the maxFallSpeed is in the
			//the CharacterMotor script.
			
			fatalVelocity = motorScript.movement.maxFallSpeed;
			
			
			//The damage factor is a number that is the difference of the
			//captureVelocity and the hurtVelocity (the threshold). The 
			//damageFactor will be higher if the player was at a higher speed
			//before their fall came to a halt.
			
			damageFactor = (captureVelocity - hurtVelocity) * -1;
			
			
			//The amount of damage to apply for each meter per second
			//the player is beyond the hurtVelocity.
			
			damageRatio = 110 / (fatalVelocity + hurtVelocity);
			
			
			//Finally the damage that the player should get.
			
			damageToApply = damageFactor * damageRatio;
			
			
			//Access this player's HealthAndDamage script and tell it that it
			//was just attacked and it took fall damage. 
			
			Transform trig = myTransform.FindChild("Trigger");
			
			HealthAndDamage HDScript = trig.GetComponent<HealthAndDamage>();
			
			
			//I've given myAttacker as this player's own name. 
			//The DestroySelfScript requires an attacker
			//name before it will apply any damage.
			
			HDScript.myAttacker = myTransform.name;
			
			HDScript.iWasAttacked = true;
			
			HDScript.takingFallDamage = true;
			
			HDScript.fallDamage = damageToApply;
							
			
			//Set takeFallDamage to false so this part of the 
			//doesn't keep executing and set captureVelocity to 0
			//so that future velocities which may be lower can be
			//captured.
			
			takeFallDamage = false;
			
			captureVelocity = 0;
		}
	}
}
