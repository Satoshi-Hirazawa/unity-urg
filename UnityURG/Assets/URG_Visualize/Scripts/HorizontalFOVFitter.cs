using UnityEngine;
using HorizontalFOV;

public class HorizontalFOVFitter : MonoBehaviour 
{
    Camera mainCamera = null;
    [SerializeField] Vector2 baseAspect = new Vector2(1920, 1080);
    [SerializeField] float baseFieldOfView = 60f;

    void Awake() 
    {
        mainCamera = GetComponent<Camera>();
        if(mainCamera == null)
        {
        }
        SetFov();
    }

    void Update() 
    {
        SetFov();
    }

    void SetFov()
    {
        if(mainCamera == null) return;    
        mainCamera.fieldOfView = HorizontalFOV.HorizontalFOVCalculater.SetFieldOfView(baseFieldOfView, baseAspect.x, baseAspect.y);
        
    }

}