using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class physicsBody : MonoBehaviour {

	Rigidbody2D rb;
	List<attractor> attractors = new List<attractor>();

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody2D>();
		foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
			attractors.Add(at);
		}
	}
	 
	// Update is called once per frame
	void Update () {
		foreach (attractor at in attractors) {
			Vector2 dir = at.transform.position - transform.position;
			dir = dir.normalized;
			float magnitude = 6.674f;
			float dist = math.squareDist (transform.position, at.transform.position);
			magnitude *= rb.mass;
			magnitude *= at.mass;
			magnitude /= dist;
			//magnitude *= 10000000000000;
			rb.AddForce (dir * magnitude);
		}
	}
}
