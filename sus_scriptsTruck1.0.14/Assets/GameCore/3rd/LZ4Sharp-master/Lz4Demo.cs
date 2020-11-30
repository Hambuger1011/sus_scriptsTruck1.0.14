using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using LZ4Sharp;

public class Lz4Demo : MonoBehaviour {
    [ContextMenu("RunLz4")]
    public void Runlz4()
    {
        string file = "Library/ScriptAssemblies/Assembly-CSharp.dll";
        var srcData = File.ReadAllBytes(file);
        var compressData = LZ4.Compress(srcData);
        var decompressData = LZ4.Decompress(compressData);
        bool bSuc = false;
        if(srcData.Length == decompressData.Length)
        {
            bSuc = true;
            for (int i=0;i<srcData.Length;++i)
            {
                if(srcData[i] != decompressData[i])
                {
                    bSuc = false;
                    break;
                }
            }
        }
        if(bSuc)
        {
            LOG.Info("压缩:" + file+",srcSize="+srcData.Length+",decompressSize="+compressData.Length+",rate="+((float)compressData.Length/srcData.Length));

        }
        else
        {
            LOG.Error("压缩失败:" + file);
        }
    }


    [ContextMenu("Run")]
    public void Run()
    {
        string directory = "O://bulletml0_21.zip";
        //if (args.Length != 1)
        //{
        //    Debug.Log("LZ4Sharp performance test");
        //    Debug.Log("usage: LZ4Sharp directory");
        //    Debug.Log("This will time compressing and decompressing all the files in 'directory' ignoring file read time");
        //    Debug.Log("Done. Press a key.");
        //    return;
        //}

        LOG.Info("This application is running as a " + (IntPtr.Size == 4 ? "32" : "64") + " bit process.");
        LOG.Info("\n");

        LOG.Info("Test LZ4 32 bit compression");
        TestEmpty(new LZ4Compressor32(), new LZ4Decompressor32());
        Test(directory, new LZ4Compressor32(), new LZ4Decompressor32());
        TestUnknownSize(directory, new LZ4Compressor32(), new LZ4Decompressor32());

        /* Example
        var compressor = LZ4CompressorFactory.CreateNew();
        var compressed = compressor.Compress(yourBytesHere);
        var decompressor = LZ4DecompressorFactory.CreateNew();
        var decompressed = decompressor.Decompress(compressed);
        // decompressed should equal yourBytesHere. If not please report a bug.
        */
        LOG.Info("==============================================");
        LOG.Info("Test LZ4 64 bit compression");
        TestEmpty(new LZ4Compressor64(), new LZ4Decompressor64());
        Test(directory, new LZ4Compressor64(), new LZ4Decompressor64());
        TestUnknownSize(directory, new LZ4Compressor64(), new LZ4Decompressor64());

        LOG.Info("Done. Press a key.");
        Console.ReadLine();
    }

    private static unsafe void TestEmpty(ILZ4Compressor compressor, ILZ4Decompressor decompressor)
    {
        var bytes = new byte[50];
        byte[] dst = compressor.Compress(bytes);
        var result = decompressor.Decompress(dst);
        LOG.Info(result.Length == 50);
    }

    private static void TestUnknownSize(string directory, ILZ4Compressor compressor, ILZ4Decompressor decompressor)
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            var bytes = File.ReadAllBytes(file);
            var compressed = compressor.Compress(bytes);
            var decompressed = decompressor.Decompress(compressed);
            if (!bytes.SequenceEqual(decompressed))
                throw new Exception("Compressing/Decompressing " + file + " failed.");
        }
    }

    private static void Test(string directory, ILZ4Compressor compressor, ILZ4Decompressor decompressor)
    {
        var w = new Stopwatch();
        var dw = new Stopwatch();
        long compressedTotal = 0;
        long uncompressedTotal = 0;

        for (int j = 0; j < 10; j++)
            foreach (var bytes in Read(directory))
            {
                uncompressedTotal += bytes.Length;
                byte[] compressed = new byte[compressor.CalculateMaxCompressedLength(bytes.Length)];
                w.Start();
                int compressedLength = compressor.Compress(bytes, compressed);
                compressedTotal += compressedLength;
                w.Stop();

                byte[] uncompressed = new byte[bytes.Length];

                dw.Start();
                decompressor.DecompressKnownSize(compressed, uncompressed, uncompressed.Length);
                dw.Stop();

                for (int i = 0; i < uncompressed.Length; i++)
                {
                    if (uncompressed[i] != bytes[i])
                        throw new Exception("Original bytes and decompressed bytes differ starting at byte " + i);
                }
            }

        LOG.Info("Ratio = " + compressedTotal * 1.0 / uncompressedTotal);
        LOG.Info("Compression Time (MB / sec) = " + uncompressedTotal / 1024.0 / 1024.0 / w.Elapsed.TotalSeconds);
        LOG.Info("Uncompression Time (MB / sec) = " + uncompressedTotal / 1024.0 / 1024.0 / dw.Elapsed.TotalSeconds);
    }

    static IEnumerable<byte[]> Read(string directory)
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            using (var reader = new BinaryReader(File.OpenRead(file)))
            {
                while (reader.BaseStream.Length != reader.BaseStream.Position)
                    yield return reader.ReadBytes(8 * 1024 * 1024);
            }
        }
    }
}
