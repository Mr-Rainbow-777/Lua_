using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoSingleton<EntityManager>
{
    Dictionary<string,GameObject> m_Entity=new Dictionary<string, GameObject>();

    Dictionary<string,Transform> m_EntityGroups=new Dictionary<string, Transform>();


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
            m_EntityGroups.Add(temp,go.transform);
        }
    }


    public Transform GetEntityGroup(string group)
    {
        if(!m_EntityGroups.ContainsKey(group))
        {
            Debug.LogError("没有找到这个分组");
        }
        return m_EntityGroups[group];
    }

    public void ShowEntity(string entityName,string group,string luaName)
    {
        GameObject entity=null;
        if(m_Entity.TryGetValue(entityName,out entity))
        {
            EntityLogic eLogic=entity.GetComponent<EntityLogic>();
            eLogic.OnShow();
            return;
        }

        SourceManager.Instance.LoadPrefab(entityName,(UnityEngine.Object obj)=>
        {
            Transform trans= GetEntityGroup(group);
            entity=Instantiate(obj) as GameObject;
            entity.transform.SetParent(trans,false);
            m_Entity.Add(entityName,entity);
            EntityLogic eLogic=entity.AddComponent<EntityLogic>();
            eLogic.Init(luaName);   //这里只有当第一次获取它的时候才init一次，且仅有一次
            eLogic.OnShow();

        }
        );
    }


}
