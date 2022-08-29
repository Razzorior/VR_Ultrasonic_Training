using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RPCProcedures : MonoBehaviourPun
{
    public void Awake()
    {
        if (this.photonView.IsMine)
        {
            GlobalReferences.instance._photonView = photonView;
            GlobalReferences.instance._localUser = this.gameObject;

            GlobalReferences.instance._audioSource = GetComponent<AudioSource>();

        }
    }
}
