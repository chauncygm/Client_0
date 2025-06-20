using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class ExportScene : EditorWindow
    {
        static ExportScene()
        {
            _cutMinY = 0;
            _cutMaxY = 0;
            _cutMaxX = 0;
        }

        private const string CutLBObjPath = "export/bound_lb";
        private const string CutRTObjPath = "export/bound_rt";

        private static float _autoCutMinX = 1000;
        private static float _autoCutMaxX;
        private static float _autoCutMinY = 1000;
        private static float _autoCutMaxY;

        private static float _cutMinX;
        private static float _cutMaxX;
        private static float _cutMinY;
        private static float _cutMaxY;

        private static long _startTime;
        private static int _totalCount;
        private static int _count;
        private static int _counter;
        private static int _progressUpdateInterval = 10000;

        [MenuItem("ExportScene/ExportSceneToObj")]
        [MenuItem("GameObject/ExportScene/ExportSceneToObj")]
        [Obsolete("Obsolete")]
        public static void Export()
        {
            ExportSceneToObj(false);
        }

        [MenuItem("ExportScene/ExportSceneToObj(AutoCut)")]
        [MenuItem("GameObject/ExportScene/ExportSceneToObj(AutoCut)")]
        [Obsolete("Obsolete")]
        public static void ExportAutoCut()
        {
            ExportSceneToObj(true);
        }

        [MenuItem("ExportScene/ExportSelectedObj")]
        [MenuItem("GameObject/ExportScene/ExportSelectedObj", priority = 44)]
        public static void ExportObj()
        {
            var selectObj = Selection.activeGameObject;
            if (selectObj == null)
            {
                Debug.LogWarning("Select a GameObject");
                return;
            }
            var path = GetSavePath(false, selectObj);
            if (string.IsNullOrEmpty(path)) return;

            var terrain = selectObj.GetComponent<Terrain>();
            var mfs = selectObj.GetComponentsInChildren<MeshFilter>();
            var smrs = selectObj.GetComponentsInChildren<SkinnedMeshRenderer>();
            Debug.Log(mfs.Length + "," + smrs.Length);
            ExportSceneToObj(path, terrain, mfs, smrs, false, false);
        }

        [Obsolete("Obsolete")]
        private static void ExportSceneToObj(bool autoCut)
        {
            var path = GetSavePath(autoCut, null);
            if (string.IsNullOrEmpty(path)) return;
            var terrain = FindObjectOfType<Terrain>();
            var mfs = FindObjectsOfType<MeshFilter>();
            var smrs = FindObjectsOfType<SkinnedMeshRenderer>();
            ExportSceneToObj(path, terrain, mfs, smrs, autoCut, true);
        }

        private static void ExportSceneToObj(string path, Terrain terrain, MeshFilter[] mfs,
            SkinnedMeshRenderer[] smrs, bool autoCut, bool needCheckRect)
        {
            var vertexOffset = 0;
            const string title = "export GameObject to .obj ...";
            var writer = new StreamWriter(path);

            _startTime = GetMsTime();
            UpdateCutRect(autoCut);
            _counter = _count = 0;
            _progressUpdateInterval = 5;
            _totalCount = (mfs.Length + smrs.Length) / _progressUpdateInterval;
            foreach (var mf in mfs)
            {
                UpdateProgress(title);
                if (mf.GetComponent<Renderer>() != null &&
                    (!needCheckRect || IsInCutRect(mf.gameObject)))
                {
                    ExportMeshToObj(mf.gameObject, mf.sharedMesh, ref writer, ref vertexOffset);
                }
            }
            foreach (var smr in smrs)
            {
                UpdateProgress(title);
                if (!needCheckRect || IsInCutRect(smr.gameObject))
                {
                    ExportMeshToObj(smr.gameObject, smr.sharedMesh, ref writer, ref vertexOffset);
                }
            }
            if (terrain)
            {
                ExportTerrianToObj(terrain.terrainData, terrain.GetPosition(),
                    ref writer, ref vertexOffset, autoCut);
            }
            writer.Close();
            EditorUtility.ClearProgressBar();

            var endTime = GetMsTime();
            var time = (float)(endTime - _startTime) / 1000;
            Debug.Log("Export SUCCESS:" + path);
            Debug.Log("Export Time:" + time + "s");
            OpenDir(path);
        }

    private static void OpenDir(string path)
    {
        var dir = Directory.GetParent(path);
        OpenCmd("explorer.exe", dir!.FullName);
    }

        private static void OpenCmd(string cmd, string args)
        {
            System.Diagnostics.Process.Start(cmd, args);
        }

        private static string GetSavePath(bool autoCut, GameObject selectObject)
        {
            var dataPath = Application.dataPath;
            var dir = dataPath[..dataPath.LastIndexOf("/", StringComparison.Ordinal)];
            var sceneName = SceneManager.GetActiveScene().name;
            string defaultName;
            if (selectObject == null)
            {
                defaultName = (autoCut ? sceneName + "(autoCut)" : sceneName);
            }
            else
            {
                defaultName = (autoCut ? selectObject.name + "(autoCut)" : selectObject.name);
            }
            return EditorUtility.SaveFilePanel("Export .obj file", dir, defaultName, "obj");
        }

        private static long GetMsTime()
        {
            return DateTime.Now.Ticks / 10000;
            //return (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        private static void UpdateCutRect(bool autoCut)
        {
            _cutMinX = _cutMaxX = _cutMinY = _cutMaxY = 0;
            if (autoCut) return;
            var lbPos = GetObjPos(CutLBObjPath);
            var rtPos = GetObjPos(CutRTObjPath);
            _cutMinX = lbPos.x;
            _cutMaxX = rtPos.x;
            _cutMinY = lbPos.z;
            _cutMaxY = rtPos.z;
        }

        private static void UpdateAutoCutRect(Vector3 v)
        {
            if (v.x < _autoCutMinX) _autoCutMinX = v.x;
            if (v.x > _autoCutMaxX) _autoCutMaxX = v.x;
            if (v.z < _autoCutMinY) _autoCutMinY = v.z;
            if (v.z > _autoCutMaxY) _autoCutMaxY = v.z;
        }

        private static bool IsInCutRect(GameObject obj)
        {
            if (_cutMinX == 0 && _cutMaxX == 0 && _cutMinY == 0 && _cutMaxY == 0) return true;
            var pos = obj.transform.position;
            return pos.x >= _cutMinX && pos.x <= _cutMaxX && pos.z >= _cutMinY && pos.z <= _cutMaxY;
        }

        private static void ExportMeshToObj(GameObject obj, Mesh mesh, ref StreamWriter writer, ref int vertexOffset)
        {
            var r = obj.transform.localRotation;
            var sb = new StringBuilder();
            foreach (var vertice in mesh.vertices)
            {
                var v = obj.transform.TransformPoint(vertice);
                UpdateAutoCutRect(v);
                sb.AppendFormat("v {0} {1} {2}\n", -v.x, v.y, v.z);
            }
            foreach (var nn in mesh.normals)
            {
                var v = r * nn;
                sb.AppendFormat("vn {0} {1} {2}\n", -v.x, -v.y, v.z);
            }
            foreach (Vector3 v in mesh.uv)
            {
                sb.AppendFormat("vt {0} {1}\n", v.x, v.y);
            }
            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                var triangles = mesh.GetTriangles(i);
                for (var j = 0; j < triangles.Length; j += 3)
                {
                    sb.AppendFormat("f {1} {0} {2}\n",
                        triangles[j] + 1 + vertexOffset,
                        triangles[j + 1] + 1 + vertexOffset,
                        triangles[j + 2] + 1 + vertexOffset);
                }
            }
            vertexOffset += mesh.vertices.Length;
            writer.Write(sb.ToString());
        }

        private static void ExportTerrianToObj(TerrainData terrain, Vector3 terrainPos,
            ref StreamWriter writer, ref int vertexOffset, bool autoCut)
        {
            var tw = terrain.heightmapResolution;
            var th = terrain.heightmapResolution;

            var meshScale = terrain.size;
            meshScale = new Vector3(meshScale.x / (tw - 1), meshScale.y, meshScale.z / (th - 1));
            var uvScale = new Vector2(1.0f / (tw - 1), 1.0f / (th - 1));

            Vector2 terrainBoundLB, terrainBoundRT;
            if (autoCut)
            {
                terrainBoundLB = GetTerrainBoundPos(new Vector3(_autoCutMinX, 0, _autoCutMinY), terrain, terrainPos);
                terrainBoundRT = GetTerrainBoundPos(new Vector3(_autoCutMaxX, 0, _autoCutMaxY), terrain, terrainPos);
            }
            else
            {
                terrainBoundLB = GetTerrainBoundPos(CutLBObjPath, terrain, terrainPos);
                terrainBoundRT = GetTerrainBoundPos(CutRTObjPath, terrain, terrainPos);
            }

            var bw = (int)(terrainBoundRT.x - terrainBoundLB.x);
            var bh = (int)(terrainBoundRT.y - terrainBoundLB.y);

            var w = bh != 0 && bh < th ? bh : th;
            var h = bw != 0 && bw < tw ? bw : tw;

            var startX = (int)terrainBoundLB.y;
            var startY = (int)terrainBoundLB.x;
            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;

            Debug.Log($"Terrian:tw={tw},th={th},sw={bw},sh={bh},startX={startX},startY={startY}");

            var tData = terrain.GetHeights(0, 0, tw, th);
            var tVertices = new Vector3[w * h];
            var tUV = new Vector2[w * h];

            var tPolys = new int[(w - 1) * (h - 1) * 6];

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var pos = new Vector3(-(startY + y), tData[startX + x, startY + y], (startX + x));
                    tVertices[y * w + x] = Vector3.Scale(meshScale, pos) + terrainPos;
                    tUV[y * w + x] = Vector2.Scale(new Vector2(x, y), uvScale);
                }
            }
            var index = 0;
            for (var y = 0; y < h - 1; y++)
            {
                for (var x = 0; x < w - 1; x++)
                {
                    tPolys[index++] = (y * w) + x;
                    tPolys[index++] = ((y + 1) * w) + x;
                    tPolys[index++] = (y * w) + x + 1;
                    tPolys[index++] = ((y + 1) * w) + x;
                    tPolys[index++] = ((y + 1) * w) + x + 1;
                    tPolys[index++] = (y * w) + x + 1;
                }
            }
            _count = _counter = 0;
            _progressUpdateInterval = 10000;
            _totalCount = (tVertices.Length + tUV.Length + tPolys.Length / 3) / _progressUpdateInterval;
            const string title = "export Terrain to .obj ...";
            for (var i = 0; i < tVertices.Length; i++)
            {
                UpdateProgress(title);
                var sb = new StringBuilder(22);
                sb.AppendFormat("v {0} {1} {2}\n", tVertices[i].x, tVertices[i].y, tVertices[i].z);
                writer.Write(sb.ToString());
            }
            for (var i = 0; i < tUV.Length; i++)
            {
                UpdateProgress(title);
                var sb = new StringBuilder(20);
                sb.AppendFormat("vt {0} {1}\n", tUV[i].x, tUV[i].y);
                writer.Write(sb.ToString());
            }
            for (var i = 0; i < tPolys.Length; i += 3)
            {
                UpdateProgress(title);
                var x = tPolys[i] + 1 + vertexOffset;
                var y = tPolys[i + 1] + 1 + vertexOffset;
                var z = tPolys[i + 2] + 1 + vertexOffset;
                var sb = new StringBuilder(30);
                sb.AppendFormat("f {0} {1} {2}\n", x, y, z);
                writer.Write(sb.ToString());
            }
            vertexOffset += tVertices.Length;
        }

        private static Vector2 GetTerrainBoundPos(string path, TerrainData terrain, Vector3 terrainPos)
        {
            var go = GameObject.Find(path);
            if (!go) return Vector2.zero;
            var pos = go.transform.position;
            return GetTerrainBoundPos(pos, terrain, terrainPos);
        }

        private static Vector2 GetTerrainBoundPos(Vector3 worldPos, TerrainData terrain, Vector3 terrainPos)
        {
            var tpos = worldPos - terrainPos;
            return new Vector2((int)(tpos.x / terrain.size.x * terrain.heightmapResolution),
                (int)(tpos.z / terrain.size.z * terrain.heightmapResolution));
        }

        private static Vector3 GetObjPos(string path)
        {
            var go = GameObject.Find(path);
            return go ? go.transform.position : Vector3.zero;
        }

        private static void UpdateProgress(string title)
        {
            if (_counter++ != _progressUpdateInterval) return;
            _counter = 0;
            var process = Mathf.InverseLerp(0, _totalCount, ++_count);
            var currTime = GetMsTime();
            var sec = ((float)(currTime - _startTime)) / 1000;
            var text = $"{_count}/{_totalCount}({sec:f2} sec.)";
            EditorUtility.DisplayProgressBar(title, text, process);
        }
    }
}