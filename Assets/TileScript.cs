using UnityEngine;
using System.Collections;
using Assets.Helpers;

public class TileScript : MonoBehaviour {

	/*
	 * Tile should receive both the tile and lat/lon info.
	 */

	string tileInfo;
	public int size;
	public int zoom;
	//public Vector2 tile;
	public Rect rect;
	public Vector2 tileCoords; //x and y of a tile in the collection
	public Vector2 center;
	public double lat, lon;
	double[] latLonArr = new double[2];
	//Vector2 latLon;

	MapController mapController;

	public bool loaded = false;

	void OnEnable()
	{
		mapController = FindObjectOfType<MapController> ();
		zoom = mapController.settings.zoom;
		size = mapController.settings.size;
	}

	//public void SetupTile(double _lat, double _lon)
	public void SetupTile(Vector2 _tileCoords)
	{
		Debug.Log ("SetupTile fired for " + gameObject.name);
		//tileCoords = _tileCoords;s
		tileCoords = _tileCoords;
		rect = GM.TileBounds (tileCoords, zoom);
		center = new Vector2 (rect.x + rect.width / 2, rect.y + rect.height / 2);

		double [] tempLatLon = GM.DoubleMetersToLatLon (center);
		lat = tempLatLon[0];
		lon = tempLatLon[1];

		//latLon [0] = _lat;
		//latLon [1] = _lon;
		//Debug.Log ("SetupData were " + latLon[0] + "/" + latLon[1]);
		StartCoroutine(LoadSnapshot ());
	}

	IEnumerator LoadSnapshot() {
		Debug.Log ("Tile snapshot is being loaded...");

		//latLon = GM.DoubleMetersToLatLon (center);
		//GM.DoubleMetersToLatLon (center);

		Debug.Log ("Lat lon FLOATED is: " + (float)lat + "/" + (float)lon);

		/*
		 * WARNING: DEBUG, PLEASE DELETE!!!
		 */

		//lat = 52.2267f;
		//lon = 21.01223f;


		string url = System.String.Format ("https://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", (float)lon, (float)lat, zoom, size, mapController.settings.key, mapController.settings.style);
		//Debug.Log ("url: " + url.ToString ());
		WWW www = new WWW (url);
		yield return www;
		Debug.Log ("RETURNED WWW");
		GetComponent<Renderer> ().material.mainTexture = www.texture;

		loaded = true;
		UpdateTileInfo ();
	}

	void UpdateTileInfo()
	{
		tileInfo = name.ToString () + "\nsize: " + size.ToString () + "\nzoom: " + zoom.ToString () + "\ntile: " + tileCoords.ToString () + "\nloaded: " + loaded.ToString ();
		GetComponentInChildren<TextMesh> ().text = tileInfo;
	}
}