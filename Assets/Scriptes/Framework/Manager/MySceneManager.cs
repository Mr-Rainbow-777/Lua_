using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoSingleton<MySceneManager>
{


    private string m_logicName="[SceneLogic]";


    private void Awake() {
         SceneManager.activeSceneChanged+=OnActiveSceneChanged;
    }


    public void OnActiveSceneChanged(Scene s1,Scene s2)
    {
        if(!s1.isLoaded||!s2.isLoaded)
        {
            return;
        }

        SceneLogic logic1=GetSceneLogic(s1);
        SceneLogic logic2=GetSceneLogic(s2);

        logic1?.OnInActive();
        logic2?.OnActive();
    }




    public void SetActive(string scenename)
    {
        Scene scene=SceneManager.GetSceneByName(scenename);
        SceneManager.SetActiveScene(scene);
    }


/// <summary>
/// 叠加场景
/// </summary>
/// <param name="sceneName"></param>
/// <param name="luaName"></param>
    public void LoadScene(string sceneName,string luaName){
        SourceManager.Instance.LoadScene(sceneName,(Object obj)=>
        {
            StartCoroutine(StartLoadScene(sceneName,luaName,LoadSceneMode.Additive));
        }
        );
    }


/// <summary>
/// 替换场景
/// </summary>
/// <param name="sceneName"></param>
/// <param name="luaName"></param>
    public void Change(string sceneName,string luaName){
        SourceManager.Instance.LoadScene(sceneName,(Object obj)=>
        {
            StartCoroutine(StartLoadScene(sceneName,luaName,LoadSceneMode.Single));
        }
        );
    }






    private bool IsLoadScene(string sceneName)
    {
        Scene scene=SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }



   IEnumerator StartLoadScene(string sceneName,string luaName,LoadSceneMode mode)
   {
       if(IsLoadScene(sceneName))
       {
           yield break;
       }

       AsyncOperation async=SceneManager.LoadSceneAsync(sceneName,mode);
       async.allowSceneActivation=true;
       yield return async;

       Scene scene=SceneManager.GetSceneByName(sceneName);
       GameObject go=new GameObject(m_logicName);

       SceneManager.MoveGameObjectToScene(go,scene);

       Debug.Log(sceneName);
       SceneLogic logic=go.AddComponent<SceneLogic>();
       logic.sceneName=sceneName;
       logic.Init(luaName);
       logic.OnEnter();
   }


    private IEnumerator UnloadScene(string sceneName)
    {
        Scene scene=SceneManager.GetSceneByName(sceneName);
        if(!scene.isLoaded)
        {
            Debug.LogError("scene is not load");
            yield break;
        }
        SceneLogic logic=GetSceneLogic(scene);
        logic?.OnQuit();
        AsyncOperation async=SceneManager.UnloadSceneAsync(scene);
        yield return async;

    }

    public SceneLogic GetSceneLogic(Scene scene)
    {
        GameObject[] gameObjects=scene.GetRootGameObjects();
        foreach (var go in gameObjects)
        {
            if(go.name.CompareTo(m_logicName)==0)
            {
                SceneLogic logic=go.GetComponent<SceneLogic>();
                return logic;
            }
        }
        return null;
    }


}
