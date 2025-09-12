using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset.Editor;

[DisplayName("打包特效纹理（自定义）")]
public class PackEffectTexture : IPackRule
{
    private const string PackDirectory = "Assets/Effect/Textures/";

    PackRuleResult IPackRule.GetPackRuleResult(PackRuleData data)
    {
        var assetPath = data.AssetPath;
        if (!assetPath.StartsWith(PackDirectory))
            throw new Exception($"Only support folder : {PackDirectory}");
    
        var assetName = Path.GetFileName(assetPath).ToLower();
        var firstChar = assetName.Substring(0, 1);
        var bundleName = $"{PackDirectory}effect_texture_{firstChar}";
        return new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
    }
}

[DisplayName("打包视频（自定义）")]
public class PackVideo : IPackRule
{
    public PackRuleResult GetPackRuleResult(PackRuleData data)
    {
        var bundleName = RemoveExtension(data.AssetPath);
        var fileExtension = Path.GetExtension(data.AssetPath).Remove(0, 1);
        return new PackRuleResult(bundleName, fileExtension);
    }

    /// <summary>
    /// 移除文件扩展名
    /// "assets/config/test.unity3d" --> "assets/config/test"
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string RemoveExtension(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        var index = str.LastIndexOf(".", StringComparison.Ordinal);
        return index == -1 ? str : str.Remove(index);
    }
}