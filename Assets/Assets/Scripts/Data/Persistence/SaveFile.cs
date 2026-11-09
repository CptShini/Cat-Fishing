using System;
using System.IO;

namespace Assets.Scripts.Data.Persistence
{
    public record SaveFile
    {
        public readonly string Name;
        public readonly DateTime Timestamp;

        public SaveFile(FileInfo file)
        {
            Name = Path.GetFileNameWithoutExtension(file.Name);
            Timestamp = file.LastWriteTime;
        }

        public override string ToString() => 
            $"{Name}\n({Timestamp.ToShortTimeString()} - {Timestamp.ToShortDateString()})";
    }
}