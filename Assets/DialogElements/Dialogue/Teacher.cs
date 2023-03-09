using System;
using System.Collections.Generic;

using UnityEngine;

class TeacherBot : Chatbot
{


    public static System.Random rand = new System.Random();

    private int state = 0;
    public int next = 6;

    private bool bravo = false;

    /* implémentation de la méthode getCurrentQuestion() */
    public override int getCurrentQuestion()
    {
        Debug.Log("State actual: " + state.ToString());
        if (state == 0) //introduction
            return 1;
        if (state == 1) //farewell
            return 2;
        if (state == 2) //activity selector
            return 3;
        if (state == 3) { //proverb
            if (next < 9) next ++;
            else next = 7;
            return next;
        }
        if (state == 4) //quizz
            return 10;
        if (state == 5) {   //continue
            if (bravo) return 5;
            else return 4; 
        }

        return 2;   
    }  

    public override int[] getPossibleAnswers(int question) {
        //Debug.Log("Pregunta a hacer: " + question.ToString());
        switch (question) {
            case 1: //intro
                return new int[] { 1 };
            case 2: //farewell
                return new int[] { 2 };
            case 3: //activity selector
                return new int[] { 4, 5, 6 };
            case 4: 
            case 5:
                return new int[] { 3 };
            case 7:
            case 8:
            case 9:
                return new int[] { 7, 8 };
            case 10:
                return new int[] { 9, 10, 11 };
            default:
                return new int[] { 1 };
        }
    }

    /* réécriture de la méthode afterAnswer pour gérer le dialogue */
    public override void afterAnswer(int lastQuestion, int lastAnswer)
    {   
        Debug.Log("Ultima respuesta: " + lastAnswer.ToString());
        if (lastQuestion == 1)
            state = 2;
        else if (state == 1)
            stopDialogue();
        else if (state == 3)
            state = 2;
        else if (state == 2) {   //Here we're in the selector
            if (lastAnswer == 6)   // we want to stop learning
                state = 1;
            else if (lastAnswer == 5) { //proverb
                state = 3;
            } else
                state = 4; //quiz
        } else if (state == 4) {
            bravo = checkAnswer(lastQuestion, lastAnswer);
            state = 5;
        } else if (state == 5) {
            bravo = false;
            state = 2;
        }
    }

    private bool checkAnswer(int q, int a) {
        return q == (a + 1);
    }

}