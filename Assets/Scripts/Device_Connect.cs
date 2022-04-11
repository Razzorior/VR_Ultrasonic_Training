using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device_Connect : MonoBehaviour
{
	
    string deviceName;
	
    // Start is called before the first frame update
    void Start()
    {
		
        deviceName = SystemInfo.deviceName;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
