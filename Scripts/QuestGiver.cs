using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    List<Quest> quests = new List<Quest>();
    GameObject oButton;
    // objects to dispaly alerts above their head when they're ready to give or hand in a quest
    public GameObject giveAlert;
    public GameObject completeAlert;

    public static QuestManager.QuestDialogue onQuestDialogue = new QuestManager.QuestDialogue();
    public static QuestManager.QuestNarrative onNarrative = new QuestManager.QuestNarrative();

    enum Alert
    {
        None,
        Give,
        TurnIn
    }
    Alert alert;

    public float interactRange = 1;
    public GameObject player;

    public bool inRange = false;
    private Quest currentObj;
    private bool firstQuestStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        Player.onInteract.AddListener(Interact);
        Player.onAccept.AddListener(UpdateDialogue);
        // make sure we update our alert every time a quest updates
        Quest.onQuestUpdate.AddListener(CheckAlerts);
        // and we update our quests every time one is compeleted
        Quest.onQuestComplete.AddListener(CheckQuests);
        StartCoroutine(StartUp());
    }

    IEnumerator StartUp()
    {
        // perform start up actions
        player.GetComponent<PlayerInput>().actions.Disable();
        foreach (Quest q in GetComponents<Quest>())
        {
            quests.Add(q);
            if (q.isProgressive)
            {
                foreach (Quest qu in q.GetComponent<QuestProgressive>().objectives)
                {
                    if (quests.Contains(qu)) { quests.Remove(qu); }
                }
            }
        }
        yield return new WaitForSeconds(1);
        // play a particle for the guide teleporting in
        GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<PlayerInput>().actions.Enable();
        if (SceneManager.GetActiveScene().name == "MovementTutorialMaster")
        {
            player.GetComponent<PlayerInput>().actions.FindAction("Heavy").Disable();
            player.GetComponent<PlayerInput>().actions.FindAction("Light").Disable();
            player.GetComponent<PlayerInput>().actions.FindAction("FormChangeLeft").Disable();
            player.GetComponent<PlayerInput>().actions.FindAction("FormChangeRight").Disable();
        }

        CheckQuests(null);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < interactRange)
        {
            // show button icon
            inRange = true;
        }
        else { inRange = false; }

        if (GetComponent<QuestProgressive>())
            if (GetComponent<QuestProgressive>().state == Quest.State.Active) { currentObj = GetComponent<QuestProgressive>().GetCurrentObjective(); }
    }

    public void Interact()
    {
        if (inRange)
        {
            if (!firstQuestStarted && SceneManager.GetActiveScene().name == "MovementTutorialMaster")
            {
                //Text text = GameObject.Find("Dialogue Popup(Clone)").GetComponent<Text>();
                //if (text)
                //text.text = "";
                oButton = GameObject.Find("OButton");
                if (oButton)
                    oButton.SetActive(false);

                firstQuestStarted = true;
            }

            // any completed quests
            foreach (Quest q in quests)
            {
                QuestLog.instance.activeQuest = q;
                if (currentObj != null)
                    if (currentObj.ReadyToTurnIn()) { q.GetComponent<QuestProgressive>().StageCompleted(completeAlert); return; }

                if (q.ReadyToTurnIn())
                {
                    AudioManager.instance.Play("Interact");

                    // remove the alert
                    completeAlert.SetActive(false);

                    // invoke completed text
                    if (q.completedText.Length < 1) 
                    { 
                        onQuestDialogue.Invoke(null); 
                        q.Complete(q);
                        QuestLog.instance.quests.Remove(q);
                        QuestLog.instance.activeQuest = null;
                    }
                    else { onQuestDialogue.Invoke(q.completedText[q.currentText]); }
                    q.currentText++;
                    return;
                }
            }

            foreach (Quest q in quests)
            {
                if (q.state == Quest.State.Available) 
                {
                    AudioManager.instance.Play("Interact");

                    // remove the alert
                    giveAlert.SetActive(false);

                    // add to the quest log if available
                    QuestLog.instance.quests.Add(q);

                    // Invoke event to run the intro text before setting it to the active quest
                    onQuestDialogue.Invoke(q.introText[q.currentText]);
                    q.currentText++;
                    

                    // new quests become the active
                    QuestLog.instance.activeQuest = q;

                    return;
                }
                if (currentObj != null)
                {
                    // specific to progressive quests
                    if (currentObj.state == Quest.State.Available)
                    {
                        AudioManager.instance.Play("Interact");

                        giveAlert.SetActive(false);
                        QuestLog.instance.activeQuest = currentObj;
                        onQuestDialogue.Invoke(currentObj.introText[currentObj.currentText]);
                        currentObj.currentText++;

                        return;
                    }
                }
            }
        }
    }

    public void UpdateDialogue(Quest q)
    {
        // if we aren't in a dialogue scene return
        if (!Quest.inDialogue) { return; }

        // if the quest isn't active yet, we are still in the intro text
        if (q.state == Quest.State.Available)
        {
            DialogueEnded(q, q.introText);
            return;
        }

        // if the quest is already active
        if (q.state == Quest.State.Active)
        {
            if (q.isProgressive) 
            {
                // if a progressive quest, perform specific functions
                if (currentObj.state == Quest.State.Available) { DialogueEnded(currentObj, currentObj.introText); }
                else if (currentObj.ReadyToTurnIn())
                {
                    if (DialogueEnded(currentObj, currentObj.completedText))
                    {
                        currentObj.Complete(currentObj);
                        q.GetComponent<QuestProgressive>().Increase();
                        QuestLog.instance.quests.Remove(currentObj);
                        q.GetComponent<QuestProgressive>().index++;
                    }
                }
                else if (currentObj.state == Quest.State.Active) { DialogueEnded(currentObj, currentObj.inProgressText); }

            }
            // if ready to turn in, display completed text
            if (q.ReadyToTurnIn())
            {
                if (DialogueEnded(q, q.completedText))
                {
                    // when diagloue ends complete the quest and remove it from the quest log
                    q.Complete(q);
                    QuestLog.instance.quests.Remove(q);
                    QuestLog.instance.activeQuest = null;
                }
            }
            else
            {
                if (!q.isProgressive) { DialogueEnded(q, q.inProgressText); }
            }
        }
    }

    // can be used to trigger events after each dialogue section has ended
    public bool DialogueEnded(Quest q, string[] text)
    {
        if (q.currentText >= text.Length)
        {
            // if outside of array, end the conversation as there is no more dialogue to display
            onQuestDialogue.Invoke(null);
            onNarrative.Invoke(q.currentText);
            q.currentText = 0;

            // finally activate the quest after intro text is over
            q.state = Quest.State.Active;
            if (q.isProgressive) { q.GetComponent<QuestProgressive>().Activate(); }
            Quest.onQuestUpdate.Invoke();
            return true;
        }

        // continue with intro text if there is more to display
        onQuestDialogue.Invoke(text[q.currentText]);
        onNarrative.Invoke(q.currentText);
        q.currentText++;
        return false;
    }

    // check all hidden quests, and see if any are ready to be made available now
    void CheckQuests(Quest quest)
    {
        // check whether the questgiver should alert the player to come and chat
        alert = Alert.None;

        foreach (Quest q in quests)
        {
            if (!q.isTrigger) 
            {
                if (q.isProgressive) { q.GetComponent<QuestProgressive>().CheckQuests(); }

                if (q.state == Quest.State.Unavailable)
                {
                    bool available = true;
                    foreach (Quest pre in q.prerequisites)
                        if (pre.state != Quest.State.Completed)
                            available = false;
                    if (available)
                    {
                        // a new quest is available, alert the player and make it ready to give
                        q.state = Quest.State.Available;
                    }
                }
            }
        }
        CheckAlerts();
    }

    // check the state of all quests to see what our alert status should be - ready to give or turn in
    void CheckAlerts()
    {
        // check whether the questgiver should alert the player to come and chat
        alert = Alert.None;

        foreach (Quest q in quests)
        {
            if (q.isProgressive)
            {
                foreach (Quest qu in q.GetComponent<QuestProgressive>().objectives)
                {
                    if (qu.ReadyToTurnIn() && alert < Alert.TurnIn)
                        alert = Alert.TurnIn;
                    if (qu.state == Quest.State.Available && alert < Alert.Give)
                        alert = Alert.Give;
                }
            }
            if (q.ReadyToTurnIn() && alert < Alert.TurnIn)
                alert = Alert.TurnIn;
            if (q.state == Quest.State.Available && alert < Alert.Give)
                alert = Alert.Give;
        }

        // turn the alert objects on or off
        giveAlert.SetActive(alert == Alert.Give);
        completeAlert.SetActive(alert == Alert.TurnIn);
    }
}
