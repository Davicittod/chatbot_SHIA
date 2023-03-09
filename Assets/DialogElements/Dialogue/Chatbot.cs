using System;                           // pour les entrées-sorties (Console)
using System.Collections.Generic;       // pour les List<T>
using System.IO;                        // pour la lecture et l'écriture de fichiers
using Newtonsoft.Json;                 // pour la manipulation de fichiers JSON

/*  La classe abstraite "Chatbot" est le support pour définir un chatbot.

Pour programme un chatbot, il faut écrire une sous-classe de la classe Chatbot.

1) Dans la classe DialogManager, on appelle "readDialogueFile" pour lire le fichier
JSON contenant les données du dialogue puis appeler la méthode "nextDialogue".
Le type de sortie de "readDialogueFile" est la sous-classe que vous
implémentez. Cela donne :

class Truc : Chatbot { // Truc est une sous-classe de Chatbot
   
}

2) Le fichier JSON ("mon_dialogue.json" situé dans le répertoire "data" dans
l'exempe ci-dessus) contient des questions et des réponses, définies par des numéros.
Vous devez alors implémenter 5 méthodes dans le chatbot :
- beforeSpeak qui est appelée au début du tour de dialogue, avant que l'agent parle
  (vous pouvez décider de laisser cette méthode vide).
- getCurrentQuestion qui est appelée par le moteur de dialogue au début du tour
  pour savoir quoi dire. Cette méthode doit renvoyer un numéro de question.
- getPossibleAnswers qui donne la liste des réponses possibles suite à la question
  de l'agent. Elle renvoie une liste de numéros de réponses associées à la question.
- triggerAffectiveReaction qui sera utilisée, plus tard, pour faire jouer des réactions
  émotionnelles à l'agent.
- afterAnswer qui est appelée à la fin du tour (vous pouvez décider de laisser cette
  méthode vide).
  NB: fondamentalement, rien ne distingue triggerAffectiveReaction de afterAnswer (les
  deux méthodes ont la même signature et sont appelées exactemet l'une après l'autre)
  mais utiliser deux méthode différentes permet de mieux séparer les étapes du dialogue.

Pour redéfinir ces méthodes dans votre classe, vous utilisez override. Par exemple :

    public override void beforeSpeak() {
        ... votre code ...
    }

Vous pouvez définir, dans le constructeur de votre classe, les variables
que vous souhaitez utiliser pour gérer le dialogue, calculer des réponses
affectives, etc. Toutefois, ce constructeur ne peut pas prendre de paramètres.

Enfin, vous pouvez utiliser trois méthodes :
- dans la méthode triggerAffectiveReaction, la méthode "playAnimation(int[] aus, int length)"
  qui permet d'activer l'animation d'un ensemble d'AUs pour une durée donnée.
- à tout instant dans votre code, la méthode "stopDialogue()" pour arrêter le dialogue à la
  fin du tour actuel.
- à tout instant dans votre code, la méthode "getPhraseFromNumber(List<Phrase> lp, int n)" qui vous permet de
  retrouver a phrase correspondant au numéro donné dans votre fichier JSON. Elle s'utilise
  de la manière suivante :
        getPhraseFromNumber(questions OU answers (sans guillemets), numéro)
*/
public abstract class Chatbot
{

    /*************************/
    /* Méthodes à instancier */
    /*************************/

    /* Cette méthode est appelée au début du tour de dialogue.
       Elle reçoit en argument la question précédente (ou null au premier tour)
       L'implémentation par défaut ne fait rien */
    public virtual void beforeSpeak(int lastQuestion)
    {
        // rien
    }
    /* Cette méthode est appelée immédiatement après beforeSpeak pour déterminer
       la question que l'agent doit poser. Elle doit être obligatoirement
       réimplémentée : un exemple est fourni dans le fichier example.cs */
    public abstract int getCurrentQuestion();
    /* Cette méthode est appelée immédiatement après getCurrentQuestion
       pour déterminer les réponses possibles de l'agent. Elle reçoit
       en argument le numéto de la question (le résultat de getCurrentQuestion) et
       doit renvoyer les numéros des réponses possibles.
       L'implémentation par défaut va chercher les réponses possibles dans le champ
       "autoanswer" du fichier JSON, mais cette méthode peut-être réimplémentée
       si besoin. */
    public virtual int[] getPossibleAnswers(int question)
    {
        foreach (AutoAnswer aa in autoanswers)
            if (aa.q == question)
                return aa.a.ToArray();
        return new int[] { };
    }
    /* Cette méthode est appelée immédiatement après le choix de réponse
       de l'utilisateur pour déclencher la réaction affective de l'agent.
       Elle reçoit en argument le numéro de réponse choisie par l'utilisateur.
       L'implémentation par défaut ne fait rien */
    public virtual void triggerAffectiveReaction(int lastQuestion, int lastAnswer)
    {
        // par défaut, l'agent n'est pas expressif
    }
    /* Cette méthode est appelée à la fin du tour de dialogue.
       Elle reçoit en argument le numéro de réponse choisie par
       l'utilisateur.
       L'implémentation par défaut ne fait rien. */
    public virtual void afterAnswer(int lastQuestion, int lastAnswer)
    {
        // rien
    }

