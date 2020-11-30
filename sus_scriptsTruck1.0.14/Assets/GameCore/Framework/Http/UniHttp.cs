/*
 * http://tool.chinaz.com/Tools/httptest.aspx
 * http://www.hackyin.com/1049/
 * post测试：http://coolaf.com/
 */
#define USE_UNITY_WEB
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using XLua;


[CSharpCallLua]
public delegate void HttpHandler(HttpObject obj, long responseCode, string result);

public class HttpObject
{
    public string url;
    public string apiName;
    public int maxTryCount;
    public int tryCount;
    public byte[] paramData;
    public HttpHandler callback;
    public Dictionary<string, string> param;
    public Dictionary<string, string> Headers;
    public int timeoutMS;
    public bool isMask;
    public bool isShowLoadUI;

    public HttpObject(string apiName, Dictionary<string, string> param, HttpHandler callback, int timeoutMS, int tryCount,bool _isShowLoadUI)
    {
        // if (timeoutMS < 1000 * 3)
        // {
        //     timeoutMS =  3;
        // }
        this.url = apiName;
        this.param = param;
        this.callback = callback;
        this.timeoutMS = timeoutMS;
        this.maxTryCount = tryCount;
        this.tryCount = 0;
        this.Headers = new Dictionary<string, string>();
        this.isShowLoadUI = _isShowLoadUI;

        if (param != null && param.Count != 0)
        {
            StringBuilder buffer = new StringBuilder();
            bool first = true;
            foreach (var itr in param)
            {
                if (!first) buffer.Append("&");
                buffer.Append(WWW.EscapeURL(itr.Key, System.Text.Encoding.UTF8));
                //buffer.Append(itr.Key);
                buffer.Append("=");
                //buffer.Append(itr.Value);
                buffer.Append(WWW.EscapeURL(itr.Value, System.Text.Encoding.UTF8));
                if (first) first = false;
            }
            paramData = Encoding.UTF8.GetBytes(buffer.ToString());
        }
    }
    

#if USE_UNITY_WEB
    public void Complete(string result)
    {
        if (this.callback != null)
        {
#if ENABLE_DEBUG
            var retJson = JsonHelper.JsonToJObject(result);
            if (retJson.code != 200 && retJson.code != 280 && retJson.code != 281 && retJson.code != 378 && retJson.code <20000)
            {
                UIAlertMgr.Instance.Show("TIPS", retJson.msg);
            }
#endif
            //UINetLoadingMgr.Instance.Close();
            this.callback(this, 200, result);

        }
    }

    public void Error(HttpStatusCode statusCode, string msg)
    {
        //UINetLoadingMgr.Instance.Close();
        var str = "协议失败:code=" + statusCode + "," + this.url + "\n" + msg;
        Debug.LogError(str);
        if (this.callback != null)
        {
            this.callback(this, (long)statusCode, msg);
        }
    }
#else
    public void Complete(string result)
    {
        if (this.callback != null)
        {
            CTimerManager.Instance.AddTimer(0, 1, (_) =>
            {
                //UINetLoadingMgr.Instance.Close();
                this.callback(this, 200, result);

#if ENABLE_DEBUG
                var retJson = JsonHelper.JsonToJObject(result);
                if (retJson.code != 200)
                {
                    UIAlertMgr.Instance.Show("TIPS", retJson.msg);
                }
#endif
            });
        }
    }

    public void Error(HttpStatusCode statusCode, string msg)
    {
        CTimerManager.Instance.AddTimer(0, 1, (_) =>
        {
            //UINetLoadingMgr.Instance.Close();
            var str = "协议失败:code=" + statusCode + "," + this.url + "\n" + msg;
            Debug.LogError(str);
            if (this.callback != null)
            {
                this.callback(this, (long)statusCode, msg);
            }
        });
    }
#endif
}

