using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player and it causes the
/// ConstructionBlockGuide GameObject to appear wherever the 
/// player can place a ConstructionBlock.
/// 
/// This script is almost an exact copy of the
/// PlayerAsksToDropConstructionBlock script.
/// </summary>


public class ConstructionBlockGuide : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	private LayerMask constructionLayer = 1 << 8;
	
	private LayerMask foundationLayer = 1 << 9;
	
	
	private float gridWidth = 0.6f;
	
	
	private bool addingToConstructionLayer = false;
	
	
	private float range = 50f;
	
	
	private RaycastHit hit;
		
	
	private Transform myTransform;
	
	private Transform myCameraHeadTransform;
	
	private GameObject constructionGuideBlock;
	
	private ChangeWeapon changeScript;
	
	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{	
			myTransform = transform;
				
			myCameraHeadTransform = myTransform.FindChild("CameraHead");	
			
			constructionGuideBlock = GameObject.Find("ConstructionBlockGuide");
			
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
		if(Screen.lockCursor == true && changeScript.selectedWeapon == ChangeWeapon.State.blockEraser)
		{
			//Fire a ray from the camerahead heading straight forward. The ray should only be cast if there
			//if there is an object in layer 8 (the construction layer I setup).
			
			if(Physics.Raycast(myCameraHeadTransform.position, myCameraHeadTransform.forward, out hit, range, constructionLayer))
			{	
				addingToConstructionLayer = true;
				
				
				//Construction blocks can only be placed if we are 1.2 meters away
				//from an existing one so our Guide block should only appear if we
				//satisfy that condition.
				
				if(Vector3.Distance(transform.position, hit.transform.position) > gridWidth * 2)
				{																		
					Vector3 position = hit.transform.position + hit.normal / (1f / gridWidth);
					
					float posY = hit.point.y / gridWidth;	
					
					
					//Take into account whether the other block is above or below the player.
					//This will help determine whether the position of the guide block
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
					
					
					//If there are no obstructions between the player and the check position
					//then make the guide block visible and place it at the position that
					//that a new construction block can be placed.
					
					if(!Physics.Linecast(myTransform.position, checkPosAbove) &&
					   !Physics.Linecast(myTransform.position, checkPosBelow))
					{
						constructionGuideBlock.renderer.enabled = true;
						
						constructionGuideBlock.transform.position = position;
						
						constructionGuideBlock.transform.rotation = Quaternion.identity;
					}
					
					else
					{
						constructionGuideBlock.renderer.enabled = false;	
					}
				}
				
				else
				{
					constructionGuideBlock.renderer.enabled = false;	
				}
			}
			
			
			//The constructionLayer has priority and I want blocks to be added to the
			//other construction cubes rather than the floor, otherwise when the player
			//fires their ray the foundationLayer will get priority and a cube will be
			//instantiated on the floor. If I didn't use this bool check it would also be
			//possible to constantly add blocks to the same point on the floor.
			
			if(addingToConstructionLayer == false)
			{
				if(Physics.Raycast(myCameraHeadTransform.position, myCameraHeadTransform.forward, out hit, range, foundationLayer))
				{	
					//Construction blocks can only be placed if we are 1.2 meters away
					//from an existing one so our Guide block should only appear if we
					//satisfy that condition.
						
					if(Vector3.Distance(transform.position, hit.point) > gridWidth * 2)
					{
						//This caluclation will place the guide block on the "Floor"
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
							constructionGuideBlock.renderer.enabled = true;
							
							constructionGuideBlock.transform.position = position;
						
							constructionGuideBlock.transform.rotation = Quaternion.identity;
						}
						
						else
						{
							constructionGuideBlock.renderer.enabled = false;	
						}
					}
					
					else
					{
						constructionGuideBlock.renderer.enabled = false;	
					}
				}
				
				else
				{
					constructionGuideBlock.renderer.enabled = false;	
				}
			}
		}
		else
		{
			constructionGuideBlock.renderer.enabled = false;
		}
		addingToConstructionLayer = false;
	}
}
