using UnityEngine;

// Allows a quest to hold multiple objectives, so quests can have a sort of progression
public class QuestProgressive : Quest
{
    public Quest[] objectives; // Can have multiple quests before finishing these types of quests

    public int index; // where we are in the quest objectives

    public override string GetDescription()
    {
        return objectives[index].GetDescription();
    }

    public void StageCompleted(GameObject alert)
    {
        // remove the alert
        alert.SetActive(false);

        // invoke completed text
        if (objectives[index].completedText.Length < 1)
        {
            QuestGiver.onQuestDialogue.Invoke(null);
            objectives[index].Complete(objectives[index]);
            index++;
            Increment(1);
        }
        else { QuestGiver.onQuestDialogue.Invoke(objectives[index].completedText[currentText]); }
        objectives[index].currentText++;
    }

    public Quest GetCurrentObjective()
    {
        return objectives[index];
    }

    public void Increase()
    {
        Increment(1);
    }

    // check all hidden quests, and see if any are ready to be made available now
    public void CheckQuests()
    {
        foreach (Quest q in objectives)
        {
            if (q.state == State.Unavailable)
            {
                bool available = true;
                foreach (Quest pre in q.prerequisites)
                    if (pre.state != State.Completed)
                        available = false;
                if (available)
                {
                    // a new quest is available, alert the player and make it ready to give
                    q.state = State.Available;
                }
            }
        }
    }

    public void Activate()
    {
        if (objectives[index].state == State.Available) { objectives[index].state = State.Active; }
    }
}
