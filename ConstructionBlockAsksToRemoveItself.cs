using UnityEngine;
using System.Collections;

/// This script is attached to each construction block and
/// is used to tell the ServerRemovesConstructionBlock script
/// (attached to the BlockManager) this block's name and that
/// it is to be destroyed.
/// 
/// This script accesses the ServerRemovesConstructionBlock script
/// to signal if this block is to be removed.
/// 
/// This script is accessed by the FireBlockEraser script when it has
/// been struck by the BlockEraser.

public class ConstructionBlockAsksToRemoveItself : MonoBehaviour {
	
	//Variables Start_________________________________________________________
		
	public bool iAmHit = false;
	
	//Variables End___________________________________________________________
	

	
	// Update is called once per frame
	void Update () 
	{
		if(Network.isServer)
		{		
			if(iAmHit == true)
			{
				DeleteBox(gameObject.name);
			}
			
			iAmHit = false;
		}
	}
	
	
	//This function supplies the ServerRemovesConstructionBlock script
	//with the unique name of this construction block and a positive
	//signal that it is to be destroyed.
	
	void DeleteBox(string blockNme)
	{
		GameObject go = GameObject.Find("BlockManager");
		
		ServerRemovesConstructionBlock script = go.GetComponent<ServerRemovesConstructionBlock>();
		
		script.destroyedConstructionBlocks.Add(blockNme);
		
		script.destroySignal = true;
	}
}
