using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script is attached to each player character and allows the player
/// to change their currently selected build option.
/// 
/// The PlayerAsksServerToPlaceConstructionBlock script accesses this 
/// script to determine if the ConstructionBlock is the currently 
/// selected build option.
/// 
/// The ConstructionBlockGuide script accesses this script.
/// </summary>

public class ChangeBuildOption : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	//The names of the block types that will
	//be available to the player.
	
	public enum State
	{
		none,
		
		constructionBlock,
				
		airBlock
	}
	
	//buildOption will store the currently
	//selected build option.
	
	public State buildOption = State.none;
	
	
	//Used for showing the currently selected
	//build option
	
	private Rect buildRect;
	
	private int buildLeft = 460;
	
	private int buildTop = 150;
	
	private int buildWidth = 140;
	
	private int buildHeight = 140;
	
	private int iconWidth = 128;
	
	private int iconHeight = 128;
	
	private int padding = 20;
	
	private GUIStyle buildStyle = new GUIStyle();
	
	public Texture conBlockTex;
	
	
	//Used in cycling through build options for
	//quick selection.
	
	private int selectedbuildOption = 0;
	
	
	//This list is used to store all the build option states 
	//and will be used for quick selection of a build option.
	
	public List<ChangeBuildOption.State> buildList = new List<ChangeBuildOption.State>();
	
	//Variables End___________________________________________________________
	
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{		
			buildStyle.normal.textColor = Color.white;
			
			buildStyle.fontStyle = FontStyle.Bold;
			
			buildStyle.wordWrap = true;
			
			buildStyle.alignment = TextAnchor.MiddleLeft;
			
			
			buildList.Add(State.none);
			
			buildList.Add(State.constructionBlock);			
		}
		
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//The player can quickly change weapons using the Change Weapon button.
			
		if(Input.GetButtonDown("Change Build Option"))
		{	
			selectedbuildOption ++;
			
					
			if(selectedbuildOption ==  buildList.Count)
			{
				selectedbuildOption = 0;	
			}
			
			buildOption =  buildList[selectedbuildOption];
		}
	}
	
	//Contents of the window that will display the currently selected
	//build option.
	
	void selectedBuildOptionWindow (int windowID)
	{
		if(buildOption == ChangeBuildOption.State.constructionBlock)
		{
			GUILayout.Label(conBlockTex, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));	
		}
	}
	
	
	void OnGUI()
	{	
		//Only show the window if selected build option is
		//not none.
		
		if(buildOption != ChangeBuildOption.State.none)
		{	
			//I want the window to be placed below the crosshair and to the left of it.
			
			buildLeft = Screen.width / 2 + padding + buildWidth;
			
			buildTop = Screen.height / 2 - Screen.height / 4;
			
			buildRect = new Rect(Screen.width - buildLeft, Screen.height - buildTop, buildWidth, buildHeight);
			
			buildRect = GUI.Window(10, buildRect, selectedBuildOptionWindow, "Selected Build");
		}	
	}
}
