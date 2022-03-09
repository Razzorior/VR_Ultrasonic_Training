using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDP_Transform_Request : MonoBehaviour
{
    public GameObject cube;
	
	public Vector3 positionOffset = new Vector3( 0.0f, 0.0f, 0.0f );
	public Vector3 rotationOffset = new Vector3( 0.0f, 0.0f, 0.0f );
	
	public String ipAdress = "127.0.0.1";
	public int portNumber = 4246;
	
	public UdpClient udpClient;
	public IPEndPoint ipEndPoint;
	
    void Start()
    {
		cube = GameObject.Find("Schallkopf");
		udpClient = new UdpClient();
				
		try {
			udpClient.Connect(ipAdress, portNumber);
			
			ipEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
    }

    void Update()
    {
		
		try {
			Byte[] sendBytes = Encoding.ASCII.GetBytes("Request Position & Rotation Data");
			udpClient.Send(sendBytes, sendBytes.Length);

			if( udpClient.Available > 0 ) {
				Byte[] receiveBytes = udpClient.Receive(ref ipEndPoint);			
				string returnData = Encoding.ASCII.GetString(receiveBytes);
				string[] posiitonsAndRotations = returnData.Split(' ');
				
				Debug.Log( returnData );
							
				float posZ = Convert.ToSingle(posiitonsAndRotations[1]) / 100;
				float posX = Convert.ToSingle(posiitonsAndRotations[2]) / 100;
				float posY = Convert.ToSingle(posiitonsAndRotations[3]) / 100;
				
				float rotZ = Convert.ToSingle(posiitonsAndRotations[4]);
				float rotX = Convert.ToSingle(posiitonsAndRotations[5]);
				float rotY = Convert.ToSingle(posiitonsAndRotations[6]);
				
				cube.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ) * Quaternion.Euler(rotationOffset);
				//cube.transform.eulerAngles = new Vector3(rotX,rotY,rotZ);
				cube.transform.position = new Vector3(-posX,-posY,-posZ) + positionOffset;
			
			}

		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
		
    }
}
