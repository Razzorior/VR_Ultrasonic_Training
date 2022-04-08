using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

enum currentMessageState
{
    calibrationReminder,
    nothing,
    calibriationOngoingMessage
}

public class QuestManager : MonoBehaviour
{
    public SteamVR_Action_Boolean skipText = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    public SteamVR_Action_Boolean calibrate = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("calibrate");
    public SteamVR_Behaviour_Pose controllerPoseScript;
    public GameObject Canvas;
    private bool calibratedMainikin = false;
    private currentMessageState currentMsgState = currentMessageState.calibrationReminder;
    public TextMeshProUGUI textComponent;
    public Transform leftController;
    public GameObject cameraRig;
    public Vector3 chestOfPatientPos = Vector3.zero;

    //Calibration Variables
    public int nbrMeasurements = 0;
    private Vector3 positionValues = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMsgState == currentMessageState.calibrationReminder && skipText.GetStateDown(controllerPoseScript.inputSource))
        {
            Canvas.SetActive(false);
            currentMsgState = currentMessageState.nothing;
        }

        if(!calibratedMainikin && calibrate.GetStateDown(controllerPoseScript.inputSource))
        {
            Canvas.SetActive(true);
            textComponent.text = "Calibrating for 5 sec... Please don't touch the controller";
            currentMsgState = currentMessageState.calibriationOngoingMessage;
            calibratedMainikin = true;
            StartCoroutine(Calibrate());
        }
    }

    IEnumerator Calibrate()
    {
        cameraRig.transform.rotation = Quaternion.Euler(0f, -90f, 0f) * Quaternion.Inverse(Quaternion.Euler(new Vector3(0f,leftController.rotation.eulerAngles.y, 0f)));
        float timePassed = 0f;
        while(timePassed < 5f)
        {
            timePassed += .1f;
            positionValues += leftController.position;
            nbrMeasurements++;
            yield return new WaitForSeconds(.1f);
        }

        
        positionValues = positionValues / nbrMeasurements;
        Debug.Log(positionValues);
        cameraRig.transform.position = cameraRig.transform.position + (chestOfPatientPos - positionValues);
        yield return null;
    }

}
