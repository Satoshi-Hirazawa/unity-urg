using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace URG {
    public class AdjustDataConfigurator : MonoBehaviour {


        [SerializeField] InputField displayWidthInput;
        [SerializeField] InputField displayHeightInput;
        [SerializeField] InputField offsetXInput;
        [SerializeField] InputField offsetYInput;
        [SerializeField] InputField angleZInput;
        [SerializeField] InputField distanceInput;
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

        [SerializeField] Button save;
        [SerializeField] Button apply;
        public AdjustData Data;
        public Action OnApply;

        public void Load() {
            string filePath = Application.dataPath + "/adjust.json";

            if (!File.Exists(filePath)) {
                Debug.Log("create new adjust data");
                Data = new AdjustData();

            } 
            else {
                Debug.Log("load adjust data");
                string stringData = File.ReadAllText(filePath);
                Data = JsonUtility.FromJson<AdjustData>(stringData);
            }

            displayWidthInput.onValueChanged.AddListener(ChangeDisplayWidth);
            displayHeightInput.onValueChanged.AddListener(ChangeDisplayHeight);
            offsetXInput.onValueChanged.AddListener(ChangeOffsetX);
            offsetYInput.onValueChanged.AddListener(ChangeOffsetY);
            angleZInput.onValueChanged.AddListener(ChangeAngleZ);
            distanceInput.onValueChanged.AddListener(ChangeDistance);

            calibX.onValueChanged.AddListener(ChangeCalibX);
            calibY.onValueChanged.AddListener(ChangeCalibY);

            sizeMinThreshold.onValueChanged.AddListener(ChangeSizeMinThreshold);
            sizeMaxThreshold.onValueChanged.AddListener(ChangeSizeMaxThreshold);
            centerThreshold.onValueChanged.AddListener(ChangeCenterThreshold);

            save.onClick.AddListener(Save);
            apply.onClick.AddListener(Apply);
        }

        public void Save() {
            string filePath = Application.dataPath + "/adjust.json";
            string stringData = JsonUtility.ToJson(Data);
            Debug.Log(stringData);
            File.WriteAllText(filePath, stringData);
            Debug.Log(filePath);
            Debug.Log("save adjust data");
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

        public void ChangeOffsetX(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.offsetX = value;
        }

        public void ChangeOffsetY(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.offsetY = value;
        }

        public void ChangeAngleZ(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.angleZ = value;
        }

        public void ChangeDistance(string arg) {
            float value;
            float.TryParse(arg, out value);
            Data.distance = value;
        }

        public void DisplayValues() {
            if (displayWidthInput != null) {
                displayWidthInput.text = Data.displayWidth.ToString();
            }
            if (displayHeightInput != null) {
                displayHeightInput.text = Data.displayHeight.ToString();
            }
            if (offsetXInput != null) {
                offsetXInput.text = Data.offsetX.ToString();
            }
            if (offsetYInput != null) {
                offsetYInput.text = Data.offsetY.ToString();
            }
            if (angleZInput != null) {
                angleZInput.text = Data.angleZ.ToString();
            }
            if (distanceInput != null) {
                distanceInput.text = Data.distance.ToString();
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
        }
        
        public void Apply() {
            if (OnApply != null) {
                OnApply();
            }
        }
    }
}
