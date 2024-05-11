using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class FT_ButtonAction : MonoBehaviour
{
    public SteamVR_Action_Boolean booleanAction;
    public GameObject VideoPlane;
    bool isVRContorll_Down = false;

    //0.0075   0.0025
    bool isUnityBtn_Down = false;


    // Start is called before the first frame update
    void Start()
    {
        booleanAction.onStateDown += OnStateDown;
    }
    
    public void OpenVideoPlane()
    {
        if (!isUnityBtn_Down)
        {
         
            VideoPlane.GetComponent<CalculateWindow>().windowAnchor = TextAnchor.MiddleCenter;
            isUnityBtn_Down = true;
            VideoPlane.gameObject.SetActive(true);
        }
        else
        {
           
            VideoPlane.GetComponent<CalculateWindow>().windowAnchor = TextAnchor.LowerLeft;
            isUnityBtn_Down = false;
            VideoPlane.gameObject.SetActive(false);
        }
    }
    public void OnDestroy()
    {
        booleanAction.onStateDown -= OnStateDown;
    }
    private void OnStateDown(SteamVR_Action_Boolean fromAction,SteamVR_Input_Sources fromSource)
    {
        if (!isVRContorll_Down)
        {
            VideoPlane.SetActive(true);
            isVRContorll_Down = true;
        }
        else
        {
            VideoPlane.SetActive(false);
            isVRContorll_Down = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            VideoPlane.SetActive(true);
        }
    }
}
