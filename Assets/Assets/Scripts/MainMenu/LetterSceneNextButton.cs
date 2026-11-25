using UnityEngine;
using Assets.Scripts.Audio.SFX;
using Assets.Scripts.Management;

namespace Assets.Scripts.Letter
{
    public class LetterSceneNextButton : MonoBehaviour
    {
        public void Next()
        {
            SFXManager.Instance.Play(SFXId.OpeningDoor);
            GameManager.Instance.ChangeScene(Scenes.Aquarium,"Going to aquarium...");
        }
    }
}