using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to players and causes them
/// to see a hurt effect whenever they lose health.
/// 
/// This script constantly accesses the HealthAndDamage 
/// script on the player to see if their health has reduced.
/// 
/// This script accesses the PlayerStats script to check what
/// the player's max health is.
/// </summary>

public class HurtEffect : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	public Texture hurtEffect;
	
	
	private Transform myTransform;

	private HealthAndDamage HDScript;
	
	
	public float previousHealth;
	
	private float displayTime = 1.5f;
	
	private bool displayHurtEffect = false;
	

	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			myTransform = transform;
			
			Transform trig = myTransform.FindChild("Trigger");
			
			HDScript = trig.GetComponent<HealthAndDamage>();
			
			
			GameObject gameManager = GameObject.Find("GameManager");
			
			PlayerStats script = gameManager.GetComponent<PlayerStats>();
			
			previousHealth = script.maxHealth;
		}
		
		else
		{
			enabled = false;	
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(previousHealth >  HDScript.maxHealth)
		{
			previousHealth = HDScript.maxHealth;	
		}
		
		
		if(HDScript.myHealth < previousHealth)
		{
			previousHealth = HDScript.myHealth;
			
			
			//Checking if displayHurtEffect is false stops the hurt effect
			//from flickering. Flickering could happen when the player is hit
			//by the particle cannon.
			
			if(displayHurtEffect == false)
			{
				displayHurtEffect = true;
				
				StartCoroutine(StopDisplayingEffect());
			}
		}
		
		//Recognise that the players health regenerates and set previous health
		//to the current health.
		
		if(HDScript.myHealth > previousHealth && HDScript.myHealth <= HDScript.maxHealth)
		{
			previousHealth = HDScript.myHealth;	
		}
	}
	
	
	void OnGUI ()
	{
		if(displayHurtEffect == true)
		{
			//The hurt effect is displyed using a DrawTexture and the texture is stretched to fill
			//the screen.
			
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hurtEffect, ScaleMode.StretchToFill);
		}
	}
	
	
	//Let the hurt effect texture display for a bit before
	//setting the displayHurtEffect bool to false.
	
	IEnumerator StopDisplayingEffect() 
	{
		yield return new WaitForSeconds(displayTime);	
						
		displayHurtEffect = false;
	}
}
