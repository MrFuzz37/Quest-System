using UnityEngine;

// An objective for quests
public class Objective : MonoBehaviour
{
    public bool inRange;
    public float interactRange = 1;
    GameObject player;
    public GameObject guide;
    public GameObject button;
    public Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Player.onInteract.AddListener(Interact);
        guide = GameObject.Find("Guide").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < interactRange)
        {
            // show button icon
            button.GetComponent<SpriteRenderer>().enabled = true;
            inRange = true;
        }
        else 
        { 
            inRange = false;
            button.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void Interact()
    {
        if (inRange) 
        {
            // remove the blockage so player can progress
            if (name == "Objective_2(Clone)") 
            { 
                Destroy(GameObject.Find("Blockade_3")); 
                Destroy(GameObject.Find("Trigger"));
            }

            if (name == "Objective_3")
            {
                Destroy(GameObject.Find("Blockade_4"));
                Destroy(GameObject.Find("Trigger_2"));
            }

            button.GetComponent<SpriteRenderer>().enabled = false;
            // add sfx here - LUKEY        // sounds added
            AudioManager.instance.Play("Interact");
            DialogueTrigger.onGoal.Invoke(gameObject);

            // add particles for teleporting
            guide.GetComponent<Animator>().Play("Teleport");
            AudioManager.instance.Play("Guide Teleport");
            guide.transform.position = pos;
            guide.GetComponent<QuestGiver>().Interact();
            Destroy(gameObject);
        }
    }
}
