using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior;
using System;
public class EntityLogic : LuaBehavior
{
    Action m_LuaShow;
    Action m_LuaHide;

    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        m_SciptEnv.Get("OnShow",out m_LuaShow);
        m_SciptEnv.Get("OnHide",out m_LuaHide);        
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaShow=null;
        m_LuaHide=null;
    }


    public void OnShow()
    {
        m_LuaShow?.Invoke();
    }

    public void OnHide()
    {
        m_LuaHide?.Invoke();
    }
}
