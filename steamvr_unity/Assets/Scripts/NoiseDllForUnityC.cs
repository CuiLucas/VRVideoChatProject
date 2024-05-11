using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NoiseDllForUnityC
{
    [DllImport("NoiseDllForUnity", CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr Delete(int a, int b);
    [DllImport("NoiseDllForUnity", CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr Noise(IntPtr wavin, int fs, int wav_length);
}