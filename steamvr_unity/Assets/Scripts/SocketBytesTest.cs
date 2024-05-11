using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketBytesTest : MonoBehaviour
{

    void Start()
    {
        int bytesLength = 97889;
        byte[] bytes = new byte[bytesLength];
        for (int i = 0; i < bytesLength; i++)
        {
            bytes[i] = (byte) i;
        }
        List<byte[]> list = SocketBytesUtils.GetByetsList(bytes, 1);

        byte[] afterbytes = SocketBytesUtils.GetReceiveUseBytes(list, bytesLength);

        for (int i = 0; i < bytesLength; i++)
        {
            if (bytes[i] != afterbytes[i])
            {
                Debug.LogError("数据不同, i(" + i + ")");
                break;
            }
        }

        Debug.LogError("结束");
    }
}