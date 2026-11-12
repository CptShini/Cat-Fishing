using System.Collections;
using Assets.Scripts.Audio;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Management
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool Transitioning { get; private set; } = true;

        [Header("References")] 
        [SerializeField] private SceneList sceneList;
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private MusicPlayer musicPlayer;

        [Space]
        
        [Header("Settings")]
        [SerializeField] private float sceneTransitionTime = 2f;
        [SerializeField] private float musicFadeInTime = 2f;

        private SceneInfo _currentScene;
        private DataPersistenceManager _dpm;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            _dpm = GetComponentInChildren<DataPersistenceManager>();
            _currentScene = sceneList.GetCurrentScene();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (Instance == this)
                Instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => FadeIn();

        public void ChangeScene(Scenes scene, string info) => StartCoroutine(LoadSceneWithFade(scene, info));

        private IEnumerator LoadSceneWithFade(Scenes scene, string transitionInfo)
        {
            _currentScene = sceneList.GetSceneInfo(scene);
            FadeOut(transitionInfo);
            
            yield return new WaitForSeconds(sceneTransitionTime);
            
            SceneManager.LoadScene(_currentScene.BuildIndex);
        }

        private void FadeOut(string text)
        {
            if (Transitioning) return;
            
            Transitioning = true;
            
            _dpm.SaveGameToData();
            
            sceneTransition.FadeOut(text);
            musicPlayer.StopTrack(sceneTransitionTime);
        }

        private void FadeIn()
        {
            if (!Transitioning) return;
            
            SaveManager.Instance.Initialize();
            SaveManager.Instance.RefetchSaves();
            _dpm.FindDPOs();
            _dpm.LoadDataToGame();
            
            musicPlayer.PlayTrack(_currentScene.BGM, musicFadeInTime);
            sceneTransition.FadeIn();
            
            Transitioning = false;
        }
    }
}