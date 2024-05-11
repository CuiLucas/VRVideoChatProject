using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AudioReceive : MonoBehaviour
{
    [SerializeField] AudioSource mAudioSource;
    [SerializeField] SocketClient client;
    bool stop = false;

    const int RECEIVE_TYPE_LENGTH = 15;

    private void Start()
    {
        Application.runInBackground = true;
    }

    public void Init()
    {
        stop = false;

        Receiver();
    }

    async void Receiver()
    {
        while (!stop)
        {
            byte[] buffer = client.receivedBuffer;
            //LOG("获取音频buffer:" + buffer.ToString());
            if (buffer != null && buffer.Length > RECEIVE_TYPE_LENGTH)
            {
                //LOG("有效音频buffer， length" + buffer.Length);
                byte[] typeBytes = new byte[RECEIVE_TYPE_LENGTH];
                byte[] audioBytes = new byte[buffer.Length - RECEIVE_TYPE_LENGTH];
                Array.Copy(buffer, 0, typeBytes, 0, RECEIVE_TYPE_LENGTH);
                Array.Copy(buffer, RECEIVE_TYPE_LENGTH, audioBytes, 0, audioBytes.Length);

                InfoType type = (InfoType) BitConverter.ToInt32(typeBytes, 0);
                //LOGERROR("数据类型：" + type + ", 音频长度：" + audioBytes.Length);
                if (type == InfoType.Audio && audioBytes.Length > 0)
                {
                    client.receivedBuffer = null;
                    LOGERROR("播放音频, 音频数据长度：" + audioBytes.Length);
                    audioBytes = CompressedUtils.GZipDeCompress(audioBytes);
                    AudioClip audioClip = AudioUtils.AudioByteToClip(audioBytes, 1, AudioUtils.FREQUENCY);
                    mAudioSource.clip = audioClip;
                    mAudioSource.loop = false;
                    mAudioSource.Play();
                }
            }

            await Task.Delay(10);
        }
    }

    private void LOG(string str)
    {
        Debug.Log(str);
    }

    private void LOGERROR(string str)
    {
        Debug.LogError(str);
    }

    private void OnApplicationQuit()
    {
        stop = true;
    }
}