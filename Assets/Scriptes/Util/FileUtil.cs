using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FileUtil 
{

    //检测文件是否存在
    public static bool IsExists(string path)
    {
        FileInfo file=new FileInfo(path);
        return file.Exists;
    }


    public static void WriteFile(string path,byte[] data)
    {
        path=PathUtil.GetStandardPath(path);
        string dir=path.Substring(0,path.LastIndexOf("/"));  //找到放文件的那个文件夹
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        FileInfo file=new FileInfo(path);
        if(file.Exists)
        {
            file.Delete();
        }
        try
        {
            using(FileStream fs=new FileStream(path,FileMode.Create,FileAccess.Write))
            {
                fs.Write(data,0,data.Length);
                fs.Close();
            }
        }
        catch(IOException e)
        {
            Debug.LogError(e.Message);
        }
    }
}
