using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URG {
    [Serializable]
    public class AdjustData {
        public float displayWidth = 450;
        public float displayHeight = 300;
        public float sensorOffsetX = 0;
        public float sensorOffsetY = 0;
    
        public float angleZ = 180;
        public float debugDrawOffsetX = 0;
        public float debugDrawOffsetY = 0;
        public float worldPointOffsetX = 0;
        public float worldPointOffsetY = 0;

        public float distance = 250;

        public float calibX = 1f;
        public float calibY = 1f;

        public float min = 0.01f;
        public float max = 0.5f;
        public float center = 0.5f;

        public float sensorPosZ = 0;
        public int horizontal = 0;
        public int vertical = 0;
    }
}