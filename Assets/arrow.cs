using UnityEngine;
using System.Collections;
//this script controls the lifespan of the arrow and its motion
//while not actually handling the physics behind the arrows movement
//this script does control how long it stays active for before disappearing
//once it has landed on a planet

public class arrow : MonoBehaviour {

	Rigidbody2D rb;
	//rb is the rigidbody attatched to this object, this is what allows the arrow object to take advantage
	//of the physics system by using methods like addforce etc.
	bool landed = false;
	//landed is a boolean that determines whether or not the arrow has landed on a planet, used later
	//to determine when the arrow should disappear
	public int owner;
	//owner is an integer that will be set to the player ID number based on the player that fired the
	//arrow
	float lifetime = 0f;
	//lifetime is a float that will tick up every frame to measure the duration that the arrow has been flying / landed for.
	float maxTime = 1000f;
	//maxTime is a float which determines the max time that the arrow can fly/stay landed for.
	public Mesh sphere;
	//sphere is a mesh that represents a sphere, which is used later to determine how the particles that will fly out
	//from the arrow will move when the arrow despawns.

	//Start is called when this class is first created / the game is launched, whichever comes last
	void Start () {
		//setting the variable rb to the Rigidbody2D component attached to this object
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		//getting the physicsBody.cs script attatched to this object and checking if the sleeping variable is true or not
		if (!GetComponent<physicsBody> ().sleeping) {
			//if the sleeping variable is not true, the game is not puased
			//so, continue
			
			//adding the time that has passed in the last frame to the float lifetime, so lifetime ticks up
			// as time passes
			lifetime += Time.deltaTime;
			
			//checking if both: the rigidbody component is not set to kinematic, and the time that has passed
			//since the arrow has been created (lifetime) is more than or equal to the max time that the arrow 
			//is allowed to be alive. If these are both true, the arrow has been in flight for too long, and
			//it is now time to destroy the arrow
			if (lifetime >= maxTime && !rb.isKinematic) {
				//destroying the gameObject that this script is attatched to which, in this case, is the
				//arrow that this script is controlling
				Destroy (this.gameObject);
			}
			//now checking if both:the arrow has landed (since landed will be true) and the time that has passed
			//since the arrow has been created (lifetime) is more than or equal to the max time that the arrow 
			//is allowed to be alive. If these are both true, the arrow has been landed on a planet for too long, and
			//it is now time to destroy the arrow
			if (lifetime >= maxTime && landed) {
				//starting a couroutine named die, detailed below, with input argument false
				//this causes the arrow to disappear in a fancy way
				StartCoroutine (Die (false));
			}
			//then, if neither of the last two if statements are true, we will check if we have not laneded
			//since if we havent landed, we must be flying
			if (!landed) {
				//so when we are flying, we will set our rotation to lerp between our current rotation
				//and one calculated based on the arrow is currently moving
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0f, 0f, 90f + math.getAngle (rb.velocity)), Time.deltaTime * 20f);
				//lerp smoothly interpolates between two rotations, transform.rotation and the euler calculated
				//from math.getAngle, which will in the end set the rotation of the arrow in such a way that
				//the tip of the arrow points in the direciton that the arrow is moving
			}
		}
	}
	
	//OnCollisionEnter is called when this object collides with another object
	//so this arrow may have hit another arrow, or hit a planet, or a person
	void OnCollisionEnter2D(Collision2D other) {
		//checking that we havent already landed, if we had we can just ignore the collision
		if (!landed) {
			//checking if the object that we landed with has the tag 'attractor', if it did then we know we collided with
			//a planet
			if (other.gameObject.tag == "attractor") {
				//setting landed to true, so that we know in the future we have already landed.
				landed = true;
				//setting isKinematic to true so that we do not move anymore as a result of the physics system
				rb.isKinematic = true;
				//disabling the Box collider component attached to this object. this saves cpu power, since
				//idle colliders in the scene can use a lot of unneccesary cpu power and memory
				GetComponent<BoxCollider2D> ().enabled = false;
				/*
				var em = transform.GetChild (0).GetComponent<ParticleSystem> ().emission;
				var rate = em.rate;
				rate.constantMax = 0f;
				em.rate = rate;
				*/
				
				//stopping any emission from the arrow so it does not continue to spew particles from its tail end
				transform.GetChild(0).GetComponent<ParticleSystem>().emissionRate = 0f;
				//setting lifetime to 0, since it has been 0 seconds since we landed on a planet. this will be used
				//to determine how long to wait until we despawn the arrow
				lifetime = 0f;
			//check if the thing we hit was instead a player
			} else if (other.gameObject.tag == "player") {
				//we have collided with a player, need to check whether or not we hit our own player object, or if it
				//was another player
				//this is done by comparing the player's characterControl script's playerID against the id we have
				//listed in our 'owner' int previously defined
				if (other.gameObject.GetComponent<characterControl> ().playerID != owner) {
					//we hit a player that is not our own player, so time to kill the other player
					landed = true;
					//calling the killPlayer routine on the other player's characterControl script
					GameObject.FindObjectOfType<gameManager> ().killPlayer (other.gameObject.GetComponent<characterControl> ().playerID);
					//now we destroy the player gameObject
					Destroy (other.gameObject);
					//now finally we will start the coroutine Die with an argument true
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
		
		//TODO SET LIFETIME TO 5 SECONDS SO THAT ALL PARTICLES DISAPPEAER BEFORE THE GAME OBJECT IS DELETED
		
		ps.Emit (100);
		GetComponent<Renderer> ().enabled = false;
		yield return new WaitForSeconds (5f);
		Destroy (this.gameObject);
	}
}
