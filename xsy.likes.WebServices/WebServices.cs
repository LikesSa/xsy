using System;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace xsy.likes.WebServices
{
    public class WebServices : IWeb
    {
        public bool RequestGetData(string url,out string result, string chartSet = "utf-8")
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            result = string.Empty;
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType =  "application/x-www-form-urlencoded";

                response = (HttpWebResponse)request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var streamReader = new StreamReader(stream, Encoding.GetEncoding(chartSet)))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }
            }
            catch /*(Exception exception)*/
            {
                return false;
                //throw new Exception(exception.Message);
            }
            finally
            {
                response?.Close();
            }
            return true;
        }

        public bool RequestGetDataShaderJson<T>(string url, out T t, string chartSet = "utf-8") where T : class, new()
        {
            string result;
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            bool operateResult = RequestGetData(url, out result, chartSet);

            if (operateResult)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        t = null;
                    }
                    else
                    {
                        var jss = new JavaScriptSerializer();
                        t = jss.Deserialize<T>(result);
                    }
                }
                catch
                {
                    t = null;
                }
            }
            else
            {
                t = null;
            }

            return operateResult;
        }


        public bool RequestPostData(string url, string input, out string result,string contenttype = "application/x-www-form-urlencoded", string chartSet = "utf-8")
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            result = string.Empty;
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = contenttype;    //"application/x-www-form-urlencoded";
                var buffer = Encoding.GetEncoding(chartSet).GetBytes(input);
                //var buffer = Encoding.UTF8.GetBytes(input);
                //var buffer = Encoding.Default.GetBytes(input);
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                response = (HttpWebResponse)request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var streamReader = new StreamReader(stream, Encoding.GetEncoding(chartSet)))
                        {
                            var ec = streamReader.CurrentEncoding;
                            result = streamReader.ReadToEnd();
                        }
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                return false;
                throw new Exception(exception.Message);
            }
            finally
            {
                response?.Close();
            }
            return true;
        }

        public bool RequestPostDataShaderJson<T>(string url, string input, out T t, string chartSet) where T : class, new()
        {
            string result;
            bool operateResult = RequestPostData(url,input, out result, chartSet);

            if (operateResult)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        t = null;
                    }
                    else
                    {
                        var jss = new JavaScriptSerializer();
                        t = jss.Deserialize<T>(result);
                    }
                }
                catch
                {
                    t = null;
                }
            }
            else
            {
                t = null;
            }

            return operateResult;
        }


        //添加头信息的post方法
        public bool RequestPostAddHeadData(string url, string input, string code, out string result, string chartSet,string ContentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            result = string.Empty;
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = ContentType;// "application/x-www-form-urlencoded";
                request.Headers.Add("Authorization", code);

                var buffer = Encoding.GetEncoding(chartSet).GetBytes(input);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                response = (HttpWebResponse)request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var streamReader = new StreamReader(stream, Encoding.GetEncoding(chartSet)))
                        {
                            var ec = streamReader.CurrentEncoding;
                            result = streamReader.ReadToEnd();
                        }
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                return false;
                throw new Exception(exception.Message);
            }
            finally
            {
                response?.Close();
            }
            return true;
        }


        //添加头信息的Patch方法
        public bool RequestPatchAddHeadData(string url, string input, string code, out string result, string chartSet)
        {
           




            WebClient wc = new System.Net.WebClient();
            result = string.Empty;
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            try
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                //wc.Headers.Add("X-AjaxPro-Method", "ShowList");
                wc.Headers.Add("Authorization", code);
                wc.Encoding = Encoding.UTF8;
                //wc.Headers.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.1) Web-Sniffer/1.0.24");

                //StringEntity postingString =
                result = wc.UploadString(url, "PATCH", input);


            }
            catch (Exception exception)
            {
                return false;
                throw new Exception(exception.Message);
            }
            finally
            {
                wc?.Dispose();
            }
            return true;


        }


        //添加头信息的get方法
        public bool RequestGettAddHeadData(string url, string code, out string result, string chartSet)
        {

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            result = string.Empty;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Authorization", code);
                response = (HttpWebResponse)request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var streamReader = new StreamReader(stream, Encoding.GetEncoding(chartSet)))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }
            }
            catch /*(Exception exception)*/
            {
                return false;
                //throw new Exception(exception.Message);
            }
            finally
            {
                response?.Close();
            }
            return true;



        }

        //
        public bool DownloadOneFileByURL(string fileName, string url, string localPath, int timeout)
        {
            WebClient wc = new System.Net.WebClient();
            try
            {
                if (File.Exists(localPath + fileName)) { File.Delete(localPath + fileName); }
                if (Directory.Exists(localPath) == false) { Directory.CreateDirectory(localPath); }
                wc.DownloadFile(url, localPath+"\\"+fileName);

            } catch(Exception ex)
            {

                string str = ex.ToString();
                return false;
            }

            return true;




        }

        public string HttpPost(string url)
        {
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded"; // 因为POST无参，不写字符集也没问题
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 20000;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            streamReader.Close();
            httpWebResponse.Close();
            httpWebRequest.Abort();

            return responseContent;
        }

        public string getUrl(string url)
        {
            throw new NotImplementedException();
        }
    }

}

