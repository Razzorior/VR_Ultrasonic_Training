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
        rigTarget.localRotation = Quaternion.Euler(-vrRotation.y, vrRotation.z, -vrRotation.x) * Quaternion.Euler(rotationOffset);
    }

    public void ApplyAdditionalOffset(float offset)
    {
        if (offset != 0) { rigTarget.localRotation *= Quaternion.Euler(-offset, 0f, 0f); }
    }
}

public class VR_Rig : MonoBehaviour
{
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Transform spine;
    public Transform headConstraint;
    public Vector3 headBodyOffset;
    public Vector3 spineOffset;

    public float maxHeadRotation = 70f;
    public float maxSpineRotation = 20f;

    // Values needed to track rotation on the 0 and 360 degree threshold
    private float totalRotation = 0f;
    private float lastYRotation = 0f;
    private float currentHeadOffset = 0f;
    private float currentSpineRotation = 0f;
    
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


        //Vector3 tfRot = transform.rotation.eulerAngles;
        //tfRot.y = headConstraint.localRotation.eulerAngles.x;
        //transform.rotation = Quaternion.Euler(tfRot);
        // This is again due to different local koordinate systems beeing used. But this code causes the head to stay still in one direction ..
        // .. while the body moves. So you have to memorize theses changes and add them to the rotation Offset of the head so that the body rotation doesn't just
        // cancel the head rotation out. 

        head.Map();
        head.ApplyAdditionalOffset(currentHeadOffset);
        totalRotation += GetTotalYRoationChange(head);
        ManageSpineAndBodyRotation(head);
        
        leftHand.Map();
        rightHand.Map();


        // Another great addition would be to keep the feet on ground level. Additionally one could take the starting height of the user to change the avatar size at the beginning
        // To match the size of the user. 
    }

    private float GetTotalYRoationChange(VRMap head)
    {
        float headRotation = head.vrTarget.transform.localRotation.eulerAngles.y;
        float rotationChange = head.vrTarget.transform.localRotation.eulerAngles.y - lastYRotation;

        if(rotationChange > 250f)
        {
            rotationChange = headRotation - (lastYRotation + 360f);
        }
        else if (rotationChange < -250f)
        {
            rotationChange = (headRotation + 360f) - lastYRotation;
        }

        lastYRotation = headRotation; 
        return rotationChange;
    }

    private void ManageSpineAndBodyRotation(VRMap head)
    {
        float headRotation = totalRotation - currentSpineRotation - currentHeadOffset;
        Debug.Log(headRotation + currentSpineRotation);

        if (headRotation + currentSpineRotation > maxHeadRotation)
        {

            float newSpineRotation = headRotation - maxHeadRotation;
            currentSpineRotation += newSpineRotation;
            if (currentSpineRotation <= maxSpineRotation)
            {
                spine.localRotation *= Quaternion.Euler(-newSpineRotation, 0f, 0f);
                Vector3 rot = head.rigTarget.localRotation.eulerAngles;
                head.rigTarget.localRotation = Quaternion.Euler(-maxHeadRotation, rot.y, rot.z);
            }
            else
            {
                spine.localRotation *= Quaternion.Inverse(Quaternion.Euler(-(currentSpineRotation - newSpineRotation), 0f, 0f));
                head.rigTarget.localRotation *= Quaternion.Euler(maxHeadRotation, 0f, 0f);
                this.transform.rotation *= Quaternion.Euler(0f, 90f, 0f);
                currentSpineRotation = 0f;
                currentHeadOffset += 90f;
            }
        }
        else if (headRotation + currentSpineRotation < -maxHeadRotation)
        {

            float newSpineRotation = headRotation + maxHeadRotation;
            currentSpineRotation += newSpineRotation;
            if (currentSpineRotation >= -maxSpineRotation)
            {
                spine.localRotation *= Quaternion.Euler(-newSpineRotation, 0f, 0f);
                Vector3 rot = head.rigTarget.localRotation.eulerAngles;
                head.rigTarget.localRotation = Quaternion.Euler(maxHeadRotation, rot.y, rot.z);
            }
            else
            {
                spine.localRotation *= Quaternion.Inverse(Quaternion.Euler(-(currentSpineRotation - newSpineRotation), 0f,  0f));
                head.rigTarget.localRotation *= Quaternion.Inverse(Quaternion.Euler(maxHeadRotation, 0f, 0f));
                this.transform.rotation *= Quaternion.Euler(0f, -90f, 0f);
                currentSpineRotation = 0f;
                currentHeadOffset -= 90f;
            }
        }
    }

}
