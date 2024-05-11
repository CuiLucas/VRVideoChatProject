using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SocketBytesUtils
{
    public static int MAX_BUFFER_LENGTH = 65045;
    public static int ID_LENGTH = 15;
    public static int LIST_COUNT = 15;
    public static int LIST_INDEX = 15;

    /// <summary>
    /// 分包，每个数据包包括:
    /// 用户ID + 包数量 + 此包索引 + 本包内有效数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static List<byte[]> GetByetsList(byte[] buffer, int id)
    {
        // 有效数据长度    = 最大传输长度      - ID占位    - 包数量     - 包索引
        int singleUseBytes = MAX_BUFFER_LENGTH - ID_LENGTH - LIST_COUNT - LIST_INDEX;

        // 包数量
        int listCount = buffer.Length / singleUseBytes;
        listCount = buffer.Length % singleUseBytes <= 0 ? listCount : listCount + 1;

        // ID -> bytes
        byte[] idFullBytes = new byte[ID_LENGTH];
        IntToBytes(id, idFullBytes);

        // 包数量 -> bytes
        byte[] listCountFullBytes = new byte[LIST_COUNT];
        IntToBytes(listCount, listCountFullBytes);

        // 初始化list
        List<byte[]> bytesList = new List<byte[]>();
        for (int i = 0; i < listCount; i++)
        {
            // 包索引 -> bytes
            byte[] listIndexFullBytes = new byte[LIST_INDEX];
            IntToBytes(i, listIndexFullBytes);

            // 拼包
            // 单个包所有数据
            byte[] singleFullBytes = new byte[MAX_BUFFER_LENGTH];

            // 最后一条，长度为剩余的所有byte
            if (i == listCount - 1)
            {
                singleUseBytes = buffer.Length - singleUseBytes * i;
                singleFullBytes = new byte[singleUseBytes + GetHeadLength()];
            }

            // 复制id数据
            int startCopyIndex = 0;
            Array.Copy(idFullBytes, 0, singleFullBytes, startCopyIndex, idFullBytes.Length);

            // 复制包数量数据
            startCopyIndex += idFullBytes.Length;
            Array.Copy(listCountFullBytes, 0, singleFullBytes, startCopyIndex, listCountFullBytes.Length);

            // 复制包索引数据
            startCopyIndex += listCountFullBytes.Length;
            Array.Copy(listIndexFullBytes, 0, singleFullBytes, startCopyIndex, listIndexFullBytes.Length);

            // 复制本包有效数据
            startCopyIndex += listIndexFullBytes.Length;

            Array.Copy(buffer, (MAX_BUFFER_LENGTH - GetHeadLength()) * i, singleFullBytes, startCopyIndex, singleUseBytes);

            // 添加到list内
            bytesList.Add(singleFullBytes);
        }

        return bytesList;
    }

    /// <summary>
    /// 获取接收到数据的用户ID
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static int GetReceiveByteID(byte[] buffer)
    {
        byte[] idBytes = new byte[ID_LENGTH];
        Array.Copy(buffer, 0, idBytes, 0, ID_LENGTH);
        return BytesToInt32(idBytes);
    }

    /// <summary>
    /// 获取接收到数据的包数量
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static int GetReceivePackageCount(byte[] buffer)
    {
        byte[] countBytes = new byte[LIST_COUNT];
        Array.Copy(buffer, ID_LENGTH, countBytes, 0, LIST_COUNT);
        return BytesToInt32(countBytes);
    }

    /// <summary>
    /// 获取接收到数据的包索引
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static int GetReceivePackageIndex(byte[] buffer)
    {
        byte[] indexBytes = new byte[LIST_INDEX];
        Array.Copy(buffer, ID_LENGTH + LIST_COUNT, indexBytes, 0, LIST_INDEX);
        return BytesToInt32(indexBytes);
    }

    /// <summary>
    /// 组包
    /// </summary>
    /// <param name="packageList"></param>
    /// <param name="useByteLength"></param>
    /// <returns></returns>
    public static byte[] GetReceiveUseBytes(List<byte[]> packageList, int useByteLength)
    {
        Debug.LogError("GetReceiveUseBytes: listCount(" + packageList.Count + "), useByteLength(" + useByteLength + ")");
        Debug.LogError("GetReceiveUseBytes: lastBytesLength(" + packageList[packageList.Count - 1].Length + ")");
        byte[] useBytes = new byte[useByteLength];
        // 单个包有效数据起点
        int headLength = GetHeadLength();
        for (int i = 0; i < packageList.Count; i++)
        {
            byte[] singlePackage = packageList[i];
            // 单个包内有效数据长度
            int singleUseLength = singlePackage.Length - headLength;
            // 此包的索引，一般情况下与i相等
            int singleIndex = GetReceivePackageIndex(singlePackage);
            // 单个包内有效数据最大长度
            int singleMaxUseLength = MAX_BUFFER_LENGTH - headLength;
            // 最后一个包
            if (i == packageList.Count - 1)
            {
                // 有效数据 = 接收数组长度 - 之前所有数据长度
                singleUseLength = useBytes.Length - singleIndex * singleMaxUseLength;
            }

            if (singleUseLength > useBytes.Length - singleIndex * singleMaxUseLength || singleUseLength <= 0)
            {
                Debug.LogError("数据接收错误");
            }
            // 依次拷贝进useBytes
            Array.Copy(singlePackage, headLength, useBytes, singleIndex * singleMaxUseLength, singleUseLength);
        }
        return useBytes;
    }

    /// <summary>
    /// bytes转int
    /// </summary>
    /// <returns></returns>
    public static int BytesToInt32(byte[] bytes)
    {
        int id = BitConverter.ToInt32(bytes, 0);
        return id;
    }

    /// <summary>
    /// 获取包头长度
    /// </summary>
    /// <returns></returns>
    public static int GetHeadLength()
    {
        return ID_LENGTH + LIST_COUNT + LIST_INDEX;
    }

    /// <summary>
    /// Int转Bytes
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fullBytes"></param>
    public static void IntToBytes(int value, byte[] fullBytes)
    {
        // 清空数据
        Array.Clear(fullBytes, 0, fullBytes.Length);
        //Convert int to bytes
        byte[] bytesToSendCount = BitConverter.GetBytes(value);
        //Copy result to fullBytes
        bytesToSendCount.CopyTo(fullBytes, 0);
    }
}