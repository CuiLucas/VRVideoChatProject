using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AudioSend : MonoBehaviour
{
    bool stop = false;
    AudioClip mAudioClip;
    string mDevice;

    [SerializeField] SocketClient client;

    const int SEND_TYPE_LENGTH = 15;

    private void Start()
    {
        Application.runInBackground = true;
    }

    public void Init()
    {
        stop = false;

        mDevice = Microphone.devices[0];

        StartCoroutine(StartSendBytes());
    }

    private IEnumerator StartSendBytes()
    {
        //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //stopwatch.Start();

        mAudioClip = Microphone.Start(mDevice, true, 60, AudioUtils.FREQUENCY);

        //stopwatch.Stop();
        //Debug.LogError("麦克风启动时间：" + stopwatch.ElapsedMilliseconds);

        yield return new WaitForSecondsRealtime(2.5f);
        lastPoint = Microphone.GetPosition(mDevice);

        while (!stop)
        {
            //Set readyToGetFrame false
            readyToGetFrame = false;
            SendBytes();
            //StartCoroutine(CoroutineSendBytes());
            while (!readyToGetFrame)
            {
                if (stop)
                {
                    break;
                }
                yield return Task.Delay(5);
            }
        }
    }

    bool readyToGetFrame = false;
    int lastPoint = 0;
    /// <summary>
    /// 异步发送音频
    /// </summary>
    private async void SendBytes()
    {
        //LOG("开始发送音频");
        await Task.Delay(500);
        int point = Microphone.GetPosition(mDevice);

        if (point > lastPoint)
        {
            float[] audioFloats = new float[point - lastPoint];
            mAudioClip.GetData(audioFloats, lastPoint);
            lastPoint = point;
            byte[] audioBytes = AudioUtils.FloatToByte(audioFloats);
            audioBytes = CompressedUtils.GZipCompress(audioBytes);

            byte[] frameBytesLength = new byte[SEND_TYPE_LENGTH];
            //Fill total byte length to send. Result is stored in frameBytesLength
            byteLengthToFrameByteArray((int) InfoType.Audio, frameBytesLength);

            byte[] buffer = new byte[audioBytes.Length + SEND_TYPE_LENGTH];

            Array.Copy(frameBytesLength, 0, buffer, 0, SEND_TYPE_LENGTH);
            Array.Copy(audioBytes, 0, buffer, SEND_TYPE_LENGTH, audioBytes.Length);

            //LOGERROR("音频数据长度：" + buffer.Length);
            await client.Send(buffer);
            
            LOG("发送音频结束");
        }
        else
        {
            //LOG("本轮录音结束");
            lastPoint = 0;
        }
        readyToGetFrame = true;
    }

    /// <summary>
    /// 协程发送音频
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoroutineSendBytes()
    {
        //LOG("开始发送音频");
        yield return new WaitForSecondsRealtime(100 / 1000f);
        int point = Microphone.GetPosition(mDevice);

        if (point > lastPoint)
        {
            float[] audioFloats = new float[point - lastPoint];
            mAudioClip.GetData(audioFloats, lastPoint);
            lastPoint = point;
            byte[] audioBytes = AudioUtils.FloatToByte(audioFloats);
            audioBytes = CompressedUtils.GZipCompress(audioBytes);

            byte[] frameBytesLength = new byte[SEND_TYPE_LENGTH];
            //Fill total byte length to send. Result is stored in frameBytesLength
            byteLengthToFrameByteArray((int) InfoType.Audio, frameBytesLength);

            byte[] buffer = new byte[audioBytes.Length + SEND_TYPE_LENGTH];

            Array.Copy(frameBytesLength, 0, buffer, 0, SEND_TYPE_LENGTH);
            Array.Copy(audioBytes, 0, buffer, SEND_TYPE_LENGTH, audioBytes.Length);

            //LOGERROR("音频数据长度：" + buffer.Length);
            yield return client.Send(buffer);

            LOG("发送音频结束");
        }
        else
        {
            //LOG("本轮录音结束");
            lastPoint = 0;
        }
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

    private void LOG(string msg)
    {
        Debug.Log(msg);
    }

    private void LOGERROR(string msg)
    {
        Debug.LogError(msg);
    }

    private void OnApplicationQuit()
    {
        stop = true;
        Microphone.End(mDevice);
    }
}