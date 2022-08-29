using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GlobalReferences : MonoBehaviour
{
    protected GlobalReferences() { }

    public static GlobalReferences instance = null;

    [HideInInspector]
    public GameObject _localUser;

    [HideInInspector]
    public PhotonView _photonView;

    public AudioSource _audioSource { get; set; }

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }


    public PhotonView CommandsAndRPC()
    {
        if (_localUser)
            return _photonView;
        //return _localUser.GetComponent<PhotonView>();
        else
            return null;
    }

    public int GetLocalPlayerNetId()
    {
        return _photonView.OwnerActorNr;
        //return _localUser.GetComponent<PhotonView>().OwnerActorNr;
    }

    public bool _isLocalPlayer()
    {
        return _photonView.IsMine;
        //return _localUser.GetComponent<PhotonView>().IsMine;
    }

    public bool _isMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

}
