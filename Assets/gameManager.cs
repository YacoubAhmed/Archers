using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gameManager : MonoBehaviour {
	public int turn = 1, playerCount = 0, currentPlayers, gameType;
	public int[] selectedButtons, playersArr;
	Vector3 moveToPos;
	public Vector3[] positions;
	public Color[] playerColors;
	public bool paused = false;
	public GameObject planet, player, sheet;
	GameObject currentScreen;
	public RenderTexture rendTex;

	//0 turn based 1 multiplayer so on maybe 2 ink
	//red blue green yellow orange white

	void Start () {
		//StartCoroutine (spawnPlanetsLots (10000));
		//GenerateLevel (2);
		selectedButtons = new int[GameObject.Find ("playerSelect").transform.childCount-1];
		moveToPos = Camera.main.transform.position;
	}

	void Update () {
		Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, moveToPos, Time.deltaTime * 10f);
		if (Input.GetKeyDown (KeyCode.Escape)) {//check if game is running first
			pause ();
		}
	}

	IEnumerator spawnPlanetsLots(int times) {
		for(int i = 0; i < times; i ++) {
			yield return new WaitForEndOfFrame ();
			foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
				Destroy (at.gameObject);
			}
			yield return new WaitForEndOfFrame ();
			GenerateLevel (6);
			//yield return new WaitForEndOfFrame ();
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void switchTurn() {
		turn = turn % playerCount;
		turn++;
		while (playersArr [turn-1] == 0) {
			turn = turn % playerCount;
			turn++;
		}
	}

	public void playerChange(int num) {
		num--;
		if (selectedButtons [num] != 1) {
			selectedButtons [num] = 1;
			playerCount++;
		} else {
			//remove player
			selectedButtons [num] = 0;
			playerCount--;
		}
		int u = 1;
		for (int i = 0; i < selectedButtons.Length; i++) {
			if (selectedButtons [i] == 1) {
				GameObject.Find ("playerSelect").transform.GetChild (i).GetChild (0).GetComponent<Text> ().text = "" + u;
				u++;
			} else {
				GameObject.Find ("playerSelect").transform.GetChild (i).GetChild (0).GetComponent<Text> ().text = "+";
			}
		}
		if (playerCount > 1) {
			GameObject.Find ("playerSelect").transform.GetChild (6).GetChild(0).GetComponent<Text> ().text = "-->";
		} else {
			GameObject.Find ("playerSelect").transform.GetChild (6).GetChild(0).GetComponent<Text> ().text = "X";
		}
	}

	public void startGame() {
		currentScreen = GameObject.Find ("movie0");
		if (playerCount > 1) {
			//spawn players
			currentPlayers = playerCount;
			playersArr = new int[playerCount];
			for (int i = 0; i < playerCount; i++) {
				playersArr [i] = 1;
			}
			StartCoroutine(GenerateLevel(playerCount));
			moveCam(2);
		}
	}

	public void killPlayer(int deadPlayer) {
		playersArr [deadPlayer-1] = 0;
		currentPlayers--;
		if (turn == deadPlayer) {
			switchTurn ();
		}
		if (currentPlayers == 1) {
			//new level
			GameObject.Find ("overlay").transform.GetChild(0).gameObject.GetComponent<moveTo>().location = new Vector3(-105, -10, 10);
		}
	}

	public void nextLevel() {
		Texture2D temp = new Texture2D (400, 300);
		temp.filterMode = FilterMode.Point;
		Camera.main.Render ();
		RenderTexture.active = rendTex;
		temp.ReadPixels (new Rect (0, 0, rendTex.width, rendTex.height), 0, 0);
		temp.Apply ();
		currentScreen.GetComponent<Renderer> ().material.mainTexture = temp;

		GameObject sheetGO = (GameObject)Instantiate (sheet, new Vector3 (-80, 0, 15), Quaternion.Euler(90, 180, 0));

		GameObject.Find ("overlay").transform.GetChild(0).gameObject.GetComponent<moveTo>().location = new Vector3(-105, -20, 10);
		currentPlayers = playerCount;
		for (int i = 0; i < playerCount; i++) {
			playersArr [i] = 1;
		}
		Destroy (GameObject.FindObjectOfType<characterControl> ().gameObject);
		foreach (arrow ar in GameObject.FindObjectsOfType<arrow>()) {
			Destroy (ar);
		}
		turn = 1;
		StartCoroutine(GenerateLevel(playerCount));
	}

	public void pause() {
		paused = !paused;
		foreach (physicsBody pb in GameObject.FindObjectsOfType<physicsBody>()) {
			print ("pause");
			pb.sleep (paused);
		}
		moveCam (paused ? 3 : 2);
	}

	IEnumerator GenerateLevel(int num) {
		bool generated = false;
		int tries = 0;
		while (!generated) {
			tries++;
			if (tries > 4) {
				break;
			}
			yield return new WaitForEndOfFrame ();
			generated = Generation (num);
		}
	}

	bool Generation(int players) {
		float camSize = 15f;
		if (players > 3) {
			camSize = 20f;
		}
		if (players == 6) {
			camSize = 30f;
		}
		GameObject.Find ("scanCam").GetComponent<Camera> ().orthographicSize = camSize;
		foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
			Destroy (at);
		}
		float safeDist = 8f;
		int i = 0;
		int planetNum = Random.Range (players, players + 3);
		int attempts = 0;
		while(i < planetNum) {
			attempts++;
			if (attempts > camSize * 100) {
				foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
					Destroy (at.gameObject);
				}
				print("FAIL");
				return false;
			}
			//get random vector from -20 - 15 to 20 15
			int diameter = Random.Range(3, 6 + 2 * players);
			Vector2 position = math.randomVector2D(-camSize*(4/3) + diameter, camSize*(4/3) - diameter, -camSize + diameter, camSize - diameter);
			bool safeDistance = true;
			foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
				if (Mathf.Sqrt (math.squareDist (position, at.transform.position)) - 0.5*(diameter+at.transform.localScale.x) < safeDist) {
					safeDistance = false;
				}
			}
			if (!safeDistance) {
				continue;
			}
			GameObject planetGO = (GameObject)Instantiate (planet, position, Quaternion.identity);
			planetGO.transform.localScale = Vector3.one * diameter;
			planetGO.name = "planet" + (int)(i+1);
			i ++;
		}

		for(int u = 1; u <= players; u ++) {
			GameObject planet = GameObject.Find ("planet" + u);
			Vector2 pos = (Vector2)planet.transform.position + new Vector2 (0, planet.transform.lossyScale.y + 1f);
			GameObject playerGO = (GameObject)Instantiate (player, pos, Quaternion.identity);
			playerGO.GetComponent<characterControl> ().playerID = u;
		}
		return true;
	}

	public void moveCam(int camPos) {
		moveToPos = positions [camPos];
	}
}
