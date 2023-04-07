using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * La classe DialogManager permet de centraliser les fonctionnalités liés à l'aspect conversationnel de l'agent. 
 * C'est notamment ici qu'un Chatbot est chargé et est géré. Le DialogManager met à jour l'interface de conversation
 * suivant l'état du dialogue du Chatbot.
 */
public class DialogManager : MonoBehaviour
{
    
    public AudioSource audioSource;
    
    public float volume = 0.5f;
    
    private Chatbot dialog;

    public Transform informationPanel;
    public Transform textPanel;
    public Transform buttonPanel;
    public GameObject ButtonPrefab;
    public FacialExpression faceExpression;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        //dialog = Chatbot.readDialogueFile<ExampleBot>("Assets/DialogElements/Dialogue/json/dialogue0.json", this);
        //dialog = Chatbot.readDialogueFile<Swahili>("Assets/DialogElements/Dialogue/json/swahili.json", this);
        dialog = Chatbot.readDialogueFile<TeacherBot>("Assets/DialogElements/Dialogue/json/teacher_final.json", this);
        InformationDisplay("");
        dialog.nextDialogue();

    }

    // Update is called once per frame
    void Update()
    {

    }
    /*
     * Cette méthode permet de jouer un fichier audio depuis le répertoire Resources/Sounds dont le nom est de la forme <entier>.mp3 
     */
    public void PlayAudio(int a)
    {
        /*
        try
        {
            //Charge un fichier audio depuis le répertoire Resources
            AudioClip music = (AudioClip)Resources.Load("Sounds/"+a);
            audioSource.PlayOneShot(music, volume);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        */
    }

    /*
     * Cette méthode affiche du texte dans le panneau d'affichage à gauche de l'UI
     */
    public void InformationDisplay(string s)
    {

        Text text = informationPanel.transform.GetComponentInChildren<Text>().GetComponent<Text>();
        text.text = s;

    }
    /*
     * Cette méthode affiche le texte de la question dans la partie basse de l'UI
     */
    public void DisplayQuestion(string s)
    {
        Text text = textPanel.transform.GetComponentInChildren<Text>().GetComponent<Text>();
        text.text = s;
    }

    /* 
     * Cette méthode affiche les réponses sous forme de boutons dans l'UI.
     */
    public void DisplayAnswers(List<string> proposals)
    {


        if (proposals.Count == 0)
        {
            Debug.Log("** Il y a une erreur dans votre code: la liste de proposition est vide. Ou alors c'est la fin du dialogue?");
        }


        int i = 0;
        //On retire tout d'abord tous les boutons de l'interface
        foreach (Button child in buttonPanel.transform.GetComponentsInChildren<Button>())
        {
            Destroy(child.gameObject);
        }
        //Pour chaque valeur, on rajoute un bouton, et on lui associe la fonction responseSelected pour quand le bouton est cliqué
        for (int j = 0; j < proposals.Count; j++)
        {
            GameObject button = (GameObject)Instantiate(ButtonPrefab);
            button.GetComponentInChildren<Text>().text = proposals[j];
            int temp = j;
            button.GetComponent<Button>().onClick.AddListener(delegate { responseSelected(temp); });
            button.GetComponent<RectTransform>().position = new Vector3(i * 170.0f + 90.0f, 39.0f, 0.0f);
            button.transform.SetParent(buttonPanel);
            
            i = i + 1;
        }


    }

    public void EndDialog()
    {
        foreach (Button child in buttonPanel.transform.GetComponentsInChildren<Button>())
        {
            Destroy(child.gameObject);
        }
        anim.SetTrigger("Greet");
    }

    //Quand une réponse est choisie, on appelle la méthode du Chatbot qui gère la réponse et ensuite, si le dialogue n'est pas 
    //en train de réaliser une action spéciale, on avance dans la question suivante.
    public void responseSelected(int response)
    {
        dialog.handleResponse(response);
        dialog.nextDialogue();
    }
    /*
     * Cette méthode permet de faire jouer des AUs à l'agent
     */
    public void DisplayAUs(int[] aus, int[] intensities, float duration)
    {
        faceExpression.setFacialAUs(aus, intensities, duration);
    }
}
