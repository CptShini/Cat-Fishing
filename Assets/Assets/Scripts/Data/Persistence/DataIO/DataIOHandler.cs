using Assets.Scripts.Data.Domain;

namespace Assets.Scripts.Data.Persistence.DataIO
{
    public interface DataIOHandler
    {
        SaveFile[] GetSaves();
        SaveFile Create(string saveName);
        GameData Load(SaveFile saveFile);
        void Save(GameData data, SaveFile saveFile);
        void Delete(SaveFile saveFile);
    }
}