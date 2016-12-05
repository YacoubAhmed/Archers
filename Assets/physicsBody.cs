using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class physicsBody : MonoBehaviour {

	Rigidbody2D rb;
	List<attractor> attractors = new List<attractor>();
	//list of all other attracting bodies in the scene
	//required to add force to this physics body to simulate gravity
	Vector3 storedVel;
	//vector to represent the velocity of this object
	//used when pausing the game, the physics body will store its velocity, set its actual velocity to 0 and wait
	//then when unpaused all physicsbody objects wll
	public bool sleeping;
	//boolean to represent whether or not this physicsbody is paused or not
	

	//start void called when the game scene is run, at the start of the game
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
				//define a float distance that represents the distance squared from the current objects position
				//and the attractors object position
				magnitude *= rb.mass;
				magnitude *= at.mass;
				//multiplying the magnitude of the vector by the current object's and the attractor
				//objects masses
				magnitude /= dist;
				//dividing the magnitude float by the square distance float
				rb.AddForce (dir * magnitude);
				//adding a vector force that is the direction vector
				//mutliplied by the magnitude float
			}
		}
	}
	public void sleep(bool toSleep) {
		//call to put the physics object to sleep or wake it up based on the toSleep bool
		sleeping = toSleep;
		//sets the local variable sleeping to toSleep
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
