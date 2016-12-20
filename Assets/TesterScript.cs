using UnityEngine;
using System.Collections;
using Assets.Helpers;

public class TesterScript : MonoBehaviour {

	public float _x;
	public float _y;
	public float _z;

	// Use this for initialization
	void Start () {

		Vector2 test1 = GM.LatLonToMeters (new Vector2 (_x, _y));
		Debug.Log ("latlotometers: " + test1.ToString ());
		Vector2 test2 = GM.MetersToLatLon (test1);
		Debug.Log ("reversed: " + test2.ToString ());
	}
}