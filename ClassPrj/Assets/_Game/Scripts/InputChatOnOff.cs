using UnityEngine;
using System.Collections;

public class InputChatOnOff : MonoBehaviour {

    public CanvasGroup canvasGroupChat;

    //metodi richiamati da eventi nelle animazioni della chat:
	public void ChatOn()
    {
        canvasGroupChat.interactable = true;
    }
    public void ChatOff()
    {
        canvasGroupChat.interactable = false;
    }
}
