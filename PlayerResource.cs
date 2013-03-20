using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player and it governs their Resource.
/// 
/// This script is accessed by the StatDisplay script which checks the 
/// player's current resource.
/// 
/// This script is accessed by the PlayerAsksServerToPlace... script to
/// reduce the player's resource when they place a block.
/// 
/// This script is accessed by the FireBlockEraser to reduce the player's
/// resource when they remove a block.
/// </summary>

public class PlayerResource : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	public float resource;
	
	public float baseResource = 100;
	
	public float baseRechargeRate = 0.5f;
	
	public float regeneratorModifier = 0;
	
	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			//Access playerStats
			GameObject gameManager = GameObject.Find ("GameManager");
			PlayerStats statScript = gameManager.GetComponent<PlayerStats>();
			baseResource = statScript.maxResource;
			resource = baseResource;
		}
		
		else
		{
			enabled = false;
		}
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		//If the player's resource falls below their
		//base resource then start recharging it.
		
		if(resource < baseResource)
		{
			resource = resource + (baseRechargeRate + regeneratorModifier) * Time.deltaTime;	
		}
		
		
		//Prevent the player's resource from exceeding the baseResource
		//or falling below 0.
		
		if(resource > baseResource)
		{
			resource = baseResource;	
		}
		
		if(resource < 0)
		{
			resource = 0;	
		}
	}
}
