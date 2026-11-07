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
            musicPlayer.PlayTrack(SceneManager.GetActiveScene().buildIndex,
                musicFadeInTime
            );
            
            SaveFile[] saves = saveManager.GetSaves();
            if (saves.Length > 0) saveManager.Select(saves[0].Name);
            
            saveManager.Load();
            saveManager.LoadData();
        }

        public void ChangeScene(int buildIndex) => StartCoroutine(LoadSceneWithFade(buildIndex));

        private IEnumerator LoadSceneWithFade(int buildIndex)
        {
            Transitioning = true;
            
            saveManager.SaveData();
            musicPlayer.StopTrack(sceneTransitionTime);
            sceneTransition.FadeIn();

            yield return new WaitForSeconds(sceneTransitionTime);

            SceneManager.LoadScene(buildIndex);
            
            sceneTransition.FadeOut();
            musicPlayer.PlayTrack(buildIndex, musicFadeInTime);
            saveManager.LoadData();
            
            Transitioning = false;
        }
    }
}