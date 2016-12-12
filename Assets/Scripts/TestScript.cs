using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestScript : MonoBehaviour {

	public bool debugPC;
	public float debugLat, debugLon;
	public Image canvasImage;

	[SerializeField]
	private Settings settings;

	[SerializeField]
	private Texture2D texture;
	private GameObject tile;

	GameObject textObject;
	float lat, lon, tempLat, tempLon;
	string infoString, url;

	// Use this for initialization
	void Start () {
		Debug.Log ("start");

		Input.location.Start ();
		if (textObject == null)
			textObject = GameObject.Find ("TestText");

		//canvasImage = GameObject.Find ("CanvasImage").GetComponent<Image>();

		StartCoroutine (UpdateLocation());
	}

	void Update()
	{
		//StartCoroutine (testcoroutine ());
	}

	IEnumerator UpdateLocation() {
		Debug.Log ("running updatelocation");

		while (true) {
			if (Input.location.isEnabledByUser || debugPC) {
				if (!debugPC) {
					tempLat = Input.location.lastData.altitude;
					tempLon = Input.location.lastData.longitude;
				} else {
					tempLat = debugLat;
					tempLon = debugLon;
				}

				if (tempLat != 0)
					lat = tempLat;
				if (tempLon != 0)
					lon = tempLon;

				Debug.Log ("lat: " + lat.ToString () + " lon: " + lon.ToString ());

				//Temporary solution, in future should search for "NULL", not 0, which actually CAN be (?) valid data from GPS 
				if (lat != 0 && lon != 0) {
					infoString = ("lat: " + lat.ToString () + " lon: " + lon.ToString ());
					//LoadTiles ();
					StartCoroutine (LoadTiles ());
				} else {
					infoString = ("Signal lost, searching...");
				}
			} else {
				infoString = ("Please enable GPS!");
			}
			textObject.GetComponent<Text> ().text = infoString;
			Debug.Log (infoString);

			yield return new WaitForSeconds (1f);
		}
	}

	IEnumerator LoadTiles() {

		Debug.Log ("LoadTiles() for lat: " + lat + " lon: " + lon);
		url = System.String.Format ("https://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", lon, lat, settings.zoom, settings.size, settings.key, settings.style);
		Debug.Log ("url: " + url.ToString ());
		WWW www = new WWW (url);
		yield return www;

		//if (www.isDone == true) {
			texture = www.texture;

			if (tile == null) {
				tile = GameObject.CreatePrimitive (PrimitiveType.Plane);
			//Debug.Log("scale was: " + tile.transform.localScale);
				tile.transform.localScale = Vector3.one * settings.scale;
			tile.transform.eulerAngles = new Vector3 (90, 0, 0);
			//Debug.Log("new scale: " + tile.transform.localScale);
				tile.GetComponent<Renderer> ().material = settings.material;
				tile.transform.parent = transform;
			}

		settings.material.mainTexture = texture;
		tile.GetComponent<Renderer> ().material.mainTexture = texture;

		Debug.Log ("Tried to refresh map");

		yield return new WaitForSeconds (1f);
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