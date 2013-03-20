using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {
	
	public Texture crosshairTex;
	private float crosshairDimension = 256;
	private float halfW = 128;

	// Use this for initialization
	void Start () {
		
		if(networkView.isMine == false)
		{
			enabled = false;
		}
	}
	
	void OnGUI()
	{
		if(Screen.lockCursor == true)
		{
			GUI.DrawTexture(new Rect(Screen.width / 2 - halfW, Screen.height /2 - halfW, crosshairDimension, crosshairDimension), crosshairTex);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
