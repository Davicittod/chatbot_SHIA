using System;
using System.Collections.Generic;

/* Cette classe implémente un chatbot prof de swahili */
class Swahili : Chatbot
{

    /* la probabilité de poser une question sur l'humeur */
    public const double FREQ_HUMEUR = 0.05; // 0.05 est une valeur raisonnable

    /* un générateur aléatoire */
    public static Random rand = new Random();

    /* une variable d'état pour savoir dans quelle phase du dialogue nous sommes
        0 = introduction
        1 = menu principal
        2 = apprendre
        3 = réciter
        4 = partir
        5 = fin
        6 = feedback interrogation
    */
    private int etat = 0;

    /* une variable pour savoir si on est dans la phase d'interruption sur l'humeur
        0 = non
        1 = cause
    */
    private int q_humeur = 0;

    /* numéro du prochain mot à apprendre */
    private int prochain = 10;

    /* bravo ou pas bravo */
    private bool bravo = false;



    /* Les méthodes de la classe Chatbot */
    public override void beforeSpeak(int lastQuestion)
    {
        // rien pour l'instant
    }
    public override int getCurrentQuestion()
    {
        /* intro */
        if (etat == 0)
            return 1;
        /* interruption ? */
        if (q_humeur == 0 && rand.NextDouble() < FREQ_HUMEUR)
        {
            q_humeur = 1;
            return 2;
        }
        if (q_humeur == 1)
        {
            q_humeur = 0;
            return 3;
        }
        /* choix de l'activité */
        if (etat == 1)
            return 4;
        /* partir */
        if (etat == 4)
            return 5;
        if (etat == 5)
            return 8;
        /* apprendre */
        if (etat == 2)
        {
            if (prochain < 18)
                prochain++;
            return prochain;
        }
        /* réciter */
        if (etat == 3)
        {
            int q = rand.Next(7);
            return q + 21; // 21 à 27
        }
        /* feedback (etat==6) */
        if (bravo)
            return 6;
        return 7;
    }
    public override int[] getPossibleAnswers(int question)
    {
        switch (question)
        {
            case 1: // intro
            case 6: // bravo
            case 7: // erreur
            case 8: // au revoir
                return new int[] { 1 };     // continuer
            case 2: // humeur
                return new int[] { 21, 22, 23 };    // humeurs
            case 3: // cause humeur
            case 5: // quitter
                return new int[] { 2, 3 };      // oui ou non
            case 4: // activité
                return new int[] { 41, 42, 43 };    // activités
            case < 20: // apprentissage
                return new int[] { 1 };     // continuer
            case < 30: // interrogation
                return possibleAnswers(question, 5);    // 5 réponses possibles
            default: // ne devrait jamais arriver
                return new int[] { 1 };
        }
    }
    public override void triggerAffectiveReaction(int lastQuestion, int lastAnswer)
    {
        // TP3
        /*if (lastQuestion == 2 && lastAnswer == 21)
        {
            int[] aus = { 6, 12 };
            int[] intensities = { 60, 60 };
            playAnimation(aus, intensities, 1.0f);
        }
        if (lastQuestion == 2 && lastAnswer == 22)
        {
            int[] aus = { 1,4, 15 };
            int[] intensities = { 60,60, 60 };
            playAnimation(aus, intensities, 1.0f);
        }
        */
        
    }
    public override void afterAnswer(int lastQuestion, int lastAnswer)
    {
        /* intro ou apprentissage ou feedback */
        if (etat == 0 || etat == 2 || etat == 6)
            etat = 1;
        /* choix de l'activité */
        else if (etat == 1)
            if (lastAnswer == 41)
                etat = 2; // apprendre
            else if (lastAnswer == 42)
                etat = 3; // réciter
            else
                etat = 4; // quitter
        /* option "arrêter" */
        else if (etat == 4)
            if (lastAnswer == 2)
                etat = 5; // fin
            else
                etat = 1; // reprise
        else if (etat == 5)
            stopDialogue();
        /* option réciter */
        else
        { /* (etat==3) */
            etat = 6;
            bravo = correct(lastQuestion, lastAnswer);
        }
    }

    /* Renvoie n réponses dont n-1 différentes choisies au hasard parmi les réponses possibles (51-57)
       et la réponse "je ne sais pas" (58), en s'assurant que l'une des réponses possibles est la
       bonne réponse */
    private int[] possibleAnswers(int q, int n)
    {
        /* on prépare la liste des réponses */
        List<int> l = new List<int>();
        for (int i = 51; i <= 57; i++)
            l.Add(i);
        /* on met n-1 réponses choisies au hasard */
        List<int> res = new List<int>();
        for (int i = 0; i < n - 1; i++)
        {
            int idx = rand.Next(l.Count);
            res.Add(l[idx]);
            l.RemoveAt(idx);
        }
        /* s'il manque la bonne réponse, on remplace une aléatoirement */
        int correct = q + 30;
        if (!res.Contains(correct))
        {
            int idx = rand.Next(res.Count);
            res[idx] = correct;
        }
        /* enfin on ajoute "je ne sais pas" à la fin */
        res.Add(58);
        return res.ToArray();
    }

    /* Renvoie true ssi la réponse est correcte pour la question posée */
    private bool correct(int q, int a)
    {
        return (q == a - 30);
    }
}

