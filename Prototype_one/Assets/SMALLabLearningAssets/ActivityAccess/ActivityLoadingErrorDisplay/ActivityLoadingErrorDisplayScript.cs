using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityLoadingErrorDisplayScript : MonoBehaviour {

    Text errorHeader, errorMessage, errorButtonText;

    void Awake()
    {
        errorHeader = gameObject.transform.Find("InfoPanel/Header").GetComponent<Text>();
        errorMessage = gameObject.transform.Find("InfoPanel/Message").GetComponent<Text>();
        errorButtonText = gameObject.transform.Find("InfoPanel/OkButton/Text").GetComponent<Text>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    public void ShowMessagePanel(bool show)
    {
        GetComponent<CanvasGroup>().interactable = show;
        GetComponent<CanvasGroup>().alpha = show ? 1.0f : 0.0f;
        GetComponent<CanvasGroup>().blocksRaycasts = show;
    }

    public void UpdatePanelText(string header, string message, string button)
    {
        errorHeader.text = header;
        errorMessage.text = message;

        errorButtonText.text = button;
    }

    public void OkButtonPressed()
    {
        ShowMessagePanel(false);   
    }
}
