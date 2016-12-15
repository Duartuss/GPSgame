using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	float _time;
	public Vector2 movementAxis;
	//Vector2 tempCoords;
	public Vector3 targetPosition;
	Vector3 tsp;
	bool isLerping;

	MapController mapController;

	void Start() {
		mapController = FindObjectOfType<MapController> ();
	}

	public void StartLerpToLocation(Vector3 offset)
	{
		_time = 0f;

		isLerping = true;
		targetPosition = offset;
		//tsp = transform.position;
		Debug.Log ("Lerp begun... target is " + targetPosition.ToString());
		StartCoroutine(LerpToTargetLocation ());
	}

	IEnumerator LerpToTargetLocation()
	{
		//while(tempCoords != targetCoords)
		//Debug.Log("Lerp running...");
		Vector3 tsp = transform.position;
		while (_time < 1)
		{
			transform.position = Vector3.Lerp (tsp, targetPosition, _time);
			//Debug.Log ("Lerp is running! pos is: " + transform.position.ToString() + " and Time is: " + _time.ToString());
			yield return new WaitForSeconds (0.01f);
			_time += Time.deltaTime;
			yield return StartCoroutine (LerpToTargetLocation ());
		}
		Debug.Log ("Lerp done!");
		yield break;

		/*
		while (Vector3.SqrMagnitude (tsp - targetPosition) < 0.001) {
			//transform.position = Vector3.Lerp (tsp, targetPosition, 1f * Time.deltaTime);
			Debug.Log ("tsp is " + tsp.ToString () + " target is " + targetPosition.ToString ());
			yield return StartCoroutine (LerpToTargetLocation ());
		}
		isLerping = false;
		Debug.Log ("Lerp finished! tsp is: " + tsp + " while target is: "
			+ targetPosition + " so magnitude is " + Vector3.SqrMagnitude(tsp - targetPosition).ToString());
		yield break;
		*/
	}
}