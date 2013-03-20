using UnityEngine;
using System.Collections;

public class PlayerScore : MonoBehaviour {
	
	public string destroyedEnemyName;
	public bool iDestroyedEnemy = false;
	public int enemiesDestroyedInOneHit;
	public int myScore;

	// Use this for initialization
	void Start () {
		if(networkView.isMine == true)
		{
			//Player spawns and needs to get their PlayerList to retrieve score.
			GameObject gameManager = GameObject.Find ("GameManager");
			PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
			for(int i = 0; i < dataScript.PlayerList.Count; i++)
			{
				if(dataScript.PlayerList[i].networkPlayer == int.Parse(Network.player.ToString ()))
				{
					myScore = dataScript.PlayerList[i].playerScore;
					
					//Player destroyed, deletes all RPCs so tell PlayerDatabase to update itself
					UpdateScoreInPlayerDatabase(myScore);
				}
			}
		}
	else{
			enabled=false;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		//Increment player score when killing enemy
		if(iDestroyedEnemy == true)
		{
			for(int i=0; i < enemiesDestroyedInOneHit; i++)
			{
				myScore++;
				UpdateScoreInPlayerDatabase(myScore);
				UpdateTeamScore();
			}
			enemiesDestroyedInOneHit = 0;
			iDestroyedEnemy = false;
		}
	}
	
	void UpdateScoreInPlayerDatabase(int score)
	{
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		dataScript.scored = true;
		dataScript.playerScore = score;
		
	}
	
	void UpdateTeamScore()
	{
		GameObject spawnManager = GameObject.Find ("SpawnManager");
		{
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			GameObject gameManager = GameObject.Find ("GameManager");
			ScoreTable tableScript = gameManager.GetComponent<ScoreTable>();
			
			if(spawnScript.onBlue == true)
			{
				tableScript.updateBlueScore = true;
				tableScript.enemiesDestroyedInOneHit = enemiesDestroyedInOneHit;
			}
			
			
			if(spawnScript.onRed == true)
			{
				tableScript.updateRedScore = true;
				tableScript.enemiesDestroyedInOneHit = enemiesDestroyedInOneHit;
			}
		}
	}
	
	
}
