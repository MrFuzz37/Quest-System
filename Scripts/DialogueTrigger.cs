using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue : UnityEvent<string> { }

    public static Dialogue onTrigger = new Dialogue();
    public static QuestManager.QuestGoal onGoal = new QuestManager.QuestGoal();
    public static QuestManager.QuestGoal onReset = new QuestManager.QuestGoal();

    [Multiline]
    public string dialogue; // text to be displayed for once off text (only used in tutorial scene)
    public GameObject guide;
    public Vector3 position;
    private bool isActivated = false;
    public GameObject buttonSprite;
    public bool isBoss = false;
    public float time = 10;
    private float timer = 0;
    public GameObject boss;
    public GameObject player;

    public void Update()
    {
        if (isBoss && isActivated) { timer += Time.deltaTime; }
        if (timer > time) 
        { 
            boss.GetComponent<Animator>().SetBool("Sleep", true);
            player.GetComponent<PlayerInput>().actions.Enable();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // special boss trigger
        if (isBoss && !isActivated) { BossStart(); return; }

        // if a player walks into the trigger
        if (!isActivated && collision.CompareTag("Player"))
        {
            if (GetComponent<Goal>())
            {
                // if it's the goal objective, invoke the event
                onGoal.Invoke(gameObject);
            }
            if (guide.transform.position != position)
            {
                // teleport guide if he isn't already there
                guide.GetComponent<Animator>().Play("Teleport");
                AudioManager.instance.Play("Guide Teleport");
                guide.transform.position = position;
            }
            if (buttonSprite != null)
            {
                buttonSprite.SetActive(true);
            }

            // invoke the dialogue popup
            onTrigger.Invoke(dialogue);
            isActivated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isBoss && isActivated) { Destroy(gameObject); }

        if (collision.CompareTag("Player"))
        {
            if (name == "Start Trigger")
            {
                gameObject.SetActive(false);
            }

            if (QuestLog.instance)
                if (QuestLog.instance.activeQuest)
                    if (QuestLog.instance.activeQuest.questName == "To the Pub!")
                    {
                       Destroy(gameObject);
                    }

            if (GetComponent<Goal>())
            {
                onReset.Invoke(gameObject);
            }
            if (isActivated)
            {
                // make it all disappear
                onTrigger.Invoke(null);
                isActivated = false;
                if (buttonSprite != null)
                {
                    buttonSprite.SetActive(false);
                }
            }
        }
    }

    public void BossStart()
    {
        // specific boss event stuff
        player.GetComponent<PlayerInput>().actions.Disable();
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (buttonSprite != null)
        {
            buttonSprite.SetActive(true);
        }

        onTrigger.Invoke(dialogue);
        isActivated = true;

        AudioManager.instance.Play("321_Fight");
        AudioManager.instance.Play("321_Fight_Reverb");
    }
}
