using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SensorUtility;
using System;

using HorizontalFOV;
using URG;

public class RayCaster : MonoBehaviour
{
        [SerializeField] AdjustDataConfigurator adjustDataConfigurator;
        [SerializeField] URGHandler urgHandler;
        [SerializeField] Text positionText;
 
        [SerializeField] Camera urgCamera;

        [SerializeField] Vector2 baseAspect = new Vector2(1920, 1080);
        [SerializeField] float baseFieldOfView = 60f;

        [SerializeField, Range(0.01f, 1)] float filterVal = 0.05f;

        [SerializeField] float offsetX;
        [SerializeField] float offsetY;

        bool isDetect = false;
        Vector2 detectPos;
        Camera mainCamera;

        public Action<RaycastHit> OnAlienHit;
        public Action OnTurtleTrailHit;
        public Action OnGlawTrailHit;
        public Action OnStart;
        public Action OnConnectionURG;


        void Start() {
            adjustDataConfigurator.Load();
            adjustDataConfigurator.DisplayValues();
            adjustDataConfigurator.OnApply += OnApplyListener;

            baseFieldOfView = 2.0f * Mathf.Atan(adjustDataConfigurator.Data.displayHeight * 0.5f / (Mathf.Abs(urgCamera.transform.position.z) * 1000f) ) * Mathf.Rad2Deg;
            urgCamera.fieldOfView = HorizontalFOV.HorizontalFOVCalculater.SetFieldOfView(baseFieldOfView, baseAspect.x, baseAspect.y);

            urgHandler.Init();
            urgHandler.SetAdjustData(adjustDataConfigurator.Data);
            urgHandler.OnConnectionURG += ConnectionURG;
            urgHandler.OnDetect += OnDetectListener;
            urgHandler.OnDetectionPosition += OnDetectPosListener;
            urgHandler.OnDetectionPositionArry += OnDetectPosArrayListener;
            urgHandler.OnDetectionEnd += OnDetectEndListener;

            urgHandler.SetDebugDraw(true);

//             if(SceneLoader.GetSceneName() != "Adjust")
//             {   
//                 adjustDataConfigurator.transform.parent.gameObject.SetActive(false);
//                 urgHandler.SetDebugDraw(false);            
//                 positionText.transform.gameObject.SetActive(false);
//             }

// #if UNITY_EDITOR

// #else
//         Cursor.visible = false;
//         if(SceneLoader.GetSceneName() == "Adjust")
//         {
//             Cursor.visible = true;    
//         }
// #endif

        }

        // Update is called once per frame
        void Update() 
        {
            baseFieldOfView = 2.0f * Mathf.Atan(adjustDataConfigurator.Data.displayHeight * 0.5f / (Mathf.Abs(urgCamera.transform.position.z) * 1000f) ) * Mathf.Rad2Deg;
            urgCamera.fieldOfView = HorizontalFOV.HorizontalFOVCalculater.SetFieldOfView(baseFieldOfView, baseAspect.x, baseAspect.y);

            if(Input.GetKey(KeyCode.LeftShift))
            {
                if(Input.GetKeyUp(KeyCode.X))
                {
                    adjustDataConfigurator.transform.parent.gameObject.SetActive(!adjustDataConfigurator.transform.parent.gameObject.activeSelf);
                    positionText.transform.gameObject.SetActive(adjustDataConfigurator.transform.parent.gameObject.activeSelf);    
                    Cursor.visible = adjustDataConfigurator.transform.parent.gameObject.activeSelf;                
                }
                if(Input.GetKeyDown(KeyCode.D))
                {

                    urgHandler.SetDebugDraw(!urgHandler.GetDebugDraw());
                }
            } 

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray,out hit))
                {
                    if(hit.collider.CompareTag("Test"))
                    {
                        hit.collider.transform.gameObject.GetComponent<CubeTest>().ChangeColor();
                        Debug.Log(hit.collider.gameObject.name);
                    } 
                }
            }
        }

        public void CameraInit()
        {

        }
        void ConnectionURG()
        {
            if(OnConnectionURG != null)
            {
                OnConnectionURG();
            }
        }
        void OnApplyListener() {
            urgHandler.SetAdjustData(adjustDataConfigurator.Data);
        }

        void OnDetectListener(Vector2 pos) {
            // isDetect = true;
        }

        void OnDetectEndListener() {
            isDetect = false;
        }

        void OnDetectPosArrayListener(Vector2[] pos) 
        {
            for(int i = 0; i < pos.Length; i++)
            {

                pos[i].x = (pos[i].x * adjustDataConfigurator.Data.calibX) + adjustDataConfigurator.Data.worldPointOffsetX;
                pos[i].y = (pos[i].y * adjustDataConfigurator.Data.calibY) + adjustDataConfigurator.Data.worldPointOffsetY;
            
                Vector2 screenPos = urgCamera.WorldToScreenPoint(pos[i]);
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                if(Physics.Raycast(ray,out hit))
                {
                    
                    if(hit.collider.CompareTag("Test"))
                    {
                        hit.collider.transform.gameObject.GetComponent<CubeTest>().ChangeColor();
                        Debug.Log(hit.collider.gameObject.name);
                    }
                }
                positionText.rectTransform.position = screenPos;
            }

        }
        void OnDetectPosListener(Vector2 pos) {
        }
}
