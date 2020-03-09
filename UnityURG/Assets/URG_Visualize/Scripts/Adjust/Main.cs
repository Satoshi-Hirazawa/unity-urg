﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AmebaStrike.Adjust {
    public class Main : MonoBehaviour {
        [SerializeField]
        AdjustDataConfigurator adjustDataConfigurator;
        [SerializeField]
        URGHandler urgHandler;
        [SerializeField]
        Text positionText;

        [SerializeField]
        GameObject trail;
        [SerializeField]
        TrailRenderer trailRenderer;

        Camera mainCamera;

        bool isDetect = false;

        [SerializeField, Range(0.01f, 1)] float filterVal = 0.05f;

        Vector2 oldPos;

        void Start() {
            adjustDataConfigurator.Load();
            adjustDataConfigurator.DisplayValues();
            adjustDataConfigurator.OnApply += OnApplyListener;
            mainCamera = Camera.main;
            mainCamera.transform.position = new Vector3(0, 0, -adjustDataConfigurator.Data.distance * 0.001f);
            mainCamera.fieldOfView = 2.0f * Mathf.Atan(
                adjustDataConfigurator.Data.displayHeight * 0.5f / adjustDataConfigurator.Data.distance) * Mathf.Rad2Deg;

            urgHandler.SetIsDebugDraw(true);
            urgHandler.Init();
            urgHandler.SetAdjustData(adjustDataConfigurator.Data);
            urgHandler.OnDetect += OnDetectListener;
            urgHandler.OnNotDetect += OnNotDetectListener;
        }

        // Update is called once per frame
        void Update() {

        }
        void OnApplyListener() {
            urgHandler.SetAdjustData(adjustDataConfigurator.Data);
        }
        void OnNotDetectListener() {
            isDetect = false;
        }

        void OnDetectListener(Vector2 pos) {

            Vector2 filtering;
            filtering.x = lowPass(oldPos.x, pos.x);
            filtering.y = lowPass(oldPos.y, pos.y);

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

        float lowPass(float _oldData, float _rawData){
            float filterVal = 0.1f;
            float _filtering = (_oldData * (1 - filterVal)) + (((float)_rawData)  *  filterVal);
            return _filtering;
        }
    }
}