using System.Collections;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Helpers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool Transitioning { get; private set; } = true;
        
        [Header("References")]
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private MusicPlayer musicPlayer;

        [Space]
        
        [Header("Settings")]
        [SerializeField] private float sceneTransitionTime = 2f;
        [SerializeField] private float musicFadeInTime = 2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (Instance == this)
                Instance = null;
        }

        public void ChangeScene(int buildIndex) => StartCoroutine(LoadSceneWithFade(buildIndex));

        private IEnumerator LoadSceneWithFade(int buildIndex)
        {
            FadeOut();
            
            yield return new WaitForSeconds(sceneTransitionTime);
            
            SceneManager.LoadScene(buildIndex);
        }

        private void FadeOut()
        {
            if (Transitioning) return;
            
            Transitioning = true;
            
            SaveManager.Instance.SaveData();
            
            sceneTransition.FadeOut();
            musicPlayer.StopTrack(sceneTransitionTime);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!Transitioning) return;
            
            SaveManager.Instance.Initialize();
            SaveManager.Instance.FindDPOs();
            SaveManager.Instance.LoadData();
            
            musicPlayer.PlayTrack(scene.buildIndex, musicFadeInTime);
            sceneTransition.FadeIn();
            
            Transitioning = false;
        }
    }
}