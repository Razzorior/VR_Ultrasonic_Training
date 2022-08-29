using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity;
using Photon.Voice.PUN;


public class VoiceHighlighter : MonoBehaviour
{
    private PhotonVoiceView photonVoiceView;

    [SerializeField]
    private int intervalTime = 3;

    [SerializeField]
    private Image recorderSprite;

    [SerializeField]
    private Image speakerSprite;

    [SerializeField]
    private Text bufferLagText;

    private bool showSpeakerLag;

    private void Awake()
    {
        this.photonVoiceView = this.GetComponentInParent<PhotonVoiceView>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Time.frameCount % intervalTime == 0)
        {
            this.recorderSprite.enabled = this.photonVoiceView.IsRecording;
            this.speakerSprite.enabled = this.photonVoiceView.IsSpeaking;
            this.bufferLagText.enabled = this.showSpeakerLag && this.photonVoiceView.IsSpeaking;
            if (this.bufferLagText.enabled)
            {
                this.bufferLagText.text = string.Format("{0}", this.photonVoiceView.SpeakerInUse.Lag);
            }
        }
    }
}
