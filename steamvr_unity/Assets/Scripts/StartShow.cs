using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartShow : MonoBehaviour
{
    [SerializeField] Button connectBtn;
    [SerializeField] SocketClient mSocket;
    [SerializeField] VideoSend mVideoSend;
    [SerializeField] VideoReceive mVideoReceive;
    [SerializeField] AudioSend mAudioSend;
    [SerializeField] AudioReceive mAudioReceive;
    
    void Start()
    {
        connectBtn.onClick.AddListener(OnConnectClick);
    }

    private void OnConnectClick()
    {
        InitAll();
    }

    private void InitAll()
    {
        mSocket.Init();
        mVideoSend.Init();
        mVideoReceive.Init();
        mAudioSend.Init();
        mAudioReceive.Init();
    }
}