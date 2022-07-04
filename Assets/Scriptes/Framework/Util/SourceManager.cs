using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UObject=UnityEngine.Object;


public class SourceManager : MonoSingleton<SourceManager>
{

    

    internal class bundleInfo{
        public string assetname;   //资源路径名
        public string bundlename;  //Bundle路径名
        public List<string> Dependence;   //依赖文件集   Value是资源路径  也就是assetname
    }

    private Dictionary<string,bundleInfo> m_BundleInfos=new Dictionary<string, bundleInfo>();

//存放Bundle资源的集合
    private Dictionary<string,AssetBundle> m_AssetBundles=new Dictionary<string, AssetBundle>();

    public void ParseVersionFile()
    {
        string url=Path.Combine(PathUtil.BundleResourcePath,"pathlist.txt");
        string[] data=File.ReadAllLines(url);

        for (int i = 0; i < data.Length; i++)
        {
            bundleInfo bundleinfo=new bundleInfo();
            string[] info=data[i].Split('|');
            bundleinfo.assetname=info[0];
            bundleinfo.bundlename=info[1];
            bundleinfo.Dependence=new List<string>(info.Length-2);
            for (int j = 2; j < info.Length; j++)
            {
                bundleinfo.Dependence.Add(info[j]);
            }
            m_BundleInfos.Add(bundleinfo.assetname,bundleinfo);

            if(info[0].IndexOf("LuaScripts")>0)
            {
                LuaManager.Instance.LuaNames.Add(info[0]);
            }
        }

    }

    IEnumerator LoadBundleAsync(string assetname,Action<UObject> action=null)
    {
        if(m_BundleInfos.ContainsKey(assetname))
        {
            string bundlename=m_BundleInfos[assetname].bundlename;
            string bundlepath=Path.Combine(PathUtil.BundleResourcePath,bundlename);   
            List<string> dependence=m_BundleInfos[assetname].Dependence;

            AssetBundle bundle=GetBundle(bundlename);
            if(bundle==null)
            {
            
                if(dependence!=null&&dependence.Count>0)
                {
                    for (int i = 0; i < dependence.Count; i++)
                    {
                        yield return LoadBundleAsync(dependence[i]);
                    }
                }
                AssetBundleCreateRequest request =AssetBundle.LoadFromFileAsync(bundlepath);
                yield return request;
                bundle=request.assetBundle;
                m_AssetBundles.Add(bundlename,bundle);
            }
            if(assetname.EndsWith(".unity"))
            {
                action?.Invoke(null);
                yield break;
            }
            AssetBundleRequest bundlerequest=bundle.LoadAssetAsync(assetname);
            yield return bundlerequest;

            action?.Invoke(bundlerequest?.asset);
        }
        else{
            Debug.Log("未找到资源路径");
            yield return null;
        }
    }


    private AssetBundle GetBundle(string name){
        AssetBundle bundle=null;
        if(m_AssetBundles.TryGetValue(name,out bundle))
        {
            return bundle;
        }
        else
        {
            return null;
        }
    }

    private void LoadAsset(string assetname,Action<UObject> action ){  
        #if UNITY_EDITOR      
        if(Appconst.Instance.playMode==PlayMode.EditorMode)
        {
            EditorLoadAsset(assetname,action);
        }
        else
        #endif
        {
            StartCoroutine(LoadBundleAsync(assetname,action));
        }
    }


#if UNITY_EDITOR
    //编辑器下加载    防止Bundle很大   采用编辑器直接加载原资源路径下的文件
    void EditorLoadAsset(string name,Action<UObject> action=null)
    {
        UObject obj=UnityEditor.AssetDatabase.LoadAssetAtPath(name,typeof(UObject));
        if(obj==null)
        {
            Debug.Log("不存在obj");
        }
        action?.Invoke(obj);
    }
#endif

    public void LoadUI(string name,Action<UObject> action=null)
    {
        LoadAsset(PathUtil.GetUIPrefab(name),action);
    }

    public void LoadLua(string name,Action<UObject> action=null)
    {
        LoadAsset(name,action);
    }
    public void LoadPrefab(string name,Action<UObject> action=null)
    {
        LoadAsset(PathUtil.GetModelPrefab(name),action);
    }
    public void LoadScene(string name,Action<UObject> action=null)
    {
        LoadAsset(PathUtil.GetScenePath(name),action);
    }

    public void LoadMusic(string name,Action<UObject> action=null)
    {
        LoadAsset(PathUtil.GetMusicPath(name),action);
    }

    public void LoadSound(string name,Action<UObject> action=null)
    {
        LoadAsset(PathUtil.GetSoundPath(name),action);
    }
    private void Awake() {
        ParseVersionFile();
    }



}
