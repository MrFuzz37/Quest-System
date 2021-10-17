
public class QuestNarrative : Quest
{
    public string objectiveText; // display the current objective
    public int dialogueNumber = 0; // how many dialogue boxes to display before the quest ends

    // Start is called before the first frame update
    void Start()
    {
        // add listener here for an onGoal event
        QuestGiver.onNarrative.AddListener(NarrativeFinished);
    }

    public void NarrativeFinished(int count)
    {
        if (count == dialogueNumber)
            Complete(this);
    }

    public override string GetDescription()
    {
        // display a different message when the quest goal has been achieved
        return objectiveText;
    }
}
