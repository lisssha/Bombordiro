using UnityEngine;

[CreateAssetMenu(fileName = "NewEvolutionData", menuName = "Evolution Data")]
public class EvolutionData : ScriptableObject
{
    [System.Serializable]
    public class EvolutionStage
    {
        public GameObject prefab;      // Префаб животного (акула, крокодил...)
        public AudioClip mergeSound;  // Звук при слиянии
        public GameObject nextStage;   // Во что эволюционирует
    }

    public EvolutionStage[] stages;    // Все ступени эволюции

    public EvolutionStage GetStageForPrefab(GameObject obj)
    {
        string originalName = obj.name.Replace("(Clone)", "").Trim();
        return System.Array.Find(stages, s => s.prefab.name == originalName);
    }
}