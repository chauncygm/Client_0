using System.Collections;
using System.Threading.Tasks;
using GameBase;
using GameMain.Scripts.Utils;
using UnityEngine;
using YooAsset;

namespace GameMain.Scripts
{
    public class Boot : MonoBehaviour
    {

        private IEnumerator Start()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
    
            print($"Application version: {Application.version}");
            print($"Application isEditor: {Application.isEditor}");
            print($"Application platform: {Application.platform}");
            print($"Application dataPath: {Application.dataPath}");
            print($"Application Streaming Assets Path: {Application.streamingAssetsPath}");
            print($"Application Persistent Data Path: {Application.persistentDataPath}");
            print($"Application Temporary Cache Path: {Application.temporaryCachePath}");
            print($"Application consoleLogPath: {Application.consoleLogPath}");
            print($"Application Identifier: {Application.identifier}");
            print($"Application genuine: {Application.genuine}");
            print($"Application installMode: {Application.installMode}");
            print($"Application absoluteURL: {Application.absoluteURL}");
            return null;
        }
        
    }
}