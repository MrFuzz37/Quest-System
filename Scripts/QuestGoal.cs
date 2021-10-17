using UnityEngine;

public class QuestGoal : Quest
{
    public string objectiveText; // display the current objective
    public string objectiveReached; // when the goal has been reached but not completed
    public GameObject objectGoal; // the object that acts as the quest objective

    // Start is called before the first frame update
    void Start()
    {
        // add listener here for an onGoal event
        DialogueTrigger.onGoal.AddListener(GoalReached);
    }

    public void GoalReached(GameObject goal)
    {
        if (state == State.Active && objectGoal.name + "(Clone)" == goal.name)
            Increment(1);
        else if (state == State.Active && objectGoal.name == goal.name)
            Increment(1);
    }

    public override string GetDescription()
    {
        // display a different message when the quest goal has been achieved
        if (current == value) { return objectiveReached; }
        else { return objectiveText; }
    }
}
