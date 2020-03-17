using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SensorUtility{
    public static class SensorUtil{

        public static  float lowPass(float oldData, float rawData, float strength = 0.1f){
            float val = (oldData * (1 - strength)) + (((float)rawData)  *  strength);
            return val;
        }
    }
}