    /************************************************/
    /* Structure C# du dialogue, alimentée via JSON */
    /* Vous n'avez pas besoin de lire cette partie  */
    /************************************************/

    /* La classe pour les phrases : un numéro associé à un texte */
    public class Phrase
    {
        public int num { get; set; }
        public string text { get; set; } = ""; // pour supprimer les warning nullable, on initialise les variables
    }

    /* La classe pour les réponses codées dans le JSON */
    public class AutoAnswer
    {
        public int q { get; set; }
        public List<int> a { get; set; } = new List<int>();
    }

    /* Maintenant, un Dialogue est un ensemble de variables et de questions */
    public List<Phrase> questions { get; set; } = new List<Phrase>();
    public List<Phrase> answers { get; set; } = new List<Phrase>();
    public List<AutoAnswer> autoanswers { get; set; } = new List<AutoAnswer>();
    public DialogManager DialogManager { get; set; }

    /******************************************/
    /* Implémentation du moteur de dialogue : */
    /* vous n'avez pas besoin de lire ce code */
    /******************************************/

    /* La méthode pour lire un dialogue
       ATTENTION : T doit être une sous-classe de Dialogue */
    public static T readDialogueFile<T>(string filename, DialogManager menu) where T : Chatbot
    {
        string sdial = File.ReadAllText(filename);
        T toRet = JsonConvert.DeserializeObject<T>(sdial);
        if (toRet != null)
        {
            numOk(toRet.questions, "questions", filename);
            numOk(toRet.answers, "answers", filename);
            toRet.DialogManager = menu;
            return toRet;
        }
        throw new Exception("Erreur lors de la lecture du fichier JSON : " + filename);
    }

    /* pour savoir si le dialogue est terminé */
    private bool done = false;

    /* numéro de la dernière question posée, -1 par défaut */
    private int lastQuestion = -1;

    /* tableau stockant les dernières réponses proposées */
    private int[] lastAnswers;

    /* La méthode stopDialogue peut être appelée n'importe quand
       pour arrêter le dialogue (le tour de dialogue en cours n'est
       pas interrompu) */
    public void stopDialogue()
    {
       done = true;
    }

    /* La méthode nextDialogue qui doit être appelée depuis
       la classe DialogManager au moins une fois pour lancer le dialogue */
    public void nextDialogue()
    {

        if (!done)
        {
            beforeSpeak(lastQuestion);
            int q = getCurrentQuestion();
            string text = getPhraseFromNumber(questions, q);
            UIplayAudio(q);
            UIdisplayQuestion(getPhraseFromNumber(questions, q));
            lastQuestion = q;
            lastAnswers = getPossibleAnswers(q);
            UIdisplayPossibleAnswers(lastAnswers); // attention : la méthode prend en argument les numéros des questions
        }
        else
        {
            DialogManager.EndDialog();
        }
    }

    /* La méthode handleResponse est appelée par l'UI pour déclencher le traitement de
       la réponse de l'utilisateur. */
    public void handleResponse(int resp)
    {
        triggerAffectiveReaction(lastQuestion, lastAnswers[resp]);
        afterAnswer(lastQuestion, lastAnswers[resp]);
    }

    /* Obtenir la phrase associée à un numéro dans une liste.
       Renvoie null si le numéro n'est pas trouvé dans la liste. */
    public string getPhraseFromNumber(List<Phrase> lp, int n)
    {
        foreach (Phrase p in lp)
            if (p.num == n)
                return p.text;
        throw new Exception("Erreur dans le fichier JSON : la question n°" + n + " n'existe pas");
    }

    /* Cette méthode vérifie une liste pour s'assurer qu'il n'y a pas deux fois
       le même numéro. */
    private static void numOk(List<Phrase> l, string name, string filename)
    {
        for (int i = 0; i < l.Count; i++)
            for (int j = 0; j < l.Count; j++)
                if (i != j && l[i].num == l[j].num)
                    throw new Exception("Dans le fichier JSON \"" + filename + "\", le numéro \"" + l[i].num + "\" apparaît plusieurs fois dans la liste \"" + name + "\"");
    }

    
    /* La méthode pour afficher à la question
        */
    public void UIdisplayQuestion(string q)
    {
        DialogManager.DisplayQuestion(q);
    }

    /* La méthode pour afficher à la question
        */
    public void UIplayAudio(int a)
    {
        DialogManager.PlayAudio(a);
    }

    /* La méthode pour afficher les réponses possibles. Elle prend
       en argument des numéros de questions du fichier JSON.
       */
    public void UIdisplayPossibleAnswers(int[] reps)
    {

        List<String> proposals = new List<string>();
        for (int i = 0; i < reps.Length; i++)
        {
            proposals.Add(getPhraseFromNumber(answers, reps[i]));
        }
        DialogManager.DisplayAnswers(proposals);
    }

    /* Déclenchement des AUs */
    public void playAnimation(int[] aus, int[] intensities, float duration)
    {
        if (aus.Length != intensities.Length)
            throw new Exception("Erreur de programmation : les deux tableaux aus et intensities ne sont pas de la même taille");
        if (aus.Length == 0)
            return;
        
        DialogManager.DisplayAUs(aus, intensities,duration);
    }
}
