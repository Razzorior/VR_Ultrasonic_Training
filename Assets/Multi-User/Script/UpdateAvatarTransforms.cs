using UnityEngine;
using Photon.Pun;


public class UpdateAvatarTransforms : MonoBehaviourPun
{
    [HideInInspector]
    public Transform _deviceHead, _deviceLeftHand, _deviceRightHand;
    public GameObject _avatarHead, _avatarLeftHand, _avatarRightHand;
    public bool _hideOwnAvatar = true;

    private void Start()
    {

        if (this.photonView.IsMine && _hideOwnAvatar)
        {
            _avatarLeftHand.transform.GetChild(0).gameObject.SetActive(false);
            _avatarRightHand.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        //only process input, if it comes from the client itself
        if (!this.photonView.IsMine)
            return;

        if (_deviceHead)
            _avatarHead.transform.SetPositionAndRotation(_deviceHead.position, _deviceHead.rotation);

        if (_deviceLeftHand)
        {
            _avatarLeftHand.transform.SetPositionAndRotation(_deviceLeftHand.position, _deviceLeftHand.rotation);
        }

        if (_deviceRightHand)
        {
            _avatarRightHand.transform.SetPositionAndRotation(_deviceRightHand.position, _deviceRightHand.rotation);
        }
    }
}