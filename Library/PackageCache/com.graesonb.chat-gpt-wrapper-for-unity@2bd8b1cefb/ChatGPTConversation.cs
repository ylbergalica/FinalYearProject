using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Reqs;

namespace ChatGPTWrapper {

    public class ChatGPTConversation : MonoBehaviour
    {
        // [SerializeField]
        private bool _useProxy = false;
        // [SerializeField]
        private string _proxyUri = null;

        // [SerializeField]
        private string _apiKey = "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";

        public enum Model {
            ChatGPT,
            Davinci,
            Curie
        }
        // [SerializeField]
        public Model _model = Model.ChatGPT;
        private string _selectedModel = null;
        // [SerializeField]
        private int _maxTokens = 200;
        // [SerializeField]
        private float _temperature = 0.5f;
        
        private string _uri;
        private List<(string, string)> _reqHeaders;
        

        private Requests requests = new Requests();
        private Prompt _prompt;
        private Chat _chat;
        private string _lastUserMsg;
        private string _lastChatGPTMsg;

        private string _chatbotName = "ChatGPT";

        [TextArea(4,6)]
        private string _initialPrompt = "You are an assistant tasked with helping a user design their game. The user's game must include a description of the following elements: [GameName, Environment]. No other elements are needed. The user may request your assistance in filling in some requirements or coming up with ideas. Keep track of every requirement provided. Once all necessary requirements for the game have been filled, you will respond with a final message. The final message must include the keyphrase 'Commencing World Creation' exactly as specified. In the final message you should include: GameName (the name of the game), Environment (a description of the environment of the game).";

		private string _finalPrompt = "Format and extract from the following text response these aspects in the exact folling way:\n\n{GameName}: (the name of the game here) {Environment}: (a description of the environment here).\n\nInclude in your response the exact keyphrase 'Processing World':\n\n";

		[SerializeField]
        private UnityStringEvent chatGPTResponse = new UnityStringEvent();
		[SerializeField]
		private UnityStringEvent finalResponse = new UnityStringEvent();

        private void OnEnable()
        {
            
            TextAsset textAsset = Resources.Load<TextAsset>("APIKEY");
            if (textAsset != null) {
                _apiKey = textAsset.text;
            }
            
            
            _reqHeaders = new List<(string, string)>
            { 
                ("Authorization", $"Bearer {_apiKey}"),
                ("Content-Type", "application/json")
            };
            switch (_model) {
                case Model.ChatGPT:
                    _chat = new Chat(_initialPrompt);
                    _uri = "https://api.openai.com/v1/chat/completions";
                    _selectedModel = "gpt-3.5-turbo";
                    break;
                case Model.Davinci:
                    _prompt = new Prompt(_chatbotName, _initialPrompt);
                    _uri = "https://api.openai.com/v1/completions";
                    _selectedModel = "text-davinci-003";
                    break;
                case Model.Curie:
                    _prompt = new Prompt(_chatbotName, _initialPrompt);
                    _uri = "https://api.openai.com/v1/completions";
                    _selectedModel = "text-curie-001";
                    break;
            }
        }

        public void ResetChat(string initialPrompt) {
            switch (_model) {
                case Model.ChatGPT:
                    _chat = new Chat(initialPrompt);
                    break;
                default:
                    _prompt = new Prompt(_chatbotName, initialPrompt);
                    break;
            }
        }

        public void SendToChatGPT(string message)
        {
            _lastUserMsg = message;

            if (_model == Model.ChatGPT) {
                if (_useProxy) {
                    ProxyReq proxyReq = new ProxyReq();
                    proxyReq.max_tokens = _maxTokens;
                    proxyReq.temperature = _temperature;
                    proxyReq.messages = new List<Message>(_chat.CurrentChat);
                    proxyReq.messages.Add(new Message("user", message));

                    string proxyJson = JsonUtility.ToJson(proxyReq);

                    StartCoroutine(requests.PostReq<ChatGPTRes>(_proxyUri, proxyJson, ResolveChatGPT, _reqHeaders));
                } else {
                    ChatGPTReq chatGPTReq = new ChatGPTReq();
                    chatGPTReq.model = _selectedModel;
                    chatGPTReq.max_tokens = _maxTokens;
                    chatGPTReq.temperature = _temperature;
                    chatGPTReq.messages = _chat.CurrentChat;
                    chatGPTReq.messages.Add(new Message("user", message));
            
                    string chatGPTJson = JsonUtility.ToJson(chatGPTReq);
                    
                    StartCoroutine(requests.PostReq<ChatGPTRes>(_uri, chatGPTJson, ResolveChatGPT, _reqHeaders));
                }
                
            } else {

                _prompt.AppendText(Prompt.Speaker.User, message);

                GPTReq reqObj = new GPTReq();
                reqObj.model = _selectedModel;
                reqObj.prompt = _prompt.CurrentPrompt;
                reqObj.max_tokens = _maxTokens;
                reqObj.temperature = _temperature;
                string json = JsonUtility.ToJson(reqObj);

                StartCoroutine(requests.PostReq<GPTRes>(_uri, json, ResolveGPT, _reqHeaders));
            }
        }

        private void ResolveChatGPT(ChatGPTRes res)
        {
			Debug.Log("USER: " + _lastUserMsg);
			Debug.Log("GPT Response: " + res.choices[0].message.content);

            _lastChatGPTMsg = res.choices[0].message.content;
            _chat.AppendMessage(Chat.Speaker.User, _lastUserMsg);
            _chat.AppendMessage(Chat.Speaker.ChatGPT, _lastChatGPTMsg);

			if (_lastChatGPTMsg.Contains("Commencing World Creation", StringComparison.OrdinalIgnoreCase)) {
				SendToChatGPT(_finalPrompt);
				chatGPTResponse.Invoke(_lastChatGPTMsg);
			}
			else if (_lastChatGPTMsg.Contains("Processing World", StringComparison.OrdinalIgnoreCase)) {
				finalResponse.Invoke(_lastChatGPTMsg);
			}
			else {
				chatGPTResponse.Invoke("Bot> " + _lastChatGPTMsg);
			}
        }

		// only if using proxy, we are not.
        private void ResolveGPT(GPTRes res)
        {			
            _lastChatGPTMsg = res.choices[0].text
                .TrimStart('\n')
                .Replace("<|im_end|>", "");

            _prompt.AppendText(Prompt.Speaker.Bot, _lastChatGPTMsg);
            chatGPTResponse.Invoke(_lastChatGPTMsg);
        }
    }
}
