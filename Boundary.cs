using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player and it warns them
/// when they are leaving the level boundary and it will destroy
/// them if they leave the level boundary.
/// 
/// This script accesses the HealthAndDamage script on this player.
/// </summary>


public class Boundary : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	private Transform myTransform;
	
	private GUIStyle warningStyle = new GUIStyle();
	
	
	//The centre of the level.
	//-277.7899 96.55779
	//-242.1718 -390.3434
	private Vector3 levelCentre = new Vector3(-206.5651f,0,-157.9133f);
	
	
	//The player is warned or destroyed when at these distances
	//from the level centre.
	
	private float warningRange = 270;
	
	private float destroyRange = 380;
	
	//Variables End___________________________________________________________

	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{	
			myTransform = transform;
			
			warningStyle.fontSize = 24;
		
			warningStyle.normal.textColor = Color.white;
		
			warningStyle.fontStyle = FontStyle.Bold;
		
			warningStyle.alignment = TextAnchor.MiddleCenter;
		}
		
		else
		{
			enabled = false;
		}

	}
	
	
	void OnGUI () 
	{
		//Keep checking if the player is at a warning distance from
		//the level centre. If they are then display a warning message.
					
		if(Vector3.Distance(myTransform.position, levelCentre) > warningRange)
		{
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
		
			GUI.Box(new Rect(0,0,Screen.width, Screen.height),"Turn Back. You Are Leaving The Level Boundary.", warningStyle);	
		}
		
		//Destroy the player if they leave they get too far from the levelCentre.
		
		if(Vector3.Distance(myTransform.position, levelCentre) > destroyRange)
		{
			Transform trigger = transform.FindChild("Trigger");
			
			HealthAndDamage HDScript = trigger.GetComponent<HealthAndDamage>();	
		
			HDScript.myAttacker = myTransform.name;
			
			HDScript.iWasAttacked = true;
			
			HDScript.enterDestroyBoundary = true;	
		}
	}
}
