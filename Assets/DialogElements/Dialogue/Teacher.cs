using System;
using System.Collections.Generic;

using UnityEngine;

class TeacherBot : Chatbot
{
    public static System.Random rand = new System.Random();

    private const int NUM_CURIOSITIES = 3;
    private const int NUM_QUESTIONS = 2;

    private int state = 0;
    private bool isNativeSpeaker = false;

    public int nextCuriosity = 0;
    public int nextQuestion = 0;

    private bool bravo = false;

    /*
    States of the chatbot:
    0: Right when you start the chatbot
    -1: He says goodbye to you, gets out of the program
    1: We ask for the level of the student
    2: Selector of the next activity
    3: Tells you some curiosity
    4: Ask you a question
    5: Continue window
    */

    // Finds out which question we have to ask, depending on the state we are
    public override int getCurrentQuestion()
    {
        //Debug.Log("State actual: " + state.ToString());
        switch(state) {
            case -1:
                return -1;
            case 0:
                return 1;
            case 1:
                return 2;
            case 2:
                return 3;
            case 3:
                nextCuriosity ++;
                return 7 + nextCuriosity%NUM_CURIOSITIES;
            case 4:
                nextQuestion ++;
                return 20 + nextQuestion%NUM_QUESTIONS;
            case 5:
                if (bravo) return 5;
                else return 4; 
            default:
                return 2; 
        }
    }  

    //Choose the answers after the question is posed
    public override int[] getPossibleAnswers(int question) {
        //Debug.Log("Pregunta a hacer: " + question.ToString());
        switch (question) {
            case -1: //farewell
                return new int[] { -1 };
            case 1: //intro
                return new int[] { 1 };
            case 2:
                return new int[] { 2, 3 };
            
            case 3: //activity selector
                return new int[] { 5, 6, 7 };
            case 4: 
            case 5:
                return new int[] { 4 };
            case 7: //show interest answers
            case 8:
            case 9:
                return new int[] { 8, 9 };
            case 20: //quiz
            case 21:
                return new int[] { 20, 21, 22 };
            default:
                return new int[] { 1 };
        }
    }

    // We modify the state after the answer we chose
    public override void afterAnswer(int lastQuestion, int lastAnswer) {   
        //From the welcome question we go to the level selector window
        if (lastQuestion == 1)
            state = 1;
        //We get out of the program
        else if (state == -1)
            stopDialogue();
        //We ask if the person has a good level of spanish, then we go to the activity selector window
        else if (state == 1) {
            if (lastAnswer == 2) {
                isNativeSpeaker = true;
            }
            state = 2;
        }
        //After a curiosity we only agree with it and go back to the selector
        else if (state == 3)            
            state = 2;
        //If we answered the quiz we find out i we have to congratulate or encourage to the next time                
        else if (state == 4) {
            bravo = checkAnswer(lastQuestion, lastAnswer);
            state = 5;
        //From the selector we go to another activity depending on the answer we chose
        } else if (state == 2) {
            //We want to stop learning         
            if (lastAnswer == 7)            
                state = -1;
            //Proverb
            else if (lastAnswer == 6) {     
                state = 3;
            //Quiz
            } else
                state = 4;  
        //We just congratulated/encouraged the person, we are coming back to the selector
        } else if (state == 5) {
            bravo = false;
            state = 2;
        }
    }

    private bool checkAnswer(int q, int a) {
        return q == a ;
    }

    /*
    public override void triggerAffectiveReaction(int lastQuestion, int lastAnswer) {
        if (lastQuestion == 10) {
            if (checkAnswer(lastQuestion, lastAnswer)) {
                playAnimation(new int[]{6,12}, new int[]{100,100}, .5f);
            } else {
                playAnimation(new int[]{1,4,15}, new int[]{100,100,100}, .5f);
            }
        }
    }
    */

    
    private double getGoal(int lastQuestion, int lastAnswer) {
        //If we are leaving
        if (lastQuestion == -1) {
            //We are sad because our student didn't learned a lot
            if ( (nextQuestion+nextCuriosity) < 3) {
                return -.7;
            } else 
                return .7;
        }

        //If we ask a question
        if (lastQuestion >= 20) {
            if(checkAnswer(lastQuestion, lastAnswer)) 
                return .9;
            else
                return -.4;
        }

        return 0;
    }

    public const int AGENT = 0;
    public const int USER = 1;
    public const int OTHER = 2;

    private int getCause(int lastQuestion, int lastAnswer) {
        if (lastQuestion >= 20) {
            //Environment information to check wether the person is native or not
            if (isNativeSpeaker) 
                //If is native and makes a mistake, it is the user's fault
                return USER;
            //If we have already asked this question, is the user's fault
            else if (nextQuestion > NUM_QUESTIONS)
                return USER;
            else
                return OTHER;
        }
        return OTHER;
    }

    public const int PAST = 0;
    public const int FUTURE = 1;

    private int getTime(int lastQuestion, int lastAnswer) {
        //We are scared of being responsible because our student didn't even tried to learn something
        if ( (nextQuestion+nextCuriosity) < 1) 
            return FUTURE;
        else
            return PAST;
    }

    public override void triggerAffectiveReaction(int lastQuestion, int lastAnswer) {

        double goal = getGoal(lastQuestion, lastAnswer);
        int cause = getCause(lastQuestion, lastAnswer);
        int time = getTime(lastQuestion, lastAnswer);
        int i = (int)(50+Math.Abs(goal)*50); // intensity
        /* catégorie émotionnelle OCC */
        if (time == PAST) {
            if (goal>0)
                // événement passé et désirable -> joie 0,5 secondes
                playAnimation(new int[]{6,12}, new int[] {i,i}, .5f);
            else if (goal<0)
                if (cause==USER)
                    // événement passé, indésirable et causé par autrui -> colère
                    playAnimation(new int[]{4,5,7,23}, new int[] {i,i,i,i}, .5f);
                else
                    // événement passé, indésirable et causé par soi ou le monde -> tristesse
                    playAnimation(new int[]{1,4,15}, new int[] {i,i,i}, .5f);
            // pas de else : si goal=0, on ne déclenche pas de réaction affective
        } else { // time == FUTURE
            if (goal<0)
                // événement futur et indésirable -> peur
                playAnimation(new int[]{1,2,4,5,7,20,26}, new int[] {i,i,i,i,i,i,i}, .5f);
            // pas de else : si goal>=0, on ne déclenche pas de réaction affective
        }
    }
}