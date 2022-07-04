using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class BuildTool : Editor
{



    [MenuItem("Tools/Build Windows Bundle")]
    static void BundleWindowsBuild()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Android Bundle")]
    static void BundleAndroidBuild()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("Tools/Build IOS Bundle")]
    static void BundleIOSBuild()
    {
        Build(BuildTarget.iOS);
    }



    static void Build(BuildTarget target)
    {
        
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        //文件信息列表
        List<string> bundleInfos=new List<string>();
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories); //得到这个路径下所有的文件 
        for (int i = 0; i < files.Length; i++)
        {
            if(files[i].EndsWith(".meta"))  //丢掉Meta文件
            {
                continue;
            }

            AssetBundleBuild assetBuild = new AssetBundleBuild();  // Bundle构建类 

            string filename = PathUtil.GetStandardPath(files[i]);  //得到文件名的完整路径
            Debug.Log("files:" + filename);

            string assetname = PathUtil.GetUnityPath(filename);   //Asset开头的路径
            assetBuild.assetNames = new string[] { assetname };
            string bundleName = filename.Replace(PathUtil.BuildResourcesPath+'/', "").ToLower();  //Bundle Name
            assetBuild.assetBundleName = bundleName+".ab" ;  //文件路径名+后缀
            builds.Add(assetBuild);


            //添加文件和依赖信息
            List<string> dependenceInfo=GetDependence(assetname);
            string bundleInfo=assetname+"|"+bundleName+".ab";

            if(dependenceInfo.Count>0)
            {
                bundleInfo=bundleInfo+"|"+string.Join("|",dependenceInfo);
            }
            bundleInfos.Add(bundleInfo);


        }

        if(Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, builds.ToArray(), 
                                        BuildAssetBundleOptions.None, target);
        File.WriteAllLines(PathUtil.BundleOutPath+"/"+"pathlist.txt",bundleInfos);   //Pathlist文件

        AssetDatabase.Refresh();
    }


    #region 获取依赖文件        格式： 文件路径名|Bundle名|依赖文件列表
    public static List<string> GetDependence(string curFile)
    {
        List<string> dependence=new List<string>();
        string[] files=AssetDatabase.GetDependencies(curFile);   //GetDependencies() 会返回材质资源的路径，而不是 GameObjects，因为这些不是磁盘上的资源。
        dependence=files.Where(file=> !file.EndsWith(".cs")&&!file.Equals(curFile)).ToList();
        return dependence;
    }
    #endregion


}
