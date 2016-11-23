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
				//using newtons law of gravity,
				//Fgrav = G * m1 * m2 / r^2
				//The force due to gravity is equal to
				//the gravitational constant (normally 6.674x10^-11 but i have just increased it to 6.674
				//to reduce floating point rounding errors, while also making the need for large masses on the
				//planets much less (baisically removing all of the powers of 10 to make maths easier))
				//multiplied by the mass of the first object (the arrow)
				//multiplied by the mass of the second object (the attractors mass)
				//and divided by the distance between these two objects (calculated by math.squareDist)
				Vector2 dir = at.transform.position - transform.position;
				//getting the direction in which we should apply the force by subtracting the current position vector
				//of the arrow from the current position vector of the planet
				dir = dir.normalized;
				//normalising the direction vector so that its magnitude is only 1. This will make the direciton
				//vector only be a direction and have no effect on the magnitude of the force that acts, since its
				//magnitude is only 1.
				float magnitude = 6.674f;
				//define a float magnitude to have the previously mentioned universal gravitational field constant
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
