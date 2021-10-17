using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public Text text; // overlay quest objective text
    public Quest quest; // the quest it's tied to

    // make this object display the details of the specified quest
    public void SetQuest(Quest q)
    {
        quest = q;

        if (!quest)
        {
            // remove the quest overlay
            Destroy(gameObject);
            return;
        }

        // show its description
        if (text)
            text.text = quest.GetDescription();
    }
}
