using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace TaskManager.Core
{
    /// <summary>
    /// 文件压缩帮助类库
    /// </summary>
    public class CompressHelper
    {
        /// <summary>
        /// 二进制数组转压缩文件
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="path"></param>
        public static void ConvertToFile(byte[] buff, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
            {
                FileStream _FileStream = new FileStream(path, FileMode.CreateNew);
                BinaryWriter _BinaryWriter = new BinaryWriter(_FileStream);
                _BinaryWriter.Write(buff, 0, buff.Length);
                _BinaryWriter.Close();
                _FileStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通用解压 支持rar,zip  TBD
        /// </summary>
        /// <param name="compressfilepath"></param>
        /// <param name="uncompressdir"></param>
        public static void UnCompress(string compressfilepath, string uncompressdir)
        {
            string ext = Path.GetExtension(compressfilepath).ToLower();
            if (ext == ".rar")
                UnRar(compressfilepath, uncompressdir);
            else if (ext == ".zip")
                UnZip(compressfilepath, uncompressdir);
        }
        /// <summary>
        /// 解压rar
        /// </summary>
        /// <param name="compressfilepath"></param>
        /// <param name="uncompressdir"></param>
        private static void UnRar(string compressfilepath, string uncompressdir)
        {
            using (Stream stream = File.OpenRead(compressfilepath))
            {
                using (var reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(uncompressdir,  new ExtractionOptions() { ExtractFullPath=true,Overwrite= true});
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 解压zip
        /// </summary>
        /// <param name="compressfilepath"></param>
        /// <param name="uncompressdir"></param>
        private static void UnZip(string compressfilepath, string uncompressdir)
        {
            using (var archive = ArchiveFactory.Open(compressfilepath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(uncompressdir, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }
        }
    }
}
