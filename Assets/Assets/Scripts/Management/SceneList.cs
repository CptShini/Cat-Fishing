using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Management
{
    [CreateAssetMenu(fileName = "New Scene List", menuName = "ScriptableObjects/Scene List")]
    public class SceneList : ScriptableObject
    {
        [Space]
        [SerializeField] private SceneInfo 
            mainMenu,
            aquarium,
            diving,
            dailyReport;

        public SceneInfo GetSceneInfo(Scenes scene) =>
            scene switch
            {
                Scenes.MainMenu => mainMenu,
                Scenes.Aquarium => aquarium,
                Scenes.Diving => diving,
                Scenes.DailyReport => dailyReport,
                _ => throw new ArgumentOutOfRangeException(nameof(scene), scene, null)
            };

        public SceneInfo GetCurrentScene()
        {
            int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
            return 
                currentBuildIndex == mainMenu.BuildIndex ? mainMenu :
                currentBuildIndex == aquarium.BuildIndex ? aquarium :
                currentBuildIndex == diving.BuildIndex ? diving :
                currentBuildIndex == dailyReport.BuildIndex ? dailyReport :
                throw new ArgumentOutOfRangeException(nameof(currentBuildIndex), currentBuildIndex, null);
        }
    }
}