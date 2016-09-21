using UnityEngine;
using System.Collections;

public class attractor : MonoBehaviour {

	public float mass;
	public float radius;

	// Use this for initialization
	void Start () {
		radius = Mathf.Sqrt (transform.localScale.x * transform.localScale.y);
		GetComponent<Rigidbody2D> ().mass = 25 * radius;
		mass = GetComponent<Rigidbody2D>().mass;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
