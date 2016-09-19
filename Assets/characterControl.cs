using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterControl : MonoBehaviour {

	List<attractor> attractors = new List<attractor>();

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

	}

	float squareDist(Vector2 vectA, Vector2 vectB) {
		float sqrDist = (vectA.x - vectB.x) * (vectA.x - vectB.x) + (vectA.y - vectB.y) * (vectA.y - vectB.y);
		return sqrDist;
	}
}
