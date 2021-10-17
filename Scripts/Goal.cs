using UnityEngine;

// For having object goals that the player must reach
public class Goal : MonoBehaviour
{
    public GameObject player;
    public GameObject guide;

    private void Start()
    {
        DialogueTrigger.onReset.AddListener(UpdateGoal);
    }

    public void UpdateGoal(GameObject go)
    {
        Quest.onQuestUpdate.Invoke();
    }
}
