using UnityEngine;
using System.Collections;
//the attractor class is used to find each different celestial body so that the physicsbody scripts know in which direction to add
//forces to simulate gravity
public class attractor : MonoBehaviour {
	//two floats, mass and radius, used to determine both the edge of the planet and the mass of planet, so we can calculate the
	//magnitude of the force due to gravity we need to add to each physicsbody
	public float mass;
	public float radius;

	// Use this for initialization
	void Start () {
		//setting the variable radius to the scale of this planet
		radius = transform.localScale.x;
		//using the equation for volume of a sphere:
		//V = 4/3 * Pi * r^3
		//we can determine the mass of this object like so:
		GetComponent<Rigidbody2D> ().mass = 4/3 * Mathf.PI * radius * radius * radius;
		//setting this script's mass variable to the calculated one
		mass = GetComponent<Rigidbody2D>().mass;
	}
}
