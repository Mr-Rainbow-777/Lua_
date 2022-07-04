using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public enum PlayMode
    {
        EditorMode=1,
        PackgeMode=2,
        UpdateMode=3,
    }
public class Appconst:Singleton<Appconst>
{
    public PlayMode playMode;

    public  string ResourceUrl="http://localhost/AssetBundles";
    public  string FileList="pathlist.txt";
}
