using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject obj_MiniMap;
    public GameObject obj_VideoPlane;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Open", 1f);
    }
    void Open()
    {
        obj_MiniMap.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (obj_VideoPlane.activeSelf)
            {
                obj_VideoPlane.SetActive(false);
            }
            else
            {
                obj_VideoPlane.SetActive(true);
            }
           
        }
    }
}
