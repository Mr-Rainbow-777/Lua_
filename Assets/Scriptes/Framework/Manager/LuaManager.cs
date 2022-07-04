using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;
using System;
public class LuaManager : Singleton<LuaManager>
{
    //所有的Lua文件名   //Assets开头
    public List<string> LuaNames=new List<string>();
    //缓存Lua脚本内容
    private Dictionary<string,byte[]> m_LuaScripts;

    public LuaEnv Luaenv;
    public Action InitOK;

    private void Awake() {

        
    }


    public void Init(Action initok){
        this.InitOK=initok;
        Luaenv=new LuaEnv();
        Luaenv.AddLoader(Loader);
        m_LuaScripts=new Dictionary<string, byte[]>();

        #if UNITY_EDITOR
        if(Appconst.Instance.playMode==PlayMode.EditorMode)
        {
            EditorLoadLuaScript();
        }
        else
        #endif
        {
            LoadLuaScripts();
        }
    }

    byte[] Loader(ref string name)
    {
        return GetLuaScripts(name);
    }

    public void StartLua(string name)
    {
        Luaenv.DoString(string.Format("require '{0}'",name));
    }

    public byte[] GetLuaScripts(string name)
    {
        //require  ui.login.register
        name=name.Replace(".","/");
        Debug.LogFormat("开始读取Lua脚本:{0}",name);
        string fileName=PathUtil.GetLuaPath(name);   //处理
        Debug.LogFormat("filename:{0}",fileName);
        byte[] luaScript=null;
        if(!m_LuaScripts.TryGetValue(fileName,out luaScript))
        {
            Debug.LogError("lua script is not exist"+fileName);
        }
        return luaScript;

    }

    public void AddLuaScripts(string filename,byte[] luaScript)
    {
        m_LuaScripts[filename]=luaScript;
    }


    #if UNITY_EDITOR
        void EditorLoadLuaScript()
        {
            string[] luaFiles=Directory.GetFiles(PathUtil.LuaPath,"*.bytes",SearchOption.AllDirectories);
            for (int i = 0; i < luaFiles.Length; i++)
            {
                string fileName=PathUtil.GetStandardPath(luaFiles[i]);
                Debug.LogFormat("luaFile:{0}",fileName);
                byte[] file=File.ReadAllBytes(fileName);
                AddLuaScripts(PathUtil.GetUnityPath(fileName),file);
            }
            //所有lua加载完毕
            InitOK?.Invoke();
        }
    #endif

    void LoadLuaScripts()
    {
        foreach (string name in LuaNames)
        {
            SourceManager.Instance.LoadLua(name,(UnityEngine.Object obj)=>
            {
                AddLuaScripts(name ,(obj as TextAsset).bytes);  //拿到这个LuaScripts的文件后
                if(m_LuaScripts.Count>=LuaNames.Count)
                {
                    //所有lua加载完毕
                    InitOK?.Invoke();
                    LuaNames.Clear();
                    LuaNames=null;
                }
            }
            );
        }
    }

    private void Update()
    {
        if(Luaenv!=null)
        {
            Luaenv.Tick();
        }
    }


    private void  OnDestroy() 
    {
        if(Luaenv!=null)
        {
            Luaenv.Dispose();
            Luaenv=null;
        }
    }
    
}
