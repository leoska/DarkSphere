using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;

public class SFS2X_Connect : MonoBehaviour {

    // XML Config
    public string ConfigFile = "Scripts/sfs-config.xml";
    public bool UseConfigFile = false;

    // Init values
    public string ServerIP = "192.168.1.170";
    public int ServerPort = 9933;
    public string ZoneName = "BasicExamples";
    public string UserName = "";
    public string RoomName = "The Lobby";

    SmartFox sfs;

	// Use this for initialization
	void Start () {
        sfs = new SmartFox();
        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoad);
        sfs.AddEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigFail);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);

        if (UseConfigFile)
        {
            sfs.LoadConfig(Application.dataPath + "/" + ConfigFile);
        }
        else
        {
            sfs.Connect(ServerIP, ServerPort);
        }
    }

    void OnConfigLoad(BaseEvent e)
    {
        Debug.Log("Config File Loaded");
        sfs.Connect(sfs.Config.Host, sfs.Config.Port);
    }

    void OnConfigFail(BaseEvent e)
    {
        Debug.Log("Failed to load Config File");
    }

    void OnLogin(BaseEvent e)
    {
        Debug.Log("Loggin In: " + e.Params["user"]);
        sfs.Send(new JoinRoomRequest(RoomName));
    }

    void OnLoginError(BaseEvent e)
    {
        Debug.Log("Login Error (" + e.Params["errorCode"] + "): " + e.Params["errorMessage"]);
    }

    void OnJoinRoom(BaseEvent e)
    {
        Debug.Log("Joined Room: " + e.Params["room"]);
        sfs.Send(new PublicMessageRequest("Hello World!"));
    }

    void OnJoinRoomError(BaseEvent e)
    {
        Debug.Log("JoinRoom Error (" + e.Params["errorCode"] + "): " + e.Params["errorMessage"]);
    }

    void OnPublicMessage(BaseEvent e)
    {
        Room room = (Room)e.Params["room"];
        User sender = (User)e.Params["sender"];
        Debug.Log("[" + room.Name + "] " + sender.Name + ": " + e.Params["message"]);
    }

    void OnConnection(BaseEvent e)
    {
        if ((bool)e.Params["success"])
        {
            Debug.Log("Successfully Connected!");
            if (UseConfigFile)
                ZoneName = sfs.Config.Zone;
            sfs.Send(new LoginRequest(UserName, "", ZoneName));
        }
        else
        {
            Debug.Log("Connection Failed");
        }
    }
	
	// Update is called once per frame
	void Update () {
        sfs.ProcessEvents();
	}

    private void OnApplicationQuit()
    {
        if (sfs.IsConnected)
            sfs.Disconnect();
    }
}
