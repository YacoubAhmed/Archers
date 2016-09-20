using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterControl : MonoBehaviour {

	public List<attractor> attractors = new List<attractor>();

	// Use this for initialization
	void Start () {
		foreach (attractor at in GameObject.FindObjectsOfType<attractor>()) {
			attractors.Add(at);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float highestMag = 0f;
		attractor highestAttractor = attractors[0];
		foreach (attractor at in attractors) {
			float magnitude = 1000f;
			float dist = squareDist (transform.position, at.transform.position);
			magnitude /= dist;
			if (magnitude > highestMag) {
				highestMag = magnitude;
				highestAttractor = at;
			}
		}
		print (highestAttractor.gameObject.name);
		Ray2D ray = new Ray2D (transform.position, (highestAttractor.transform.position - transform.position));
		Debug.DrawRay (transform.position, (highestAttractor.transform.position - transform.position));
		RaycastHit2D hit = Physics2D.Raycast (transform.position, (highestAttractor.transform.position - transform.position));
		print ("hit " + hit.transform.name);
		Quaternion targetRot = Quaternion.Euler (90 * hit.normal);
		print ("normal " + hit.normal);
		transform.rotation = targetRot;
		//transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, Time.deltaTime * 20f);
	}

	float squareDist(Vector2 vectA, Vector2 vectB) {
		float sqrDist = (vectA.x - vectB.x) * (vectA.x - vectB.x) + (vectA.y - vectB.y) * (vectA.y - vectB.y);
		return sqrDist;
	}
}
