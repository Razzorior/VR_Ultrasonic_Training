using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Texture : MonoBehaviour
{
	
	public GameObject screen;
	public string deviceName = "";
	public int resolution_x = 640;
	public int resolution_y = 360;
	public int fps = 15;
	public Vector2[] uvs = { new Vector2(0.0f,0.0f), new Vector2(1.0f,0.0f), new Vector2(0.0f,1.0f), new Vector2(1.0f,1.0f) };
	
    // Start is called before the first frame update
    void Start()
    {
        
		screen = GameObject.Find("Screen");
		Renderer renderer = screen.GetComponent<Renderer>();
		Mesh mesh = screen.GetComponent<MeshFilter>().mesh;
		
		if(mesh.uv.Length == 4) {
			mesh.uv = uvs;
		}
		
        WebCamDevice[] devices = WebCamTexture.devices;
		
		for( int i=0; i<devices.Length; i++ ) {
			Debug.Log( "Webcam: " + devices[i].name );
		}
		
		string webCamDeviceName = deviceName.Length == 0 ? devices[0].name : deviceName;
		
		Debug.Log("Using " + webCamDeviceName + " as Texture");
		WebCamTexture tex = new WebCamTexture( webCamDeviceName, resolution_x, resolution_y, fps );
		
		renderer.material.SetTexture("_MainTex", tex);
		tex.Play();
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
