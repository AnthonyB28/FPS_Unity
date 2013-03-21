using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the players and it determines how the water
/// texture should be drawn across the screen.
/// </summary>

public class WaterEffect : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	//Quick references.
	
	private Transform myTransform;
	
	private GameObject cameraTop;
	
	private GameObject cameraBottom;
		
	
	//Used in drawing the water texture.
	
	private float waterHeight = 14;
	
	public Texture waterTex;
	
	private float proportion;
	
	private bool submerged = false;
	
	
	//Fog settings
	
	private float normalFogStartDistance = 100;
	
	private float normalFogEndDistance = 350;
	
	private float waterFogStartDistance = 70;
	
	private float waterFogEndDistance = 250;
	
	
	//Variables End___________________________________________________________	
	

	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			myTransform = transform;
			
			cameraTop = GameObject.Find("CameraTop");
			
			cameraBottom = GameObject.Find("CameraBottom");
										
			
			//Apply initial fog settings
			
			RenderSettings.fog = true;
			
			RenderSettings.fogMode = FogMode.Linear;
			
			RenderSettings.fogStartDistance = normalFogStartDistance;
				
			RenderSettings.fogEndDistance = normalFogEndDistance;
		}
		
		else
		{
			enabled = false;	
		}
	}
	
	
	void OnGUI()
	{	
		//If the player is standing more than half way in water then we need to start drawing the
		//water effect across the screen.
		
		if(myTransform.position.y < waterHeight)
		{
			//Calculate the proportion of the camera that is not underwater (h1 - h0)/(h1 - h2).
			
			proportion = (cameraTop.transform.position.y - waterHeight)/(cameraTop.transform.position.y - cameraBottom.transform.position.y);			
			
			
			//Proportion > 0 means that the camera is not fully submerged so the water texture is drawn
			//such that it only covers the portion of the screen that is underwater.
			
			if(proportion >= 0)
			{			
				GUI.DrawTexture(new Rect(0, Screen.height * proportion, Screen.width, Screen.height), waterTex, ScaleMode.StretchToFill);	
				
				RenderSettings.fogStartDistance = normalFogStartDistance;
				
				RenderSettings.fogEndDistance = normalFogEndDistance;
			}	
			
			//When proportion < 0 that means the camera is fully submerged and so we can draw the
			//water texture fully across the screen.
			
			if(proportion < 0)
			{	
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), waterTex, ScaleMode.StretchToFill);
				
				RenderSettings.fogStartDistance = waterFogStartDistance;
				
				RenderSettings.fogEndDistance = waterFogEndDistance;
				
				submerged = true;
			}
		}
		
		if(proportion >= 0 && submerged == true)
		{
			RenderSettings.fogStartDistance = normalFogStartDistance;
				
			RenderSettings.fogEndDistance = normalFogEndDistance;
			
			submerged = false;
		}
	}
}
