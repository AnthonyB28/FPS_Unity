using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to each player and it
/// allows them to place Construction Blocks. This
/// script determines the position where a construction
/// block is to be placed and then asks the server to build
/// a block at that location.
/// 
/// This script accesses the ServerCreatesConstructionBlock script
/// in the BlockManager.
/// </summary>

public class PlayerAsksServerToPlaceConstructionBlock : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	//The construction layer in the inspector is 8
	//and the foundationLayer is 9 and I only want 
	//these layers to be recognised when casting the ray.	
	
	private LayerMask constructionLayer = 1 << 8;
	
	private LayerMask foundationLayer = 1 << 9;
	
	
	//Used for setting the virtual grid spacing.
	
	private float gridWidth = 0.6f;
	
	
	//Used to prevent the player from being able to place
	//blocks in each other when aiming at the floor.
	
	private bool addingToConstructionLayer = false;
	
	
	//The range of the cast ray.
	
	private float range = 50f;
	
	
	//The name of the ray cast.
	
	RaycastHit hit;
	
	
	//cached items.
	
	private Transform myTransform;
	
	private Transform myCameraHeadTransform;
	
	private ChangeWeapon changeScript;
	
	
	//The tempBlock is dropped so that construction
	//looks immediate to the player dispite the lag.
	//That way they will be able to do construction 
	//quickly even in a laggy network. The temp
	//block gets rid of itself after a few moments.
	
	public GameObject tempBlock;
	
	public float cost = 5;
	private PlayerResource resourceScript;
	
	
	
	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{	
			myTransform = transform;

			
			//The ray is cast based on the direction the camera head is facing
			//Remember the camera head tilts up and down because of the 
			//MouseLook script attached to it.
			
			myCameraHeadTransform = myTransform.FindChild("CameraHead");
			resourceScript = gameObject.GetComponent<PlayerResource>();
			
			changeScript = myTransform.GetComponent<ChangeWeapon>();
		}
		
		else
		{
			enabled = false;	
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Screen.lockCursor == true && Input.GetButtonDown("Fire Weapon") && resourceScript.resource >= cost
			&& changeScript.selectedWeapon == ChangeWeapon.State.blockEraser)
		{
			//Fire a ray from the camerahead heading straight forward. The ray should only be cast if there
			//if there is an object in layer 8.
			
			if(Physics.Raycast(myCameraHeadTransform.position, myCameraHeadTransform.forward, out hit, range, constructionLayer))
			{
				addingToConstructionLayer = true;
				
				//Only call for a construction block to be placed if we are 1.2 meters away
				//from an existing one. This prevents us from placing construction blocks
				//on our own player. It feels buggy if we let that happen.
				
				if(Vector3.Distance(transform.position, hit.transform.position) > gridWidth * 2)
				{
					//This calculation places a new construction block adjacent to the targeted block
					//and the height of the new construction block depends on whether the player is
					//aiming above or below the midheight of the targeted block. Construction blocks
					//can only be placed at 0.6 intervals in the virtual grid and that includes the y
					//direction.
					
					Vector3 position = hit.transform.position + hit.normal / (1f / gridWidth);
						
					float posY = hit.point.y / gridWidth;
					
					
					//Take into account whether the other block is above or below the player.
					//This will help determine whether the placement of the new construction
					//block should tend upwards or downwards.
					
					Vector3 relativePosition = myTransform.InverseTransformPoint(hit.transform.position);
					
							
					//if the block is below us
						
					if(relativePosition.y < 0)
					{
						posY = Mathf.Round(posY) + 0.5f;
					}
					
					
					//if the block is above us
					
					if(relativePosition.y > 0)
					{
						posY = Mathf.Round(posY) - 0.5f;	
					}
											
					posY = posY * gridWidth;
					
					position = new Vector3(position.x, posY, position.z);
					
					
					//Set the check position above or below the intended placement
					//position depending on the relative position. This is to try and
					//prevent blocks from being placed within one another.
					
					
					Vector3	checkPosAbove = new Vector3(position.x, position.y + .1f, position.z);
			
					Vector3	checkPosBelow = new Vector3(position.x, position.y - .1f, position.z);
					
					if(!Physics.Linecast(myTransform.position, checkPosAbove) &&
						   !Physics.Linecast(myTransform.position, checkPosBelow))
					{
						//If nothing is blocking the linecast then send an RPC to the server 
						//telling it the position the construction block should be placed at and its rotation.	
						
						networkView.RPC("AskServerToDropConstBlock", RPCMode.Server, position, hit.transform.rotation);
						
						
						//Place the temp block so that construction looks instantaneous to 
						//the player despite the lag. 
						
						DropTempBlock(position, hit.transform.rotation);
						resourceScript.resource = resourceScript.resource - cost;
					}
				}
			}
			
			
			
			//The constructionLayer has priority and I want blocks to be added to the
			//other construction cubes rather than the floor first, otherwise when the player
			//fires their ray the foundationLayer will get priority and a cube will be
			//instantiated on the floor. If I didn't use this bool it would also be
			//possible to constantly add blocks to the same point on the floor.
			
			if(addingToConstructionLayer == false)
			{
				if(Physics.Raycast(myCameraHeadTransform.position, myCameraHeadTransform.forward, out hit, range, foundationLayer))
				{
					//Only call for a construction block to be placed if we are 1.2 meters away
					//from an existing one. This prevents us from placing construction blocks
					//on our own player. It feels buggy if we let that happen.
					
					if(Vector3.Distance(transform.position, hit.point) > gridWidth * 2)
					{
						//This caluclation will place a new construction block on the "Floor"
						//and will position it so that it sits within a virtual grid.
													
						Vector3 position = hit.point;
						
						position /= gridWidth;
						
						position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y) + 0.5f, Mathf.Round(position.z));
						
						position *= gridWidth;
						
						
						Vector3	checkPosAbove = new Vector3(position.x, position.y + .1f, position.z);
			
						Vector3	checkPosBelow = new Vector3(position.x, position.y - .1f, position.z);
						
						
						if(!Physics.Linecast(myTransform.position, checkPosAbove) &&
						   !Physics.Linecast(myTransform.position, checkPosBelow))
						{
							//If nothing is blocking the linecast then send an RPC to the server 
							//telling it the position the construction block should be placed at and its rotation.	
							
							networkView.RPC("AskServerToDropConstBlock", RPCMode.Server, position, hit.transform.rotation);
							
							//Place the temp block so that construction looks instantaneous to 
							//the player despite the lag. 
							
							DropTempBlock(position, hit.transform.rotation);
							resourceScript.resource = resourceScript.resource - cost;
						}
					}
				}
			}
		}
		
		addingToConstructionLayer = false;
	}
	
	
	
	
	[RPC]
	void AskServerToDropConstBlock (Vector3 pos, Quaternion rot)
	{	
		//Find the BlockManager gameObject and assign the position and rotation values
		//to its script ServerCreatesConstructionBlock.
		
		GameObject go = GameObject.Find("BlockManager");
		
		ServerCreatesConstructionBlock script = go.GetComponent<ServerCreatesConstructionBlock>();
		
		script.dropPosition = pos;
		
		script.dropRotation = rot;
		
		script.dropSignal = true;
	}
	
	void DropTempBlock (Vector3 pos, Quaternion rot)
	{
		Instantiate(tempBlock, pos, rot);	
	}
}
