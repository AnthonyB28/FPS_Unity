using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the GameManager.
/// 
/// This script accesses the SpawnScript to check if 
/// firstSpawn is true.
/// </summary>


public class AssignHealth : MonoBehaviour {
	
	//Variables Start___________________________________
	
	private GameObject[] redTeamPlayers;
	
	private GameObject[] blueTeamPlayers;
	
	private float waitTime = 5;
	
	//Variables End_____________________________________
	
	
	
	void OnConnectedToServer ()
	{
		StartCoroutine(AssignHealthOnJoiningGame());	
	}
	
	
	IEnumerator AssignHealthOnJoiningGame()
	{
		//Don't execute the code till the wait time has
		//elapsed.
		
		yield return new WaitForSeconds (waitTime);
		
			
		//Find the Trigger GameObjects of all players in both teams and
		//place a reference to them in the two arrays.
		
		redTeamPlayers = GameObject.FindGameObjectsWithTag("RedTeamTrigger");
		
		blueTeamPlayers = GameObject.FindGameObjectsWithTag("BlueTeamTrigger");
		
		
		//Assign the buffered previous health value to the player's current health.
		//If we didn't do this then a newcomer to the game would have an incorrect
		//picture of everyone's health as everyone would appear to have health.
		
		foreach(GameObject red in redTeamPlayers)
		{
			HealthAndDamage HDScript = red.GetComponent<HealthAndDamage>();	
			
			HDScript.myHealth = HDScript.previousHealth;
		}
		
		foreach(GameObject blue in blueTeamPlayers)
		{
			HealthAndDamage HDScript  = blue.GetComponent<HealthAndDamage>();	
			
			HDScript.myHealth = HDScript.previousHealth;
		}

		
		//Disable this script as we only needed it once.
		
		enabled = false;
	
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
