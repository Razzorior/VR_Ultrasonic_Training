using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class AdvancedNetworkManager : MonoBehaviourPunCallbacks
{
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = true;

    /// <summary>Used as PhotonNetwork.GameVersion.</summary>
    public byte Version = 1;
    public string userName = "default";
    public GameObject UserPrefabsToInstantiate;

    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;

    [Header("User Group")]
    public byte userGroup = 0;
    public GameObject[] userRolePrefabs;
    public string _roomName = "CollaborativeUsersPlanning";
    public byte _maxPlayer = 4;
    private bool rejoin;

    /// <summary>
    /// Checking internet connection variables
    /// </summary>
    private const bool allowCarrierDataNetwork = false;
    private const string pingAddress = "8.8.8.8"; // Google Public DNS server
    private const float waitingTime = 2.0f;
    private const float timeBetweenChecks = 3.0f;
    private float pingStartTime;
    private bool retryDone = false;

    [Header("Device Transforms")]
    [SerializeField]
    Transform _deviceHead, _deviceLeftHand, _deviceRightHand;

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public void Start()
    {
        if (this.AutoConnect)
        {
            this.ConnectNow();
        }

        var userTrans = UserPrefabsToInstantiate.GetComponent<UpdateAvatarTransforms>();
        if (_deviceHead)
            userTrans._deviceHead = _deviceHead;
        if (_deviceLeftHand)
            userTrans._deviceLeftHand = _deviceLeftHand;
        if (_deviceRightHand)
            userTrans._deviceRightHand = _deviceRightHand;
    
    }

    public void ConnectNow()
    {
        Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = this.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
    }


    // below, we implement some callbacks of the Photon Realtime API.
    // Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.");
        retryDone = true;
        PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions() { MaxPlayers = _maxPlayer }, null);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected to Lobby.");
        PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions() { MaxPlayers = _maxPlayer }, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed() was called by PUN. code: " + returnCode + " msg: "+ message);
        if (rejoin && !PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.LogError("Error trying to reconnect and rejoin");
        }
    }

    // the following methods are implemented to give you some context. re-implement them as needed.
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected(" + cause + ")");
        //StartCoroutine(RetryConnection());

        switch (cause)
        {
            // add other disconnect causes that could happen while joined
            case DisconnectCause.ClientTimeout:
                rejoin = true;
                Debug.Log("Cause error: " + cause);
                this.ConnectNow();
                //StartCoroutine(RetryConnection());
                break;
            default:
                rejoin = false;
                break;
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running.");

        if (UserPrefabsToInstantiate)
        {
            PhotonNetwork.LocalPlayer.NickName = userName;
            PhotonNetwork.Instantiate(UserPrefabsToInstantiate.name, Vector3.zero, Quaternion.identity, userGroup);
        }

    }

    #region Internetcheck

    private IEnumerator InternetCheck()
    {
        bool internetPossiblyAvailable = false;
        while (!internetPossiblyAvailable)
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    internetPossiblyAvailable = true;
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    internetPossiblyAvailable = allowCarrierDataNetwork;
                    break;
                default:
                    internetPossiblyAvailable = false;
                    break;
            }
            if (!internetPossiblyAvailable)
            {
                Debug.Log("Trying to rejoin Photon, Internet isn't available");
                //Wait to check again.
                yield return new WaitForSeconds(timeBetweenChecks);
            }
        }

        Ping ping = new Ping(pingAddress);
        pingStartTime = Time.time;

        while (!ping.isDone && Time.time - pingStartTime < waitingTime)
        {
            //Wait one frame;
            yield return null;
        }

        if (ping.isDone && ping.time >= 0)
        {
            Debug.Log("internet available");
            PhotonNetwork.ReconnectAndRejoin();
        }
        else
            Debug.Log("Sorry! there is no internet");
    }

    private IEnumerator RetryConnection()
    {
        pingStartTime = Time.time;
        while (!retryDone && Time.time - pingStartTime < waitingTime)
        {
            Debug.Log("Retry connection");
            PhotonNetwork.ReconnectAndRejoin();
            yield return null;
        }
    }

    public void ConnectOfflineMode()
    {
        //PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
        //PhotonNetwork.PhotonServerSettings.AppSettings.Port = 0;
        //PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
        Debug.Log("Start offline");
        PhotonNetwork.OfflineMode = true;
    }

    public void ConnectToNetwork(string IpAdress, int portNumber, bool isTCP)
    {
        Debug.Log("Connect IP: " + IpAdress);
        PhotonNetwork.PhotonServerSettings.StartInOfflineMode = false;
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = IpAdress;
        PhotonNetwork.PhotonServerSettings.AppSettings.Port = portNumber;
        var _protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
        if (isTCP)
            _protocol = ExitGames.Client.Photon.ConnectionProtocol.Tcp;

        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = _protocol;
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.SerializationProtocolType = ExitGames.Client.Photon.SerializationProtocol.GpBinaryV16;
        PhotonNetwork.GameVersion = this.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
        PhotonNetwork.ConnectUsingSettings();

    }

    public void ConnectOnlineNetwork()
    {
        Debug.Log("Start online");
        PhotonNetwork.PhotonServerSettings.StartInOfflineMode = false;
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
        PhotonNetwork.PhotonServerSettings.AppSettings.Port = 0;
        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
        this.ConnectNow();
    }

    #endregion


}
