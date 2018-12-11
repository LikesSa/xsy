using System;
using System.IO;
using xsy.likes.Base;

namespace xsy.likes.pwmd
{
    public class GetTokenReturn : IDisposable
    {
        private const string xmlFileName = @".\XmlModel\GetTokenReturn.xml";  //string Path1 = @".\Data\test.txt"; 
        private readonly XmlFiles xmlFile;
        public string id { get; set; }
        /// <summary>
        /// 调用 API 的唯一凭证，与用户信息 关联，需要妥善保管；该 token 的 有效期为 24 个小时
        ///refresh_token后 调用 API 的唯一凭证，与用户信 息关联，需要妥善保管；该 token 的有效期为 2 个小时
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 生成授权时间，为当前计算机时间 和 GMT 时间 1970 年 1 月 1 号 0 时0 分 0 秒所差的毫秒数 
        /// </summary>
        public string issued_at { get; set; }
        /// <summary>
        /// 返 回 值 为 Bearer ； 在 使 用 access_token 调用 API 时，token 之前需要加此参数 
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 可以通过刷新接口获取新的 access_token，但是需要妥善保 管此token；此token的有效期为 2 个月 
        /// </summary>
        public string refresh_token { get; set; }

        public GetTokenReturn()
        {
            if (File.Exists(xmlFileName))
            {
                xmlFile = new XmlFiles(xmlFileName);
                id = xmlFile.FindNode("GetTokenReturn/id").InnerText;
                access_token = xmlFile.FindNode("GetTokenReturn/access_token").InnerText;
                issued_at = xmlFile.FindNode("GetTokenReturn/issued_at").InnerText;
                token_type = xmlFile.FindNode("GetTokenReturn/token_type").InnerText;
                refresh_token = xmlFile.FindNode("GetTokenReturn/refresh_token").InnerText;
            }
            else
            {
                //Directory.CreateDirectory("XmlModel");
                //FileStream NewText = File.Create(xmlFileName);
                //NewText.Close();
                //Directory.CreateDirectory(xmlFileName);
                File.Create(xmlFileName);
                //xmlFile = new XmlFiles(xmlFileName);
            }
        }
        private void SaveXml()
        {
            xmlFile.UpdateNodeValue("GetTokenReturn/id", id);
            xmlFile.UpdateNodeValue("GetTokenReturn/access_token", access_token);
            xmlFile.UpdateNodeValue("GetTokenReturn/issued_at", issued_at);
            xmlFile.UpdateNodeValue("GetTokenReturn/token_type", token_type);
            xmlFile.UpdateNodeValue("GetTokenReturn/refresh_token", refresh_token);
            if (File.Exists(xmlFileName))
            {
                File.Delete(xmlFileName);
            }
            xmlFile.Save(xmlFileName);
        }

        public void Dispose()
        {
            try
            {
                SaveXml();
            }
            catch
            {
            }
        }

        ~GetTokenReturn()
        {
            Dispose();
        }
    }
}