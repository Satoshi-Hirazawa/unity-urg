using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SensorUtility;

using System;

namespace URG.Adjust {
    // public class Main : MonoBehaviour {
    //     [SerializeField] AdjustDataConfigurator adjustDataConfigurator;
    //     [SerializeField] URGHandler urgHandler;
    //     [SerializeField] Text positionText;
    //     [SerializeField, Range(0.01f, 1)] float filterVal = 0.05f;


    //     public Action<Vector2> OnDetect;
    //     public Action<Vector2, int> OnDetectionPosition;
    //     public Action OnDetectionEnd;

    //     bool isDetect = false;
    //     Vector2 oldPos;
    //     Camera mainCamera;

    //     void Start() {
    //         adjustDataConfigurator.Load();
    //         adjustDataConfigurator.DisplayValues();
    //         adjustDataConfigurator.OnApply += OnApplyListener;
    //         mainCamera = Camera.main;
    //         mainCamera.transform.position = new Vector3(0, 0, -adjustDataConfigurator.Data.distance * 0.001f);
    //         mainCamera.fieldOfView = 2.0f * Mathf.Atan(adjustDataConfigurator.Data.displayHeight * 0.5f / 
    //                                                     adjustDataConfigurator.Data.distance) * Mathf.Rad2Deg;

    //         urgHandler.SetIsDebugDraw(true);
    //         urgHandler.Init();
    //         urgHandler.SetAdjustData(adjustDataConfigurator.Data);
    //         urgHandler.OnDetect += OnDetectListener;
    //         urgHandler.OnDetectionPosition += OnDetectPosListener;
    //         urgHandler.OnDetectionEnd += OnDetectEndListener;
    //     }

    //     // Update is called once per frame
    //     void Update() {

    //     }

    //     void OnApplyListener() {
    //         urgHandler.SetAdjustData(adjustDataConfigurator.Data);
    //     }

    //     void OnDetectListener(Vector2 pos) {
    //         // isDetect = true;
    //     }

    //     void OnDetectEndListener() {
    //         isDetect = false;
    //     }

    //     void OnDetectPosListener(Vector2 pos, int num) {

    //         Vector2 filtering;
    //         filtering.x = SensorUtil.lowPass(oldPos.x, pos.x);
    //         filtering.y = SensorUtil.lowPass(oldPos.y, pos.y);

    //         // Vector2 screenPos = mainCamera.WorldToScreenPoint(filtering);
    //         Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);
    //         Debug.Log(screenPos.x + " : " + screenPos.y);

    //         // if(!isDetect){
    //         //     filtering.x = pos.x;
    //         //     filtering.y = pos.y;
    //         //     trailRenderer.enabled = false;
    //         // }
    //         // trailRenderer.enabled = true;
    //         // isDetect = true;

    //         positionText.rectTransform.position = screenPos;
    //         // trail.transform.position = filtering;
    //         // oldPos = filtering;
    //     }
    // }
}