using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

[Serializable]
public class PositionRotationJSON
{
    public Vector3 position;
    public Quaternion rotation;
    public string name;
}

[Serializable]
public class JSONList<PositionRotationJSON>
{
    public PositionRotationJSON[] objects;
}

public class Device_Connect : MonoBehaviour
{
	
    string deviceName;
    public String ipAdress = "127.0.0.1";
	public int portNumber = 4000;
	
	public UdpClient udpClient;
	public IPEndPoint ipEndPoint;
    
    public List<GameObject> retrieveTransform = new List<GameObject>();
    public List<GameObject> sendTransform = new List<GameObject>();
    	
    // Start is called before the first frame update
    void Start()
    {
		
        deviceName = SystemInfo.deviceName;
     
        udpClient = new UdpClient();
				
		try {
			udpClient.Connect(ipAdress, portNumber);
			
			ipEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
             
    }

    // Update is called once per frame
    void Update()
    {
        /*
            Send object transform to UDP server
        */
        
        string json = "[";
                
        sendTransform.ForEach(delegate(GameObject go)
        {
            PositionRotationJSON s = new PositionRotationJSON();
            s.position = go.transform.position;
            s.rotation = go.transform.rotation;
            s.name = go.name;
            json += JsonUtility.ToJson(s) + ",";
        });
        
        json = json.TrimEnd(',');
        json += "]";

        
        try {
			Byte[] sendBytes = Encoding.ASCII.GetBytes("set___" + deviceName + "___" + json);

			udpClient.Send(sendBytes, sendBytes.Length);

		} catch (Exception e) {
			Debug.Log("Error sending object transform to server");
			Debug.Log(e.ToString());
		}
        
        /*
            Retrieve object transform from UDP server
        */
        
        try {
			Byte[] sendBytes = Encoding.ASCII.GetBytes("get___" + deviceName);
			udpClient.Send(sendBytes, sendBytes.Length);

			if( udpClient.Available > 0 ) {
				Byte[] receiveBytes = udpClient.Receive(ref ipEndPoint);			
				string returnData = Encoding.ASCII.GetString(receiveBytes);
				
                JSONList<PositionRotationJSON> list = JsonUtility.FromJson<JSONList<PositionRotationJSON>>(returnData);
				
                foreach (PositionRotationJSON posRot in list.objects)
                {
                    GameObject obj = retrieveTransform.Find( o => o.name == posRot.name );
                    obj.transform.position = posRot.position;
                    obj.transform.rotation = posRot.rotation;
                }
										
			}

		} catch (Exception e) {
            Debug.Log("Error retrieving object transform from server");
			Debug.Log(e.ToString());
		}
    }
}
