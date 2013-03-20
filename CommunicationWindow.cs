using UnityEngine;
using System.Collections;

public class CommunicationWindow : MonoBehaviour {
	
	public string playerName;
	private string messageToSend;
	private string communication;
	private bool showTextBox = false;
	private bool sendMessage = false;
	public bool unlockCursor = false;
	
	private Rect windowRect;
	private int windowLeft = 10;
	private int windowTop;
	private int windowW = 300;
	private int windowHeight = 140;
	private int padding = 20;
	private int textFieldHeight = 30;
	private Vector2 scrollPosition;
	
	private GUIStyle myStyle = new GUIStyle();
	private GameObject spawnManager;
	private SpawnScript spawnScript;
	
	public bool iJoined = true;
	
	// Use this for initialization
	void Awake () {
		Input.eatKeyPressOnTextFieldFocus = false;
		messageToSend = "";
		myStyle.normal.textColor = Color.white;
		myStyle.wordWrap = true;
		
	}
	
	void Start()
	{
		spawnManager = GameObject.Find ("SpawnManager");
		spawnScript = spawnManager.GetComponent<SpawnScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			if(Input.GetButtonDown("Communication") && showTextBox == false)
			{
				showTextBox = true;
			}
			
			if(Input.GetButtonDown("Send Message") && showTextBox == true)
			{
				sendMessage = true;
			}
		}
		if(Network.isClient && iJoined == true && playerName != "")
		{
			networkView.RPC("TellEveryonePlayerJoined", RPCMode.All, playerName);
			
			iJoined = false;
		}
	}
	
	
	void CommLogWindow(int windowID)
	{
		//Scroll view
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowW - padding),
												  GUILayout.Height (windowHeight - padding - 5));
		
		GUILayout.Label (communication, myStyle);
		
		GUILayout.EndScrollView();
	}
	
	void OnGUI()
	{
		if(Network.peerType	!= NetworkPeerType.Disconnected)
		{
			windowTop = Screen.height - windowHeight - textFieldHeight;
			windowRect = new Rect(windowLeft, windowTop, windowW, windowHeight);
			if(spawnScript.onRed == true || spawnScript.onBlue == true || Network.isServer == true)
			{
				windowRect = GUI.Window(5, windowRect, CommLogWindow, "Communication Log");
				
				GUILayout.BeginArea(new Rect(windowLeft, windowTop + windowHeight, windowW, windowHeight));
				if(showTextBox == true)
				{
					unlockCursor = true;
					Screen.lockCursor = false;
					
					GUI.SetNextControlName("MyTextField");
					messageToSend = GUILayout.TextField (messageToSend, GUILayout.Width (windowW));
					
					
					//Focus onto textfield to type
					GUI.FocusControl("MyTextField");
					if(sendMessage == true)
					{
						if(messageToSend != "")
						{
							if(Network.isClient == true)
							{
								networkView.RPC ("SendMessageToEveryone", RPCMode.All, messageToSend, playerName);
							}
							if(Network.isServer == true)
							{
								networkView.RPC ("SendMessageToEveryone", RPCMode.All, messageToSend, "Server");
							}
						}
						
						//HideTextbox give control to player
						sendMessage = false;
						showTextBox = false;
						unlockCursor = false;
						messageToSend = "";
					}
				}
				
				GUILayout.EndArea();
			}
		}
	}

	[RPC]
	void SendMessageToEveryone(string messageToSend, string pName)
	{
		communication = pName + " : " + messageToSend + "\n" + "\n" + communication;
	}
	
	[RPC]
	[RPC]
	void TellEveryonePlayerJoined (string pName)
	{
		communication = pName + " has joined the game." + "\n" + "\n" + communication;	
	}
}

