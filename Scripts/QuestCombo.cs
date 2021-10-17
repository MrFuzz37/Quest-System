using System.Collections.Generic;
using UnityEngine;

public class QuestCombo : Quest
{
    [System.Serializable]
    public struct Combos
    {
        public List<Action> actions;
        public int performed;
    }

    public Combos[] combos;

    private int index = 0;
    private int counter = 0;
    private int currentCount = 0;
    private string[] actionNames = new string[3];
    float timer = 0;
    float timer2 = 0;

    // Start is called before the first frame update
    void Start()
    {
        ComboManager.onComboUpdate.AddListener(ComboCheck);
        combos[current].performed = 0;
        counter = 1;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        timer2 += Time.deltaTime;
    }

    public void ComboCheck(List<Action> combo)
    {
        // if the quest is completed return
        if (current == value) { return; }

        // so multiple hit attacks do not trigger this again
        if (timer2 < 0.25f) { return; }

        if (combo.Count == counter)
        {
            if (combo[counter - 1].name == combos[current].actions[counter - 1].name)
            {
                counter++;
                currentCount = combo.Count;
                timer2 = 0;
            }
        }
        else if (currentCount != combo.Count) { counter = 1; }

        // timer ensures if the attack hits multiple enemies at once we only trigger the quest once
        if (counter == 4 && timer > 1) 
        {
            counter = 0;
            foreach (Action a in combo)
            {
                if (a.name != combos[current].actions[counter].name) { return; }
                counter++;
            }
            combos[current].performed++;
            timer = 0;
            counter = 1;
        }

        // when the combo has been performed enough times, increment the quest objective
        if (combos[current].performed >= 3) { Increment(1); }
        onQuestUpdate.Invoke();
    }

    public override string GetDescription()
    {
        // if the quest is ready to turn in, display a different overlay text
        if (current == value) { Complete(this); return ""; }

        index = 0;
        foreach (Action a in combos[current].actions)
        {
            if (a.id == 0) { actionNames[index] = "Light"; }
            else if (a.id == 2) { actionNames[index] = "Heavy"; }
            else { actionNames[index] = "Air Heavy"; }

            index++;
        }

        index = 0;

        return "Perform the following combo: " + actionNames[index] + ", " + actionNames[index + 1] + ", " + actionNames[index + 2] + " " + combos[current].performed + "/" + "3";
    }
}
