using Assets.Scripts.Data.Domain;

namespace Assets.Scripts.Data.Persistence.DataIO
{
    public interface DataIOHandler
    {
        SaveFile[] GetSaves();
        GameData Load(string saveName);
        void Save(GameData data, string saveName);
    }
}