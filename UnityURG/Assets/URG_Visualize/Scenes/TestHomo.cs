using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using OpenCvSharp;
using UnityEngine.UI;
public class TestHomo : MonoBehaviour
{

    // [SerializeField] GameObject[] go;
    // [SerializeField] GameObject[] toGo;
    // Vector2[] goPos;

    // [SerializeField] float x, y;

    // [SerializeField] GameObject point;


    // // Start is called before the first frame update
    // void Start()
    // {

    //     x = point.transform.position.x;
    //     y = point.transform.position.y;
    //     var srcPoints = new Point2f[] {
    //         new Point2f(toGo[0].transform.position.x, toGo[0].transform.position.y),
    //         new Point2f(toGo[1].transform.position.x, toGo[1].transform.position.y),
    //         new Point2f(toGo[2].transform.position.x, toGo[2].transform.position.y),
    //         new Point2f(toGo[3].transform.position.x, toGo[3].transform.position.y),
    //     };

    //     Debug.Log(srcPoints[0]);
    //     Debug.Log(srcPoints[1]);
    //     Debug.Log(srcPoints[2]);
    //     Debug.Log(srcPoints[3]);

    //     Debug.Log("==============");

    //     goPos = new Vector2[go.Length];
    //     for (int i = 0; i < go.Length; i++)
    //     {
    //         goPos[i] = go[i].transform.position;         
    //     }

    //     var dstPoints = new Point2f[] {
    //         new Point2f(goPos[0].x, goPos[0].y),
    //         new Point2f(goPos[1].x, goPos[1].y),
    //         new Point2f(goPos[2].x, goPos[2].y),
    //         new Point2f(goPos[3].x, goPos[3].y),
    //     };

    //     Mat homo = Cv2.FindHomography(InputArray.Create(srcPoints),  InputArray.Create(dstPoints));
    //     Point2f[] pos = Cv2.PerspectiveTransform(srcPoints, homo); 

    //     Matrix4x4 homoMx = Matrix4x4.identity;

    //     homoMx.m00 = pos[0].X; 
    //     homoMx.m01 = pos[0].Y; 
    //     homoMx.m02 = pos[1].X;
    //     homoMx.m10 = pos[1].Y; 
    //     homoMx.m11 = pos[2].X; 
    //     homoMx.m12 = pos[2].Y;
    //     homoMx.m20 = pos[3].X; 
    //     homoMx.m21 = pos[3].Y; 
    //     homoMx.m22 = 1f;

    //     Matrix4x4 sensorMx = Matrix4x4.identity;
    //     sensorMx.m00 = x;
    //     sensorMx.m10 = y;
    //     sensorMx.m20 = 1.0f;

    //     Matrix4x4 posMx  = Matrix4x4.identity;
    //     posMx = homoMx * sensorMx;

    //     Debug.Log(pos[0]);
    //     Debug.Log(pos[1]);
    //     Debug.Log(pos[2]);
    //     Debug.Log(pos[3]);

    //     Debug.Log("==============");

    //     Debug.Log(posMx.m00);
    //     Debug.Log(posMx.m01);
    //     Debug.Log(posMx.m02);
    //     Debug.Log(posMx.m03);
    //     Debug.Log(posMx.m10);
    //     Debug.Log(posMx.m11);
    //     Debug.Log(posMx.m12);
    //     Debug.Log(posMx.m20);
    //     Debug.Log(posMx.m21);
    //     Debug.Log(posMx.m22);

    //     Debug.Log("==============");

    //     point.transform.position = new Vector3(posMx[0,0], posMx[1,0], posMx[2,0]);

    //     toGo[0].transform.position = new Vector3(pos[0].X, pos[0].Y, 0f);
    //     toGo[1].transform.position = new Vector3(pos[1].X, pos[1].Y, 0f);
    //     toGo[2].transform.position = new Vector3(pos[2].X, pos[2].Y, 0f);
    //     toGo[3].transform.position = new Vector3(pos[3].X, pos[3].Y, 0f);

    // }

    // Update is called once per frame
    void Update()
    {

    }	

}
