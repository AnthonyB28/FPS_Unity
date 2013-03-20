using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the RechargeHealthPickup. When a player
/// walks into this object they are given some health and the pickup 
/// dissappears for some duration.
/// 
/// This script accesses the player's HealthAndDamage script to 
/// increase their health.
/// </summary>

public class RechargeHealth : MonoBehaviour {
	
	//Variables Start_________________________________________________________	
	
	private bool taken = false;
	
	public float HealthGain = 50;
	
	public float reSpawnTime = 20;
	
	private float rotateSpeed = 1;
	
	//Variables End___________________________________________________________
	
		
	
	// Update is called once per frame
	void Update () 
	{
		//Make the RechargeHealthPickup constantly rotate.
		
		transform.Rotate(Vector3.up * (rotateSpeed + Time.deltaTime));
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "BlueTeamTrigger" || other.tag == "RedTeamTrigger" && taken == false)
		{			
			HealthAndDamage HDScript = other.GetComponent<HealthAndDamage>();
			
			HDScript.myHealth += HealthGain;
			
			//If health jumps up above max health then set it
			//back to the max health level.
			
			if(HDScript.myHealth > HDScript.maxHealth)
			{
				HDScript.myHealth = HDScript.maxHealth;	
			}
			
			
			//Only the server deactivates and reactivates the pickup. It does this
			//across the network and uses buffered RPCs so players just joining can't
			//use the pickups.
			
			if(Network.isServer)
			{					
				networkView.RPC("DeactivateHealthPickup", RPCMode.AllBuffered);
				
				StartCoroutine(ReSpawn());
			}

		}
		
	}
	
	IEnumerator ReSpawn() 
	{	
		//After a certain duration make the cube visible again
		//and turn its light back on.
		
        yield return new WaitForSeconds(reSpawnTime);
		
		networkView.RPC("ReactivateHealthPickup", RPCMode.AllBuffered);
    }
	
	
	[RPC]
	void DeactivateHealthPickup ()
	{			
		transform.renderer.enabled = false;
		
		transform.light.enabled = false;
		
		taken = true;
	}
	
	
	[RPC]
	void ReactivateHealthPickup ()
	{
		taken = false;
		
		transform.renderer.enabled = true;
		
		transform.light.enabled = true;
	}
}
