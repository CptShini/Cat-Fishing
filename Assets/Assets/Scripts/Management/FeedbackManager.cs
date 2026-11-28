using System.Collections;
using Sentry.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Management
{
    public class FeedbackManager : MonoBehaviour
    {
        [SerializeField] private Button submitFeedbackButton;
        
        [SerializeField] private TMP_InputField
            nameInputField,
            descriptionInputField;
        
        [SerializeField] private Toggle addScreenshot;
        
        [SerializeField] private UnityEvent onSubmitFeedback, onSubmitFeedbackWithScreenshot;
        
        private string 
            _name,
            _description;

        private void Start()
        {
            _name = PlayerPrefs.GetString("FeedbackName", null);
            
            if (!string.IsNullOrWhiteSpace(_name)) nameInputField.text = _name;
        }

        public void OnNameChanged(string name) => _name = name;
        public void OnDescriptionChanged(string description)
        {
            _description = description;
            submitFeedbackButton.interactable = !string.IsNullOrWhiteSpace(description);
        }
        
        public void GiveFeedback()
        {
            if (addScreenshot.isOn)
            {
                StartCoroutine(HideFormAndCaptureFeedback());
                return;
            }
                
            SendFeedback(false);
            onSubmitFeedback.Invoke();
        }

        private IEnumerator HideFormAndCaptureFeedback()
        {
            PauseControllerManager.Instance.ResumeGame();
            
            yield return new WaitForEndOfFrame();
            
            SendFeedback(true);
            onSubmitFeedbackWithScreenshot.Invoke();
        }

        private void SendFeedback(bool takeScreenshot)
        {
            bool canScreenshot = takeScreenshot && !Application.isEditor;
            SentrySdk.CaptureFeedback(_description, null, _name, canScreenshot);
            descriptionInputField.text = string.Empty;
        }

        private void OnDestroy() => PlayerPrefs.SetString("FeedbackName", _name);
    }
}