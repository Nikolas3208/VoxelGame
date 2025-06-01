using SFML.Audio;

namespace VoxelGame.Resources
{
    public static class AudioManager
    {
        private static Dictionary<string, SoundBuffer> soundBuffers = new Dictionary<string, SoundBuffer>();
        private static Dictionary<string, Sound> activeSounds = new Dictionary<string, Sound>();

        public static string BasePath { get; set; } = "Assets/Audios";


        /// <summary>
        /// Загрузить все аудио файлы
        /// </summary>
        public static void LoadAll()
        {
            var files = Directory.GetFiles(BasePath, "*.wav", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);

                LoadSound(name, file);
            }

            files = Directory.GetFiles(BasePath, "*.ogg", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);

                LoadSound(name, file);
            }
        }

        /// <summary>
        /// Загрузить аудио файл
        /// </summary>
        /// <param name="name"> Имя файла </param>
        /// <param name="filePath"> Путь с именем </param>
        public static void LoadSound(string name, string filePath)
        {
            if (!soundBuffers.ContainsKey(name))
            {
                var soundBuffer = new SoundBuffer(filePath);
                soundBuffers.Add(name, soundBuffer);
            }
        }

        /// <summary>
        /// Играть звук
        /// </summary>
        /// <param name="name"> Имя </param>
        /// <param name="volume"> Громкость </param>
        /// <param name="loop"> Повторять? </param>
        /// <exception cref="KeyNotFoundException"> Если в скписке нет звука с таким именем </exception>
        public static void PlaySuond(string name, float volume = 100f, bool loop = false)
        {
            if (soundBuffers.TryGetValue(name, out var soundBuffer))
            {
                if (!activeSounds.ContainsKey(name))
                {
                    var sound = new Sound(soundBuffer)
                    {
                        Volume = volume,
                        Loop = loop,
                    };

                    sound.Play();
                    activeSounds.Add(name, sound);
                }
            }
        }

        /// <summary>
        /// Остановить звук
        /// </summary>
        /// <param name="name"> Имя </param>
        public static void StopSound(string name)
        {
            if (activeSounds.TryGetValue(name, out var sound))
            {
                sound.Stop();
                activeSounds.Remove(name, out sound);
            }
        }

        /// <summary>
        /// Установить громкость звука
        /// </summary>
        /// <param name="name"> Имя </param>
        /// <param name="volume"> Громкость </param>
        public static void SetVolumeSound(string name, float volume)
        {
            if (activeSounds.TryGetValue(name, out var sound))
            {
                sound.Volume = volume;
            }
        }


        /// <summary>
        /// Получить громкость звука
        /// </summary>
        /// <param name="name"> Имя </param>
        /// <returns></returns>
        public static float GetVolumeSound(string name)
        {
            return activeSounds.TryGetValue(name, out var sound) ? sound.Volume : 0;
        }

        /// <summary>
        /// Установить повторение звука
        /// </summary>
        /// <param name="name"> Имя </param>
        /// <param name="loop"> Повторение (true or false) </param>
        public static void SetLoopSound(string name, bool loop)
        {
            if (activeSounds.TryGetValue(name, out var sound))
            {
                sound.Loop = loop;
            }
        }

        /// <summary>
        /// Получить поворение звука
        /// </summary>
        /// <param name="name"> Имя </param>
        /// <returns></returns>
        public static bool GetLoopSound(string name)
        {
            return activeSounds.TryGetValue(name, out var sound) ? sound.Loop : false;
        }

        /// <summary>
        /// Удалить из активных звуков все которые доиграли до конца
        /// </summary>
        public static void Cleanup()
        {
            var stopedSound = activeSounds.Where(s => s.Value.Status == SoundStatus.Stopped).ToDictionary();

            foreach (var s in stopedSound)
            {
                activeSounds.Remove(s.Key);
            }
        }

        /// <summary>
        /// Уничтожить ресурсы аудио менеджера
        /// </summary>
        public static void Dispose()
        {
            //Уничтожить все активные звуки
            foreach (var sound in activeSounds.Values)
            {
                sound.Dispose();
            }

            //Очистить списки активных звуков
            activeSounds.Clear();

            //Уничтожить все звуковые буферы
            foreach (var buffer in soundBuffers.Values)
            {
                buffer.Dispose();
            }

            //Очистить списки звуковых буферов
            soundBuffers.Clear();
        }

        /// <summary>
        /// Остановить все звуки
        /// </summary>
        public static void StopAllSound()
        {
            // Остановить все активные звуки
            foreach (var sound in activeSounds.Values)
            {
                sound.Stop();
            }
            // Очистить список активных звуков
            activeSounds.Clear();
        }
    }
}
