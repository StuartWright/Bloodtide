using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.SceneUtils;
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    public delegate void Buttons();
    public event Buttons AcceptClicked;    
    public event Buttons CloseClicked;    
    public Text DialogText;
    public Button AcceptButton;
    public GameObject LeaveButton;
    public GameObject DialogPanel;
    
    void Awake()
    {
        Instance = this;
    }

    public void UIText(Dialog Dialog)
    {
        DialogPanel.SetActive(true);
        DialogText.text = Dialog.WhatToSay();
        PlaceTargetWithMouse.Instance.CanMove = false;
    }
   
    public void Accept()
    {
        AcceptClicked();
    }
    public void ClosePanel()
    {
        CloseClicked?.Invoke();
        DialogPanel.SetActive(false);
        PlaceTargetWithMouse.Instance.CanMove = true;
    }
}
