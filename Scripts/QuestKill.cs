using System.Collections.Generic;

// simple kill quest 
public class QuestKill : Quest
{
    public List<string> enemyName;

    // Start is called before the first frame update
    void Start()
    {
        Character.onKilled.AddListener(Kill);
    }

    public void Kill(string enemy)
    {
        // if we kill the right enemy for this quest, increase the count
        if (enemyName.Count > 1)
        {
            if (state == State.Active && enemy == enemyName[0])
            {
                if (enemy == "Cheech") { enemyName.Remove(enemy); }
                Increment(1);
            }
            else if (state == State.Active && enemy == enemyName[1])
            {
                if (enemy == "Chong") { enemyName.Remove(enemy); }
                Increment(1);
            }
        }
        else
        {
            if (state == State.Active && enemy == enemyName[0])
                Increment(1);
        }
    }

    public override string GetDescription()
    {
        // if the quest is ready to turn in, display a different overlay text
        if (current == value)
            return "Return to the guide";

        if (value == 1) { return "Kill " + enemyName[0]; }

        if (value == 2) 
        {
            if (current == 1) { return "Kill " + enemyName[0]; }
            else { return "Kill " + enemyName[0] + "\nKill " + enemyName[1]; }
        }
        // otherwise display the progress of the quest
        return "Enemies killed " + current + "/" + value;
    }
}
