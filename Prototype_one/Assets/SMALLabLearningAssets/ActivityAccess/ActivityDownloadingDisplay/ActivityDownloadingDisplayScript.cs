using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityDownloadingDisplayScript : MonoBehaviour {

    Text header, message, buttonText;

    void Awake()
    {
        header = gameObject.transform.Find("InfoPanel/Header").GetComponent<Text>();
        message = gameObject.transform.Find("InfoPanel/Message").GetComponent<Text>();
        //buttonText = gameObject.transform.Find("InfoPanel/OkButton/Text").GetComponent<Text>();
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
        this.header.text = header;
        this.message.text = message;
        //this.buttonText.text = button;

    }

}
