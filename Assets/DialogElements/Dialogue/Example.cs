/* Cette classe implémente un exemple très simple de dialogue en utilisant les données du fichier dialogue0.json */
class ExampleBot : Chatbot
{

    /* Nous utilisons une variable pour savoir quelle option a choisit l'utilisateur.
    Nous mettons cette valeur à -2 en début de dialogue, puis -1 après les salutations. */
    private int choix = -2;

    /* implémentation de la méthode getCurrentQuestion() */
    public override int getCurrentQuestion()
    {
        if (choix == -2)    // au premier tour (choix=-2), nous disons bonjour
            return 1;
        if (choix == -1)    // au deuxième tour (choix=-1), nous demandons à l'utilisateur de choisir une réponse
            return 2;
        return 3;        // au dernier tour, nous disons au revoir.
    }

    /* réécriture de la méthode afterAnswer pour gérer le dialogue */
    public override void afterAnswer(int lastq, int r)
    {
        if (lastq == 1)     // après la question 1 (bonjour), nous passons dans l'état -1
            choix = -1;
        if (lastq == 2)    // après la réponse à la question 2, nous notons le choix de l'utilisateur dans la variable choix
            choix = r;
        if (lastq == 3)    // et au dernier tour, nous arrêtons le dialogue
            stopDialogue();
    }

}