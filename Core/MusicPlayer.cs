using System.Diagnostics;
using System.IO;

namespace Core
{
    public class MusicPlayer
    {
        private readonly string _path;

        public MusicPlayer(string path)
        {
            _path = path;
        }

        public bool IsAvailable => File.Exists(_path);

        public void Play(string musicPath)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = _path,
                    Arguments = string.Format("/play \"{0}\"", musicPath),
                    WorkingDirectory = Path.GetDirectoryName(_path) ?? "."
                }
            };
            process.Start();
        }
    }
}
