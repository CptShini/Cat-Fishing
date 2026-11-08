using System.Collections;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Helpers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public bool Transitioning { get; private set; }
        
        [Header("References")]
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private MusicPlayer musicPlayer;

        [Space]
        
        [Header("Settings")]
        [SerializeField] private float sceneTransitionTime = 3f;
        [SerializeField] private float musicFadeInTime = 3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            SceneManager.sceneLoaded += (_, _) => PostLoad();
            musicPlayer.PlayTrack(SceneManager.GetActiveScene().buildIndex,
                musicFadeInTime
            );
            
            SaveFile[] saves = saveManager.GetSaves();
            if (saves.Length > 0) saveManager.Select(saves[0].Name);
            
            saveManager.Load();
            
            saveManager.RefreshDPOs();
            saveManager.LoadData();
        }

        public void ChangeScene(int buildIndex) => StartCoroutine(LoadSceneWithFade(buildIndex));

        private IEnumerator LoadSceneWithFade(int buildIndex)
        {
            PreLoad();
            
            yield return new WaitForSeconds(sceneTransitionTime);
            
            SceneManager.LoadScene(buildIndex);
        }

        private void PreLoad()
        {
            if (Transitioning) return;
            
            Transitioning = true;
            
            saveManager.SaveData();
            
            sceneTransition.FadeIn();
            musicPlayer.StopTrack(sceneTransitionTime);
        }

        private void PostLoad()
        {
            if (!Transitioning) return;
            
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            musicPlayer.PlayTrack(currentScene, musicFadeInTime);
            sceneTransition.FadeOut();
            
            saveManager.RefreshDPOs();
            saveManager.LoadData();
            
            Transitioning = false;
        }
    }
}