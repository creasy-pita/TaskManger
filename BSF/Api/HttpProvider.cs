using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSF.Serialization;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace BSF.Api
{
    /// <summary>
    /// Http操作类库
    /// 包含Http的socket连接池，优化性能
    /// </summary>
    public class HttpProvider
    {
        [Obsolete("字典传参方式会有一定的局限,已废弃")]
        public string Post(string url, Dictionary<string, string> param)
        {
            var content = new FormUrlEncodedContent(param);
            return PostBase(url, content);
        }
        public string Post(string url, List<KeyValuePair<string, string>> param)
        {
            var content = new FormUrlEncodedContent(param);
            return PostBase(url, content);
        }

        public string Post(string url, List<KeyValuePair<string, string>> param, List<KeyValuePair<string, HttpFileInfo>> fileparam)
        {

            var form = new MultipartFormDataContent();

            foreach (var p in param)
            {
                form.Add(new StringContent(p.Value), p.Key);
            }
            foreach (var f in fileparam)
            {
                form.Add(new ByteArrayContent(f.Value.UploadFileBytes, 0, f.Value.UploadFileBytes.Count()), f.Key, f.Value.FileName);
            }
            return PostBase(url, form);
        }

        public string PostWithJson(string url, Dictionary<string, Object> param)
        {
            var content = new StringContent(new JsonProvider(Serialization.JsonAdapter.EnumJsonMode.JavaScriptBussiness).Serializer(param));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return PostBase(url, content);
        }


        private string PostBase(string url,HttpContent content)
        {
            //此处未来需要添加HttpClient连接池,复用连接
            using (var client = new HttpClient())
            {
                var result = client.PostAsync(url, content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }
    }

    /// <summary>
    /// Http 文件传输参数
    /// </summary>
    public class HttpFileInfo: System.Web.HttpPostedFileBase
    {
        /// <summary>
        /// 文件字节数组
        /// </summary>
        public byte[] UploadFileBytes { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string UploadFileName { get; set; }

        public HttpFileInfo(byte[] uploadFileBytes,string uploadFileName)
        {
            UploadFileBytes = uploadFileBytes;
            UploadFileName = uploadFileName;
        }

        public override Stream InputStream
        {
            get
            {
                return new MemoryStream(UploadFileBytes);
            }
        }

        public override int ContentLength
        {
            get
            {
                return UploadFileBytes.Length;
            }
        }

        public override string FileName
        {
            get
            {
                return UploadFileName;
            }
        }
    }
}
