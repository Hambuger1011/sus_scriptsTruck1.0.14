using AB;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

public class DownloadMgr : CMonoSingleton<DownloadMgr>
{
    public static readonly object m_lock = new object();
    public static int TIMEOUT = 30;
    public static int TIMEOUT_MAX = 120;
    public static int TIMEOUT_STEP = 10;

    public class Task : IRefCount
    {
        public string filename;
        public string url;
        public string version;
        public int size;
        public bool stopIfError;

        public int downloadedBytes;
        public bool isDone = false;
        public float waitTime = 0;
        public float TIME = 10;
        public string strError;

        //依赖资源未下载完成flag
        public int unDownloadNum = 1;
        public HttpWebRequest webReq;
        public bool isDownload = false;

        public Task(string url, string filename, string version, int size, bool stopIfError)
        {
            this.url = url;
            this.filename = filename;
            this.version = version;
            this.size = size;
            this.stopIfError = stopIfError;
            this.TIME = TIMEOUT;
        }

        public void CheckTimeOut()
        {
            lock (m_lock)
            {
                if (!isDone)
                {
                    waitTime += Time.deltaTime;
                    if (waitTime > TIME)
                    {
                        waitTime = 0;
                        if (TIME < TIMEOUT_MAX)
                        {
                            TIME += TIMEOUT_STEP;
                        }
                        lock (m_lock)
                        {
                            strError = "下载超时:" + waitTime + "-url->" + url;
                            LOG.Error(strError);
                            //if (response != null)
                            //{
                            //    response.Close();
                            //}
                            if (webReq != null)
                            {
                                webReq.Abort();
                                webReq = null;
                            }
                        }
                    }
                }
            }
        }



        public bool IsNeedDownload()
        {
            if(string.IsNullOrEmpty(this.version))
            {
                return true;
            }
            string version = CFileManager.ReadFileString(filename + ".txt");
            if (version == this.version && File.Exists(filename))//已经下载过
            {
                return false;
            }
            downloadedBytes = 0;
            return true;
        }

        /// <summary>
        /// 全部下载完成，包含依赖
        /// </summary>
        private event Action onCompleted;
        /// <summary>
        /// 只有自己下载完成，不含依赖资源
        /// </summary>
        private event Action onDownloaded;
        public void Downloaded()
        {
            try
            {
                if(this.strError == null)
                {
                    CFileManager.WriteFileString(filename + ".txt", this.version);
                }

                if (onDownloaded != null)
                {
                    var t = onDownloaded;
                    onDownloaded = null;
                    t();
                }
                this.ReduceUnDowloadNum();
            }
            catch(Exception ex)
            {
                LOG.Error(ex);
            }
        }

        /// <summary>
        /// 未下载完成flag
        /// </summary>
        public void ReduceUnDowloadNum()
        {
            this.unDownloadNum -= 1;
            if(this.unDownloadNum > 0)
            {
                return;
            }
            if (onCompleted != null)
            {
                var t = onCompleted;
                onCompleted = null;
                t();
            }
        }

        public bool IsDone()
        {
            return this.isDone;
        }

        public float Progress()
        {
            lock (m_lock)
            {
                if (this.size <= 0)
                {
                    return 0;
                }
                return (float)this.downloadedBytes / this.size;
            }
        }

        public int GetAllSize()
        {
            return size;
        }

        public void AddComplete(Action func)
        {
            if (this.unDownloadNum > 0)
            {
                onCompleted += func;
            }
            else
            {
                func();
            }
        }

        public void AddSelfComplete(Action func)
        {
            if (isDone)
            {
                func();
                return;
            }
            onDownloaded += func;
        }

        #region 引用计数
        int m_refCount = 0;
        public void Retain()
        {
           ++m_refCount;
        }

        public void Release()
        {
            --m_refCount;
        }

        public int GetRefCount()
        {
            return m_refCount;
        }

        public void OnDestroy()
        {
            onDownloaded = null;
            onCompleted = null;
            this.isDone = true;
        }

        public void Retain(object o)
        {
            this.Retain();
        }

        public void Release(object o)
        {
            this.Release();
        }
        #endregion
    }


#if NETFX_CORE
        static async void QueueWorkItem(WorkItemHandler action, object state = null)
        {
            await ThreadPool.RunAsync(action, state);
        }
#else
    public void QueueWorkItem(WaitCallback action, object state = null)
    {
        ThreadPool.QueueUserWorkItem(action, state);
    }
#endif

