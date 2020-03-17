using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SensorUtility;

namespace URG.Adjust {
    public class Main : MonoBehaviour {
        [SerializeField] AdjustDataConfigurator adjustDataConfigurator;
        [SerializeField] URGHandler urgHandler;
        [SerializeField] Text positionText;

        [SerializeField] GameObject trail;
        [SerializeField] TrailRenderer trailRenderer;
        [SerializeField, Range(0.01f, 1)] float filterVal = 0.05f;


        bool isDetect = false;
        Vector2 oldPos;
        Camera mainCamera;

        void Start() {
            adjustDataConfigurator.Load();
            adjustDataConfigurator.DisplayValues();
            adjustDataConfigurator.OnApply += OnApplyListener;
            mainCamera = Camera.main;
            mainCamera.transform.position = new Vector3(0, 0, -adjustDataConfigurator.Data.distance * 0.001f);
            mainCamera.fieldOfView = 2.0f * Mathf.Atan(adjustDataConfigurator.Data.displayHeight * 0.5f / 
                                                        adjustDataConfigurator.Data.distance) * Mathf.Rad2Deg;

            urgHandler.SetIsDebugDraw(true);
            urgHandler.Init();
            urgHandler.SetAdjustData(adjustDataConfigurator.Data);
            urgHandler.OnDetectionPosition += OnDetectPosListener;
            urgHandler.OnDetectionEnd += OnDetectEndListener;
        }

        // Update is called once per frame
        void Update() {

        }

        void OnApplyListener() {
            urgHandler.SetAdjustData(adjustDataConfigurator.Data);
        }

        void OnDetectEndListener() {
            isDetect = false;
        }

        void OnDetectPosListener(Vector2 pos, int num) {

            Vector2 filtering;
            filtering.x = SensorUtil.lowPass(oldPos.x, pos.x);
            filtering.y = SensorUtil.lowPass(oldPos.y, pos.y);

            Vector2 screenPos = mainCamera.WorldToScreenPoint(filtering);
            // Debug.Log(filtering.x + " : " + filtering.y);

            if(!isDetect){
                filtering.x = pos.x;
                filtering.y = pos.y;
                trailRenderer.enabled = false;
            }
            trailRenderer.enabled = true;
            isDetect = true;

            trail.transform.position = filtering;
            positionText.rectTransform.position = screenPos;
            oldPos = filtering;
        }
    }
}