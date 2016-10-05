using UnityEngine;
using System.Collections;

public class arrow : MonoBehaviour {

	Rigidbody2D rb;
	bool landed = false;
	public int owner;
	float lifetime = 0f;
	float maxTime = 1000f;
	public Mesh sphere;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!GetComponent<physicsBody> ().sleeping) {
			lifetime += Time.deltaTime;
			if (lifetime >= maxTime && !rb.isKinematic) {
				Destroy (this.gameObject);
			}
			if (lifetime >= maxTime && landed) {
				StartCoroutine (Die (false));
			}
			if (!landed) {
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0f, 0f, 90f + math.getAngle (rb.velocity)), Time.deltaTime * 20f);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (!landed) {
			if (other.gameObject.tag == "attractor") {
				landed = true;
				rb.isKinematic = true;
				GetComponent<BoxCollider2D> ().enabled = false;
				/*
				var em = transform.GetChild (0).GetComponent<ParticleSystem> ().emission;
				var rate = em.rate;
				rate.constantMax = 0f;
				em.rate = rate;
				*/
				transform.GetChild(0).GetComponent<ParticleSystem>().emissionRate = 0f;
				lifetime = 0f;
			} else if (other.gameObject.tag == "player") {
				if (other.gameObject.GetComponent<characterControl> ().playerID != owner) {
					landed = true;
					GameObject.FindObjectOfType<gameManager> ().killPlayer (other.gameObject.GetComponent<characterControl> ().playerID);
					Destroy (other.gameObject);
					StartCoroutine (Die (true));
				}
			}
		}
	}

	IEnumerator Die(bool player) {
		ParticleSystem ps = GetComponentInChildren<ParticleSystem> ();
		ps.transform.localPosition = new Vector3 (0f, 0f, 0f);
		/*
		var sh = ps.shape;
		sh.enabled = true;
		sh.shapeType = ParticleSystemShapeType.Sphere;
		sh.randomDirection = true;
		sh.radius = 0.01f;
		*/
		ps.startSpeed = 20f;
		ps.Emit (100);
		GetComponent<Renderer> ().enabled = false;
		yield return new WaitForSeconds (5f);
		Destroy (this.gameObject);
	}
}