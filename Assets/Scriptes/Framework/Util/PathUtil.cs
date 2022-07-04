using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathUtil 
{
    //源路径  E:/UnityProject/LuaProject/Assets
    public static readonly string AssetsPath = Application.dataPath;

    //需要打Bundle的资源路径
    public static readonly string BuildResourcesPath = AssetsPath+ "/BuildResources";  //这个是资源放置的路径

    // Bundle输出路径
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    //Lua脚本路径
    public static readonly string LuaPath = "Assets/BuildResources/LuaScripts";

    //只读路径
    public static readonly string ReadPath = Application.streamingAssetsPath;
    //读写路径
    public static readonly string ReadWritePath = Application.persistentDataPath;

    public static string BundleResourcePath
    {

        get{
            if(Appconst.Instance.playMode==PlayMode.UpdateMode)
            {
            return ReadWritePath;
            }
            return ReadPath;
        }
    }


    /// <summary>
    /// 拿到Unity的assets往后的路径名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetUnityPath(string path)
    {
        if(string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        return path.Substring(path.IndexOf("Assets"));

    }
    /// <summary>
    /// 得到标准路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        return path.Trim().Replace("\\","/");    //删除前后的空格，然后把\\ 替换成/
    }

    public static string GetLuaPath(string name)
    {
        return string.Format("Assets/BuildResources/LuaScripts/{0}.bytes",name);
    }

    public static string GetLuaPath()
    {
        return "Assets/BuildResources/LuaScripts/main.bytes";
    }

    public static string GetUIPrefab(string name)
    {
        return string.Format("Assets/BuildResources/UI/Prefab/{0}.prefab",name);
    }

    public static string GetModelPrefab(string name)
    {
        return string.Format("Assets/BuildResources/Model/Prefab/{0}.prefab",name);
    }

        public static string GetScenePath(string name)
    {
        return string.Format("Assets/BuildResources/Scene/{0}.unity",name);
    }
    public static string GetSpritesPath(string name)
    {
        return string.Format("Assets/BuildResources/Scripts/{0}",name);
    }

    public static string GetMusicPath(string name)
    {
        return string.Format("Assets/BuildResources/Music/{0}.mp3",name);
    }
    public static string GetSoundPath(string name)
    {
        return string.Format("Assets/BuildResources/Music/{0}.mp3",name);
    }
}
