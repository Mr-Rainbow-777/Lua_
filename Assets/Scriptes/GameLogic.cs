using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField]
    PlayMode playmode;
    void Start()
    {
        Appconst.Instance.playMode=playmode;


        LuaManager.Instance.Init(()=>{
            
            LuaManager.Instance.StartLua("main");
        });


        XLua.LuaFunction func=LuaManager.Instance.Luaenv.Global.Get<XLua.LuaFunction>("Main");
        func.Call();
    }


}
