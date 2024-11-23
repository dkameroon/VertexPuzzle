using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectCollection", menuName = "Audio/SoundEffectCollection", order = 1)]
public class SoundEffectCollection : ScriptableObject
{
    [System.Serializable]
    public class SoundEntry
    {
        public string soundName;
        public AudioClip audioClip;
    }

    [Header("Sound Effects")]
    public List<SoundEntry> sounds = new List<SoundEntry>();
    
    public AudioClip GetSound(string name)
    {
        SoundEntry entry = sounds.Find(s => s.soundName == name);
        return entry != null ? entry.audioClip : null;
    }
}