    public void DoWork(object state)
    {
        lock (m_lock)
        {
            DownloadMgr.Instance.curDownloadNum += 1;
        }
        Task info = (Task)state;
        byte[] buffer = null;
        string strError = null;

        int TotalBytes = 0;
        int ReceivedBytes = info.downloadedBytes;
        var filename = info.filename;
        var url = info.url;
        FileStream fs = null;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            lock (m_lock)
            {
                if (info.downloadedBytes > 0)
                {
                    LOG.Info("下载完成:" + (info.downloadedBytes + "/" + info.downloadedBytes) + "\n" + url);
                    info.isDone = true;
                }
            }
            return;
        }
        bool isDone = false;
        while (m_alive && fs != null && !isDone)
        {
            strError = null;
            var offset = ReceivedBytes;
            if (offset < 0 || (TotalBytes > 0 && offset >= TotalBytes))
            {
                Debug.LogError("offset不合法:" + offset + ",max=" + TotalBytes + "\n" + url);
                offset = 0;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = null;
            req.ServicePoint.ConnectionLimit = 512;
            if (offset > 0)
            {
                req.AddRange((int)offset);
            }
            try
            {
                lock (m_lock)
                {
                    info.webReq = req;
                    info.waitTime = 0;
                }
                response = (HttpWebResponse)req.GetResponse();//阻塞
                Stream st = response.GetResponseStream();//阻塞
                fs.Seek(offset, SeekOrigin.Begin);
                int cLength = (int)response.ContentLength;
                if (offset > 0 && response.StatusCode != HttpStatusCode.PartialContent)
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    offset = 0;
                    UnityEngine.Debug.LogError("服务器不支持断点下载:StatusCode=" + response.StatusCode + "\n" + url);
                }
                if (cLength <= 0)
                {
                    strError = "获取文件错误:" + response.StatusCode + "\n" + url;
                    continue;
                }
                if (TotalBytes == 0)
                {
                    TotalBytes = cLength;
                    fs.SetLength(cLength);
                    lock (m_lock)
                    {
                        if (info.size < cLength)
                        {
                            info.size = cLength;
                        }
                    }
                }
                if (fs.Position > 0)
                {
                    LOG.Error("断点位置:" + fs.Position + "/" + TotalBytes + "\n" + url);
                }

                if (buffer == null)
                {
                    int length = (cLength <= -1 || cLength > 32 * 1024) ? 32 * 1024 : cLength;
                    buffer = new byte[length];
                }

                int nread = 0;
                int notify_total = 0;
                while ((nread = st.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (fs == null)
                    {
                        break;
                    }
                    notify_total += nread;

                    lock (m_lock)
                    {
                        info.waitTime = 0;
                        ReceivedBytes = offset + notify_total;
                        info.downloadedBytes = ReceivedBytes;
                    }

                    fs.Write(buffer, 0, nread);
                }
                if (fs != null)
                {
                    if (ReceivedBytes > 0 && ReceivedBytes >= TotalBytes)
                    {
                        isDone = true;
                    }
                }
            }
            catch (WebException ex)
            {
                var msg = ex.ToString();
                strError = "WebException:" + url + "\n" + msg;
                if (info.stopIfError && response == null)
                {
                    //isDone = true;
                }
                else if (msg.Contains("Not Satisfiable"))
                {
                    Debug.LogError("offset:" + offset + "/" + TotalBytes);
                    offset = 0;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.ToString();
                strError = url + "\n" + msg;
            }
            finally
            {
                lock (m_lock)
                {
                    //info.isDone = true;
                    info.strError = strError;
                }
            }
            if (strError != null)
            {
                UnityEngine.Debug.LogError("下载失败:" + strError);
            }
            if (isDone)
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                    //LOG.Info("*///////////" + filename);
                }
                lock (m_lock)
                {

                    if (info.downloadedBytes > 0)
                    {
                        LOG.Info("下载完成:" + (info.downloadedBytes + "/" + info.downloadedBytes) + "\n" + url);
                        info.isDone = true;
                    }
                }
                break;
            }
        }
        lock (m_lock)
        {
            DownloadMgr.Instance.curDownloadNum -= 1;
        }
    }


    static bool m_alive = true;
    private void OnDestroy()
    {
        m_alive = false;
    }

    /// <summary>
    /// unity回调
    /// </summary>
    private void Update()
    {
        for(int i= 0; i < this.m_taskList.Count; ++i)
        {
            var key = m_taskList[i];
            var task = this.m_taskMap[key];
            if (task.isDone)
            {
                this.StopDownload(task);
                this.m_taskMap.Remove(key);
                m_taskList.RemoveAt(i);
                task.Downloaded();
                --i;
            }else
            {
                if (task.isDownload)
                {
                    //task.CheckTimeOut();
                }
                else
                {
                    this.StartDownload(task);
                }
            }
        }
    }


    private float delayTime = 0f;
    private PhoneUtil.EnumPhoneLevel level;
    private void StartDownload(Task task)
    {
        if (task.isDone)
        {
            return;
        }
        if (task.isDownload)
        {
            return;
        }

        DownloadMgr.Instance.delayTime += Time.deltaTime;
        //Debug.LogError("DownloadMgr:::::" + level);
        float nums = 1f;
        if (level == PhoneUtil.EnumPhoneLevel.High)
        {
            //高端机
            nums = 0.2f;
        }
        else if (level == PhoneUtil.EnumPhoneLevel.Medium)
        {
            //中端机
            nums = 0.6f;
        }
        else
        {
            //低端机
            nums = 1f;
        }
        if (DownloadMgr.Instance.delayTime < nums)
        {
            return;
        }

        DownloadMgr.Instance.delayTime = 0;
        //Debug.LogError("++++curDownloadNum====>>"+DownloadMgr.Instance.curDownloadNum +"--CBundle.MAX_BUNDLE_ASYNC_NUM-->"+ CBundle.MAX_BUNDLE_ASYNC_NUM);
        if (DownloadMgr.Instance.curDownloadNum >= CBundle.MAX_BUNDLE_ASYNC_NUM)
        {
            return;
        }
        // Debug.LogError("+++curDownloadNum====>>"+DownloadMgr.Instance.curDownloadNum+"==times=>"+nums);
        
        task.isDownload = true;
        QueueWorkItem(DoWork, task);
    }
    private void StopDownload(Task task)
    {
        if (!task.isDownload)
        {
            return;
        }
        task.isDownload = false;
    }

    private Dictionary<string, Task> m_taskMap = new Dictionary<string, Task>();
    private List<string> m_taskList = new List<string>();
    //[NonSerialized]
    public int curDownloadNum = 0;
    public Task Download(string url, string filename, string version, int size,bool stopIfError,bool autoConnectVersion = true)
    {
        level = GameFrameworkImpl.Instance.level;
        if (autoConnectVersion)
            url = url + "?" + version;

        Task task;
        if (!m_taskMap.TryGetValue(url, out task))
        {
            task = new Task(url, filename, version, size, stopIfError);
            m_taskMap.Add(url,task);
            m_taskList.Add(url);

            if (task.IsNeedDownload())
            {
                Debug.Log("需要下载:version:" + task.version + "=>" + url + "\n" + filename);
            }else
            {
                Debug.Log("已经下载过:" + url + "\n" + filename);
                task.isDone = true;
                if(task.size <= 0)
                {
                    task.size = 1;
                }
                task.downloadedBytes = task.size;
            }
        }
        task.Retain();
        return task;
    }


    //public Task DownloadAsset(string t)
    //{
    //    var assetName = AbUtility.NormalizerAbName(t);
    //    var abConfig = AB.ABSystem.ui.bundle.resConfig;
    //    var item = abConfig.GetConfigItemByAssetName(assetName);
    //    if (item == null)
    //    {
    //        Debug.LogError("asset资源未打进包:" + assetName);
    //        return null;
    //    }
    //    return DownloadAsset(item);
    //}

    //public Task DownloadAsset(AbResItem item)
    //{
    //    var abConfig = AB.ABSystem.ui.bundle.resConfig;
    //    var info = this.Download(item.url, item.abFilePath, item.crc.ToString(), item.size);
    //    //加载所有依赖
    //    var needLoadDependencyNum = item.dependencyList.Length;
    //    info.flag = 1 + needLoadDependencyNum;
    //    for (int i = 0, iMax = needLoadDependencyNum; i < iMax; ++i)
    //    {
    //        string depAssetName = item.dependencyList[i];
    //        var item2 = abConfig.GetConfigItemByAbName(depAssetName);
    //        var depInfo = this.Download(item2.url, item2.abFilePath, item2.crc.ToString(), item2.size);
    //        depInfo.AddSelfComplete(() =>
    //        {
    //            info.ReduceFlag();
    //        });
    //    }
    //    info.AddSelfComplete(() =>
    //    {
    //        info.ReduceFlag();
    //    });
    //    return info;
    //}
    public float GetProgress()
    {
        float x = 0;
        float y = 0;
        for (int i = this.m_taskList.Count - 1; i >= 0; --i)
        {
            var key = m_taskList[i];
            var task = this.m_taskMap[key];
            x += task.downloadedBytes;
            y += task.size;
        }
        return x / y;
    }
    public bool IsCompleted()
    {
        for (int i = this.m_taskList.Count - 1; i >= 0; --i)
        {
            var key = m_taskList[i];
            var task = this.m_taskMap[key];
            if (!task.isDone)
            {
                return false;
            }
        }
        return true;
    }
    
    
    public Task DownloadImage(string url)
    {
        string[] ImageMane = url.Split('/');
        var fName = ImageMane[ImageMane.Length - 1];//获得图片的名称
        var filename = string.Format("{0}cache/download/image/{1}", GameUtility.WritablePath, fName);
        var task = this.Download(url, filename, UserDataManager.Instance.ResVersion, 0, true);
        return task;
    }



    public Task DownloadBanner(int bookID)
    {
        var url = GameHttpNet.Instance.GetResourcesUrl() + "image/book_banner/" + bookID + ".jpg";
        var filename = string.Format("{0}cache/book/banner/{1}.jpg", GameUtility.WritablePath, bookID);
        var version = UserDataManager.Instance.GetBookVersion(bookID);
        var task = this.Download(url, filename, version, 1048576, true);
        return task;
    }

    public Task DownloadChapterBG(int bookID)
    {
        var url = GameHttpNet.Instance.GetResourcesUrl() + "image/book_description/" + bookID + ".jpg";
        var filename = string.Format("{0}cache/book/description/{1}.jpg", GameUtility.WritablePath, bookID);
        var version = UserDataManager.Instance.GetBookVersion(bookID);
        var task = this.Download(url, filename, version, 1048576, true);
        return task;
    }
    public Task DownloadBookCover(int bookID)
    {
        var url = GameHttpNet.Instance.GetResourcesUrl() + "image/book_loading/" + bookID + ".jpg";
        var filename = string.Format("{0}cache/book/loading/{1}.jpg", GameUtility.WritablePath, bookID);
        var version = UserDataManager.Instance.GetBookVersion(bookID);
        var task = this.Download(url, filename, version, 1048576, true);
        return task;
    }
    
    public Task DownloadBookSceneBG(int bookId,string name)
    {
        var url = GameHttpNet.Instance.GetResourcesUrl() + "image/book_scene/" + name + ".jpg";
        var filename = string.Format("{0}cache/book/scene/{1}.jpg", GameUtility.WritablePath, name);
        var version = UserDataManager.Instance.GetBookVersion(bookId);
        var task = this.Download(url, filename, version, 1048576, true);
        return task;
    }
    
    public Task DownloadBookDialog(int bookId,int vChapterId)
    {
        if (UserDataManager.Instance.bookJDTFormSever != null && UserDataManager.Instance.bookJDTFormSever.data != null &&
            UserDataManager.Instance.bookJDTFormSever.data.dialog_version != null)
        {
            var url = UserDataManager.Instance.bookJDTFormSever.data.dialog_version.path + string.Format("/{0}/{1}.json.gz", bookId,vChapterId);;
            var filename = string.Format("{0}cache/book/dialog/{1}/{2}.json.gz", GameUtility.WritablePath, bookId,vChapterId);
            var version = SdkMgr.Instance.GameVersion()+"_"+UserDataManager.Instance.bookJDTFormSever.data.dialog_version.version;
            #if ENABLE_DEBUG
                version += GameDataMgr.Instance.GetCurrentUTCTime();
            #endif
            url += "?" + version;
            var task = this.Download(url, filename, version, 1048576, true);
            return task;
        }

        return null;
    }
    
    public Task DownloadLoadImg(string version,Action<int, UnityObjectRefCount, string> callback)
    {
        //此时还未请求getversion  待调整
        var url = GameHttpNet.Instance.GetResourcesUrl() + "image/book_loading/0.jpg";
        var filename = string.Format("{0}cache/book/loading/0.jpg", GameUtility.WritablePath, 0);
        var task = DownloadMgr.Instance.Download(url, filename, version, 1048576, true);
        task.AddComplete(() =>
        {
            var sprite = XLuaHelper.LoadSprite(task.filename);
            UnityObjectRefCount refCount = SpriteRefCount.Create(sprite);
            callback(0, refCount, version);
        });
        return task;
    }
}

