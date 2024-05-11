using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnClickClear : MonoBehaviour
{
    public InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length-1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
