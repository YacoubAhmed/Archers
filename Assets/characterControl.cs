//TODO multiplayer, hidden gamemode with light up arrows, maybe paint gamemode, can use 2d shadows scripts toooooooo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterControl : MonoBehaviour {

	List<attractor> attractors = new List<attractor>();
	Rigidbody2D  rb;
	float speed = 2f;
	bool canJump;
	bool mouseHeld = false;
	Vector2 slingStart;
	Vector2 slingEnd;
	Vector2 dir;
	gameManager gm;
	public GameObject arrow;
	public int player;
	bool canControl = false, buffer = false;

	// Use this for initialization
	void Start () {
		canJump = false;
		gm = GameObject.FindObjectOfType<gameManager> ();
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
		Quaternion targetRot = Quaternion.Euler (0f, 0f, 90 + math.getAngle (hit.normal));
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, Time.deltaTime * 10f);

		if (canControl) {
			if (rb.velocity.magnitude < 5f) {
				if (Input.GetKey (KeyCode.A)) {
					rb.velocity = rb.velocity + speed * (Vector2)transform.TransformDirection (Vector2.right);
				}
				if (Input.GetKey (KeyCode.D)) {
					rb.velocity = rb.velocity - speed * (Vector2)transform.TransformDirection (Vector2.right);
				}
				if (canJump) {
					if (Input.GetKey (KeyCode.Space)) {
						rb.velocity = rb.velocity + 15 * (Vector2)transform.TransformDirection (Vector2.down);
					}
				}
			}
			if (Input.GetMouseButtonDown (0)) {
				mouseHeld = true;
				slingStart = (Vector2)Camera.main.ScreenToWorldPoint (new Vector2 (Input.mousePosition.x, Input.mousePosition.y)) - (Vector2)Camera.main.transform.position;
			}
			if (Input.GetMouseButtonUp (0)) {
				mouseHeld = false;
				gm.switchTurn ();
				slingEnd = (Vector2)Camera.main.ScreenToWorldPoint (new Vector2 (Input.mousePosition.x, Input.mousePosition.y)) - (Vector2)Camera.main.transform.position;
				dir = slingEnd - slingStart;
				float strength = dir.magnitude * 2f;
				if (strength > 50f) {
					strength = 50f;
				}
				dir.Normalize ();
				GameObject instArrow = (GameObject)Instantiate (arrow, (Vector2)transform.position - dir * 1.5f, Quaternion.Euler (0f, 0f, 90f + math.getAngle (-dir)));
				instArrow.GetComponent<Rigidbody2D> ().velocity = -dir * strength;
				instArrow.GetComponent<arrow> ().owner = player;
			}
			if (mouseHeld) {
				//draw arrow here
				slingEnd = (Vector2)Camera.main.ScreenToWorldPoint (new Vector2 (Input.mousePosition.x, Input.mousePosition.y)) - (Vector2)Camera.main.transform.position;
			}
		}
		canControl = gm.turn == player;
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (slingStart, slingEnd);
	}
}
