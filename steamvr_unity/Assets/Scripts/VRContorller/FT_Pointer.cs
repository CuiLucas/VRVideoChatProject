﻿using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

public class FT_Pointer : MonoBehaviour
{
    /// <summary>
    /// 和模型交互
    /// </summary>
    public SteamVR_Action_Boolean interactWithModel = SteamVR_Actions.default_GrabPinch;

    public FT_LaserPointer vR_LaserPointer;

    public event PointerEventHandler PointerIn;

    public Image image_Show;
    public static FT_Pointer _Pointer;
    void Awake()
    {
        _Pointer = this;
    }
    void Start()
    {
        vR_LaserPointer = GetComponent<FT_LaserPointer>();

        vR_LaserPointer.interactWithModel = interactWithModel;

        vR_LaserPointer.PointerIn += OnPointerIn;
        vR_LaserPointer.PointerOut += OnPointerOut;
        vR_LaserPointer.PointerClick += OnPointerClick;
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<FT_InteractableRay>() != null && e.target.GetComponent<FT_InteractableRay>().isActive)
        {
            e.target.GetComponent<FT_InteractableRay>().RayEnter();
        }
    }
    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<FT_InteractableRay>() != null && e.target.GetComponent<FT_InteractableRay>().isActive)
        {
            e.target.GetComponent<FT_InteractableRay>().RayExit();
        }
    }
    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<FT_InteractableRay>() != null && e.target.GetComponent<FT_InteractableRay>().isActive)
        {
            e.target.GetComponent<FT_InteractableRay>().RayClick();
        
        }

   
    }
}