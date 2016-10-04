using UnityEngine;
using System.Collections;

public class math : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static float squareDist(Vector2 vectA, Vector2 vectB) {
		float sqrDist = (vectA.x - vectB.x) * (vectA.x - vectB.x) + (vectA.y - vectB.y) * (vectA.y - vectB.y);
		return sqrDist;
	}

	public static float clampAngle(float ang) {
		float ret = ang;
		if (ang < 0) {
			ret += 360;
		}
		ret = ret % 360;
		return ret;
	}

	public static float getAngle(Vector2 vect1) {
		float returnAngle = 0f;
		Vector2 relativeVect = vect1;
		//print (relativeVect);
		if (relativeVect.x == 0) {
			if (relativeVect.y > 0f) {
				returnAngle = 90f;
				return returnAngle;
			} else {
				returnAngle = 270f;
				return returnAngle;
			}
		} if (relativeVect.y == 0) {
			if (relativeVect.x > 0f) {
				returnAngle = 0f;
				return returnAngle;
			} else {
				returnAngle = 180f;
				return returnAngle;
			}
		}
		float tan = Mathf.Atan2 (Mathf.Abs(relativeVect.y), Mathf.Abs(relativeVect.x))*Mathf.Rad2Deg;
		if (relativeVect.x < 0f) {
			if (relativeVect.y < 0f) {
				// x -, y-
				returnAngle = 180 + tan;
			} else if (relativeVect.y > 0f) {
				// x -, y+
				returnAngle = 180 - tan;
			}
		} else if (relativeVect.x > 0f) {
			if (relativeVect.y < 0f) {
				// x +, y-
				returnAngle = 360 - tan;
			} else if (relativeVect.y > 0f) {
				// x +, y+
				returnAngle = tan;
			}
		}
		return returnAngle;
	}

	public static Vector2 randomVector2D (float xFrom, float xTo, float yFrom, float yTo) {
		float x = Random.Range (xFrom, xTo);
		float y = Random.Range (yFrom, yTo);
		return new Vector2 (x, y);
	}
}
