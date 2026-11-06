using Assets.Scripts.Data.Domain;

namespace Assets.Scripts.Data.Persistence
{
    public interface IDataPersistence
    {
        void LoadData(GameData data);
        void SaveData(ref GameData data);
    }
}