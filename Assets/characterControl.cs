//TODO multiplayer, hidden gamemode with light up arrows, maybe paint gamemode, can use 2d shadows scripts toooooooo
//TODO with different levels let each level branch off of the previous one, then when going to options menu you will be able to pan through all of the previous ones
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//importing unity packages

public class characterControl : MonoBehaviour {

	List<attractor> attractors = new List<attractor>();
	//defining a list of attractors, a list is used here since there can be a different number of attractors on each level, so
	//using an array would be harder to use due to its specific size constraints.
	Rigidbody2D  rb;
	//di
	float speed = 2f, strength, walktime;
	Vector2 slingStart, slingEnd, dir;
	gameManager gm;
	public GameObject arrow;
	public int playerID;
	bool canControl = false, right = true, mouseHeld = false, canJump, walkSprite = false;
	Color playerColor;
	Sprite[] sprites;

	void Start () {
		gm = GameObject.FindObjectOfType<gameManager> ();
		playerColor = gm.playerColors [playerID - 1];
		sprites = (Sprite[])Resources.LoadAll<Sprite> ("sprites");
		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
			sr.material.color = playerColor;
		}
		canJump = false;
		foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
			attractors.Add(at);
		}
		rb = this.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		float highestMag = 0f;
		attractor highestAttractor = attractors[0];
		canJump = false;
		foreach (attractor at in attractors) {
			if(rb.IsTouching(at.gameObject.GetComponent<CircleCollider2D>())) {
				canJump = true;
			}
			float magnitude = 1000f;
			float dist = math.squareDist (transform.position, at.transform.position);
			magnitude /= dist;
			if (magnitude > highestMag) {
				highestMag = magnitude;
				highestAttractor = at;
			}
		}
		if (math.squareDist (transform.position, highestAttractor.transform.position) < (highestAttractor.radius - 0.9f) * (highestAttractor.radius - 1f)) {
			canJump = true;
		}
		Debug.DrawRay (transform.position, (highestAttractor.transform.position - transform.position));
		RaycastHit2D hit = Physics2D.Raycast (transform.position, (highestAttractor.transform.position - transform.position));
		Debug.DrawRay (transform.position, hit.normal);
		Quaternion targetRot = Quaternion.Euler (0f, 0f,math.getAngle (hit.normal) - 90f);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, Time.deltaTime * 10f);
		if (rb.velocity.magnitude > 0.2f) {
			walktime -= Time.deltaTime;
			if(walktime <= 0) {
				if (walkSprite) {
					transform.GetChild (0).GetChild (3).GetComponent<SpriteRenderer> ().sprite = sprites [12];
				} else {
					transform.GetChild (0).GetChild (3).GetComponent<SpriteRenderer> ().sprite = sprites [13];
				}
				walkSprite = !walkSprite;
				walktime = 0.1f;
			}
		} else {
			walktime = 0;
			transform.GetChild (0).GetChild (3).GetComponent<SpriteRenderer> ().sprite = sprites [12];
		}

		if (canControl) {
			if (rb.velocity.magnitude < 5f) {
				if (Input.GetKey (KeyCode.A)) {
					if (!mouseHeld) {
						right = false;
					}
					rb.velocity = rb.velocity - speed * (Vector2)transform.TransformDirection (Vector2.right);
				}
				if (Input.GetKey (KeyCode.D)) {
					if (!mouseHeld) {
						right = true;
					}
					rb.velocity = rb.velocity + speed * (Vector2)transform.TransformDirection (Vector2.right);
				}
				if (canJump) {
					if (Input.GetKey (KeyCode.Space)) {
						rb.velocity = rb.velocity + 50f * (Vector2)transform.TransformDirection (Vector2.down);
					}
				}
			}
			if (Input.GetMouseButtonDown (0)) {
				mouseHeld = true;
				slingStart = transform.position;
				transform.GetChild (0).GetChild (0).GetChild(0).GetComponent<SpriteRenderer> ().sprite = sprites [2];
				transform.GetChild (0).GetChild (1).GetChild(0).GetComponent<SpriteRenderer> ().sprite = sprites [11];
				transform.GetChild (0).GetChild (1).GetChild (1).GetComponent<SpriteRenderer> ().sprite = sprites [6];
				transform.GetChild (0).GetChild (1).GetChild (1).localPosition = new Vector3 (0.925f, 0.125f, 0.1f);
			}
			if (Input.GetMouseButtonUp (0)) {
				mouseHeld = false;
				gm.switchTurn ();
				if (strength > 50f) {
					strength = 50f;
				}
				dir.Normalize ();
				GameObject instArrow = (GameObject)Instantiate (arrow, (Vector2)transform.position - dir * 1.5f, Quaternion.Euler (0f, 0f, 90f + math.getAngle (-dir)));
				instArrow.GetComponent<Rigidbody2D> ().velocity = -dir * strength / 1.5f;
				instArrow.GetComponent<arrow> ().owner = playerID;
				instArrow.GetComponent<SpriteRenderer> ().material.color = playerColor;
				instArrow.GetComponentInChildren<ParticleSystem> ().startColor = playerColor;

				transform.GetChild (0).GetChild (0).GetChild(0).GetComponent<SpriteRenderer> ().sprite = sprites [0];
				transform.GetChild (0).GetChild (1).GetChild(0).GetComponent<SpriteRenderer> ().sprite = sprites [10];
				transform.GetChild (0).GetChild (1).GetChild(1).GetComponent<SpriteRenderer> ().sprite = sprites [5];
				transform.GetChild (0).GetChild (1).GetChild (1).localPosition = new Vector3 (0.3f, -0.75f, 0.1f);
				transform.GetChild (0).GetChild (1).transform.localRotation = Quaternion.identity;
			}
			if (mouseHeld) {
				slingEnd = (Vector2)Camera.main.ScreenToWorldPoint (new Vector2 (Input.mousePosition.x, Input.mousePosition.y)) - (Vector2)Camera.main.transform.position;
				dir = slingEnd - slingStart;
				strength = dir.magnitude * 3f;
				if (strength < 10f) {transform.GetChild (0).GetChild (1).GetChild(1).GetComponent<SpriteRenderer> ().sprite = sprites [6];
				} else if (strength < 25f) {transform.GetChild (0).GetChild (1).GetChild(1).GetComponent<SpriteRenderer> ().sprite = sprites [7];
				} else if (strength < 40f) {transform.GetChild (0).GetChild (1).GetChild(1).GetComponent<SpriteRenderer> ().sprite = sprites [8];
				} else {transform.GetChild (0).GetChild (1).GetChild(1).GetComponent<SpriteRenderer> ().sprite = sprites [9];
				}
				float angle = math.clampAngle(math.clampAngle(math.getAngle (slingStart - slingEnd) + 90f) - transform.rotation.eulerAngles.z);

				if (angle < 180f) {
					right = true;
				} else {
					right = false;
					angle = 360 - angle;
				}
				transform.GetChild (0).GetChild (1).transform.localRotation = Quaternion.Euler (0f, 0f, angle - 90f);
			}
		}
		canControl = gm.turn == playerID;
		if (right) {
			transform.GetChild (0).localRotation = Quaternion.Euler (0f, 0f, 0f);
		} else {
			transform.GetChild (0).localRotation = Quaternion.Euler (0f, 180f, 0f);
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			gm.pause ();
		}
	}
}
