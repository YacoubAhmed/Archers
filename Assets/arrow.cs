using UnityEngine;
using System.Collections;

public class arrow : MonoBehaviour {

	Rigidbody2D rb;
	bool landed;
	public int owner;
	float lifetime = 0f;
	float maxTime = 5f;
	gameManager gm;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindObjectOfType<gameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		lifetime += Time.deltaTime;
		if (lifetime >= maxTime && !rb.isKinematic) {
			Destroy (this.gameObject);
		}
		if (!landed) {
			transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0f, 0f, 90f + math.getAngle (rb.velocity)), Time.deltaTime * 20f);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "attractor") {
			landed = true;
			rb.isKinematic = true;
			GetComponent<BoxCollider2D> ().enabled = false;
			var em = transform.GetChild (0).GetComponent<ParticleSystem> ().emission;
			var rate = em.rate;
			rate.constantMax = 0f;
			em.rate = rate;
		} else if (other.gameObject.tag == "player") {
			//kill the player
			if (other.gameObject.GetComponent<characterControl> ().player != owner) {
				Destroy (other.gameObject);
				Destroy (this.gameObject);
			}
		}
	}
}
