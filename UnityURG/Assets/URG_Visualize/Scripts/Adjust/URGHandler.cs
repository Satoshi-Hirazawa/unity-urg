using SCIP_library;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace URG {
    public class URGHandler : MonoBehaviour {

        [SerializeField] URGTcpClient tcpClient;
        // [SerializeField] AdjustDataConfigurator adjustDataConfigurator;
        [SerializeField] DebugDraw debugDraw;

        [SerializeField] string ip = "192.168.0.10";
        [SerializeField] int port = 10940;

        [SerializeField] float start_deg = -45;
        [SerializeField] float end_deg = 225;

        [SerializeField] Color debugColor = Color.white;

        float debugDrawOffsetZ;
        int start_index;
        int end_index;
        long timestamp = 0;
        bool isUpdateSensor = false;
        bool isDebugDraw;

        Vector2 centerPos;

    	bool isThreadRunning = false;
    	Thread thread;


        // List<int> distances = new List<int>();
        List<long> distances = new List<long>();
        List<Vector2> positions = new List<Vector2>();
        List<DetectObject> detectObjectList = new List<DetectObject>();
        List<Vector2> detectionPositionArry = new List<Vector2>();

        SCIP_Parameter parameter;
        AdjustData adjustData = new AdjustData();
        LineRenderer linerend;

        enum State 
        {
            DEFAULT,
            OPEN,
            PARAMETER,
            SCAN,
            WORKING
        };
        State state;

        public Action<Vector2> OnDetect;
        public Action<Vector2> OnDetectionPosition;
        public Action<Vector2[]> OnDetectionPositionArry;
        public Action OnDetectionEnd;

        public Action OnConnectionURG;

        public void Init() 
        {
            detectObjectList = new List<DetectObject>();
            tcpClient.Open(ip, port);
            state = State.OPEN;

            // thread = new Thread(ReceivetTread);
            // thread.IsBackground = true;
            // isThreadRunning = true;
            // thread.Start();

            linerend = gameObject.AddComponent<LineRenderer>();
            linerend.material = new Material(Shader.Find("Unlit/Color"));
            linerend.material.color = debugColor;
        }

        public void SetAdjustData(AdjustData data) 
        {
            adjustData = data;
            centerPos = new Vector2(data.sensorOffsetX * 0.001f, (data.displayHeight * 0.5f + data.sensorOffsetY) * 0.001f);
            if (debugDraw != null) {
                debugDraw.SetAdjustData(data);
            }
        }

        public void SetDebugDraw(bool b) 
        {
            isDebugDraw = b;
            if (debugDraw != null) {
                debugDraw.IsDebugDraw = b;
            }
        }
        public bool GetDebugDraw() 
        {
            return isDebugDraw;
        }

        public string GetIpAddress()
        {
            return ip;
        }

        void ReceivetTread()
        {
        }

        void Update() 
        { 

            debugDrawOffsetZ = adjustData.sensorPosZ / 10f;

            // 0.0005f = 1 / 2 / 1000
            Vector3 p1 = new Vector3((-adjustData.displayWidth + adjustData.debugDrawOffsetX) *  0.0005f, ( adjustData.displayHeight + adjustData.debugDrawOffsetY) * 0.0005f, debugDrawOffsetZ);
            Vector3 p2 = new Vector3(( adjustData.displayWidth + adjustData.debugDrawOffsetX) *  0.0005f, ( adjustData.displayHeight + adjustData.debugDrawOffsetY) * 0.0005f, debugDrawOffsetZ);
            Vector3 p3 = new Vector3(( adjustData.displayWidth + adjustData.debugDrawOffsetX) *  0.0005f, (-adjustData.displayHeight + adjustData.debugDrawOffsetY) * 0.0005f, debugDrawOffsetZ);
            Vector3 p4 = new Vector3((-adjustData.displayWidth + adjustData.debugDrawOffsetX) *  0.0005f, (-adjustData.displayHeight + adjustData.debugDrawOffsetY) * 0.0005f, debugDrawOffsetZ);

            if(isDebugDraw)
            {
                Vector3[] positions = new Vector3[]
                {
                    p1, p2, p3, p4, p1
                };

                // center top
                Vector3 p = new Vector3((adjustData.debugDrawOffsetX) * 0.0005f, adjustData.displayHeight * 0.0005f, debugDrawOffsetZ);
                
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Right && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Top) 
                {
                    p = new Vector3((adjustData.debugDrawOffsetX + adjustData.displayWidth) * 0.0005f, adjustData.displayHeight * 0.0005f, debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Left && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Top) 
                {
                    p = new Vector3((adjustData.debugDrawOffsetX - adjustData.displayWidth) * 0.0005f, adjustData.displayHeight * 0.0005f, debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Center && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Bottom) 
                {
                    p = new Vector3((adjustData.debugDrawOffsetX) * 0.0005f, -adjustData.displayHeight * 0.0005f, debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Right && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Bottom) 
                {
                    p = new Vector3((adjustData.debugDrawOffsetX + adjustData.displayWidth) * 0.0005f, -adjustData.displayHeight * 0.0005f, debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Left && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Bottom) 
                {
                     p = new Vector3((adjustData.debugDrawOffsetX - adjustData.displayWidth) * 0.0005f, -adjustData.displayHeight * 0.0005f, debugDrawOffsetZ);
                }

                transform.GetComponentsOnlyChildren<Renderer>()[0].enabled = true;
                transform.GetComponentsOnlyChildren<Transform>()[0].position = p;
                transform.GetComponentsOnlyChildren<Renderer>()[0].material.color = debugColor;

                linerend.startWidth = 0.05f * adjustData.sensorPosZ / 150;
                linerend.endWidth   = 0.05f * adjustData.sensorPosZ / 150;
                linerend.widthMultiplier = 0.05f * adjustData.sensorPosZ / 150;
                linerend.positionCount = positions.Length;
                linerend.SetPositions(positions);
            }
            else
            {
                Vector3[] positions = new Vector3[]
                {
                };
                transform.GetComponentsOnlyChildren<Renderer>()[0].enabled = false;
                linerend.positionCount = positions.Length;
                linerend.SetPositions(positions);
            }

            switch (state) 
            {
                case State.OPEN:
                    if (tcpClient.GetIsConnected()) 
                    {
                        tcpClient.OnReceiveCallback += OnReceiveParameter;
                        tcpClient.Send(SCIP_Writer.PP());
                        if(OnConnectionURG != null)
                        {
                            OnConnectionURG();
                        } 
                    }
                    break;
                case State.SCAN:
                    start_index = DegToIndex(start_deg);
                    end_index = DegToIndex(end_deg);
                    tcpClient.OnReceiveCallback += OnReceiveData;
                    // tcpClient.Send(SCIP_Writer.MS(start_index, end_index));
                    tcpClient.Send(SCIP_Writer.MD(start_index, end_index));
                    if (debugDraw != null) 
                    {
                        debugDraw.SetColor(debugColor);
                        debugDraw.SetupBuffer(parameter, start_index, end_index);

                    }

                    state = State.WORKING;
                    break;
                case State.WORKING:
                    if (isUpdateSensor) 
                    {
                        lock (((ICollection)distances).SyncRoot) 
                        {
                            if (isDebugDraw) 
                            {
                                DebugDrawRay();
                            }
                            SearchObject();
                            isUpdateSensor = false;
                        }
                    }
                break;
            }
        }

        void SearchObject() 
        {
            bool isDetected = false;
            List<DetectObject> newDetectObjectList = new List<DetectObject>();

            
            
            for (int i = 0; i < distances.Count; i++) 
            {
                float deg = adjustData.angleZ + IndexToDeg(start_index + i);
                
                Vector2 position = centerPos + new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad)) * distances[i] * 0.001f;
                Vector3 startPos = new Vector3(centerPos.x + (adjustData.debugDrawOffsetX * 0.0005f), centerPos.y + (adjustData.debugDrawOffsetY * 0.0005f), debugDrawOffsetZ);
                Vector3 endPos   = new Vector3(position.x + (adjustData.debugDrawOffsetX * 0.0005f), position.y + (adjustData.debugDrawOffsetY * 0.0005f), debugDrawOffsetZ);

                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Right && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Top) 
                {
                    startPos = new Vector3(centerPos.x + ((adjustData.debugDrawOffsetX + adjustData.displayWidth) * 0.0005f), centerPos.y + (adjustData.debugDrawOffsetY * 0.0005f), debugDrawOffsetZ);
                    endPos   = new Vector3(position.x + ((adjustData.debugDrawOffsetX + adjustData.displayWidth) * 0.0005f), position.y + (adjustData.debugDrawOffsetY * 0.0005f), debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Left && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Top) 
                {
                    startPos = new Vector3(centerPos.x + ((adjustData.debugDrawOffsetX - adjustData.displayWidth) * 0.0005f), centerPos.y + (adjustData.debugDrawOffsetY * 0.0005f), debugDrawOffsetZ);
                    endPos   = new Vector3(position.x + ((adjustData.debugDrawOffsetX - adjustData.displayWidth) * 0.0005f), position.y + (adjustData.debugDrawOffsetY * 0.0005f), debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Center && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Bottom) 
                {
                    startPos = new Vector3(centerPos.x + (adjustData.debugDrawOffsetX * 0.0005f), centerPos.y + ((adjustData.debugDrawOffsetY - (adjustData.displayHeight * 2) - adjustData.sensorOffsetY * 2) * 0.0005f), debugDrawOffsetZ);
                    endPos   = new Vector3(position.x + (adjustData.debugDrawOffsetX * 0.0005f), position.y + ((adjustData.debugDrawOffsetY - (adjustData.displayHeight * 2)  - adjustData.sensorOffsetY * 2) * 0.0005f), debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Right && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Bottom) 
                {
                    startPos = new Vector3(centerPos.x + ((adjustData.debugDrawOffsetX + adjustData.displayWidth) * 0.0005f), centerPos.y + ((adjustData.debugDrawOffsetY - (adjustData.displayHeight * 2) - adjustData.sensorOffsetY * 2) * 0.0005f), debugDrawOffsetZ);
                    endPos   = new Vector3(position.x + ((adjustData.debugDrawOffsetX + adjustData.displayWidth) * 0.0005f), position.y + ((adjustData.debugDrawOffsetY - (adjustData.displayHeight * 2) - adjustData.sensorOffsetY * 2) * 0.0005f), debugDrawOffsetZ);
                }
                if(adjustData.horizontal == (int)AdjustDataConfigurator.Horizontal.Left && adjustData.vertical == (int)AdjustDataConfigurator.Vertical.Bottom) 
                {
                    startPos = new Vector3(centerPos.x + ((adjustData.debugDrawOffsetX - adjustData.displayWidth) * 0.0005f), centerPos.y + ((adjustData.debugDrawOffsetY - (adjustData.displayHeight * 2) - adjustData.sensorOffsetY * 2) * 0.0005f), debugDrawOffsetZ);
                    endPos   = new Vector3(position.x + ((adjustData.debugDrawOffsetX - adjustData.displayWidth) * 0.0005f), position.y + ((adjustData.debugDrawOffsetY - (adjustData.displayHeight * 2) - adjustData.sensorOffsetY * 2) * 0.0005f), debugDrawOffsetZ);
                }
                Debug.DrawLine(startPos, endPos, new Color(debugColor.r * 0.5f, debugColor.g * 0.5f, debugColor.b * 0.5f));

                if (0 < distances[i] && -((adjustData.displayWidth) * 0.0005f) < position.x && position.x < ((adjustData.displayWidth) * 0.0005f) &&
                                        -((adjustData.displayHeight) * 0.0005f) < position.y && position.y < (adjustData.displayHeight) * 0.0005f) 
                {
                    if (!isDetected) 
                    {
                        DetectObject detectObject = new DetectObject();
                        detectObject.count = i;
                        detectObject.indices.Add(i);
                        detectObject.positions.Add(position);
                        detectObject.degrees.Add(deg);
                        newDetectObjectList.Add(detectObject);
                        isDetected = true;
                    } 
                    else 
                    {
                        newDetectObjectList[newDetectObjectList.Count - 1].indices.Add(i);
                        newDetectObjectList[newDetectObjectList.Count - 1].positions.Add(position);
                        newDetectObjectList[newDetectObjectList.Count - 1].degrees.Add(deg);
                    }
                } 
                else 
                {
                    isDetected = false;
                }
            }
            if(detectObjectList.Count == 0)
            {
                OnDetectionEnd();
            }

            for (int i = 0; i < newDetectObjectList.Count; i++) 
            {
                // if(detectObjectQuantity <= i) break;
                newDetectObjectList[i].sensorPos = centerPos;
                newDetectObjectList[i].CalcCenter();
                newDetectObjectList[i].CalcSize();
                newDetectObjectList[i].CalcDistance();
                newDetectObjectList[i].CalcCountPerDistance();

                for (int j = 0; j < detectObjectList.Count; j++) 
                {
                    if (Vector2.Distance(newDetectObjectList[i].center, detectObjectList[j].center) < adjustData.center) 
                    {
                        newDetectObjectList[i].count = detectObjectList[j].count + 1;
                    }
                }

                if (newDetectObjectList[i].countPerDistance > adjustData.min && newDetectObjectList[i].countPerDistance < adjustData.max)   
                {
                    Debug.Log("SensorPos : " + centerPos + ", count: " + detectionPositionArry.Count + " , num:" + i + " , size:" + newDetectObjectList[i].countPerDistance + " , MIN:" + adjustData.min + " , MAX:" + adjustData.max);
                    detectionPositionArry.Add(newDetectObjectList[i].center);
                }
            }
            if (OnDetectionPositionArry != null && detectionPositionArry.Count > 0) 
            {                        
                Vector2[] detectArray = detectionPositionArry.ToArray();
                OnDetectionPositionArry(detectArray);
            }
            detectObjectList.Clear();
            detectionPositionArry.Clear();
            detectObjectList = newDetectObjectList;
        }

        public void OnReceiveParameter(string responce) 
        {

            parameter = new SCIP_Parameter();
            if (SCIP_Reader.PP(responce, ref parameter)) 
            {
                Debug.Log(
                    "model: " + parameter.MODL + "\n" +
                    " d min: " + parameter.DMIN + "\n" +
                    " d max: " + parameter.DMAX + "\n" +
                    " ares: " + parameter.ARES + "\n" +
                    " a min: " + parameter.AMIN + "\n" +
                    " a max: " + parameter.AMAX + "\n" +
                    " afrt: " + parameter.AFRT + "\n" +
                    " scan: " + parameter.SCAN
                    );
            } else 
            {
                Debug.Log("ReceiveParameter error");
                return;
            }
            state = State.SCAN;
            tcpClient.OnReceiveCallback -= OnReceiveParameter;
        }

        public void OnReceiveData(string responce) 
        {
            lock (((ICollection)distances).SyncRoot) 
            {
                // if (SCIP_Reader.MS(responce, ref timestamp, ref distances)) {
                //     isUpdateSensor = true;
                // }
                if (SCIP_Reader.MD(responce, ref timestamp, ref distances)) 
                {
                    isUpdateSensor = true;
                }
            }
        }

        public Color GetDebugColor()
        {
            return debugColor;
        }

        float IndexToDeg(int index) 
        {
            return (((index - parameter.AFRT) * 360f) / (float)parameter.ARES) + 90f;
        }

        int DegToIndex(float deg) 
        {
            return (int)((deg - 90f) * (float)parameter.ARES / 360f + parameter.AFRT);
        }

        void DebugDrawRay() 
        {
            if (debugDraw != null) 
            {
                debugDraw.UpdateValue(distances);
            }
        }

        public void StopScan() 
        {
            if (state == State.SCAN) 
            {
                tcpClient.Send(SCIP_Writer.QT());
            }
        }

        void OnDisable() 
        {
            if (state == State.SCAN) 
            {
                tcpClient.Send(SCIP_Writer.QT());
            }
        }
    }

    class DetectObject 
    {
    
        public Vector2 sensorPos;
        public Vector2 center;
        public float size;
        public List<int> indices;
        public List<Vector2> positions;
        public List<float> degrees;
        public float distance;
        public float countPerDistance;
        
        public int count;

        public DetectObject() 
        {
            indices = new List<int>();
            positions = new List<Vector2>();
            degrees = new List<float>();
        }

        public void CalcCenter() 
        {
            int centerIndex = Mathf.FloorToInt(positions.Count / 2);
            center = positions[centerIndex] + new Vector2(Mathf.Cos(degrees[centerIndex] * Mathf.Deg2Rad), Mathf.Sin(degrees[centerIndex] * Mathf.Deg2Rad)) * 30f * 0.001f;
        }

        public void CalcDistance()
        {
            distance = Vector2.Distance(sensorPos, center);
        }

        public void CalcSize() 
        {
            size = Vector2.Distance(positions[0], positions[positions.Count - 1]);
        }
        public void CalcCountPerDistance()
        {
            countPerDistance = positions.Count * distance; 
        }
    }
}