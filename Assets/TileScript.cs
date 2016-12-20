using UnityEngine;
using System.Collections;
using Assets.Helpers;

public class TileScript : MonoBehaviour {

	string tileInfo;
	public int size;
	public int zoom;
	//public Vector2 tile;
	public Rect rect;
	public Vector2 tileCoords; //x and y of a tile in the collection
	public Vector2 center;
	double[] latLon = new double[2];

	MapController mapController;

	public bool loaded = false;

	void OnEnable()
	{
		mapController = FindObjectOfType<MapController> ();
		zoom = mapController.settings.zoom;
		size = mapController.settings.size;
	}

	public void SetupTile(double _lat, double _lon)
	{
		Debug.Log ("SetupTile fired for " + gameObject.name);
		//tileCoords = _tileCoords;s
		rect = GM.TileBounds (tileCoords, zoom);
		center = new Vector2 (rect.x + rect.width / 2, rect.y + rect.height / 2);

		latLon [0] = _lat;
		latLon [1] = _lon;
		//Debug.Log ("SetupData were " + latLon[0] + "/" + latLon[1]);
		StartCoroutine(LoadSnapshot ());
	}

	IEnumerator LoadSnapshot() {
		Debug.Log ("Tile snapshot is being loaded...");

		//latLon = GM.DoubleMetersToLatLon (center);

		Debug.Log ("Lat lon is: " + latLon[0] + "/" + latLon[1]);

		string url = System.String.Format ("https://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", latLon[0], latLon[1], zoom, size, mapController.settings.key, mapController.settings.style);
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
		tileInfo = "size: " + size.ToString () + "\nzoom: " + zoom.ToString () + "\ntile: " + tileCoords.ToString () + "\nloaded: " + loaded.ToString ();
		GetComponentInChildren<TextMesh> ().text = tileInfo;
	}
}