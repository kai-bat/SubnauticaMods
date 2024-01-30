using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    public static void Build()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
