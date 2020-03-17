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
        }

        public void Save() {
            string filePath = Application.dataPath + "/adjust.json";
            string stringData = JsonUtility.ToJson(Data);
            File.WriteAllText(filePath, stringData);
            Debug.Log("save adjust data");

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
        }
        
        public void Apply() {
            if (OnApply != null) {
                OnApply();
            }
        }
    }
}
