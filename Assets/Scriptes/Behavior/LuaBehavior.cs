using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;
namespace Behavior{


/// <summary>
/// 此行为类为所有行为的父类
/// </summary>
[LuaCallCSharp]
public class LuaBehavior : MonoBehaviour
{

    private LuaEnv m_LuaEnv=LuaManager.Instance.Luaenv;

    protected LuaTable m_SciptEnv;
    private Action m_LuaInit;
    private Action m_LuaUpdate;
    private Action m_LuaOnDestroy;



    private void Awake() {
        m_SciptEnv=m_LuaEnv.NewTable();
        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = m_LuaEnv.NewTable();
            meta.Set("__index", m_LuaEnv.Global); 
            m_SciptEnv.SetMetaTable(meta);
            meta.Dispose();


            //向Lua脚本定义一些Unity的内置函数
            m_SciptEnv.Set("self", this);


    }


    //把awake和start替换掉  在awake之后执行初始化
    public virtual void Init(string LuaName)
    {
        Debug.LogFormat("拿到UI测试Lua脚本:{0}",LuaName);
        m_LuaEnv.DoString(LuaManager.Instance.GetLuaScripts(LuaName),LuaName,m_SciptEnv);
      //  m_SciptEnv.Get("Awake", out m_LuaAwake);
       // m_SciptEnv.Get("Start", out m_LuaStart);
        m_SciptEnv.Get("OnInit", out m_LuaInit);
        m_SciptEnv.Get("Update", out m_LuaUpdate);
        m_SciptEnv.Get("OnDestroy", out m_LuaOnDestroy);

        m_LuaInit?.Invoke();
    }





    // Update is called once per frame
    void Update()
    {
        m_LuaUpdate?.Invoke();
    }


    private void OnDestroy() {
        m_LuaOnDestroy?.Invoke();
        Clear();
    }


    private void OnApplicationQuit() {
        Clear();
    }


    protected virtual void Clear()
    {
        m_LuaInit=null;
        m_LuaUpdate=null;
        m_LuaOnDestroy=null;
        m_SciptEnv?.Dispose();
        m_SciptEnv=null;
    }

}
}