namespace Framework
{
    public class IndexedMap<K, V> : IDictionary<K, V>
    {
        readonly List<V> m_List = new List<V>();
        readonly Dictionary<K, int> m_Dictionary = new Dictionary<K, int>();
        IComparer comparer;

        public V this[K key]
        {
            get
            {
                int index;
                if(m_Dictionary.TryGetValue(key, out index))
                {
                    return m_List[index];
                }
                return default(V);
            }
            set
            {
                int index;
                if (m_Dictionary.TryGetValue(key, out index))
                {
                    return;
                }
                index = m_List.Count;
                m_List.Add(value);
                m_Dictionary.Add(key, index);
            }
        }

        public ICollection<K> Keys
        {
            get
            {
                return m_Dictionary.Keys;
            }
        }

        public ICollection<V> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                return m_Dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(K key, V value)
        {
            this[key] = value;
        }

        public void Add(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            m_List.Clear();
            m_Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(K key)
        {
            return this.m_Dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(K key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(K key, out V value)
        {

            int index;
            if (m_Dictionary.TryGetValue(key, out index))
            {
                value = m_List[index];
                return true;
            }
            else
            {
                value = default(V);
                return false;
            }
        }

        public V GetValueByIndex(int i)
        {
            return m_List[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class IndexedSet<T> : IList<T>
    {
        //This is a container that gives:
        //  - Unique items
        //  - Fast random removal
        //  - Fast unique inclusion to the end
        //  - Sequential access
        //Downsides:
        //  - Uses more memory
        //  - Ordering is not persistent
        //  - Not Serialization Friendly.

        //We use a Dictionary to speed up list lookup, this makes it cheaper to guarantee no duplicates (set)
        //When removing we move the last item to the removed item position, this way we only need to update the index cache of a single item. (fast removal)
        //Order of the elements is not guaranteed. A removal will change the order of the items.

        readonly List<T> m_List = new List<T>();
        Dictionary<T, int> m_Dictionary = new Dictionary<T, int>();

        public void Add(T item)
        {
            m_List.Add(item);
            m_Dictionary.Add(item, m_List.Count - 1);
        }

        public bool AddUnique(T item)
        {
            if (m_Dictionary.ContainsKey(item))
                return false;

            m_List.Add(item);
            m_Dictionary.Add(item, m_List.Count - 1);

            return true;
        }

        public bool Remove(T item)
        {
            int index = -1;
            if (!m_Dictionary.TryGetValue(item, out index))
                return false;

            RemoveAt(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            m_List.Clear();
            m_Dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public int Count { get { return m_List.Count; } }
        public bool IsReadOnly { get { return false; } }
        public int IndexOf(T item)
        {
            int index = -1;
            m_Dictionary.TryGetValue(item, out index);
            return index;
        }

        public void Insert(int index, T item)
        {
            //We could support this, but the semantics would be weird. Order is not guaranteed..
            throw new NotSupportedException("Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
        }

        public void RemoveAt(int index)
        {
            T item = m_List[index];
            m_Dictionary.Remove(item);
            if (index == m_List.Count - 1)
                m_List.RemoveAt(index);
            else
            {
                int replaceItemIndex = m_List.Count - 1;
                T replaceItem = m_List[replaceItemIndex];
                m_List[index] = replaceItem;
                m_Dictionary[replaceItem] = index;
                m_List.RemoveAt(replaceItemIndex);
            }
        }

        public T this[int index]
        {
            get { return m_List[index]; }
            set
            {
                T item = m_List[index];
                m_Dictionary.Remove(item);
                m_List[index] = value;
                m_Dictionary.Add(item, index);
            }
        }

        public void RemoveAll(Predicate<T> match)
        {
            //I guess this could be optmized by instead of removing the items from the list immediatly,
            //We move them to the end, and then remove all in one go.
            //But I don't think this is going to be the bottleneck, so leaving as is for now.
            int i = 0;
            while (i < m_List.Count)
            {
                T item = m_List[i];
                if (match(item))
                    Remove(item);
                else
                    i++;
            }
        }

        //Sorts the internal list, this makes the exposed index accessor sorted as well.
        //But note that any insertion or deletion, can unorder the collection again.
        public void Sort(Comparison<T> sortLayoutFunction)
        {
            //There might be better ways to sort and keep the dictionary index up to date.
            m_List.Sort(sortLayoutFunction);
            //Rebuild the dictionary index.
            for (int i = 0; i < m_List.Count; ++i)
            {
                T item = m_List[i];
                m_Dictionary[item] = i;
            }
        }
    }
}