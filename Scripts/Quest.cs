using UnityEngine;
using UnityEngine.Events;

public abstract class Quest : MonoBehaviour
{
    public string questName;

    public enum State
    {
        Unavailable,
        Available,
        Active,
        Completed
    }

    [Multiline]
    public string[] introText;
    [Multiline]
    public string[] inProgressText;
    [Multiline]
    public string[] completedText;

    public int currentText = 0;
    public static bool inDialogue = false;

    public State state;
    public int value; // how many are needed to complete the quest
    public int current; // how far through the quest are we
    public float GetPercentage() { return ((float)current) / value; } //returns betwen 0 and 1

    public Quest[] prerequisites; // what needs to be completed first before this quest
    public bool isProgressive = false; // whether this quest is part of a progressive quest
    public bool isTrigger = false; // for quests that activate at specific times

    // general purpose events for when any quest has changed
    public static UnityEvent onQuestUpdate = new UnityEvent();

    [System.Serializable]
    public class QuestComplete : UnityEvent<Quest> { }
    public static QuestComplete onQuestComplete = new QuestComplete();

    [System.Serializable]
    public class QuestStart : UnityEvent<Quest> { }
    public static QuestStart onQuestStart = new QuestStart();

    public abstract string GetDescription();

    // modify the current value
    protected void Increment(int delta = 1)
    {
        current += delta;
        if (current > value) { current = value; }

        // update quest information
        onQuestUpdate.Invoke();
    }

    // called when a quest is turned in to the quest giver
    public void Complete(Quest q)
    {
        state = State.Completed;
        onQuestComplete.Invoke(q);

        if (q.questName == "The Pit")
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Blockade");
            foreach (GameObject g in walls)
            {
                Destroy(g);
            }
        }
    }

    // is the quest ready to turn in with all conditions met?
    public bool ReadyToTurnIn()
    {
        return state == State.Active && GetPercentage() >= 1.0f;
    }
}
