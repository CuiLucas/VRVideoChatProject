using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class VideoReceive : MonoBehaviour
{
    [SerializeField] RawImage targetImage;
    [SerializeField] SocketClient client;
    [SerializeField] Text logText;
    Texture2D tex;

    bool stop = false;

    const int RECEIVE_TYPE_LENGTH = 15;

    private void Start()
    {
        Application.runInBackground = true;
    }

    public void Init()
    {
        stop = false;
        tex = new Texture2D(0, 0);
        ShowPNG();
        //StartCoroutine(CoroutineShowPNG());
    }

    /// <summary>
    /// 异步显示图片
    /// </summary>
    async void ShowPNG()
    {
        while (!stop)
        {
            byte[] buffer = client.receivedBuffer;

            //LOG("获取图片buffer:" + buffer.ToString());
            if (buffer != null && buffer.Length > RECEIVE_TYPE_LENGTH)
            {
                //LOG("有效图片buffer， length" + buffer.Length);
                byte[] typeBytes = new byte[RECEIVE_TYPE_LENGTH];
                byte[] pngBytes = new byte[buffer.Length - RECEIVE_TYPE_LENGTH];
                Array.Copy(buffer, 0, typeBytes, 0, RECEIVE_TYPE_LENGTH);
                Array.Copy(buffer, RECEIVE_TYPE_LENGTH, pngBytes, 0, pngBytes.Length);

                InfoType type = (InfoType) BitConverter.ToInt32(typeBytes, 0);
                if (type == InfoType.Video && pngBytes.Length > 0)
                {
                    client.receivedBuffer = null;

                    LOG("显示图片, 图片数据长度：" + pngBytes.Length);
                    bool result = tex.LoadImage(pngBytes);
                    if (result)
                    {
                        targetImage.texture = tex;
                    }
                    else
                    {
                        Debug.LogWarning("此图片数据错误，无法加载，不做显示-----------");
                    }
                }
            }
            await Task.Delay(50);
        }
    }

    /// <summary>
    /// 协程显示图片
    /// </summary>
    /// <returns></returns>
    IEnumerator CoroutineShowPNG()
    {
        while (!stop)
        {
            byte[] buffer = client.receivedBuffer;

            //LOG("获取图片buffer:" + buffer.ToString());
            if (buffer != null && buffer.Length > RECEIVE_TYPE_LENGTH)
            {
                //LOG("有效图片buffer， length" + buffer.Length);
                byte[] typeBytes = new byte[RECEIVE_TYPE_LENGTH];
                byte[] pngBytes = new byte[buffer.Length - RECEIVE_TYPE_LENGTH];
                Array.Copy(buffer, 0, typeBytes, 0, RECEIVE_TYPE_LENGTH);
                Array.Copy(buffer, RECEIVE_TYPE_LENGTH, pngBytes, 0, pngBytes.Length);

                InfoType type = (InfoType) BitConverter.ToInt32(typeBytes, 0);
                if (type == InfoType.Video && pngBytes.Length > 0)
                {
                    client.receivedBuffer = null;
                    LOG("显示图片, 图片数据长度：" + pngBytes.Length);
                    bool result = tex.LoadImage(pngBytes);
                    if (result)
                    {
                        targetImage.texture = tex;
                    }
                    else
                    {
                        Debug.LogWarning("此图片数据错误，无法加载，不做显示-----------");
                    }
                }
            }
            yield return new WaitForSecondsRealtime(50 / 1000f);
        }
    }

    private void LOG(string str)
    {
        Debug.Log(str);
        logText.text = str;
    }

    private void OnApplicationQuit()
    {
        stop = true;
    }
}