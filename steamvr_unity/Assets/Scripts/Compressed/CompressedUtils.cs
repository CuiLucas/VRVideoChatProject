using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CompressedUtils
{
    /// <summary>
    /// 压缩算法
    /// </summary>
    /// <param name="pBytes"></param>
    /// <returns></returns>
    public static byte[] Compress(byte[] pBytes)
    {
        MemoryStream mMemory = new MemoryStream();
        Deflater mDeflater = new Deflater(Deflater.BEST_COMPRESSION);
        using (DeflaterOutputStream mStream = new DeflaterOutputStream(mMemory, mDeflater, 131072))
        {
            mStream.Write(pBytes, 0, pBytes.Length);
        }

        return mMemory.ToArray();
    }

    /// <summary>
    /// 解压缩算法
    /// </summary>
    /// <param name="pBytes"></param>
    /// <returns></returns>
    public static byte[] DeCompress(byte[] pBytes)
    {
        MemoryStream mMemory = new MemoryStream();
        using (InflaterInputStream mStream = new InflaterInputStream(new MemoryStream(pBytes)))
        {
            Int32 mSize;
            byte[] mWriteData = new byte[4096];
            while (true)
            {
                mSize = mStream.Read(mWriteData, 0, mWriteData.Length);
                if (mSize > 0)
                    mMemory.Write(mWriteData, 0, mSize);
                else
                    break;
            }
        }
        return mMemory.ToArray();
    }

    public static byte[] GZipCompress(byte[] bytesToCompress)
    {

        byte[] rebyte = null;
        MemoryStream ms = new MemoryStream();

        GZipOutputStream s = new GZipOutputStream(ms);
        s.Write(bytesToCompress, 0, bytesToCompress.Length);
        s.Close();
        rebyte = ms.ToArray();

        ms.Close();
        return rebyte;
        //return ms.ToArray();
    }

    public static byte[] GZipDeCompress(byte[] data)
    {
        int orginalLen = data.Length;
        int maxDecompressLen = 20 * orginalLen;

        if (maxDecompressLen < 100000) //缓冲区最小100K,最大8M,原始数据如果大于25KB，则解压缓冲为20倍原始数据大小
        {
            maxDecompressLen = 100000;
        }
        if (maxDecompressLen > 8000000)
        {
            maxDecompressLen = 8000000;
        }
        byte[] decompressByteArr = new byte[maxDecompressLen];
        //int len = 0;
        int read = -1;
        try
        {


            GZipInputStream s2 = new GZipInputStream(new MemoryStream(data));
            MemoryStream outStream = new MemoryStream();

            read = s2.Read(decompressByteArr, 0, decompressByteArr.Length);
            while (read > 0)
            {
                outStream.Write(decompressByteArr, 0, read);
                read = s2.Read(decompressByteArr, 0, decompressByteArr.Length);
            }
            s2.Close();
            return outStream.ToArray();

        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.StackTrace);
            ////new clsWriterErr().writerErr("ZipCompress", "DeCompress", ex);
            //Log4netHelper.InvokeErrorLog(MethodBase.GetCurrentMethod().DeclaringType, "ZipCompress", ex);
            throw new Exception(string.Format("字节数组解压出现问题：{0}" + ex.Message), ex);
        }

        //return decompressByteArr;
    }
}