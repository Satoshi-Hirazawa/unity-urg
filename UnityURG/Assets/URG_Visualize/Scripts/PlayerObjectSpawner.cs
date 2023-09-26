using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SensorUtility;
using System;

using HorizontalFOV;
using URG;

public class PlayerObjectSpawner : MonoBehaviour
{
    [SerializeField] AdjustDataConfigurator adjustDataConfigurator;
    [SerializeField] URGHandler urgHandler;
    [SerializeField] GameObject debugSphere;
    [SerializeField] Text positionText;
    [SerializeField] string adjustFileName;

    [SerializeField] Camera urgCamera;

    [SerializeField] float createZPos = 10f;

    [SerializeField] Vector2 baseAspect = new Vector2(1920, 1080);
    [SerializeField] float baseFieldOfView = 60f;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<GameObject> objInstance = new List<GameObject>();
    
    int prePositionlength;
    bool isDetect = false;

    public Action<RaycastHit> OnAlienHit;
    public Action OnTurtleTrailHit;
    public Action OnGlawTrailHit;
    public Action OnStart;
    public Action OnConnectionURG;


    void Start() {
        adjustDataConfigurator.SetJsonName("/" + adjustFileName + ".json");
        adjustDataConfigurator.IpAddress(urgHandler.GetIpAddress());
        adjustDataConfigurator.Load();
        adjustDataConfigurator.DisplayValues();
        adjustDataConfigurator.OnApply += OnApplyListener;

        positionText.color = urgHandler.GetDebugColor();
        Transform header = adjustDataConfigurator.transform.parent.GetChild(1);
        if(header.name == "HeaderColor")
        {
            header.GetComponent<Image>().color = urgHandler.GetDebugColor();
        }
        
        baseFieldOfView = 2.0f * Mathf.Atan(adjustDataConfigurator.Data.displayHeight * 0.5f / (Mathf.Abs(urgCamera.transform.position.z) * 1000f) ) * Mathf.Rad2Deg;
        urgCamera.fieldOfView = HorizontalFOV.HorizontalFOVCalculater.SetFieldOfView(baseFieldOfView, baseAspect.x, baseAspect.y);
        
        urgHandler.Init();
        urgHandler.SetAdjustData(adjustDataConfigurator.Data);
        urgHandler.OnConnectionURG += ConnectionURG;
        urgHandler.OnDetect += OnDetectListener;
        urgHandler.OnDetectionEnd += OnDetectEndListener;
        urgHandler.OnDetectionPositionArry += OnDetectPosArrayListener;
        urgHandler.SetDebugDraw(true);

//         if(SceneLoader.GetSceneName() != "Adjust")
//         {        
//             adjustDataConfigurator.transform.parent.gameObject.SetActive(false);
//             urgHandler.SetDebugDraw(false);            
//             positionText.transform.gameObject.SetActive(false);
//         }

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
    }

    void ConnectionURG()
    {
        if(OnConnectionURG != null)
        {
            OnConnectionURG();
        }
    }
    void OnApplyListener() 
    {
        urgHandler.SetAdjustData(adjustDataConfigurator.Data);
    }

    void OnDetectListener(Vector2 pos) {

    }

    void OnDetectEndListener() {
        isDetect = false;
        for(int i = 0; i < objInstance.Count; i++)
        {
            Destroy(objInstance[i]);
        }
        objInstance.Clear();
        prePositionlength = 0;

    }

    void OnDetectPosArrayListener(Vector2[] pos) 
    {
        if(objInstance.Count != 0 && pos.Length < objInstance.Count)
        {
            for(int i = pos.Length; i < objInstance.Count; i++)
            {
                Destroy(objInstance[i]);
            }
            int cnt = objInstance.Count - pos.Length;
            objInstance.RemoveRange(pos.Length, cnt);
        }
        for(int i = prePositionlength; i < pos.Length; i++)
        {
            if(playerPrefab != null)
            {    
                objInstance.Add(null);
            }
        }
        for(int i = 0; i < pos.Length; i++)
        {

            pos[i].x = (pos[i].x * adjustDataConfigurator.Data.calibX) + adjustDataConfigurator.Data.worldPointOffsetX;
            pos[i].y = (pos[i].y * adjustDataConfigurator.Data.calibY) + adjustDataConfigurator.Data.worldPointOffsetY;
            
            Vector2 screenPos = urgCamera.WorldToScreenPoint(pos[i]);
            pos[i] = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, createZPos));

            if(objInstance[i] == null)
            {
                objInstance[i] = (Instantiate(playerPrefab, Vector3.zero, Quaternion.identity));
                objInstance[i].name = objInstance[i].name + "_" + i.ToString().PadLeft(2, '0');
            }

            objInstance[i].transform.position = new Vector3(pos[i].x, pos[i].y, createZPos);    
            positionText.rectTransform.position = screenPos;
        }
        prePositionlength = pos.Length;
    }

}