using UnityEngine;

[System.Serializable]
public class DifficultySettings
{
    public string name = "Normal";
    [UnityEngine.Tooltip("Sekunden pro Schritt. Kleiner = schneller.")]
    public float moveInterval = 0.08f;
    
}
