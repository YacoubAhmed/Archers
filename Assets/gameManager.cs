using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {
	public int turn = 1, playerCount = 2, gameType;
	Vector3 moveToPos;
	public Vector3[] positions;

	//0 turn based 1 multiplayer so on maybe 2 ink

	void Start () {
		moveToPos = Camera.main.transform.position;
	}

	void Update () {
		Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, moveToPos, Time.deltaTime * 20f);
	}

	public void switchTurn() {
		if (turn < playerCount) {
			turn++;
		} else {
			turn = 1;
		}
	}

	public void moveCam(int camPos) {
		moveToPos = positions [camPos];
	}
}
