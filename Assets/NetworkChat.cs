using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NetworkChat : Singleton<NetworkChat>
{
    public GameObject chatPreview;
    public struct ChatMessage
    {
        public string message;
        public float time;

        public ChatMessage(string _message, float _time)
        {
            message = _message;
            time = _time;
        }
    }
    public List<ChatMessage> chatMessages = new List<ChatMessage>();
    public List<KeyValuePair<GameObject, ChatMessage>> chatviewList = new List<KeyValuePair<GameObject, ChatMessage>>();
    public GameObject massageBox;
    public InputField inputField;
    public KeyCode chatToggle = KeyCode.T;
    public bool chatToggleState;
    public float chatPreviewTime = 2f;
    public int maxAmountOfMessages = 25;
    

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(chatToggle))
        {
            chatToggleState = !chatToggleState;
            inputField.gameObject.SetActive(!chatToggleState);
            if (chatToggleState)
            {
                foreach (var chat in chatviewList.ToList())
                {
                    chatviewList.Remove(chat);
                    Destroy(chat.Key);
                }
            }       
            else
            {
                foreach (var chat in chatviewList)
                {
                    Destroy(chat.Key);
                }
                for (int i = 0; i < Mathf.Min(maxAmountOfMessages, chatMessages.Count); i++)
                {
                    CreateChatView(chatMessages[i]);
                }
            }
            
        }
        if (chatToggleState && chatviewList.Count > 0)
        {
            if ((Time.realtimeSinceStartup - chatviewList[chatviewList.Count - 1].Value.time) < 0)
            {
                var key = chatviewList[chatviewList.Count - 1];
                chatviewList.Remove(key);
                Destroy(key.Key);
            }
        }
    }

    public void CreateChatView(ChatMessage _chatMessage)
    {
        var massage = Instantiate(chatPreview, massageBox.transform);
        massage.SetActive(true);
        massage.GetComponentInChildren<Text>().text = _chatMessage.message;
        chatviewList.Add(new KeyValuePair<GameObject, ChatMessage>(massage, _chatMessage));
    }
    
    public void SendMassage(string _massage)
    {
        ClientSend.SendChatMassage(inputField.text);
        inputField.text = "";
        //to make it so it cant send mega text files
    }

    public void ReciveMassage(string _massage)
    {
        var massage = new ChatMessage(_massage, Time.realtimeSinceStartup);
        chatMessages.Add(massage);
        CreateChatView(massage);
        
    }
}