[XLua.Hotfix, XLua.LuaCallCSharp]
public class UniHttp
{
    static UniHttp _Instance;
    public static UniHttp Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new UniHttp();
            }
            return _Instance;
        }
    }
    public MonoBehaviour mono;

    public UniHttp()
    {
        if (mono == null)
        {
            this.mono = Framework.GameFramework.Instance;
        }
        ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

        //若要启用 100-Continue 行为，则为 true。默认值为 true。
        System.Net.ServicePointManager.Expect100Continue = false;
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                                          | SecurityProtocolType.Tls
                                                          | SecurityProtocolType.Tls11
                                                          | SecurityProtocolType.Tls12;
    }

    private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }



    public void Post(string url, XLua.LuaTable luaTable, HttpHandler callback, int timeoutMS = 5, int tryCount = 3, bool isShowLoadUI = true)
    {
        using (luaTable)
        {
            Dictionary<string, string> param = luaTable.Cast<Dictionary<string, string>>();
            var obj = new HttpObject(url, param, callback, timeoutMS, tryCount, isShowLoadUI);
            DoWebPost(obj);
        }
    }


    public void Get(string url, XLua.LuaTable luaTable, HttpHandler callback, int timeoutMS = 5, int tryCount = 3, bool isShowLoadUI = true)
    {
        using (luaTable)
        {
            Dictionary<string, string> param = luaTable.Cast<Dictionary<string, string>>();
            var obj = new HttpObject(url, param, callback, timeoutMS, tryCount, isShowLoadUI);
            DoWebGet(obj);
        }
    }

    public void Get(string url, HttpHandler callback, int timeoutMS = 5, int tryCount = 3,bool isShowLoadUI=true)
    {
        var obj = new HttpObject(url, null, callback, timeoutMS, tryCount, isShowLoadUI);
        DoWebGet(obj);
    }

    public void Post(string url, Dictionary<string, string> param, HttpHandler callback, int timeoutMS, int tryCount, bool isShowLoadUI = true)
    {
        Dictionary<string, string> copyParam = new Dictionary<string, string>();
        foreach (var itr in param)
        {
            if (itr.Value == null)
            {
                copyParam.Add(itr.Key, string.Empty);
            }
            else
            {
                copyParam.Add(itr.Key, itr.Value);
            }
        }
        var obj = new HttpObject(url, copyParam, callback, timeoutMS, tryCount,isShowLoadUI);
        DoWebPost(obj);
    }


#if !USE_UNITY_WEB  //使用C# http
    
#if NETFX_CORE
        private async void QueueWorkItem(WorkItemHandler action,object state)
        {
            await ThreadPool.RunAsync(action, state);
        }
#else
    private void QueueWorkItem(WaitCallback action, object state)
    {
        ThreadPool.QueueUserWorkItem(action, state);
    }
