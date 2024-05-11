using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Compressed : MonoBehaviour
{
    [SerializeField] Text logText;
    
    AudioClip mAudioClip;
    string mDevice;
    
    void Start()
    {
        mDevice = Microphone.devices[0];
        mAudioClip = Microphone.Start(mDevice, true, 120, AudioUtils.FREQUENCY);

        StartCoroutine(StartTest());
    }

    private IEnumerator StartTest()
    {
        // 等待麦克风启动
        yield return new WaitForSecondsRealtime(3);

        int lastPos = Microphone.GetPosition(mDevice);

        // 采1秒数据
        yield return new WaitForSecondsRealtime(1);

        int position = Microphone.GetPosition(mDevice);
        
        float[] audioFloats = new float[position - lastPos];
        mAudioClip.GetData(audioFloats, lastPos);

        byte[] audioBytes = AudioUtils.FloatToByte(audioFloats);

        //byte[] afterBytes = CompressedUtils.Compress(audioBytes);

        //byte[] overBytes = CompressedUtils.DeCompress(afterBytes);
        
        byte[] afterBytes = CompressedUtils.GZipCompress(audioBytes);

        byte[] overBytes = CompressedUtils.GZipDeCompress(afterBytes);

        yield return new WaitForSecondsRealtime(1);

        string initStr = "初始长度：" + audioBytes.Length + "\n";
        string afterStr = "压缩后：" + afterBytes.Length + "\n";
        string overStr = "解压后：" + overBytes.Length + "\n";

        string compareResult = "对比结果：";
        if (audioBytes.Length != overBytes.Length)
        {
            compareResult += "长度不同\n";
        }
        else
        {
            compareResult += "长度相同\n";
            bool haveDif = false;
            for (int i = 0; i < audioBytes.Length; i++)
            {
                if (audioBytes[i] != overBytes[i])
                {
                    haveDif = true;
                    break;
                }
            }
            compareResult += haveDif ? "数据不同" : "数据相同";
        }

        logText.text = initStr + afterStr + overStr + compareResult;
    }

    private void OnApplicationQuit()
    {
        Microphone.End(mDevice);
    }
}