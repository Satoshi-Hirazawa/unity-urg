using SCIP_library;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URG {
    public class DebugDraw : MonoBehaviour {

        public Material material;
        public int vertexCount = 2;
        public int instanceCount = 100;

        private ComputeBuffer bufferPoints;
        private ComputeBuffer bufferPos;
        private Vector3[] origPos;
        private Vector3[] pos;

        private ComputeBuffer bufferDistances;

        void Start() {
            // 円周上の座標を作成・格納
            var verts = new Vector3[vertexCount];
            for (var i = 0; i < vertexCount; ++i) {
                float phi = i * Mathf.PI * 2.0f / (vertexCount - 1);
                verts[i] = new Vector3(Mathf.Cos(phi), Mathf.Sin(phi), 0.0f);
            }

            ReleaseBuffers();

            // 作成した円周上の座標を ComputeBuffer へ転送
            // 12 は float (4 byte x 3）
            bufferPoints = new ComputeBuffer(vertexCount, 12);
            bufferPoints.SetData(verts);
            material.SetBuffer("buf_Points", bufferPoints);

            // 各インスタンスの初期座標を作成
            // 最初は適当にランダムな点を格納
            origPos = new Vector3[instanceCount];
            for (var i = 0; i < instanceCount; ++i)
                origPos[i] = Random.insideUnitSphere * 5.0f;

            // GPU 側へ渡す座標配列を作成
            pos = new Vector3[instanceCount];

            // 位置更新用の ComputeBuffer を作成
            // 実際の位置は Update() 内で渡す
            bufferPos = new ComputeBuffer(instanceCount, 12);
            material.SetBuffer("buf_Positions", bufferPos);

            
        }

        private void ReleaseBuffers() {
            if (bufferPoints != null) {
                bufferPoints.Release();
                bufferPoints = null;
            }
            if (bufferPos != null) {
                bufferPos.Release();
                bufferPos = null;
            }
            if (bufferDistances != null) {
                bufferDistances.Release();
                bufferDistances = null;
            }
        }

        void OnDisable() {
            ReleaseBuffers();
        }

        //void Update() {
            // 座標を更新して GPU へ転送
            
            //var t = Time.timeSinceLevelLoad;
            //for (var i = 0; i < instanceCount; ++i) {
            //    var x = Mathf.Sin((t + i) * 1.17f);
            //    var y = Mathf.Sin((t - i) * 1.0f);
            //    var z = Mathf.Cos((t + i) * 1.87f);
            //    pos[i] = origPos[i] + new Vector3(x, y, z);
            //}
            //bufferPos.SetData(pos);
            
        //}

        void OnPostRender() {
        //void Update() {
            // 最後に SetPass() したマテリアルで描画を行う
            material.SetPass(0);
            // インスタンシングにより描画
            // シェーダに頂点 ID とインスタンス ID がやってくるのでそれを利用して描画する
            Graphics.DrawProceduralNow(MeshTopology.LineStrip, vertexCount, instanceCount);
        }
        
        public void SetupBuffer(SCIP_Parameter parameter, int startIndex, int endIndex) {
            material.SetInt("_AFRT", parameter.AFRT);
            material.SetInt("_ARES", parameter.ARES);
            material.SetInt("_StartIndex", startIndex);
            material.SetInt("_EndIndex", endIndex);

            if (bufferDistances != null) {
                bufferDistances.Release();
                bufferDistances = null;
            }
            instanceCount = endIndex - startIndex + 1;
            bufferDistances = new ComputeBuffer(instanceCount, sizeof(int), ComputeBufferType.Default);
        }

        public void UpdateValue(List<int> distances) {
            
            bufferDistances.SetData(distances);
            material.SetBuffer("buf_Distances", bufferDistances);
        }

        public void SetAdjustData(AdjustData data) {
            material.SetFloat("_AngleZ", data.angleZ);
            Debug.Log(data.offsetX * 0.001f + " " + (data.displayHeight * 0.5f + data.offsetY) * 0.001f);
            //material.SetVector("_Center", new Vector3(0, 0.9f, 0));
            material.SetVector("_Center", new Vector3(data.offsetX * 0.001f, (data.displayHeight * 0.5f + data.offsetY) * 0.001f, 0));
        }
    }
}