using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the RechargeResourcePickup. When a player
/// walks into this object they are given some resource and the pickup 
/// dissappears for some duration.
/// 
/// This script accesses the player's PlayerResource script to increase their
/// resource.
/// </summary>


public class RechargeResource : MonoBehaviour {

	//Variables Start_________________________________________________________	
	
	private bool taken = false;
	
	public float resourceGain = 200;
	
	public float reSpawnTime = 20;
	
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
			
			PlayerResource resourceScript = otherParent.GetComponent<PlayerResource>();
			
			resourceScript.resource += resourceGain;
			
			
			//If the resource jumps up above the base resource then set it
			//back to the base resource level.
			
			if(resourceScript.resource > resourceScript.baseResource)
			{
				resourceScript.resource = resourceScript.baseResource;	
			}
			
			
			//Only the server deactivates and reactivates the pickup. It does this
			//across the network and uses buffered RPCs so players just joining can't
			//use the pickups.
			
			if(Network.isServer)
			{					
				networkView.RPC("DeactivateResourcePickup", RPCMode.AllBuffered);
				
				StartCoroutine(ReSpawn());
			}
		}
	}
	
	
	IEnumerator ReSpawn() 
	{	
		//After a certain duration make the cube visible again
		//and turn its light back on.
		
        yield return new WaitForSeconds(reSpawnTime);
		
		networkView.RPC("ReactivateResourcePickup", RPCMode.AllBuffered);
    }
	
	
	[RPC]
	void DeactivateResourcePickup ()
	{			
		transform.renderer.enabled = false;
		
		transform.light.enabled = false;
		
		taken = true;
	}
	
	
	[RPC]
	void ReactivateResourcePickup ()
	{
		taken = false;
		
		transform.renderer.enabled = true;
		
		transform.light.enabled = true;
	}
}
