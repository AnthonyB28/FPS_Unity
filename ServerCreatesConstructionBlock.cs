using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the BlockManager and
/// it is only executed on the server because only the server will
/// ever receive a positive dropSignal from the 
/// PlayerAsksServerToPlaceConstructionBlock script.
/// </summary>


public class ServerCreatesConstructionBlock : MonoBehaviour {
	
	//Variables Start_________________________________________________________	
	
	//The construction block prefab is attached to
	//this in the inspector.
	
	public GameObject constructionBlock;
	
	
	//These variables are set by the PlayerAsksServerToPlaceConstructionBlock
	//script.
	
	public Vector3 dropPosition;
	
	public Quaternion dropRotation;
	
	public bool dropSignal = false;
	
	
	//blockCounter is used to give each block instantiated a unique name.
	
	public int blockCounter = 0;
	
	public string blockName;
	
	private GameObject block;
	
	
	//Variables End___________________________________________________________
	
	
	
	// Update is called once per frame
	void Update () 
	{
		//Only the server may instantiate construction blocks.
		
		if(Network.isServer)
		{
			if(dropSignal == true)
			{	
				//Using blockCounter assign a unique name
				//to each block created.
				
				blockCounter++;
				
				blockName = "Construction Block " + blockCounter.ToString();
				
				
				//Send an RPC to all game instances, and buffer it, with
				//the position and rotation to create the new construction
				//block at.
				
				networkView.RPC("CreateConstructionBlock", RPCMode.AllBuffered, dropPosition, dropRotation, blockName);
				
				
				//Turn the dropSignal off so that the 
				// server doesn't continue to drop blocks 
				//in the same place.
				
				dropSignal = false;
			}
		}	
	}
	
	
	[RPC]
	void CreateConstructionBlock (Vector3 pos, Quaternion rot, string blockNme)
	{
		block = (GameObject) Instantiate(constructionBlock, pos, rot);
		
		//Assign the unique name to the created block
		//across all game instances.
		
		block.name = blockNme;
		
		
		//This bit of code adds the construction cubes created to the GroupConstructionBlock
		//so that the hierarchy window remains tidy.
		
		GameObject constructionCubeGroup = GameObject.Find("GroupConstructionBlock");
		
		block.transform.parent = constructionCubeGroup.transform;
	}
}
