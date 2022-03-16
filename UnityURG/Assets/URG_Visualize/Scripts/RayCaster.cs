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
        Vector2 oldPos;
        Camera mainCamera;

        public Action<RaycastHit> OnAlienHit;
        public Action OnStart;

        void Start() {
            adjustDataConfigurator.Load();
            adjustDataConfigurator.DisplayValues();
            adjustDataConfigurator.OnApply += OnApplyListener;
            mainCamera = Camera.main;
            urgCamera.transform.position = new Vector3(0, 0, -adjustDataConfigurator.Data.distance * 0.001f);
            baseFieldOfView = 2.0f * Mathf.Atan(adjustDataConfigurator.Data.displayHeight * 0.5f / 
                                                     adjustDataConfigurator.Data.distance) * Mathf.Rad2Deg;

            urgCamera.fieldOfView = HorizontalFOV.HorizontalFOVCalculater.SetFieldOfView(baseFieldOfView, baseAspect.x, baseAspect.y);

            urgHandler.SetIsDebugDraw(true);
            urgHandler.Init();
            urgHandler.SetAdjustData(adjustDataConfigurator.Data);
            urgHandler.OnDetect += OnDetectListener;
            urgHandler.OnDetectionPosition += OnDetectPosListener;
            urgHandler.OnDetectionEnd += OnDetectEndListener;
        }

        // Update is called once per frame
        void Update() 
        {
            // urgCamera.fieldOfView = HorizontalFOV.HorizontalFOVCalculater.SetFieldOfView(baseFieldOfView, baseAspect.x, baseAspect.y);
            // urgCamera.transform.position = Camera.main.transform.position;

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray,out hit))
                {
                    if(hit.collider.CompareTag("Test"))
                    {
                        Debug.Log(hit.collider.gameObject.name);
                    }                    
                }
            }
        }

        public void CameraInit()
        {
            
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

        void OnDetectPosListener(Vector2 pos, int num) {

            // Vector2 filtering;
            // filtering.x = SensorUtil.lowPass(oldPos.x, pos.x);
            // filtering.y = SensorUtil.lowPass(oldPos.y, pos.y);

            pos.x = pos.x * adjustDataConfigurator.Data.calibX;
            pos.y = pos.y * adjustDataConfigurator.Data.calibY;

            Vector2 screenPos = urgCamera.WorldToScreenPoint(pos);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            // Debug.DrawRay (ray.origin, ray.direction * 100, Color.red, 0.01f, false);

            if(Physics.Raycast(ray,out hit))
            {
                
                if(hit.collider.CompareTag("Test"))
                {
                    Debug.Log(hit.collider.gameObject.name);
                }
            }
        
            positionText.rectTransform.position = screenPos;
            // trail.transform.position = filtering;
            // oldPos = filtering;
        }
}
