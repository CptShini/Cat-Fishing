using Sentry.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Management
{
    public class FeedbackManager : MonoBehaviour
    {
        [SerializeField] private Button submitFeedbackButton;
        
        [SerializeField] private TMP_InputField
            nameInputField,
            emailInputField,
            descriptionInputField;
        
        private string 
            _name,
            _email,
            _description;

        private void Start()
        {
            _name = PlayerPrefs.GetString("FeedbackName", null);
            _email = PlayerPrefs.GetString("FeedbackEmail", null);
            
            if (!string.IsNullOrWhiteSpace(_name)) nameInputField.text = _name;
            if (!string.IsNullOrWhiteSpace(_email)) emailInputField.text = _email;
        }

        public void OnNameChanged(string name) => _name = name;
        public void OnEmailChanged(string email) => _email = email;
        public void OnDescriptionChanged(string description)
        {
            _description = description;
            submitFeedbackButton.interactable = !string.IsNullOrWhiteSpace(description);
        }
        
        public void GiveFeedback()
        {
            SentrySdk.CaptureFeedback(_description, _email, _name);
            descriptionInputField.text = string.Empty;
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetString("FeedbackName", _name);
            PlayerPrefs.SetString("FeedbackEmail", _email);
        }
    }
}