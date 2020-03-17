using SCIP_library;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URG {
    public class URGHandler : MonoBehaviour {

        [SerializeField] URGTcpClient tcpClient;
        [SerializeField] AdjustDataConfigurator adjustDataConfigurator;
        [SerializeField] DebugDraw debugDraw;

        [SerializeField] string ip = "192.168.0.10";
        [SerializeField] int port = 10940;

        [SerializeField] float start_deg = -45;
        [SerializeField] float end_deg = 225;

        [SerializeField, Range(0, 1)] float sizeMinThreshold = 0.05f;
        [SerializeField, Range(0.1f, 1)] float sizeMaxThreshold = 0.05f;
        [SerializeField, Range(0, 1)] float centerThreshold = 0.05f;
        [SerializeField] int detectObjectQuantity = 1;

        int start_index;
        int end_index;
        long timestamp = 0;
        bool isUpdateSensor = false;
        bool isDebugDraw;
        Vector2 centerPos;

        List<int> distances = new List<int>();
        List<Vector2> positions = new List<Vector2>();
        List<DetectObject> detectObjectList = new List<DetectObject>();

        SCIP_Parameter parameter;
        AdjustData adjustData = new AdjustData();

        enum State {
            DEFAULT,
            OPEN,
            PARAMETER,
            SCAN,
            WORKING
        };
        State state;

        public Action<Vector2> OnDetect;
        public Action<Vector2, int> OnDetectionPosition;
        public Action OnDetectionEnd;

        public void Init() {
            detectObjectList = new List<DetectObject>();
            tcpClient.Open(ip, port);
            state = State.OPEN;
        }

        public void SetAdjustData(AdjustData data) {
            adjustData = data;
            centerPos = new Vector2(data.offsetX * 0.001f, (data.displayHeight * 0.5f + data.offsetY) * 0.001f);
            if (debugDraw != null) {
                debugDraw.SetAdjustData(data);
            }
        }

        public void SetIsDebugDraw(bool b) {
            isDebugDraw = b;
        }

        void Update() {
            switch (state) {
                case State.OPEN:
                    if (tcpClient.GetIsConnected()) {
                        tcpClient.OnReceiveCallback += OnReceiveParameter;
                        tcpClient.Send(SCIP_Writer.PP());
                    }
                    break;
                case State.SCAN:
                    start_index = DegToIndex(start_deg);
                    end_index = DegToIndex(end_deg);
                    tcpClient.OnReceiveCallback += OnReceiveData;
                    tcpClient.Send(SCIP_Writer.MS(start_index, end_index));
                    if (debugDraw != null) {
                        debugDraw.SetupBuffer(parameter, start_index, end_index);
                    }

                    state = State.WORKING;
                    break;
                case State.WORKING:
                    if (isUpdateSensor) {
                        lock (((ICollection)distances).SyncRoot) {
                            if (isDebugDraw) {
                                DebugDrawRay();
                            }
                            SearchObject();
                            isUpdateSensor = false;
                        }
                    }
                    break;
            }
        }

        void SearchObject() {

            bool isDetected = false;
            List<DetectObject> newDetectObjectList = new List<DetectObject>();
            
            for (int i = 0; i < distances.Count; i++) {
                float deg = adjustData.angleZ + IndexToDeg(start_index + i);
                Vector2 position = centerPos + new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad)) * distances[i] * 0.001f;
                Debug.DrawLine(centerPos, position, Color.white);

                if (0 < distances[i] && -(adjustData.displayWidth * 0.0005f) < position.x && position.x < adjustData.displayWidth * 0.0005f &&
                                        -(adjustData.displayHeight * 0.0005f) < position.y && position.y < adjustData.displayHeight * 0.0005f) {
                    if (!isDetected) {
                        DetectObject detectObject = new DetectObject();
                        detectObject.count = 1;
                        detectObject.indices.Add(i);
                        detectObject.positions.Add(position);
                        detectObject.degrees.Add(deg);
                        newDetectObjectList.Add(detectObject);

                        isDetected = true;
                    } 
                    else {
                        newDetectObjectList[newDetectObjectList.Count - 1].indices.Add(i);
                        newDetectObjectList[newDetectObjectList.Count - 1].positions.Add(position);
                        newDetectObjectList[newDetectObjectList.Count - 1].degrees.Add(deg);
                    }
                } 
                else {
                    isDetected = false;
                }
            }
            if(detectObjectList.Count == 0){
                OnDetectionEnd();
            }

            for (int i = 0; i < newDetectObjectList.Count; i++) {
                if(detectObjectQuantity <= i) break;

                newDetectObjectList[i].CalcCenter();
                newDetectObjectList[i].CalcSize();

                for (int j = 0; j < newDetectObjectList[i].positions.Count; j++) {
                    Debug.DrawRay(Vector3.zero, newDetectObjectList[i].positions[j], Color.magenta);
                }
                for (int j = 0; j < detectObjectList.Count; j++) {
                    // Debug.Log(Vector2.Distance(newDetectObjectList[i].center, detectObjectList[j].center));
                    if (Vector2.Distance(newDetectObjectList[i].center, detectObjectList[j].center) < centerThreshold) {
                        newDetectObjectList[i].count = detectObjectList[j].count + 1;

                        if (newDetectObjectList[i].size > sizeMinThreshold && newDetectObjectList[i].size < sizeMaxThreshold) {
                            if (OnDetect != null) {
                                OnDetectionPosition(newDetectObjectList[i].center, i);
                            }
                        }
                    }
                }

                // if (newDetectObjectList[i].count == 2) {
                //     // Debug.Log(newDetectObjectList[i].size);
                //     if (newDetectObjectList[i].size > sizeMinThreshold)  {
                //         if (OnDetect != null) {
                //             OnDetect(newDetectObjectList[i].center);
                //         }
                //     }
                // }
            }
            detectObjectList.Clear();
            detectObjectList = newDetectObjectList;
        }

        public void OnReceiveParameter(string responce) {

            parameter = new SCIP_Parameter();
            if (SCIP_Reader.PP(responce, ref parameter)) {
                Debug.Log("model: " + parameter.MODL +
                    " d min: " + parameter.DMIN +
                    " d max: " + parameter.DMAX +
                    " ares: " + parameter.ARES +
                    " a min: " + parameter.AMIN +
                    " a max: " + parameter.AMAX +
                    " afrt: " + parameter.AFRT +
                    " scan: " + parameter.SCAN
                    );
                state = State.SCAN;
            } else {
                Debug.Log("ReceiveParameter error");
            }
            tcpClient.OnReceiveCallback -= OnReceiveParameter;
        }

        public void OnReceiveData(string responce) {
            lock (((ICollection)distances).SyncRoot) {
                if (SCIP_Reader.MS(responce, ref timestamp, ref distances)) {
                    isUpdateSensor = true;
                }
            }
        }

        float IndexToDeg(int index) {
            return (index - parameter.AFRT) * 360f / (float)parameter.ARES + 90f;
        }

        int DegToIndex(float deg) {
            return (int)((deg - 90f) * (float)parameter.ARES / 360f + parameter.AFRT);
        }

        void DebugDrawRay() {

            if (debugDraw != null) {
                debugDraw.UpdateValue(distances);
            }
        }

        public void StopScan() {
            if (state == State.SCAN) {
                tcpClient.Send(SCIP_Writer.QT());
            }
        }

        void OnDisable() {
            if (state == State.SCAN) {
                tcpClient.Send(SCIP_Writer.QT());
            }
        }
    }

    class DetectObject {
        public Vector2 center;
        public float size;
        public List<int> indices;
        public List<Vector2> positions;
        public List<float> degrees;

        public int count;

        public DetectObject() {
            indices = new List<int>();
            positions = new List<Vector2>();
            degrees = new List<float>();
        }

        public void CalcCenter() {
            int centerIndex = Mathf.FloorToInt(positions.Count / 2);
            center = positions[centerIndex] + new Vector2(Mathf.Cos(degrees[centerIndex] * Mathf.Deg2Rad), Mathf.Sin(degrees[centerIndex] * Mathf.Deg2Rad)) * 30f * 0.001f;
        }

        public void CalcSize() {
            size = Vector2.Distance(positions[0], positions[positions.Count - 1]);
        }
    }
}