using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest : MonoBehaviour
{
    int count = 0;
    float invincibleTime = 0.5f;
    bool isInvincible = false;

    public enum CUBE_POSITION
    {
        Center,
        BtmLeft,
        BtmRight,
        TopLeft,
        TopRight,

    }
    public CUBE_POSITION cubePositon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 p = new Vector3(0f, 0f, 5f);

        switch (cubePositon)
        {
            case CUBE_POSITION.Center   : p.x = Screen.width  / 2;   p.y = Screen.height / 2; break;
            case CUBE_POSITION.BtmLeft  : p.x = 30;                  p.y = 30;                break;
            case CUBE_POSITION.BtmRight : p.x = Screen.width-30;      p.y = 30;               break;
            case CUBE_POSITION.TopLeft  : p.x = 30;                   p.y = Screen.height-30; break;
            case CUBE_POSITION.TopRight : p.x = Screen.width-30;      p.y = Screen.height-30; break;
            default: break;
        }
        Vector2 pos = Camera.main.ScreenToWorldPoint(p);
        transform.position = new Vector3(pos.x, pos.y, 5f);        
    }

    public void ChangeColor()
    {
        if(isInvincible) return;
        count++;
        int num = count % 3;
        switch(num)
        {
            case 0: gameObject.GetComponent<Renderer>().material.color = Color.red; break;
            case 1: gameObject.GetComponent<Renderer>().material.color = Color.blue; break;
            case 2: gameObject.GetComponent<Renderer>().material.color = Color.green; break;
        }
        
        StartCoroutine(Invincible(invincibleTime));
    }
    IEnumerator Invincible(float wait)
    {
        isInvincible = true;
        yield return new WaitForSeconds(wait);
        isInvincible = false;
    }
}
