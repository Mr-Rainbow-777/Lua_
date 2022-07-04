using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    Dictionary<string,GameObject> m_UI=new Dictionary<string, GameObject>();

    Dictionary<string,Transform> m_UIGroups=new Dictionary<string, Transform>();


    [SerializeField]
    private Transform root;

    private void Awake() {
        
    }





    public void AllocateGroup(List<string> groups)
    {
        foreach (var temp in groups)
        {
            GameObject go=new GameObject("Group-"+temp);
            go.transform.SetParent(root,false);
            m_UIGroups.Add(temp,go.transform);
        }
    }


    public Transform GetUIGroup(string group)
    {
        if(!m_UIGroups.ContainsKey(group))
        {
            Debug.LogError("没有找到这个分组");
        }
        return m_UIGroups[group];
    }

    public void OpenUI(string entityName,string group,string luaName)
    {
        GameObject UI=null;
        if(m_UI.TryGetValue(entityName,out UI))
        {
            UILogic uilogic=UI.GetComponent<UILogic>();
            uilogic.OnOpen();
            return;
        }

        SourceManager.Instance.LoadUI(entityName,(UnityEngine.Object obj)=>
        {
            Transform trans= GetUIGroup(group);
            UI=Instantiate(obj) as GameObject;
            UI.transform.SetParent(trans,false);
            m_UI.Add(entityName,UI);
            UILogic uilogic=UI.AddComponent<UILogic>();
            uilogic.Init(luaName);   //这里只有当第一次获取它的时候才init一次，且仅有一次
            uilogic.OnOpen();

        }
        );
    }


}
