using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior;
using System;
public class UILogic : LuaBehavior
{
    Action m_LuaOpen;
    Action m_LuaClose;

    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        m_SciptEnv.Get("OnOpen",out m_LuaOpen);
        m_SciptEnv.Get("OnClose",out m_LuaClose);        
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaClose=null;
        m_LuaOpen=null;
    }


    public void OnOpen()
    {
        m_LuaOpen?.Invoke();
    }

    public void OnClose()
    {
        m_LuaClose?.Invoke();
    }
}
