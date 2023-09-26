using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace URG {
    public class AdjustDataConfigurator : MonoBehaviour {

        [SerializeField] Text ipAddress;

        [SerializeField] InputField displayWidthInput;
        [SerializeField] InputField displayHeightInput;
        [SerializeField] InputField sensorOffsetXInput;
        [SerializeField] InputField sensorOffsetYInput;

        [SerializeField] InputField worldPointOffsetXInput;
        [SerializeField] InputField worldPointOffsetYInput;

        [SerializeField] InputField debugDrawOffsetXInput;
        [SerializeField] InputField debugDrawOffsetYInput;
        [SerializeField] InputField angleZInput;
        [SerializeField] Slider calibX;
        [SerializeField] Slider calibY;
        [SerializeField] Text calibXValue;
        [SerializeField] Text calibYValue;

        [SerializeField] Slider sizeMinThreshold;
        [SerializeField] Slider sizeMaxThreshold;
        [SerializeField] Text sizeMinThresholdValue;
        [SerializeField] Text sizeMaxThresholdValue;

        [SerializeField] Slider centerThreshold;
        [SerializeField] Text centerThresholdValue;
        [SerializeField] Slider sensorPostionZ;
        [SerializeField] Text sensorPostionZValue;

        [SerializeField] ToggleGroup horizontalToggleGroup;
        [SerializeField] Toggle[] horizontalToggl;
        [SerializeField] ToggleGroup verticalToggleGroup;
        [SerializeField] Toggle[] verticalToggle;

        [SerializeField] Button save;
        [SerializeField] Button apply;
        public AdjustData Data;
        public Action OnApply;

        string fileName = "/adjust.json";

        public enum Horizontal
        {
            Left,
            Center,
            Right
        }
        public enum Vertical
        {
            Top,
            Bottom
        }

        public void SetJsonName(string name) {
            fileName = name;
        }

        public void Load() {
            string filePath = Application.streamingAssetsPath + fileName;

            if (!File.Exists(filePath)) {
                Debug.Log("create new adjust data");
                Data = new AdjustData();
            } 
            else {
                Debug.Log("load adjust data");
                string stringData = File.ReadAllText(filePath);

                try {
                    Data = JsonUtility.FromJson<AdjustData>(stringData);
                }
                catch (ArgumentException) {
                    Debug.Log("ArgumentException... create new adjust data");
                    Data = new AdjustData();
                }

                if (Data == null) {
                    Debug.Log("Data null... create new adjust data");
                    Data = new AdjustData();
                } 
            }

            displayWidthInput.onValueChanged.AddListener(ChangeDisplayWidth);
            displayHeightInput.onValueChanged.AddListener(ChangeDisplayHeight);
            sensorOffsetXInput.onValueChanged.AddListener(ChangeSensorOffsetX);
            sensorOffsetYInput.onValueChanged.AddListener(ChangeSensorOffsetY);

            worldPointOffsetXInput.onValueChanged.AddListener(ChangeWorldPointOffsetX);
            worldPointOffsetYInput.onValueChanged.AddListener(ChangeWorldPointOffsetY);


            angleZInput.onValueChanged.AddListener(ChangeAngleZ);
            debugDrawOffsetXInput.onValueChanged.AddListener(ChangeDebugOffsetX);
            debugDrawOffsetYInput.onValueChanged.AddListener(ChangeDebugOffsetY);

            calibX.onValueChanged.AddListener(ChangeCalibX);
            calibY.onValueChanged.AddListener(ChangeCalibY);

            sizeMinThreshold.onValueChanged.AddListener(ChangeSizeMinThreshold);
            sizeMaxThreshold.onValueChanged.AddListener(ChangeSizeMaxThreshold);
            centerThreshold.onValueChanged.AddListener(ChangeCenterThreshold);

            sensorPostionZ.onValueChanged.AddListener(ChangeSensorPosZ);

            horizontalToggl = horizontalToggleGroup.gameObject.GetComponentsInChildren<Toggle>();
            foreach (var toggle in horizontalToggl)
            {
                if (toggle.GetComponent<Toggle>() != null)
                {
                    toggle.GetComponent<Toggle>().onValueChanged.AddListener(ChangeHorizontalGroup);
                }
            }

            verticalToggle = verticalToggleGroup.gameObject.GetComponentsInChildren<Toggle>();
            foreach (var toggle in verticalToggle)
            {
                if (toggle.GetComponent<Toggle>() != null)
                {
                    toggle.GetComponent<Toggle>().onValueChanged.AddListener(ChangeVerticalGroup);
                }
            }


            save.onClick.AddListener(Save);
            apply.onClick.AddListener(Apply);
        }

        public void IpAddress(string arg) {
            ipAddress.text = arg;
        }

        public void Save() {
            string filePath = Application.streamingAssetsPath + fileName;
            string stringData = JsonUtility.ToJson(Data);
            Debug.Log(stringData);
            File.WriteAllText(filePath, stringData);
            Debug.Log(filePath);
            Debug.Log("save adjust data : " + fileName);
        }

        public void ChangeCalibX(float val)
        {
            calibXValue.text = val.ToString();
            Data.calibX = val;
        }
        public void ChangeCalibY(float val)
        {
            calibYValue.text = val.ToString();
            Data.calibY = val;
        }

        public void ChangeSizeMinThreshold(float val)
        {
            sizeMinThresholdValue.text = val.ToString();
            Data.min = val;
        }
        public void ChangeSizeMaxThreshold(float val)
        {
            sizeMaxThresholdValue.text = val.ToString();
            Data.max = val;
        }
        public void ChangeCenterThreshold(float val)
        {
            centerThresholdValue.text = val.ToString();
            Data.center = val;
        }

        public void ChangeDisplayWidth(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.displayWidth = value;
        }

        public void ChangeDisplayHeight(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.displayHeight = value;
        }

        public void ChangeSensorOffsetX(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.sensorOffsetX = value;
        }

        public void ChangeSensorOffsetY(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.sensorOffsetY = value;
        }

        public void ChangeWorldPointOffsetX(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.worldPointOffsetX = value;
        }

        public void ChangeWorldPointOffsetY(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.worldPointOffsetY = value;
        }
        public void ChangeAngleZ(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.angleZ = value;
        }

        public void ChangeDebugOffsetX(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.debugDrawOffsetX = value;
        }

        public void ChangeDebugOffsetY(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.debugDrawOffsetY = value;
        }

        public void ChangeSensorPosZ(float val){
            sensorPostionZValue.text = val.ToString();
            Data.sensorPosZ = val;
        }

        public void ChangeHorizontalGroup(bool state)
        {
            if (state)
            {
                Toggle activeToggle = horizontalToggleGroup.ActiveToggles().FirstOrDefault();
                switch(activeToggle.name)
                {
                    case "Left"     : Data.horizontal = (int)Horizontal.Left; break;
                    case "Center"   : Data.horizontal = (int)Horizontal.Center; break;
                    case "Right"    : Data.horizontal = (int)Horizontal.Right; break;

                    default : break;
                }
            }
        }

        public void ChangeVerticalGroup(bool state)
        {
            if (state)
            {
                Toggle activeToggle = verticalToggleGroup.ActiveToggles().FirstOrDefault();
                switch(activeToggle.name)
                {
                    case "Top"      : Data.vertical = (int)Vertical.Top; break;
                    case "Bottom"   : Data.vertical = (int)Vertical.Bottom; break;
                    default : break;
                }
            }
        }
        
        public void DisplayValues() {
            if (displayWidthInput != null) {
                displayWidthInput.text = Data.displayWidth.ToString();
            }
            if (displayHeightInput != null) {
                displayHeightInput.text = Data.displayHeight.ToString();
            }
            if (sensorOffsetXInput != null) {
                sensorOffsetXInput.text = Data.sensorOffsetX.ToString();
            }
            if (sensorOffsetYInput != null) {
                sensorOffsetYInput.text = Data.sensorOffsetY.ToString();
            }
            if (angleZInput != null) {
                angleZInput.text = Data.angleZ.ToString();
            }

            if (worldPointOffsetXInput != null) {
                worldPointOffsetXInput.text = Data.worldPointOffsetX.ToString();
            }
            if (worldPointOffsetYInput != null) {
                worldPointOffsetYInput.text = Data.worldPointOffsetY.ToString();
            }
            if (debugDrawOffsetXInput != null) {
                debugDrawOffsetXInput.text = Data.debugDrawOffsetX.ToString();
            }
            if (debugDrawOffsetYInput != null) {
                debugDrawOffsetYInput.text = Data.debugDrawOffsetY.ToString();
            }

            if (calibX != null) {
                calibX.value = Data.calibX;
                calibXValue.text = Data.calibX.ToString();
            }
            if (calibY != null) {
                calibY.value = Data.calibY;
                calibYValue.text = Data.calibY.ToString();
            }
            if (sizeMinThreshold != null) {
                sizeMinThreshold.value = Data.min;
                sizeMinThresholdValue.text = Data.min.ToString();
            }
            if (sizeMaxThreshold != null) {
                sizeMaxThreshold.value = Data.max;
                sizeMaxThresholdValue.text = Data.max.ToString();
            }
            if (centerThreshold != null) {
                centerThreshold.value = Data.center;
                centerThresholdValue.text = Data.center.ToString();
            }
            if (sensorPostionZ != null) {
                sensorPostionZ.value = Data.sensorPosZ;
                sensorPostionZValue.text = Data.sensorPosZ.ToString();
            }
            if (horizontalToggleGroup != null) {
                switch(Data.horizontal)
                {
                    case  0: horizontalToggl[0].isOn = true; break;
                    case  1: horizontalToggl[1].isOn = true; break;
                    case  2: horizontalToggl[2].isOn = true; break;
                    default : break;
                }
            }
            if (horizontalToggleGroup != null) {
                switch(Data.vertical)
                {
                    case  0: verticalToggle[0].isOn = true; break;
                    case  1: verticalToggle[1].isOn = true; break;
                }
            }
        }
        
        public void Apply() {
            if (OnApply != null) {
                OnApply();
            }
        }
    }
}
