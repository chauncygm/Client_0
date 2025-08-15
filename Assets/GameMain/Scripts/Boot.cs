using System.Collections;
using System.Threading.Tasks;
using GameMain.Scripts.Utils;
using UnityEngine;
using YooAsset;

namespace GameMain.Scripts
{
    public class Boot : MonoSingleton<Boot>
    {

        public int targetFPS = 60;
        
        public EPlayMode playMode = EPlayMode.OfflinePlayMode;

        private IEnumerator Start()
        {
            Application.targetFrameRate = targetFPS;
            Application.runInBackground = true;
            
            YooAssets.Initialize();
    
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
            yield return new Wait60Frames(Time.frameCount);
        }

        private class Wait60Frames : CustomYieldInstruction
        {
            private readonly int _startFrame;

            public Wait60Frames(int startFrame)
            {
                _startFrame = startFrame;
                Task.Delay(1);
            }

            public override bool keepWaiting
            {
                get { return Time.frameCount < _startFrame + 60; }
            }
        }
    }
}