using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapController : MonoBehaviour {

	public bool debugPC;
	public float debugLat, debugLon;
	public Image canvasImage;
	public GameObject player;
	public Vector3 position;

	[SerializeField]
	private Settings settings;

	[SerializeField]
	private Texture2D texture;
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

		StartCoroutine (UpdateLocation());
		StartCoroutine (LoadTiles ());
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
				position = Vector3.zero;
				geodeticOffsetInv ((float)lat * Mathf.Deg2Rad, (float)lon * Mathf.Deg2Rad, (float)oldLat * Mathf.Deg2Rad, (float)oldLon * Mathf.Deg2Rad, out x, out y);
				position.x = x;
				position.z = y;
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

				yield return new WaitForSeconds (1f);
				oldLat = lat;
				oldLon = lon;
				StartCoroutine(LoadTiles ());
				player.transform.position = new Vector3 (0f, 0.5f, 0f);

				//infoString = ("Signal lost, searching...");
			} else {
				infoString = ("Please enable GPS!");
			}
			//Debug.Log (infoString);
			yield return new WaitForSeconds (1f);
		}
	}

	IEnumerator LoadTiles() {
		Debug.Log ("LOADED TILES");
		//Debug.Log ("LoadTiles() for lat: " + lat + " lon: " + lon);
		url = System.String.Format ("https://api.mapbox.com/v4/mapbox.{5}/{0},{1},{2}/{3}x{3}@2x.png?access_token={4}", lon, lat, settings.zoom, settings.size, settings.key, settings.style);
		//Debug.Log ("url: " + url.ToString ());
		WWW www = new WWW (url);
		yield return www;

		//if (www.isDone == true) {
			texture = www.texture;
			
			if (tile == null) {
				//tile = GameObject.CreatePrimitive (PrimitiveType.Plane);
			tile = transform.GetChild(0).gameObject;
			//Debug.Log("scale was: " + tile.transform.localScale);
				//tile.transform.localScale = Vector3.one * settings.scale;
			//tile.transform.eulerAngles = new Vector3 (90, 0, 0);
			//Debug.Log("new scale: " + tile.transform.localScale);
				tile.GetComponent<Renderer> ().material = settings.material;
				tile.transform.parent = transform;
			}

		settings.material.mainTexture = texture;
		tile.GetComponent<Renderer> ().material.mainTexture = texture;

		//Debug.Log ("Tried to refresh map");

		//yield return new WaitForSeconds (1f);
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

	//SOURCE: http://stackoverflow.com/questions/4953150/convert-lat-longs-to-x-y-co-ordinates

	float GD_semiMajorAxis = 6378137.000000f;
	float GD_TranMercB     = 6356752.314245f;
	float GD_geocentF      = 0.003352810664f;

	void geodeticOffsetInv( float refLat, float refLon,
		float lat,    float lon,
		out float xOffset, out float yOffset )
	{
		float a = GD_semiMajorAxis;
		float b = GD_TranMercB;
		float f = GD_geocentF;

		float L = lon - refLon;
		float U1    = Mathf.Atan((1-f) * Mathf.Tan(refLat));
		float U2    = Mathf.Atan((1-f) * Mathf.Tan(lat));
		float sinU1 = Mathf.Sin(U1);
		float cosU1 = Mathf.Cos(U1);
		float sinU2 = Mathf.Sin(U2);
		float cosU2 = Mathf.Cos(U2);

		float lambda = L;
		float lambdaP;
		float sinSigma;
		float sigma;
		float cosSigma;
		float cosSqAlpha;
		float cos2SigmaM;
		float sinLambda;
		float cosLambda;
		float sinAlpha;
		int iterLimit = 100;
		do {
			sinLambda = Mathf.Sin(lambda);
			cosLambda = Mathf.Cos(lambda);
			sinSigma = Mathf.Sqrt((cosU2*sinLambda) * (cosU2*sinLambda) +
				(cosU1*sinU2-sinU1*cosU2*cosLambda) *
				(cosU1*sinU2-sinU1*cosU2*cosLambda) );
			if (sinSigma==0)
			{
				xOffset = 0.0f;
				yOffset = 0.0f;
				return ;  // co-incident points
			}
			cosSigma    = sinU1*sinU2 + cosU1*cosU2*cosLambda;
			sigma       = Mathf.Atan2(sinSigma, cosSigma);
			sinAlpha    = cosU1 * cosU2 * sinLambda / sinSigma;
			cosSqAlpha  = 1 - sinAlpha*sinAlpha;
			cos2SigmaM  = cosSigma - 2*sinU1*sinU2/cosSqAlpha;
			if (cos2SigmaM != cos2SigmaM) //isNaN
			{
				cos2SigmaM = 0;  // equatorial line: cosSqAlpha=0 (§6)
			}
			float C = f/16*cosSqAlpha*(4+f*(4-3*cosSqAlpha));
			lambdaP = lambda;
			lambda = L + (1-C) * f * sinAlpha *
				(sigma + C*sinSigma*(cos2SigmaM+C*cosSigma*(-1+2*cos2SigmaM*cos2SigmaM)));
		} while (Mathf.Abs(lambda-lambdaP) > 1e-12 && --iterLimit>0);

		if (iterLimit==0)
		{
			xOffset = 0.0f;
			yOffset = 0.0f;
			return;  // formula failed to converge
		}

		float uSq  = cosSqAlpha * (a*a - b*b) / (b*b);
		float A    = 1 + uSq/16384*(4096+uSq*(-768+uSq*(320-175*uSq)));
		float B    = uSq/1024 * (256+uSq*(-128+uSq*(74-47*uSq)));
		float deltaSigma = B*sinSigma*(cos2SigmaM+B/4*(cosSigma*(-1+2*cos2SigmaM*cos2SigmaM)-
			B/6*cos2SigmaM*(-3+4*sinSigma*sinSigma)*(-3+4*cos2SigmaM*cos2SigmaM)));
		float s = b*A*(sigma-deltaSigma);

		float bearing = Mathf.Atan2(cosU2*sinLambda,  cosU1*sinU2-sinU1*cosU2*cosLambda);
		xOffset = Mathf.Sin(bearing)*s;
		yOffset = Mathf.Cos(bearing)*s;
	}
}