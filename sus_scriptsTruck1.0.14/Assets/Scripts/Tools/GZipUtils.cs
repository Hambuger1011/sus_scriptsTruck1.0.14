using System.IO;
using System.IO.Compression;
using System.Text;

public class GZipUtils
     {
          /// <summary>
         /// 压缩指定字符串
          /// </summary>
          /// <param name="str">待压缩字符串</param>
          /// <returns>返回压缩后的字节数组</returns>
         public static byte[] Compress(string str)
         {
              if (string.IsNullOrEmpty(str)) return null;
  
              byte[] bytes = Encoding.UTF8.GetBytes(str);
              return Compress(bytes);
          }
  
         /// <summary>
          /// 压缩指定字节数组
          /// </summary>
          /// <param name="bytes">待压缩字节数组</param>
          /// <returns>返回压缩后的字节数组</returns>
          public static byte[] Compress(byte[] bytes)
          {
              if (bytes == null || bytes.Length <= 0) return bytes;
  
              using (var compressedStream = new MemoryStream())
              {
                  using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
                  {
                      compressionStream.Write(bytes, 0, bytes.Length);
                  }
                  return compressedStream.ToArray();
              }
          }
  
          /// <summary>
          /// 压缩指定字符串到指定文件中
          /// </summary>
          /// <param name="compressData">待压缩字符串</param>
          /// <param name="zipFilePath">压缩后的文件路径</param>
          public static void CompressToFile(string compressData, string zipFilePath)
          {
              if (!string.IsNullOrEmpty(compressData))
              {
                  byte[] bytes = Encoding.UTF8.GetBytes(compressData);
                  using (var originalStream = new MemoryStream(bytes))
                  {
                      using (FileStream compressedStream = File.Create(zipFilePath))
                      {
                          using (GZipStream compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
                          {
                              originalStream.CopyTo(compressionStream);
                          }
                      }
                  }
              }
          }
  
          /// <summary>
          /// 从指定字节数组解压出字符串
          /// </summary>
          /// <param name="bytes">待解压的字节数组</param>
          /// <returns>返回解压后的字符串</returns>
          public static string DecompressToString(byte[] bytes)
          {
              var result = Decompress(bytes);
              if (result == null || result.Length <= 0) return string.Empty;
  
              return Encoding.UTF8.GetString(result);
          }
  
          /// <summary>
          /// 从指定字节数组解压出字节数组
          /// </summary>
          /// <param name="bytes">待解压的字节数组</param>
          /// <returns>返回解压后的字节数组</returns>
          public static byte[] Decompress(byte[] bytes)
          {
              if (bytes == null || bytes.Length <= 0) return bytes;
  
              using (var originalStream = new MemoryStream(bytes))
              {
                  using (var decompressedStream = new MemoryStream())
                  {
                      using (var decompressionStream = new GZipStream(originalStream, CompressionMode.Decompress))
                      {
                          decompressionStream.CopyTo(decompressedStream);
                      }
                      return decompressedStream.ToArray();
                  }
              }
          }
  
         /// <summary>
         /// 从指定文件中解压出字符串
         /// </summary>
         /// <param name="zipFilePath">待解压的文件路径</param>
         /// <returns>返回解压后的字符串</returns>
         public static string DecompressFromFile(string zipFilePath)
         {
             if (File.Exists(zipFilePath))
             {
                 using (FileStream originalStream = File.Open(zipFilePath, FileMode.Open))
                 {
                     using (MemoryStream decompressedStream = new MemoryStream())
                     {
                         using (GZipStream decompressionStream = new GZipStream(originalStream, CompressionMode.Decompress))
                         {
                             decompressionStream.CopyTo(decompressedStream);
                         }
                         byte[] bytes = decompressedStream.ToArray();
                         return Encoding.UTF8.GetString(bytes);
                     }
                 }
             }
             return string.Empty;
         }
     }
