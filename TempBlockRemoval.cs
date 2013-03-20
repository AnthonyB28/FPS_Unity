using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to all temp blocks and its
/// purpose is to remove this temp block after a bit of
/// time. This is so that the server has had enough
/// time to create the real block across the network. The 
/// temp blocks are used so that the player has a faster
/// and more seamless building experience and so that a player
/// with a slow connection can still catch up to a player with
/// a fast connection when building.
/// </summary>

public class TempBlockRemoval : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	private float expireTime;
	
	private float latency;
	
	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		//Use the players latency to determine how long the temp block
		//should remain.
		
		//latency = Network.GetAveragePing(Network.connections[0]);
		
		latency = Network.GetLastPing(Network.connections[0]);
		
		expireTime = (latency / 100f) * 2.0f + 0.2f;
		
		StartCoroutine(DestroyMySelfAfterSomeTime());
	}

	
	IEnumerator DestroyMySelfAfterSomeTime() 
	{
		yield return new WaitForSeconds(expireTime);	
					
		Destroy(gameObject);
	}
}
