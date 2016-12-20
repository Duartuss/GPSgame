using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Helpers;

public class MapController : MonoBehaviour {

	public bool debugPC;
	public float debugLat, debugLon;
	public Image canvasImage;
	public GameObject player;
	public Vector3 position;

	public GameObject[] debugTemporaryArray;
	public GameObject[,] debugTileArray = new GameObject[3, 3];
	public Vector2 tileCoords;

	public Settings settings;

	[SerializeField]
	private Texture2D texture;
	[SerializeField]
	public GameObject tile;

	GameObject textObject;
	float x, y;
	[SerializeField]
	float oldLat, oldLon, lat, lon, tempLat, tempLon;
	string infoString, url;

	// Use this for initialization
	void Start () {
		Debug.Log ("start");

		oldLat = 0;
		oldLon = 0;

		Input.location.Start ();
		if (textObject == null)
			textObject = GameObject.Find ("TestText");

		//canvasImage = GameObject.Find ("CanvasImage").GetComponent<Image>();
		//THOSE ARE ALL DEBUG TEMPORARY SOLUTIONS
		DebugArrayToMultidimensional();


		StartCoroutine (UpdateLocation());
		//StartCoroutine (BuildMap ());
	}

	IEnumerator UpdateLocation() {
		//Debug.Log ("running updatelocation");

		while (true) {
			if (Input.location.isEnabledByUser || debugPC) {

					if (!debugPC) {
						tempLat = Input.location.lastData.altitude;
						tempLon = Input.location.lastData.longitude;
					} else {
						tempLat = debugLat;
						tempLon = debugLon;

						if (oldLat == 0)
							oldLat = debugLat;
						if (oldLon == 0)
							oldLon = debugLon;
					}

					//Set actual position only based on valid GPS data
					//if (tempLat != 0)
						lat = tempLat;
					//if (tempLon != 0)
						lon = tempLon;
					//textObject.GetComponent<Text> ().text = infoString;

				//if (oldLat != 0 && oldLon != 0) {
					//If actual position is same as last, don't bother with lerps or map changes and check again in some time

				while (lon == oldLon && lat == oldLat) {
					yield return new WaitForSeconds (1f);
					yield return StartCoroutine (UpdateLocation ());
					yield break;
				}

					//THIS is what happens when position changed on this iteration

					//Start a second-long lerp on player's side and also update the data
				Vector2 xyCoords = Vector2.zero;

				//Don't pass meters, only lat/lon, because meters get floored to one digit after the comma

				Debug.Log ("lat/lon set are : " + lat.ToString () + "/" + lon.ToString () + "\nand after translation to meters: "
					+ GM.LatLonToMeters (lat, lon) + "\nand reversed: " + GM.MetersToLatLon(GM.LatLonToMeters(lat, lon)));

				double[] testArr = new double[2];

				testArr = GM.DoubleMetersToLatLon (GM.LatLonToMeters (lat, lon));

				Debug.Log ("lon test is: " + testArr[0] + testArr[1]);

				//xyCoords = GM.LatLonToMeters (lon, lat);
				//tileCoords = GM.MetersToTile (xyCoords, settings.zoom);
				//tileCoords = xyCoords;
				//GM.TileSize = settings.size;

				//SETUP CENTER TILE
				//Debug.Log(debugTileArray[1,1].name.ToString());
				debugTemporaryArray [4].GetComponent<TileScript> ().SetupTile (testArr[0], testArr[1]);
				//geodeticOffsetInv ((float)lat * Mathf.Deg2Rad, (float)lon * Mathf.Deg2Rad, (float)oldLat * Mathf.Deg2Rad, (float)oldLon * Mathf.Deg2Rad, out x, out y);
				position.x = xyCoords.x;
				position.z = xyCoords.y;
				//Debug.Log ("BEFORE: x is " + position.x.ToString () + " , y is " + position.z.ToString ());
				position.x = x * 0.300122f;
				position.z = y * 0.123043f;
				position.y = 0.5f;
				//Debug.Log ("AFTER: x is " + position.x.ToString () + " , y is " + position.z.ToString ());
					
					if ((oldLat - lat) < 0 && (oldLon - lon) > 0 || (oldLat - lat) > 0 && (oldLon - lon) < 0) {
						position = new Vector3 (position.x, .5f, position.z);
					} else {
						position = new Vector3 (-position.x, .5f, -position.z);
					}
					
				//player.GetComponent<PlayerScript> ().StartLerpToLocation (position);

				yield return new WaitForSeconds (0.1f);
				oldLat = lat;
				oldLon = lon;

				//SETUP CENTRAL TILE
				StartCoroutine(BuildMap ());
				player.transform.position = new Vector3 (0f, 0.5f, 0f);

				//infoString = ("Signal lost, searching...");
			} else {
				infoString = ("Please enable GPS!");
			}
			//Debug.Log (infoString);
			yield return new WaitForSeconds (1f);
		}
	}

	void DebugArrayToMultidimensional()
	{
		int i, u, counter = 0;

		for(i=0; i<3; i++) {
			for(u=0; u<3; u++) {
				debugTileArray [u,i] = debugTemporaryArray [counter];
				Debug.Log (debugTileArray [u, i].name + " got index of [" + u.ToString () + "," + i.ToString () + "]");
				counter++;
			}
		}


	}

	IEnumerator BuildMap() {

		//debugTemporaryArray [4].GetComponent<TileScript> ().SetupTile (tileCoords);

		/*
		int i, u;

		for(i=0; i<3; i++) {
			for(u=0; u<3; u++) {
				//silly addition of "-1" to SetupTile arguments allows for getting proper offset, as center tile index is 1,1 and is treated as 0,0
				TileScript tempTile = debugTileArray[u,i].GetComponent<TileScript> ();
				tempTile.SetupTile (new Vector2 (tileCoords.x + u, tileCoords.y + i));
				yield return new WaitUntil (() => tempTile.loaded == true);
				Debug.Log ("Went past waituntil");
			}
		}
		*/
		yield return null;
	}
	
	[System.Serializable]
	public class Settings {
		[SerializeField]
		public Material material;
		[SerializeField]
		public int zoom;
		[SerializeField]
		public int size = 640;
		[SerializeField]
		public float scale = 1f;
		[SerializeField]
		public string key;
		[SerializeField]
		public string style = "emerald";
	}
}