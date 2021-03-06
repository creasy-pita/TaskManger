﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BSF.Serialization
{
    public class BinaryProvider
    {
        /// <summary>
        /// 序列化对象(二进制)
        /// </summary>
        /// <typeparam name=“T“>需要序列化的对象类型，必须声明[Serializable]特征</typeparam>
        /// <param name=“obj“>需要序列化的对象</param>
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 反序列化对象(二进制)
        /// </summary>
        /// <param name=“bytes“>需要反序列化的字符串</param>
        public static object Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(ms);
            }
        }
    }
}
