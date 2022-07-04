using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior;
using System;
public class SceneLogic : LuaBehavior
{

    public string sceneName;

    Action m_LuaActive;
    Action m_LuaInActive;
    Action m_LuaOnEnter;
    Action m_LuaOnQuit;

    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        m_SciptEnv.Get("OnActive",out m_LuaActive);
        m_SciptEnv.Get("OnInActive",out m_LuaInActive);
        m_SciptEnv.Get("OnEnter",out m_LuaOnEnter);
        m_SciptEnv.Get("OnQuit",out m_LuaOnQuit);        
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaActive=null;
        m_LuaInActive=null;
        m_LuaOnEnter=null;
        m_LuaOnQuit=null;
    }


    public void OnActive()
    {
        m_LuaActive?.Invoke();
    }

    public void OnInActive()
    {
        m_LuaInActive?.Invoke();
    }

        public void OnEnter()
    {
        m_LuaOnEnter?.Invoke();
    }

        public void OnQuit()
    {
        m_LuaOnQuit?.Invoke();
    }
}
