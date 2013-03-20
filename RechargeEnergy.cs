using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the EnergyRechargePickup. When a player
/// walks into this object they are given some energy and the pickup 
/// dissappears for some duration.
/// 
/// This script accesses the player's PlayerEnergy script to increase their
/// energy.
/// </summary>


public class RechargeEnergy : MonoBehaviour {
	
	//Variables Start_________________________________________________________	
	
	private bool taken = false;
	
	public float energyGain = 90;
	
	public float reSpawnTime = 15;
	
	private float rotateSpeed = 1;
		
	private Transform otherParent;
	
	//Variables End___________________________________________________________
	
	
	
	// Update is called once per frame
	void Update () 
	{
		//Make the cube constantly rotate.
		
		transform.Rotate(Vector3.up * (rotateSpeed + Time.deltaTime));
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "BlueTeamTrigger" || other.tag == "RedTeamTrigger" && taken == false)
		{
			otherParent = other.transform.parent;
			
			PlayerEnergy energyScript = otherParent.GetComponent<PlayerEnergy>();
			
			energyScript.energy += energyGain;
			
			
			//If energy jumps up above base energy then set it
			//back to the base energy level.
			
			if(energyScript.energy > energyScript.baseEnergy)
			{
				energyScript.energy = energyScript.baseEnergy;	
			}
			
			//Only the server deactivates and reactivates the pickup. It does this
			//across the network and uses buffered RPCs so players just joining can't
			//use the pickups.
			
			if(Network.isServer)
			{					
				networkView.RPC("DeactivateEnergyPickup", RPCMode.AllBuffered);
				
				StartCoroutine(ReSpawn());
			}
		}
	}	
	
	
	IEnumerator ReSpawn() 
	{	
		//After a certain duration make the cube visible again
		//and turn its light back on.
		
        yield return new WaitForSeconds(reSpawnTime);
		
		networkView.RPC("ReactivateEnergyPickup", RPCMode.AllBuffered);
    }
	
	[RPC]
	void DeactivateEnergyPickup ()
	{			
		transform.renderer.enabled = false;
		
		transform.light.enabled = false;
		
		taken = true;
	}
	
	
	[RPC]
	void ReactivateEnergyPickup ()
	{
		taken = false;
		
		transform.renderer.enabled = true;
		
		transform.light.enabled = true;
	}
}
