using UnityEditor;

namespace GameMain.Scripts
{
    public static class RenameMixamoAnimationClip
    {
        //修改  Mixamo  导出的动画名字
        [MenuItem("Assets/Auto Rename Mixamo AnimationClip")]
        private static void RenameMixamoAnimationClips()
        {
            var objs = Selection.gameObjects;
            if (objs == null || objs.Length == 0) return;

            foreach (var obj in objs)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);

                var modelImporter = (ModelImporter)AssetImporter.GetAtPath(assetPath);
                if (modelImporter == null) continue;

                var clips = modelImporter.clipAnimations; // get first clip
                if (clips == null || clips.Length == 0)
                    clips = modelImporter.defaultClipAnimations;

                foreach (var clip in clips)
                {
                    clip.name = obj.name;
                }

                modelImporter.clipAnimations = clips;
                modelImporter.SaveAndReimport();
            }
        }
    }
}
