using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.Events;

namespace FeedbackForm
{
    public class FeedbackSender : MonoBehaviour
    {
        private string trelloListId;
        private string trelloApiKey;
        private string trelloToken;

        private FeedbackSettings _settings;

        private void Start()
        {
            _settings = GetComponent<FeedbackSettings>();

            if (_settings)
            {
                Initialize(_settings.GetTrelloListId(),_settings.GetTrelloApiKey(),_settings.GetTrelloToken());
            }
            else
            {
                Debug.LogError("Please attach Feedback Settings to same object!");
                enabled = false;
            }
            
        }

        public void Initialize(string listId, string apiKey, string token)
        {
            trelloListId = listId;
            trelloApiKey = apiKey;
            trelloToken = token;
        }

        public void SendFeedbackToTrello(string title, string description, UnityEvent onSuccess, UnityEvent onFailure, GameObject panel)
        {
            StartCoroutine(TakeScreenshotAndSend(title, description, onSuccess, onFailure, panel));
        }

        private IEnumerator TakeScreenshotAndSend(string title, string description, UnityEvent onSuccess, UnityEvent onFailure, GameObject panel)
        {
            panel.SetActive(false);
            yield return new WaitForEndOfFrame();
            Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();
            byte[] imageData = screenTexture.EncodeToPNG();
            yield return new WaitForEndOfFrame();
            panel.SetActive(true);

            WWWForm form = new WWWForm();
            form.AddField("idList", trelloListId);
            form.AddField("key", trelloApiKey);
            form.AddField("token", trelloToken);
            form.AddField("name", title);
            form.AddField("desc", description);
            form.AddBinaryData("fileSource", imageData, "screenshot.png", "image/png");

            UnityWebRequest www = UnityWebRequest.Post("https://api.trello.com/1/cards?", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed submission to Trello: " + www.error);
                onFailure?.Invoke();
            }
            else
            {
                Debug.Log("Feedback sent successfully");
                onSuccess?.Invoke();
            }
        }
    }
}
