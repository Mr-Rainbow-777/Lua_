using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UObject=UnityEngine.Object;
public class HotUpdate : MonoBehaviour
{

     byte[] m_ReadPathFileListData; 
     byte[] m_ServerFileListData;

    internal class DownFileInfo
    {
        public string url;
        public string filename;
        public DownloadHandler fileData;
    }

    //下载单个文件
    IEnumerator DownLoadFile(DownFileInfo info,Action<DownFileInfo> Complete)
    {
        UnityWebRequest webRequest=UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if(webRequest.result==UnityWebRequest.Result.ProtocolError||webRequest.result==UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("下载文件出错");
            yield break;
        }
        info.fileData=webRequest.downloadHandler;
        Complete?.Invoke(info);
    }

    //批量下载文件
    IEnumerator DownLoadFile(List<DownFileInfo> infos,Action<DownFileInfo> Complete,Action DownLoadAllcomplete)
    {
        foreach (var info in infos)
        {
            yield return DownLoadFile(info,Complete);
        }
        DownLoadAllcomplete?.Invoke();
    }

    //获取文件信息
    private List<DownFileInfo> GetFileList(string filedata,string path)
    {
        string content=filedata.Trim().Replace("\r","");
        string[] files=content.Split('\n');
        List<DownFileInfo> downfileinfos=new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string[] info=files[i].Split('|');
            DownFileInfo fileinfo=new DownFileInfo();
            fileinfo.filename=info[1];     //filename就是ab包的Bundle文件路径
            fileinfo.url=Path.Combine(path,info[1]);    //url=只读路径+Bundle路径
            downfileinfos.Add(fileinfo);
        }
        return downfileinfos;
    }

    private void Start() {
        if(IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }


    private bool IsFirstInstall()
    {
        //判断只读目录是否存在版本文件
        bool IsExitsReadPath=FileUtil.IsExists(Path.Combine(PathUtil.ReadPath,Appconst.Instance.FileList));
        //检查Persistent文件下有没有Filelist   代表有没有安装过或者更新过
        bool IsExitesReadWritePath=FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath,Appconst.Instance.FileList));

        return IsExitsReadPath&&!IsExitesReadWritePath;
    }


    private void ReleaseResources()
    {
        string url=Path.Combine(Appconst.Instance.ResourceUrl,Appconst.Instance.FileList);
        DownFileInfo info=new DownFileInfo();
        info.url=url;
        StartCoroutine(DownLoadFile(info,OnDownLoadReadPahtFileListComplete));
    }


    private void OnDownLoadReadPahtFileListComplete(DownFileInfo file)   //先下载的是filelist文件
    {
        m_ReadPathFileListData=file.fileData.data;
        List<DownFileInfo> fileinfos=GetFileList(file.fileData.text,PathUtil.ReadPath); //拿到所有资源的信息
        StartCoroutine(DownLoadFile(fileinfos,OnReleaseFileComplete,OnReleaseAllFileComplete));  //接着下载不同文件夹下的资源
    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath,Appconst.Instance.FileList),m_ReadPathFileListData);  //最后再写入filelist文件，防止在下载的时候网络中断，下次打开时检查filelist是否存在来决定是否下载
        CheckUpdate();
    }


    private void OnReleaseFileComplete(DownFileInfo fileinfo)
    {
        Debug.LogFormat("Release File:{0}",fileinfo.url);
        string writeFile=Path.Combine(PathUtil.ReadWritePath,fileinfo.filename);  //把这些拿到的文件全部写到Persistent文件夹下
        FileUtil.WriteFile(writeFile,fileinfo.fileData.data);
    }

    public void CheckUpdate()
    {
        string url=Path.Combine(Appconst.Instance.ResourceUrl,Appconst.Instance.FileList);
        DownFileInfo info=new DownFileInfo();
        info.url=url;
        StartCoroutine(DownLoadFile(info,OnDownLoadServerFileListComplete));
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo file)
    {
        m_ServerFileListData =file.fileData.data;
        List<DownFileInfo> fileinfos=GetFileList(file.fileData.text,Appconst.Instance.ResourceUrl);
        List<DownFileInfo> downlistinfos=new List<DownFileInfo>();
        for (int i = 0; i < fileinfos.Count; i++)
        {
            string localfile=Path.Combine(PathUtil.ReadWritePath,fileinfos[i].filename);
            if(!FileUtil.IsExists(localfile))  //检查这个文件是否已经下载   如果是则不需要写入  如果不存在  则需要下载并写入
            {
                fileinfos[i].url=Path.Combine(Appconst.Instance.ResourceUrl,fileinfos[i].filename);
                downlistinfos.Add(fileinfos[i]);
            }
        }
        if(downlistinfos.Count>0)
        {
            StartCoroutine(DownLoadFile(fileinfos,OnUpdateFileComplete,OnUpdateAllFileComplete));
        }
        else
        {
            EnterGame();
        }
    }

    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath,Appconst.Instance.FileList),m_ServerFileListData);
        EnterGame();
    }

    private void OnUpdateFileComplete(DownFileInfo fileinfo)
    {
        Debug.LogFormat("Update File:{0}",fileinfo.url);
        string writeFile=Path.Combine(PathUtil.ReadWritePath,fileinfo.filename);
        FileUtil.WriteFile(writeFile,fileinfo.fileData.data);
    }


    private void EnterGame()
    {
        SourceManager.Instance.LoadUI("BackGround",Oncomplete);
    }

        public void Oncomplete(UObject obj)
    {
        GameObject go=Instantiate(obj) as GameObject;
        go.SetActive(true);
    }

}
