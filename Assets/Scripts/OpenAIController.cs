using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Collections.Generic;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public string userName;
    public Button submit;

    [SerializeField] private int charLimit;

    private OpenAIAPI api;
    private List<ChatMessage> messages;

    [Header("BallAI")]
    [SerializeField] private GameObject _ballAI;
    private float _originalRotationSpeed;
    private Vector3 _originalVectorScale;


    // Start is called before the first frame update
    void Start()
    {
        // stored on the local system Environment so no one can steal it from github ;))
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));

        StartConversation();
        submit.onClick.AddListener(() => GetResponse());

        _originalRotationSpeed = _ballAI.GetComponent<AIAnimation>()._rotationSpeed;
        _originalVectorScale = _ballAI.GetComponent<AIAnimation>()._vectorScale;
    }

    private void StartConversation()
    {
        messages = new List<ChatMessage>
        {
            // implement a preset "characteristic" for the AI
            new ChatMessage(ChatMessageRole.System,
            "You are a British butler who is good with humor and sarcasm. You often keep your responses short and to the point."
            )
        };

        inputField.text = "";
        string intro = "Greetings, my name is JARVIS. Let me know what you need.";
        textField.text = intro;
        Debug.Log(intro);
    }
    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return; // make sure we don't send useless input into the field
        }

        // Disable the submit button to prevent from inputting more
        submit.enabled = false;

        // fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        if (inputField.text != null)
        {
            _ballAI.GetComponent<AIAnimation>().setAIAnimationResponse(_originalRotationSpeed * 10,
                _originalVectorScale * 10);
        }
        // cut budget short and make sure we don't spam user message
        if (userMessage.Content.Length > charLimit)
        {
            userMessage.Content = userMessage.Content.Substring(0, charLimit);
        }
        // 0 for string value not just the enumerator- rawRole, and then 1 for content
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        // add the message to the list
        messages.Add(userMessage);

        // update the text field with user's own message
        textField.text = string.Format(userName + ": {0}", userMessage.Content);


        // refresh input field
        inputField.text = "";

        // insert animation here. letting it "think faster"
        _ballAI.GetComponent<AIAnimation>().setAIAnimationResponse(_originalRotationSpeed * 50,
            _originalVectorScale * 50);

        // send asynchronous chat completion request to OpenAI, set model
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo, // provide a chat version
            Temperature = 0.3, // how "creative it becomes"
            MaxTokens = 50, // make the responses short and sweet (also makes you pay less money...)
            Messages = messages // allows the chat to "remember" what you said previously
        });

        // aquire responses
        ChatMessage responseMessage = new ChatMessage();
        // response with various choices, [0] is the lazy man's choice, might need more testing
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        // add the response to the list of messages
        messages.Add(responseMessage);

        // insert the "closure" animation where the bot response with a fancy voice animation

        // update the text field with the response
        textField.text = string.Format(userName + ": {0}\n\n" +
            "Jarvis: {1}", userMessage.Content, responseMessage.Content);

        // reset animation
        _ballAI.GetComponent<AIAnimation>().setAIAnimationResponse(_originalRotationSpeed,
            _originalVectorScale);

        // enable the disabled button
        submit.enabled = true;
    }

    // oculus voice SDK
}
