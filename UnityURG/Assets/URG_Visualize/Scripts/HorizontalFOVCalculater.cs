using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HorizontalFOV
{
    public static class HorizontalFOVCalculater
    {
        public static float SetFieldOfView(float baseFov = 60f, float baseAspectX = 1920, float baseAspectY = 1080)
        {

            float baseHorizontalFOV = CalcHorizontalFOV(baseFov, CalculateAspect(baseAspectX, baseAspectY));
            float currentAspect = CalculateAspect(Screen.width, Screen.height);

            return CalculateVerticalFOV(baseHorizontalFOV, currentAspect);
        }

        static float CalculateAspect(float width, float height) 
        {
            return width / height;
        }

        static float CalcHorizontalFOV(float verticalFOV, float aspect) 
        {
            return Mathf.Atan(Mathf.Tan(verticalFOV / 2f * Mathf.Deg2Rad) * aspect) * 2f * Mathf.Rad2Deg;
        }

        static float CalculateVerticalFOV(float horizontalFOV, float aspect) 
        {
            return Mathf.Atan(Mathf.Tan(horizontalFOV / 2f * Mathf.Deg2Rad) / aspect) * 2f * Mathf.Rad2Deg;
        }

    }

}