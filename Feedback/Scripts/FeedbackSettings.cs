using UnityEngine;

namespace FeedbackForm
{
    public class FeedbackSettings : MonoBehaviour
    {
        private string trelloListId;
        private string trelloApiKey;
        private string trelloToken;

        private void Start()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load API key, token, and list ID from a secure source
            trelloListId = "YourListID";
            trelloApiKey = "YourAPIKey";
            trelloToken = "YourToken";
        }

        public string GetTrelloListId() => trelloListId;
        public string GetTrelloApiKey() => trelloApiKey;
        public string GetTrelloToken() => trelloToken;
    }
}