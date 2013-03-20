using UnityEngine;
using System.Collections;

/// <summary>
/// Camera script.
/// attaches to the player and causes the camera to continouysly follow the
/// camerahead
/// </summary>

public class CameraScript : MonoBehaviour {
	
	//Declaring variables
	private Camera myCamera;
	
	private Transform cameraHeadTransform;

	// Use this for initialization
	void Start () {
	if(networkView.isMine == true)
	{	
	myCamera = Camera.main;
	cameraHeadTransform = transform.FindChild("CameraHead");
		}
		else
		{enabled = false;}
	}
	
	// Update is called once per frame
	void Update () {
	
		//Make the camera follow the playerhead
		myCamera.transform.position = cameraHeadTransform.position;
		myCamera.transform.rotation = cameraHeadTransform.rotation;
	}
}
