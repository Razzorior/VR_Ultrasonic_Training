using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManageCPR : MonoBehaviour
{
    public float timeUntilUSTimerStarts = 0.5f;
    public float minimumTimeOfCPRRequired = 5f;
    public int maxUltraSoundTimeAllowed = 10;
    public AudioSource metronome;
    public AudioSource countdown;
    public TextMeshProUGUI timerText;
    public GameObject cPRHand;
    public GameObject otherHand;

    private Vector3 startingPos;
    //private Vector3 otherHandPosOffset = new Vector3();
    //private Vector3 otherHandRotOffset = new Vector3();
    private bool currentlyApplyingCPR = false;
    private int currentlyColliding = 0;
    private bool usTimerIsRunning = false;
    private float timePassedSinceLastCPRPress = 0f;
    private float totalCPRTime = 0f;
    private List<float> timePointsOfPresses = new List<float>(); 
    private float cprPressesPerMinute = 0;
    private Coroutine timerCoroutine;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "CPRHand" && !currentlyApplyingCPR)
        {
            if (usTimerIsRunning)
            {
                usTimerIsRunning = false;
                StopCoroutine(timerCoroutine);
                countdown.Stop();
                timerCoroutine = null;
                timerText.text = maxUltraSoundTimeAllowed.ToString();
            }
            metronome.Play();
            currentlyApplyingCPR = true;
            
            otherHand.transform.parent = (cPRHand.transform);
            otherHand.transform.localRotation = Quaternion.identity;
            otherHand.transform.localPosition = Vector3.zero;
        }

        timePassedSinceLastCPRPress = 0f;
        currentlyColliding++;
        Debug.Log("Collision detected");
    }

    private void OnCollisionExit(Collision other)
    {
        currentlyColliding--;
        Debug.Log("Collision exited");
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentlyApplyingCPR && currentlyColliding <= 0)
        {
            timePassedSinceLastCPRPress += Time.deltaTime;
            if(timePassedSinceLastCPRPress > timeUntilUSTimerStarts)
            {
                if(totalCPRTime > minimumTimeOfCPRRequired)
                {
                    totalCPRTime = 0f;
                    metronome.Stop();
                    currentlyApplyingCPR = false;
                    otherHand.transform.parent = null;
                    otherHand.transform.position = new Vector3(500f, 0f, 0f);
                    Debug.Log("8sec timer started");
                    // --- Starting 8 Sec timer ---
                    timerCoroutine = StartCoroutine(UltraSoundTimer());
                    usTimerIsRunning = true;
                }
                else
                {
                    totalCPRTime = 0f;
                    // --- ADD CPR Fail message/feedback ---
                    Debug.Log("You failed CPR");
                }
            }
        } 
        else if (currentlyApplyingCPR)
        {
            totalCPRTime += Time.deltaTime;
        }
    }
    IEnumerator CPRPressSpeedAndDepthHandler(float averageDuration)
    {
        float totalGameTimePassed = 0f;

        while (true)
        {
            totalGameTimePassed += Time.deltaTime;

            if(currentlyApplyingCPR)
            {

            }
            yield return null;
        }
    }
    IEnumerator UltraSoundTimer()
    {
        int timePassed = 0;
        timerText.text = maxUltraSoundTimeAllowed.ToString();
        yield return new WaitForSeconds(Mathf.Max((1f - timeUntilUSTimerStarts), 0));
        timePassed++;
        timerText.text = (maxUltraSoundTimeAllowed - timePassed).ToString();
        while (maxUltraSoundTimeAllowed - timePassed > 0)
        {
            timePassed += 1;
            yield return new WaitForSeconds(1f);
            if(maxUltraSoundTimeAllowed - timePassed == 4)
            {
                countdown.Play();
            }
            timerText.text = (maxUltraSoundTimeAllowed - timePassed).ToString();
        }
        Debug.Log("Ultrasound failed.");
        timerText.text = "0";
        usTimerIsRunning = false;
        yield return null;
    }

}
