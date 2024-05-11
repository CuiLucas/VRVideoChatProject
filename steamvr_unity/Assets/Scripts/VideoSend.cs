using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoSend : MonoBehaviour
{
    WebCamTexture webCam;
    Texture2D currTexture;
    bool stop = false;

    [SerializeField] RawImage mSelfCamera;
    [SerializeField] SocketClient client;

    private void Start()
    {
        Application.runInBackground = true;
    }

    const int SEND_RECEIVE_COUNT = 15;
    public void Init()
    {
        stop = false;
        
        StartCoroutine(initAndWaitForWebCamTexture());
    }

    IEnumerator initAndWaitForWebCamTexture()
    {
        // Open the Camera on the desired device, in my case IPAD pro
        webCam = new WebCamTexture();
        // Get all devices , front and back camera
        webCam.deviceName = GetFrontFacingName();
        
        // request the lowest width and heigh possible
        webCam.requestedHeight = 300;
        webCam.requestedWidth = 300;
        
        mSelfCamera.texture = webCam;
        
        webCam.Play();
        
        currTexture = new Texture2D(webCam.width, webCam.height);

        yield return new WaitForSecondsRealtime(0.1f);
        
        //Start sending coroutine
        StartCoroutine(senderCOR());
    }

    private string GetFrontFacingName()
    {
        //Debug.Log("获取前置摄像头");
        WebCamDevice[] devices = WebCamTexture.devices;
        
        foreach (WebCamDevice device in devices)
        {
            if (device.isFrontFacing)
            {
                //Debug.Log("成功获取前置摄像头");
                return device.name;
            }
        }

        if (devices.Length > 0)
        {
            //Debug.Log("成功获取第一个摄像头");
            return devices[0].name;
        }
        else
        {
            Debug.LogError("没有找到摄像头");
            return "";
        }
    }
    
    bool readyToGetFrame = true;
    IEnumerator senderCOR()
    {
        //Debug.Log("开始发送图片的协程");
        readyToGetFrame = true;

        byte[] frameBytesLength = new byte[SEND_RECEIVE_COUNT];

        //Debug.Log("开始发送图片的while");
        while (!stop)
        {
            yield return new WaitForSecondsRealtime(0.07f);

            //Debug.Log("开始获取图片bytes");
            currTexture.SetPixels(webCam.GetPixels());
            byte[] pngBytes = currTexture.EncodeToPNG();
            //Fill total byte length to send. Result is stored in frameBytesLength
            byteLengthToFrameByteArray((int)InfoType.Video, frameBytesLength);

            //Set readyToGetFrame false
            readyToGetFrame = false;

            byte[] buffer = new byte[pngBytes.Length + SEND_RECEIVE_COUNT];

            Array.Copy(frameBytesLength, 0, buffer, 0, SEND_RECEIVE_COUNT);
            Array.Copy(pngBytes, 0, buffer, SEND_RECEIVE_COUNT, pngBytes.Length);

            //Debug.Log("获取图片成功");
            SendPNG(buffer);

            //Wait until we are ready to get new frame(Until we are done sending data)
            while (!readyToGetFrame)
            {
                //LOG("Waiting To get new frame");
                yield return null;
            }
        }
    }

    async void SendPNG(byte[] pngBytes)
    {
        //Debug.Log("开始发送图片bytes");
        await client.Send(pngBytes);
        //Debug.Log("发送图片成功");
        //Sent. Set readyToGetFrame true
        readyToGetFrame = true;
    }

    //Converts the data size to byte array and put result to the fullBytes array
    void byteLengthToFrameByteArray(int byteLength, byte[] fullBytes)
    {
        //Clear old data
        Array.Clear(fullBytes, 0, fullBytes.Length);
        //Convert int to bytes
        byte[] bytesToSendCount = BitConverter.GetBytes(byteLength);
        //Copy result to fullBytes
        bytesToSendCount.CopyTo(fullBytes, 0);
    }

    private void OnApplicationQuit()
    {
        stop = true;
    }
}