namespace AB
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;



    [Serializable]
    public class AbPackage : System.IComparable<AbPackage>
    {
        public bool isRoot = false;
        public string abFileName;
        //public string assetName;

        public List<AbPackage> parents = new List<AbPackage>();
        public List<AbPackage> children = new List<AbPackage>();
        private List<string> objects = new List<string>();

        public List<string> Objects
        {
            get
            {
                return objects;
            }
        }
        public AbPackage(string abFileName)
        {
            if (abFileName == null)
            {
                this.abFileName = null;
            }
            else
            {
                this.abFileName = AbUtility.NormalizerAbName(abFileName) + ABMgr.const_extension;
            }
        }

        public override string ToString()
        {
            return abFileName;
        }

        public int CompareTo(AbPackage other)
        {
            return abFileName.CompareTo(other.abFileName);
        }

        public void AddObject(string obj)
        {
            var assetName = AbUtility.NormalizerAbName(obj);
            if (this.objects.Contains(assetName))
            {
                return;
            }
            this.objects.Add(assetName);
        }

        /// <summary>
        /// 自动设置依赖包名字
        /// </summary>
        public void AutoSetAbFileName()
        {
        }

        public void SetAbFileName(string assetName)
        {
            if (assetName != null)
            {
                this.abFileName = AbUtility.NormalizerAbName(assetName);//string.Concat(assetName, ".", ABConfiguration.const_extension);//输出文件名;
                //this.name = CFileManager.GetMd5(assetName);//输出文件名;
            }
            else
            {
                this.abFileName = null;
            }
        }

        public void AddChild(AbPackage pkg)
        {
            if (pkg == this)
            {
                return;
            }
            this.children.Add(pkg);
        }

        public void AddParent(AbPackage pkg)
        {
            if (pkg == this)
            {
                return;
            }
            this.parents.Add(pkg);
        }

        bool isAnalyzed = false;
        public void Analyze()
        {
            if (isAnalyzed)
            {
                return;
            }
            isAnalyzed = true;
            foreach (var parent in parents)
            {
                parent.Analyze();
            }

            //if (this.assetName.EndsWith(".shader"))
            //{
            //    this.abFileName = AbAnalyze.shader_ab;
            //}

            if (this.abFileName == null)
            {
                string abName = null;
                foreach (var parent in parents)
                {
                    if (parent.abFileName == null)//没有分配，跳过
                    {
                        continue;
                    }
                    if (abName == null)
                    {
                        abName = parent.abFileName;
                    }
                    else if (abName != parent.abFileName)//都多个分配，该资源被依赖多次
                    {
                        abName = null;
                        break;
                    }
                }
                if (abName == null)//所有资源目前没有分配，分配一个
                {
                    abName = this.objects[0] + ABMgr.const_extension;
                }
                this.abFileName = abName;
            }
            //foreach (var child in children)
            //{
            //    child.Analyze();
            //}
        }

    }
}
