using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URG {
    [Serializable]
    public class AdjustData {
        public float displayWidth = 450;
        public float displayHeight = 300;
        public float offsetX = 0;
        public float offsetY = 500;
        public float angleZ = 180;
        public float distance = 3000;

        public float calibX = 1f;
        public float calibY = 1f;

        public float min = 0.01f;
        public float max = 0.5f;
        public float center = 0.5f;

    }
}