using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    public bool CPRPlayer = true;
    public Device_Connect dc;
    public List<Component> cprPlayerDeactivations = new List<Component>();
    public List<Component> ultrasoundPlayerDeactivations = new List<Component>();
    public List<GameObject> cprSendingGameObjects = new List<GameObject>();
    public List<GameObject> ultrasoundSendingGameObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
        if (CPRPlayer)
        {
            foreach (Component co in cprPlayerDeactivations)
            {
                co.GetComponent<MonoBehaviour>().enabled = false;
            }
            dc.retrieveTransform = ultrasoundSendingGameObjects;
            dc.sendTransform = cprSendingGameObjects;
        } 
        else
        {
            Debug.Log("Ultrasound Player confirmed");
            foreach (Component co in ultrasoundPlayerDeactivations)
            {
                co.GetComponent<MonoBehaviour>().enabled = false;
            }
            dc.retrieveTransform = cprSendingGameObjects;
            dc.sendTransform = ultrasoundSendingGameObjects;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
