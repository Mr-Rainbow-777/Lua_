using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    /*
    IEnumerator Start() {
        AssetBundleCreateRequest request=AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/ui/prefab/background.prefab.ab");
        yield return request;
        AssetBundleRequest bundlerequest=request.assetBundle.LoadAssetAsync("Assets/BuildResources/UI/Prefab/BackGround.prefab");
        yield return bundlerequest;

        GameObject go=Instantiate(bundlerequest.asset) as GameObject;
        go.SetActive(true);

    }
    */
    
    void Update()
    {

    }
}
