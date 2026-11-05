using System;
using System.Text;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public SerializableDictionary<string, FishDTO> Collection;
        public SerializableDictionary<string, TankData> Tanks;
        
        public GameData()
        {
            Collection = new();
            Tanks = new();
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            {
                sb.AppendLine("Collection: ");
                foreach (string fishId in Collection.Keys)
                {
                    sb.AppendLine($"{fishId} -  {Collection[fishId]}");
                }
            }

            {
                
            }

            return sb.ToString();
        }
    }
}