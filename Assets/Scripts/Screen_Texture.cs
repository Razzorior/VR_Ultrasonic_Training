using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Texture : MonoBehaviour
{
	
	public GameObject screen;
	
    // Start is called before the first frame update
    void Start()
    {
        
		screen = GameObject.Find("Screen");
		Renderer renderer = screen.GetComponent<Renderer>();
		
        WebCamDevice[] devices = WebCamTexture.devices;
		
		for( int i=0; i<devices.Length; i++ ) {
			Debug.Log( "Webcam: " + devices[i].name );
		}
		
		string webCamDeviceName = devices[0].name;
		
		Debug.Log("Using " + webCamDeviceName + " as Texture");
		WebCamTexture tex = new WebCamTexture( webCamDeviceName );
		
		renderer.material.SetTexture("_MainTex", tex);
		tex.Play();
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
