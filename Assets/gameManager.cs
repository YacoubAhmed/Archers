using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {
	public int turn = 1, playerCount = 2, gameType;
	//0 turn based 1 multiplayer so on maybe 2 ink
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void switchTurn() {
		if (turn < playerCount) {
			turn++;
		} else {
			turn = 1;
		}
	}
}
