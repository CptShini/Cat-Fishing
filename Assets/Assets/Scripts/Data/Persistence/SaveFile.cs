using System;
using System.IO;

namespace Assets.Scripts.Data.Persistence
{
    public record SaveFile
    {
        public readonly string Name;
        public readonly DateTime LastPlayed;

        public SaveFile(FileInfo file)
        {
            Name = Path.GetFileNameWithoutExtension(file.Name);
            LastPlayed = file.LastWriteTime;
        }

        public override string ToString() => 
            $"{Name}\n({LastPlayed.ToShortTimeString()} - {LastPlayed.ToShortDateString()})";
    }
}