#endif

    static void SetupRequest(HttpWebRequest request, HttpObject obj)
    {
        request.Timeout = obj.timeoutMS;
        request.ReadWriteTimeout = obj.timeoutMS;
        request.ContentType = "application/x-www-form-urlencoded";
        if (obj.paramData == null)
        {
            request.ContentLength = 0;
        }
        else
        {
            request.ContentLength = obj.paramData.Length;
        }

    #region 增加头部
#if NET_4_6
        LOG.Info("headers:" + string.Join(",", obj.Headers));
#endif
        foreach (var itr in obj.Headers)
        {
            var key = itr.Key;// WWW.EscapeURL(itr.Key, System.Text.Encoding.UTF8);
            var value = itr.Value;// WWW.EscapeURL(itr.Value, System.Text.Encoding.UTF8);
            try
            {
                request.Headers.Set(key, value);
            }
            catch (Exception ex)
            {
                request.Headers.Set(key, "error");
                CTimerManager.Instance.AddTimer(0, 1, (_) =>
                {
                    Debug.LogError(ex);
                });
            }
        }
    #endregion
    }


    static char[] S_EXCEPTION_SPLIT = new char[] { '(', ')' };
    HttpStatusCode GetHttpStatusCode(WebException ex)
    {
        HttpStatusCode statusCode = 0;
        switch (ex.Status)
        {
            case WebExceptionStatus.ProtocolError:
                {
                    HttpWebResponse http_response = ex.Response as HttpWebResponse;
                    if (http_response != null)
                    {
                        statusCode = http_response.StatusCode;
                    }
                    else
                    {
                        statusCode = HttpStatusCode.InternalServerError;//"The remote server returned an error: (500) Internal Server Error."

                    }
                }
                break;
            case WebExceptionStatus.RequestProhibitedByCachePolicy:
            case WebExceptionStatus.RequestProhibitedByProxy:
                statusCode = HttpStatusCode.NotFound;
                break;
            case WebExceptionStatus.Timeout:
                statusCode = HttpStatusCode.RequestTimeout;
                break;
        }
        return statusCode;
    }

    public void DoWebPost(HttpObject obj, int tryCount = 1, bool inThread = false)
    {
#if !CHANNEL_SPAIN
        string Token = GameHttpNet.Instance.TOKEN;
        string lang = GameHttpNet.Instance.LANG;
        string uuid = GameHttpNet.Instance.UUID;

        obj.Headers["phoneimei"] = uuid;
        obj.Headers["token"] = Token;
        obj.Headers["lang"] = lang;
#endif
        obj.isMask = true;
        if (obj.isShowLoadUI)
        {
            UINetLoadingMgr.Instance.Show();
        }
        QueueWorkItem(__InternalPost, obj);
    }

    void __InternalPost(object state)
    {
        var obj = (HttpObject)state;
        string strError = null;
        string result = null;
        HttpStatusCode statusCode = (HttpStatusCode)0;
        try
        {
            obj.tryCount += 1;
            //LOG.Error("try:" + obj.tryCount);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(obj.url);
            req.Method = "POST";
            SetupRequest(req, obj);

            Stream stream = null;
            if (obj.paramData != null && obj.paramData.Length > 0)
            {

                stream = req.GetRequestStream();
                stream.Write(obj.paramData, 0, obj.paramData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();

            statusCode = response.StatusCode;
            if (statusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            else
            {
                strError = "返回码code:" + (int)response.StatusCode + "-" + response.StatusCode + "\n" + obj.url;
            }
        }
        catch (WebException ex)
        {
            strError = "异常code=" + ex.Status + "," + obj.url + "\n" + ex.Message;
            if (strError.Contains("(404)"))
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else
            {
                statusCode = GetHttpStatusCode(ex);
            }
        }
        catch (Exception ex)
        {
            strError = "异常:" + obj.url + "\n" + ex.ToString();
        }
        finally
        {
             if (req != null) { req.Abort(); req = null; }
             //if (st != null) { st.Close(); st = null; }
            if (response != null) { response.Close(); response = null; }
            if (strError != null)
            {
                //Debug.LogError(strError);

                if (obj.tryCount >= obj.maxTryCount)
                {
                    //Debug.LogError(strError);
                    obj.Error(statusCode, strError);
                }
                else
                {
                    __InternalPost(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(result))
                {
                    Debug.LogError("结果为空？result=" + (result ?? "null") + "\n" + obj.url);
                    __InternalPost(obj);
                }
                else
                {
                    obj.Complete(result);
                }
            }
        }
    }


    public void DoWebGet(HttpObject obj, int tryCount = 1)
    {
#if !CHANNEL_SPAIN
        string Token = GameHttpNet.Instance.TOKEN;
        string lang = GameHttpNet.Instance.LANG;
        string uuid = GameHttpNet.Instance.UUID;

        obj.Headers["phoneimei"] = uuid;
        obj.Headers["token"] = Token;
        obj.Headers["lang"] = lang;
#endif
        obj.isMask = true;
        if (obj.isShowLoadUI)
        {
            UINetLoadingMgr.Instance.Show();
        }
        QueueWorkItem(__InternalGet, obj);
    }

    void __InternalGet(object state)
    {

        var obj = (HttpObject)state;
        string strError = null;
        string result = null;
        HttpStatusCode statusCode = (HttpStatusCode)0;
        try
        {
            obj.tryCount += 1;
            //LOG.Error("try:" + obj.tryCount);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(obj.url);
            req.Method = "GET";
            SetupRequest(req, obj);
            req.ContentType = "text/json";
            //req.ContentLength = obj.data.Length;


            bool flag = false;

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            statusCode = response.StatusCode;
            if (statusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            else
            {
                strError = "返回码code:" + (int)response.StatusCode + "-" + response.StatusCode + "\n" + obj.url;
            }
        }
        catch (WebException ex)
        {
            strError = "异常code=" + ex.Status + "," + obj.url + "\n" + ex.Message;
            statusCode = GetHttpStatusCode(ex);
        }
        catch (Exception ex)
        {
            strError = "异常:" + obj.url + "\n" + ex.ToString();
        }
        finally
        {
            //req.Abort();
            if (strError != null)
            {
                if (obj.tryCount >= obj.maxTryCount)
                {
                    //Debug.LogError(strError);
                    obj.Error(statusCode, strError);
                }
                else
                {
                    __InternalGet(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(result))
                {
                    Debug.LogError("结果为空？result=" + (result ?? "null") + "\n" + obj.url);
                    __InternalGet(obj);
                }
                else
                {
                    obj.Complete(result);
                }
            }
        }
    }
#else   //使用unity http



    static void SetupRequest(UnityWebRequest req, HttpObject obj)
    {
        req.timeout = obj.timeoutMS;

#region 增加头部
#if NET_4_6
        LOG.Info("headers:" + string.Join(",", obj.Headers));
#endif
        foreach (var itr in obj.Headers)
        {
            var key = itr.Key;// WWW.EscapeURL(itr.Key, System.Text.Encoding.UTF8);
            var value = itr.Value;// WWW.EscapeURL(itr.Value, System.Text.Encoding.UTF8);
            try
            {
                req.SetRequestHeader(key, value);
            }
            catch (Exception ex)
            {
                req.SetRequestHeader(key, "error");
                CTimerManager.Instance.AddTimer(0, 1, (_) =>
                {
                    Debug.LogError("Error--->"+key+"="+ value+"\n"+ex);
                });
            }
        }
#endregion
    }


    public void DoWebPost(HttpObject obj, int tryCount = 1, bool inThread = false)
    {
#if !CHANNEL_SPAIN
        string Token = GameHttpNet.Instance.TOKEN;
        string lang = GameHttpNet.Instance.LANG;
        string uuid = GameHttpNet.Instance.UUID;
        int systemType = GameHttpNet.Instance.SYSTEMTYPE;
        string accessToken = UserDataManager.Instance.Accesskey;
        string iggId = UserDataManager.Instance.IGGid;

        obj.Headers["phoneimei"] = uuid;
        obj.Headers["token"] = Token;
        obj.Headers["lang"] = lang;
        obj.Headers["access-token"] = accessToken;
        obj.Headers["system-type"] = systemType.ToString();
        obj.Headers["iggid"] = iggId;
#endif
        obj.isMask = true;
        if (obj.isShowLoadUI)
        {
            UINetLoadingMgr.Instance.Show();
        }
        __InternalPost(obj);
    }


    public void DoWebGet(HttpObject obj, int tryCount = 1)
    {
#if !CHANNEL_SPAIN
        string Token = GameHttpNet.Instance.TOKEN;
        string lang = GameHttpNet.Instance.LANG;
        string uuid = GameHttpNet.Instance.UUID;
        int systemType = GameHttpNet.Instance.SYSTEMTYPE;
        string accessToken = UserDataManager.Instance.Accesskey;
        string iggId = UserDataManager.Instance.IGGid;

        obj.Headers["phoneimei"] = uuid;
        obj.Headers["token"] = Token;
        obj.Headers["lang"] = lang;
        obj.Headers["access-token"] = accessToken;
        obj.Headers["system-type"] = systemType.ToString();
        obj.Headers["iggid"] = iggId;
#endif
        obj.isMask = true;
        if (obj.isShowLoadUI)
        {
            UINetLoadingMgr.Instance.Show();
        }
        __InternalGet(obj);
    }

    void __InternalPost(HttpObject obj)
    {
        UnityWebRequest request = new UnityWebRequest(obj.url, UnityWebRequest.kHttpVerbPOST);
        {
            SetupRequest(request, obj);
            UploadHandler uploadHandler = new UploadHandlerRaw(obj.paramData);
            uploadHandler.contentType = "application/x-www-form-urlencoded";
            request.uploadHandler = uploadHandler;
            request.downloadHandler = new DownloadHandlerText();
            request.chunkedTransfer = false;
            var webRequestAsyncOp = request.SendWebRequest();
            webRequestAsyncOp.completed += (AsyncOperation operation)=>
            {
                if (request.isHttpError || request.isNetworkError) //如果其 请求失败，或是 网络错误
                {
                    Debug.LogError("req.isHttpError || req.isNetworkError Post:"+ request.error+ "  req.responseCode:"+ request.responseCode+ " req.downloadHandler.text:" + request.downloadHandler.text); //打印错误原因
                    if (obj.tryCount < obj.maxTryCount - 1)
                    {
                        Debug.LogError("POST：ReConnect = " + request.responseCode + ",Error:" + request.error + ",url:" + obj.url);
                        request.Dispose();
                        request = null;
                    
                        obj.tryCount += 1;
                        __InternalPost(obj);
                    }
                    else
                    {
                        Debug.LogError("POST：Send Fail:code = " + request.responseCode + ",Error:" + request.error + ",url:" + obj.url);
                        obj.Error(0, request.error);
                        request.Dispose();
                        request = null;
                    }
                }
                else
                {
                    if (request.responseCode == 200)
                    {
                        obj.Complete(request.downloadHandler.text);
                        request.Dispose();
                        request = null;
                    }
                }
            };
        }
    }

    void __InternalGet(HttpObject obj)
    {
        UnityWebRequest request = new UnityWebRequest(obj.url, UnityWebRequest.kHttpVerbGET);
        {
            SetupRequest(request, obj);
            request.downloadHandler = new DownloadHandlerText();
            request.chunkedTransfer = false;
            var webRequestAsyncOp = request.SendWebRequest();
            webRequestAsyncOp.completed += (AsyncOperation operation) =>
            {
                if (request.isHttpError || request.isNetworkError) //如果其 请求失败，或是 网络错误
                {
                    Debug.LogError("req.isHttpError || req.isNetworkError Post:" + request.error + "  req.responseCode:" + request.responseCode + " req.downloadHandler.text:" + request.downloadHandler.text); //打印错误原因
                    if (obj.tryCount < obj.maxTryCount - 1)
                    {
                        Debug.LogError("Get：ReConnect = " + request.responseCode + ",Error:" + request.error + ",url:" + obj.url);
                        request.Dispose();
                        request = null;

                        obj.tryCount += 1;
                        __InternalGet(obj);
                    }
                    else
                    {
                        Debug.LogError("Get：Send Fail:code = " + request.responseCode + ",Error:" + request.error + ",url:" + obj.url);

                        obj.Error(0, request.error);
                        request.Dispose();
                        request = null;
                    }
                }
                else
                {
                    if (request.responseCode == 200)
                    {
                        obj.Complete(request.downloadHandler.text);
                        request.Dispose();
                        request = null;
                    }
                }
            };
        }
    }



    public class DownloadHandlerText : DownloadHandlerScript
    {
        int contentLength = 0;
        MemoryStream ms = new MemoryStream();
        byte[] buffer = null;
        protected override byte[] GetData()
        {
            return this.buffer;
        }

        protected override void ReceiveContentLength(int contentLength)
        {
            this.contentLength = contentLength;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null)
            {
                return false;
            }
            int offset = 0;
            if (dataLength - offset > 0)
            {
                ms.Write(data, offset, dataLength - offset);
            }
            return true;
        }

        protected override void CompleteContent()
        {
            this.buffer = this.ms.ToArray();
        }

        protected override float GetProgress()
        {
            return (float)ms.Length / contentLength;
        }

        public static string GetContent(UnityWebRequest www)
        {
            return GetCheckedDownloader<DownloadHandlerText>(www).text;
        }

        protected override string GetText()
        {
            string result = "";
            byte[] bytes = GetData();
            if (bytes != null && bytes.Length > 0)
            {
                int offset = 0;
                if (bytes.Length >= 3)
                {
                    if (bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)//UTF8-BOM
                    {
                        //LOG.Error("返回值是UTF8-BOM编码");
                        offset = 3;
                    }
                }

                Encoding encodeType = System.Text.Encoding.UTF8;
                result = encodeType.GetString(bytes, offset, bytes.Length - offset);
            }

            if (bytes == null)
            {
                Debug.LogError("UnityWebRequest bytes is null !!!");
            }
            else
            {
               // Debug.LogError("UnityWebRequest bytes.Length GetText() =" + bytes.Length);
            }

            return result;
        }
    }

#endif
}