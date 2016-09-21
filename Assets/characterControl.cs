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
	public Vector2 slingStart;
	public Camera scanCam;

	// Use this for initialization
	void Start () {
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
			float dist = squareDist (transform.position, at.transform.position);
			magnitude /= dist;
			if (magnitude > highestMag) {
				highestMag = magnitude;
				highestAttractor = at;
			}
		}
		if (squareDist (transform.position, highestAttractor.transform.position) < (highestAttractor.radius - 0.9f) * (highestAttractor.radius - 1f)) {
			canJump = true;
		}
		Debug.DrawRay (transform.position, (highestAttractor.transform.position - transform.position));
		RaycastHit2D hit = Physics2D.Raycast (transform.position, (highestAttractor.transform.position - transform.position));
		Debug.DrawRay (transform.position, hit.normal);
		Quaternion targetRot = Quaternion.Euler (0f, 0f, 90 + getAngle (hit.normal));
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, Time.deltaTime * 10f);
		if(rb.velocity.magnitude < 5f) {
			if(Input.GetKey(KeyCode.A)) {
				rb.velocity = rb.velocity + speed * (Vector2)transform.TransformDirection (Vector2.right);
			}
			if (Input.GetKey (KeyCode.D)) {
				rb.velocity = rb.velocity - speed * (Vector2)transform.TransformDirection (Vector2.right);
			}
			if (canJump) {
				if(Input.GetKey(KeyCode.Space)) {
					rb.velocity = rb.velocity + 15 * (Vector2)transform.TransformDirection (Vector2.down);
				}
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			mouseHeld = true;
			//slingStart = (Vector2)Camera.main.ScreenToWorldPoint (new Vector2 (Input.mousePosition.x, Input.mousePosition.y)) - (Vector2)Camera.main.transform.position;
			slingStart = (Vector2)scanCam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
			print (slingStart);
		} if (Input.GetMouseButtonUp (0)) {
			mouseHeld = false;
		}
		if (mouseHeld) {

		}
	}

	float squareDist(Vector2 vectA, Vector2 vectB) {
		float sqrDist = (vectA.x - vectB.x) * (vectA.x - vectB.x) + (vectA.y - vectB.y) * (vectA.y - vectB.y);
		return sqrDist;
	}

	float getAngle(Vector2 vect1) {
		float returnAngle = 0f;
		Vector2 relativeVect = vect1;
		//print (relativeVect);
		if (relativeVect.x == 0) {
			if (relativeVect.y > 0f) {
				returnAngle = 90f;
				return returnAngle;
			} else {
				returnAngle = 270f;
				return returnAngle;
			}
		} if (relativeVect.y == 0) {
			if (relativeVect.x > 0f) {
				returnAngle = 0f;
				return returnAngle;
			} else {
				returnAngle = 180f;
				return returnAngle;
			}
		}
		float tan = Mathf.Atan2 (Mathf.Abs(relativeVect.y), Mathf.Abs(relativeVect.x))*Mathf.Rad2Deg;
		if (relativeVect.x < 0f) {
			if (relativeVect.y < 0f) {
				// x -, y-
				returnAngle = 180 + tan;
			} else if (relativeVect.y > 0f) {
				// x -, y+
				returnAngle = 180 - tan;
			}
		} else if (relativeVect.x > 0f) {
			if (relativeVect.y < 0f) {
				// x +, y-
				returnAngle = 360 - tan;
			} else if (relativeVect.y > 0f) {
				// x +, y+
				returnAngle = tan;
			}
		}
		return returnAngle;
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawCube (slingStart, Vector3.one * 2f);
	}
}
