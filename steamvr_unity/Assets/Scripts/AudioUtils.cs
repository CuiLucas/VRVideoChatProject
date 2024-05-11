using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AudioUtils
{
    public static int FREQUENCY = 8000;

    /// <summary>
    /// 音频转bytes
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public static byte[] FloatToByte(float[] data)
    {
        int rescaleFactor = 32767; //to convert float to Int16
        byte[] outData = new byte[data.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            short temshort = (short) (data[i] * rescaleFactor);
            byte[] temdata = BitConverter.GetBytes(temshort);
            outData[i * 2] = temdata[0];
            outData[i * 2 + 1] = temdata[1];
        }
        return outData;
    }

    /// <summary>
    /// 音频转bytes
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public static byte[] AudioClipToByte(AudioClip clip)
    {
        float[] data = new float[clip.samples];
        clip.GetData(data, 0);
        return FloatToByte(data);
    }

    /// <summary>
    /// bytes转音频
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static AudioClip AudioByteToClip(byte[] bytes, int channels, int frequency)
    {
        if (bytes.Length <= 0)
        {
            return null;
        }

        float[] data = new float[bytes.Length / 2];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = ByteToFloat(bytes[i * 2], bytes[i * 2 + 1]);
        }
        //data = NoiseReduction(data);
        AudioClip clip = AudioClip.Create("Clip", data.Length, channels, frequency, false);
        clip.SetData(data, 0);
        return clip;
    }

    /// <summary>
    /// byte转float
    /// </summary>
    /// <param name="firstByte"></param>
    /// <param name="secondByte"></param>
    /// <returns></returns>
    private static float ByteToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)
        //小端和大端顺序要调整
        short s;
        if (BitConverter.IsLittleEndian)
            s = (short) ((secondByte << 8) | firstByte);
        else
            s = (short) ((firstByte << 8) | secondByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    /// <summary>
    /// 降噪
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    //public unsafe static float[] NoiseReduction(float[] data)
    //{
    //    // float数组转为double
    //    double[] datain = new double[data.Length];
    //    for (int i = 0; i < data.Length; i++)
    //    {
    //        datain[i] = data[i];
    //    }
    //    // double数组转IntPtr
    //    IntPtr intptr;
    //    fixed (double* dataPoint = datain)
    //    {
    //        intptr = new IntPtr(dataPoint);
    //    }
    //    // 调用降噪方法
    //    IntPtr ptrOut = NoiseDllForUnityC.Noise(intptr, FREQUENCY, data.Length);
    //    // intptr转double数组
    //    double* pointOut = (double*) ptrOut.ToPointer();
    //    double[] dataout = new double[data.Length];
    //    for (int i = 0; i < dataout.Length; i++)
    //    {
    //        dataout[i] = pointOut[i];
    //    }
    //    Debug.LogError("降噪后数组长度：" + dataout.Length);
    //    // double数组转float数组
    //    float[] result = new float[dataout.Length];
    //    for (int i = 0; i < result.Length; i++)
    //    {
    //        result[i] = (float) dataout[i];
    //    }
    //    return result;
    //}
}