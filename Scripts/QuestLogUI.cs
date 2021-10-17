using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLogUI : MonoBehaviour {

    // the backend quest log we're displaying
    public QuestLog questLog;

    // spawn one of these for each quest we display
    public GameObject prefab;

    // stores which UI prefab instance is representing each quest in the quest log
    Dictionary<Quest, QuestUI> quests = new Dictionary<Quest, QuestUI>();

	// Use this for initialization
	void Start () {
        UpdateQuest();
        Quest.onQuestUpdate.AddListener(UpdateQuest);
        Quest.onQuestComplete.AddListener(RemoveQuest);
    }
	
	// Update is called once per frame
	void UpdateQuest() 
    {
        if (questLog)
            foreach (Quest q in questLog.quests)
            {

                if (quests.ContainsKey(q) == false)
                {
                    // if we don't already have a quest, instantiate a UI prefab for the overlay
                    GameObject go = Instantiate(prefab, transform);
                    quests[q] = go.GetComponent<QuestUI>();
                    Quest.onQuestStart.Invoke(q);
                }

                quests[q].SetQuest(q);
                if (quests.ContainsKey(q)) { quests[q].GetComponentInChildren<Text>().text = q.questName; }
                else { return; }
            }
	}

    void RemoveQuest(Quest q)
    {
        // remove quest ui objects so we can instantiate them again and re order them
        QuestUI[] uiObjects = GetComponentsInChildren<QuestUI>();
        if (uiObjects.Length > 0)
        {
            foreach (QuestUI g in uiObjects)
            {
                quests.Remove(g.quest);
                Destroy(g.gameObject);
            }
        }
        // when the quest is completed or failed remove it
        if (quests.ContainsKey(q)) { quests[q].SetQuest(null); }
        
        QuestLog.instance.quests.Remove(q);
        UpdateQuest();
    }
}
