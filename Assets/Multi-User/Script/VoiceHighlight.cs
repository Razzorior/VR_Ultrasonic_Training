using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Voice.PUN;
using ExitGames.Demos.DemoPunVoice;

[RequireComponent(typeof(Canvas))]
public class VoiceHighlight : MonoBehaviour {
    private Canvas canvas;

    private PhotonVoiceView m_PhotonView;

    [SerializeField]
    private Image recorderSprite;

    [SerializeField]
    private Image speakerSprite;

    [SerializeField]
    private Text bufferLagText;

    private bool showSpeakerLag;
    private bool _isSpeaking;

    private void OnEnable()
    {
        ChangePOV.CameraChanged += ChangePOV_CameraChanged;
        VoiceDemoUI.DebugToggled += VoiceDemoUI_DebugToggled;
    }

    private void OnDisable()
    {
        ChangePOV.CameraChanged -= ChangePOV_CameraChanged;
        VoiceDemoUI.DebugToggled -= VoiceDemoUI_DebugToggled;
    }

    private void VoiceDemoUI_DebugToggled(bool debugMode)
    {
        showSpeakerLag = debugMode;
    }

    private void ChangePOV_CameraChanged(Camera camera)
    {
        canvas.worldCamera = camera;
    }

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.worldCamera == null) { canvas.worldCamera = Camera.main; }

        //this.m_PhotonView = transform.parent.GetComponent<PhotonVoiceView>();
        this.m_PhotonView = GetComponentInParent<PhotonVoiceView>();
    }


    // Update is called once per frame
    private void Update()
    {
        recorderSprite.enabled = m_PhotonView.IsRecording;
        speakerSprite.enabled = m_PhotonView.IsSpeaking;
        bufferLagText.enabled = showSpeakerLag && m_PhotonView.IsSpeaking;
        if (bufferLagText.enabled)
        {
            bufferLagText.text = string.Format("{0}", m_PhotonView.SpeakerInUse.Lag);
        }

        //Set the value talking to study
        //if (m_PhotonView.isMine)
        //{
        //   // VuGlobalRef.instance._cameraNavigation._isTalking = _isSpeaking;
        //}
    }

    private void LateUpdate()
    {
        if (canvas == null || canvas.worldCamera == null) { return; } // should not happen, throw error
        transform.rotation = Quaternion.Euler(0f, canvas.worldCamera.transform.eulerAngles.y, 0f); //canvas.worldCamera.transform.rotation;
    }
}
