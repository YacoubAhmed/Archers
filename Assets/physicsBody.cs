using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class physicsBody : MonoBehaviour {

	Rigidbody2D rb;
	List<attractor> attractors = new List<attractor>();
	Vector3 storedVel;
	public bool sleeping;

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody2D>();
		foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
			attractors.Add(at);
		}
	}
	 
	// Update is called once per frame
	void Update () {
		if (!sleeping) {
			foreach (attractor at in attractors) {
				Vector2 dir = at.transform.position - transform.position;
				dir = dir.normalized;
				float magnitude = 6.674f;
				float dist = math.squareDist (transform.position, at.transform.position);
				magnitude *= rb.mass;
				magnitude *= at.mass;
				magnitude /= dist;
				rb.AddForce (dir * magnitude);
			}
		}
	}
	public void sleep(bool toSleep) {
		sleeping = toSleep;
		if (toSleep) {
			if (!rb.isKinematic) {
				storedVel = rb.velocity;
				rb.velocity = Vector3.zero;
				rb.Sleep ();
			}
			if (gameObject.tag == "projectile") {
				gameObject.transform.GetChild (0).GetComponent<ParticleSystem> ().Pause ();
			}
		} else {
			if (!rb.isKinematic) {
				rb.WakeUp ();
				rb.velocity = storedVel;
			}
			if (gameObject.tag == "projectile") {
				gameObject.transform.GetChild (0).GetComponent<ParticleSystem> ().Play ();
			}
		}
	}
}
