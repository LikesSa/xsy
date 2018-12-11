namespace xsy.likes.WebServices
{
    public interface IWeb
    {
        string getUrl(string url);
        string HttpPost(string url);
        bool RequestGetData(string url, out string result, string chartSet);
        bool RequestPostAddHeadData(string url, string input,string code,out string result, string chartSet,string ContentType = "application/x-www-form-urlencoded");
        bool RequestPatchAddHeadData(string url, string input, string code, out string result, string chartSet);
        bool RequestGetDataShaderJson<T>(string url, out T t, string chartSet) where T : class, new();
        bool RequestPostData(string url, string input, out string result,string contenttype = "application/x-www-form-urlencoded", string chartSet = "utf-8");
        bool RequestPostDataShaderJson<T>(string url, string input, out T t, string chartSet) where T : class, new();
        bool RequestGettAddHeadData(string url, string code, out string result, string chartSet);
        bool DownloadOneFileByURL(string fileName, string url, string localPath, int timeout);

    }
}
