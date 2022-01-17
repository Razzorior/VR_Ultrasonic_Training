using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget; // headset or controllers
    public Transform rigTarget;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    // set the position and rotation of the rig to the vr object position and rotation
    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(positionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(rotationOffset);
    }
}

public class VR_Rig : MonoBehaviour
{
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Transform headConstraint;
    public Vector3 headBodyOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - headConstraint.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = headConstraint.position + headBodyOffset; // this is for the position
        transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized; // this is for rotation around the y-axis

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
