using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum InfoType
{
    None = 0,
    Video = 1,
    Audio = 2,
    Max,
}

public class SocketClient : MonoBehaviour {

    [SerializeField] InputField ipInput;
    [SerializeField] InputField idInput;
    Socket client;
    IPEndPoint serverEndPoint;
    bool stop = false;
    int id;
    Dictionary<int, SocketPackage> packageCacha;

    [HideInInspector] public byte[] receivedBuffer;
    const int port = 8010;

    private void Start()
    {
        Application.runInBackground = true;
    }

    public void Init()
    {
        packageCacha = new Dictionary<int, SocketPackage>();
        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPAddress IP = IPAddress.Parse(ipInput.text);
        serverEndPoint = new IPEndPoint(IP, port);

        id = int.Parse(idInput.text);

        client.Connect(serverEndPoint);

        byte[] idBytes = new byte[15];
        SocketBytesUtils.IntToBytes(id, idBytes);

        // 先发送一条初始数据
        client.SendTo(idBytes, serverEndPoint);

        Thread t = new Thread(ReceiveMsg);
        t.IsBackground = true;
        t.Start(client);
    }

    void ReceiveMsg(object o)
    {
        Socket client = o as Socket;
        Debug.Log("开始接收");
        while (!stop)
        {
            try
            {
                ///用来保存发送方的ip和端口号
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);
                ///定义客户端接收到的信息大小
                byte[] buffer = new byte[1024 * 1024];
                Debug.Log("Before---ReceiveFrom");
                ///接收到的信息大小(所占字节数)
                int length = client.ReceiveFrom(buffer, ref point);

                byte[] singlePackage = new byte[length];
                Array.Copy(buffer, 0, singlePackage, 0, length);

                // 黑科技，查看收到的是什么类型的包，1：图片，2：音频
                if (singlePackage.Length > 45 && singlePackage[30] == 0)
                {
                    //Debug.LogError("type:" + singlePackage[45]);
                }

                Debug.Log("Before---CheckSinglePackage");
                CheckSinglePackage(singlePackage);
                //string message = Encoding.UTF8.GetString(buffer, 0, length);
                //listBox1.Items.Add(point.ToString() + "：" + message);
            }
            catch (Exception e)
            {
                Debug.Log("接收失败！ e:" + e);
                client.Close();
            }
        }
    }

    private void CheckSinglePackage(byte[] singlePackage)
    {
        if (singlePackage.Length <= SocketBytesUtils.GetHeadLength())
            return;

        int id = SocketBytesUtils.GetReceiveByteID(singlePackage);
        int count = SocketBytesUtils.GetReceivePackageCount(singlePackage);
        int index = SocketBytesUtils.GetReceivePackageIndex(singlePackage);
        SocketPackage socketPackage;
        if (packageCacha.ContainsKey(id))
        {
            socketPackage = packageCacha[id];
        }
        else
        {
            socketPackage = new SocketPackage();
            socketPackage.ID = id;
            socketPackage.count = count;
            socketPackage.list = new List<byte[]>();
            socketPackage.useByteLength = 0;
            packageCacha.Add(id, socketPackage);
        }
        socketPackage.AddBytes(singlePackage);
        // 是否是结束数据，即没有丢包，正常结束的完整数据
        bool endBytes = false;
        // 是否是错误数据
        bool errorBytes = false;

        // 本次发送的数据包量和之前的不一致，是错误数据
        if (socketPackage.count != count)
        {
            errorBytes = true;
        }
        else
        {
            // 是最后一条
            if (index == count - 1)
            {
                // list长度和count一致
                if (socketPackage.list.Count == count)
                {
                    // 有效数据
                    receivedBuffer = SocketBytesUtils.GetReceiveUseBytes(socketPackage.list, socketPackage.useByteLength);
                    endBytes = true;
                }
                else
                {
                    // 错误数据
                    errorBytes = true;
                }
            }
        }

        // 是结束的那个包 或 发现了丢包、错误数据
        if (endBytes || errorBytes)
        {
            // 删除此ID的缓存包
            packageCacha.Remove(id);
        }

        Debug.Log("CheckSinglePackage----End");
    }

    public async Task Send(byte[] buffer)
    {
        Debug.LogError("SocketClient----Send---Start");
        List<byte[]> list = SocketBytesUtils.GetByetsList(buffer, id);
        //Debug.LogError("发送：type(" + list[0][45] + ")");
        // 分包后的数据，依次发送
        for (int i = 0; i < list.Count; i++)
        {
            client.SendTo(list[i], serverEndPoint);
            await Task.Delay(1);
        }
        Debug.LogError("SocketClient----Send---End");
    }

    private void OnApplicationQuit()
    {
        stop = true;
        client.Close();
    }
}

public class SocketPackage
{
    public int ID;
    public int count;
    public int useByteLength;
    public List<byte[]> list;
    public void AddBytes(byte[] bytes)
    {
        list.Add(bytes);
        useByteLength += bytes.Length - SocketBytesUtils.GetHeadLength();
    }
}