using UnityEngine;

[CreateAssetMenu(fileName = "NewEvolutionData", menuName = "Evolution Data")]
public class EvolutionData : ScriptableObject
{
    [System.Serializable]
    public class EvolutionStage
    {
        public GameObject prefab;
        public AudioClip mergeSound;
        public GameObject nextStage;
    }

    public EvolutionStage[] stages;

    public EvolutionStage GetStageForPrefab(GameObject obj)
    {
        string originalName = obj.name.Replace("(Clone)", "").Trim();
        return System.Array.Find(stages, s => s.prefab.name == originalName);
    }
}