using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnClickNumber : MonoBehaviour
{
    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField.text = "";
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            inputField.text += gameObject.GetComponentInChildren<Text>().text;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
