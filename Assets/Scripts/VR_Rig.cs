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
        Vector3 vrRotation = vrTarget.localRotation.eulerAngles;
        rigTarget.localRotation = Quaternion.Euler(vrRotation.y, -vrRotation.z, -vrRotation.x) * Quaternion.Euler(rotationOffset);
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
        // This must be reworked as it causes spinning of the avatar.
        //transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized; // this is for rotation around the y-axis
        // The following works as you intended:
        /*
         * Vector3 tfRot = transform.rotation.eulerAngles;
         * tfRot.y = headConstraint.localRotation.eulerAngles.x;
         * transform.rotation = Quaternion.Euler(tfRot);
         */
        // This is again due to different local koordinate systems beeing used. But this code causes the head to stay still in one direction ..
        // .. while the body moves. So you have to memorize theses changes and add them to the rotation Offset of the head so that the body rotation doesn't just
        // cancel the head rotation out. 

        head.Map();
        leftHand.Map();
        rightHand.Map();

        // Another great addition would be to keep the feet on ground level. Additionally one could take the starting height of the user to change the avatar size at the beginning
        // To match the size of the user. 
    }
}
