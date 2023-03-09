using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Ce script gère les expressions faciales et l'animation des lèvres de l'agent.
 * Il est un peu basique, mais couvre les besoins minimums pour le cours IA & SHS
 */
public class FacialExpression : MonoBehaviour
{

    public AudioSource audioSource;
    public SkinnedMeshRenderer SkinnedMeshRendererTarget = null; 


    private float referenceLipTime;
    private float referenceFaceTime;
    private int choice;
    private float timeBetweenViseme = 0.175f;
    private float facialExpressionDuration = 2.0f;
    private int[] aus = { 0 };

    /*Pour chaque paramètre du visage, on va conserver la valeur cible, vers laquelle on souhaite que le muscle du visage aille,
    * et la valeur précédente (dans le paramètre Back) afin de pouvoir interpoler ensuite entre ces deux valeurs pour animer en douceur le visage
    */


    //VISEME (animation des lèvres)
    private int MouthOpenWeight = 0;
    private int UpperLipOutWeight = 0;
    private int LowerLipDown_LeftWeight = 0;
    private int LowerLipDown_RightWeight = 0;
    private int TongueUpWeight = 0;

    private int MouthNarrow_LeftWeight = 0;
    private int MouthNarrow_RightWeight = 0;
    private int LowerLipOutWeight = 0;
    private int MouthUpWeight = 0;
    private int LowerLipInWeight = 0;
    private int UpperLipInWeight = 0;
    private int UpperLipUp_LeftWeight = 0;
    private int UpperLipUp_RightWeight = 0;
    //VISEME_BACK
    private int MouthOpenWeightBack = 0;
    private int UpperLipOutWeightBack = 0;
    private int LowerLipDown_LeftWeightBack = 0;
    private int LowerLipDown_RightWeightBack = 0;
    private int TongueUpWeightBack = 0;

    private int MouthNarrow_LeftWeightBack = 0;
    private int MouthNarrow_RightWeightBack = 0;
    private int LowerLipOutWeightBack = 0;
    private int MouthUpWeightBack = 0;
    private int LowerLipInWeightBack = 0;
    private int UpperLipInWeightBack = 0;
    private int UpperLipUp_LeftWeightBack = 0;
    private int UpperLipUp_RightWeightBack = 0;
    //FACE (expression faciale des émotions par AUs)
    //AU1
    private int BrowsOuterLower_LeftWeight = 0;
    private int BrowsOuterLower_RightWeight = 0;
    //AU2
    private int BrowsUp_LeftWeight = 0;
    private int BrowsUp_RightWeight = 0;
    //AU4
    private int BrowsIn_LeftWeight = 0;
    private int BrowsIn_RightWeight = 0;
    private int BrowsDown_LeftWeight = 0;
    private int BrowsDown_RightWeight = 0;
    //AU5
    private int EyesWide_LeftWeight = 0;
    private int EyesWide_RightWeight = 0;
    //AU6
    private int Squint_RightWeight = 0;
    private int Squint_LeftWeight = 0;
    //AU12
    private int Smile_RightWeight = 0;
    private int Smile_LeftWeight = 0;
    //AU15
    private int Frown_RightWeight = 0;
    private int Frown_LeftWeight = 0;
    //AU20
    private int MouthDownWeight = 0;
    //AU23
    //MouthNarrow
    //AU26
    private int JawDownWeight = 0;

    //FACE_BACK
    //AU1
    private int BrowsOuterLower_LeftWeightBack = 0;
    private int BrowsOuterLower_RightWeightBack = 0;
    //AU2
    private int BrowsUp_LeftWeightBack = 0;
    private int BrowsUp_RightWeightBack = 0;
    //AU4
    private int BrowsIn_LeftWeightBack = 0;
    private int BrowsIn_RightWeightBack = 0;
    private int BrowsDown_LeftWeightBack = 0;
    private int BrowsDown_RightWeightBack = 0;
    //AU5
    private int EyesWide_LeftWeightBack = 0;
    private int EyesWide_RightWeightBack = 0;
    //AU6
    private int Squint_RightWeightBack = 0;
    private int Squint_LeftWeightBack = 0;
    //AU12
    private int Smile_RightWeightBack = 0;
    private int Smile_LeftWeightBack = 0;
    //AU15
    private int Frown_RightWeightBack = 0;
    private int Frown_LeftWeightBack = 0;
    //AU20
    private int MouthDownWeightBack = 0;
    //AU23
    //MouthNarrow
    //AU26
    private int JawDownWeightBack = 0;


    void Start()
    {
        referenceLipTime = Time.time;
        referenceFaceTime = Time.time;
        audioSource = GetComponent<AudioSource>();
        if (SkinnedMeshRendererTarget == null)
            SkinnedMeshRendererTarget = gameObject.GetComponent<SkinnedMeshRenderer>();


    }

    void Update()
    {
        float now = Time.time;


        //FACE LERP
        float faceLerp = (now - referenceFaceTime) / facialExpressionDuration;
        //LIP LERP (CONSTANT)
        float lipLerp = (now - referenceLipTime) / timeBetweenViseme;
        if (SkinnedMeshRendererTarget != null)
        {
            if (audioSource.isPlaying)
            {
                if (now - referenceLipTime > timeBetweenViseme)
                {
                    UpdateLipBackWeight();
                    choice = Random.Range(0, 11);
                    referenceLipTime = Time.time;
                    setRandomViseme(choice);
                }

            }
            else
            {
                setVisemeNeutral();
                if (now - referenceFaceTime > facialExpressionDuration)
                {
                    UpdateFaceBackWeight();

                    setFaceNeutral();
                    referenceFaceTime = Time.time;

                }
            }
            //Interpolation des animations
            lerpViseme(lipLerp);
            lerpFace(faceLerp);

        }
    }

    public void setFacialAUs(int[] aus, int[] intensities, float duration)
    {
        this.aus = aus;
        facialExpressionDuration = duration;
        referenceFaceTime = Time.time;
        for (int i = 0; i < aus.Length; i++)
        {
            //-joie : AUs 6 et 12
            //-tristesse : AUs 1, 4 et 15
            //-peur : AUs 1, 2, 4, 5, 7, 20 et 26
            //-colère : AUs 4, 5, 7 et 23
            //Les AUs n'étant pas directement disponible dans le modèle 3D de Unity, nous les convertissons vers les BlendShapes équivalentes
            switch (aus[i])
            {

                case 1: BrowsOuterLower_LeftWeight = intensities[i]; BrowsOuterLower_RightWeight = intensities[i]; break;
                case 2: BrowsUp_LeftWeight = intensities[i]; BrowsUp_RightWeight = intensities[i]; break;
                case 4:
                    BrowsIn_LeftWeight = intensities[i]; BrowsIn_RightWeight = intensities[i];
                    BrowsDown_LeftWeight = intensities[i]; BrowsDown_RightWeight = intensities[i]; break;
                case 5: EyesWide_LeftWeight = intensities[i]; EyesWide_RightWeight = intensities[i]; break;
                case 6: Squint_LeftWeight = intensities[i]; Squint_RightWeight = intensities[i]; break;
                case 7: Squint_LeftWeight = intensities[i]; Squint_RightWeight = intensities[i]; break;
                case 12: Smile_LeftWeight = intensities[i]; Smile_RightWeight = intensities[i]; break;
                case 15: Frown_LeftWeight = intensities[i]; Frown_RightWeight = intensities[i]; break;
                case 20: MouthDownWeight = intensities[i]; break;
                case 23: MouthNarrow_LeftWeight = intensities[i]; MouthNarrow_RightWeight = intensities[i]; break;
                case 26: JawDownWeight = intensities[i]; break;
                default: break;
            }
        }

    }

    public void UpdateLipBackWeight()
    {
        MouthOpenWeightBack = MouthOpenWeight;
        UpperLipOutWeightBack = UpperLipOutWeight;
        LowerLipDown_LeftWeightBack = LowerLipDown_LeftWeight;
        LowerLipDown_RightWeightBack = LowerLipDown_RightWeight;
        TongueUpWeightBack = TongueUpWeight;
        MouthNarrow_LeftWeightBack = MouthNarrow_LeftWeight;
        MouthNarrow_RightWeightBack = MouthNarrow_RightWeight;
        LowerLipOutWeightBack = LowerLipOutWeight;
        MouthUpWeightBack = MouthUpWeight;
        LowerLipInWeightBack = LowerLipInWeight;
        UpperLipInWeightBack = UpperLipInWeight;
        UpperLipUp_LeftWeightBack = UpperLipUp_LeftWeight;
        UpperLipUp_RightWeightBack = UpperLipUp_RightWeight;

    }

    public void UpdateFaceBackWeight()
    {
        Smile_RightWeightBack = Smile_RightWeight;
        Smile_LeftWeightBack = Smile_LeftWeight;
        BrowsUp_LeftWeightBack = BrowsUp_LeftWeight;
        BrowsUp_RightWeightBack = BrowsUp_RightWeight;
        BrowsOuterLower_LeftWeightBack = BrowsDown_LeftWeight;
        BrowsOuterLower_RightWeightBack = BrowsDown_RightWeight;
        BrowsDown_LeftWeightBack = BrowsDown_LeftWeight;
        BrowsDown_RightWeightBack = BrowsDown_RightWeight;
        BrowsIn_LeftWeightBack = BrowsIn_LeftWeight;
        BrowsIn_RightWeightBack = BrowsIn_RightWeight;
        Squint_LeftWeightBack = Squint_RightWeight;
        Squint_RightWeightBack = Squint_RightWeight;
        EyesWide_LeftWeightBack = EyesWide_LeftWeight;
        EyesWide_RightWeightBack = EyesWide_RightWeight;
        Frown_RightWeightBack = Frown_RightWeight;
        Frown_LeftWeightBack = Frown_LeftWeight;
        MouthDownWeightBack = MouthDownWeight;
        JawDownWeightBack = JawDownWeight;
    }

    /*!
       * @brief A function for getting blendshape index by name.
       * @return int
       */
    public int getBlendShapeIndex(SkinnedMeshRenderer smr, string bsName)
    {
        Mesh m = smr.sharedMesh;

        for (int i = 0; i < m.blendShapeCount; i++)
        {
            string name = m.GetBlendShapeName(i);
            if (bsName.Equals(m.GetBlendShapeName(i)) == true)
                return i;
        }

        return 0;
    }

    public void setRandomViseme(int choice)
    {

        switch (choice)
        {
            case 0: setViseme_R_ER(); break;
            case 1: setViseme_AA_AO_OW(); break;
            case 2: setViseme_m_b_p_x(); break;
            case 3: setViseme_N_NG_CH_TH_ZH_DH_j_s(); break;
            case 4: setViseme_y_EH_IY(); break;
            case 5: setViseme_L_EL(); break;
            case 6: setViseme_w(); break;
            case 7: /*setViseme_IH_AH_AE();*/setViseme_AA_AO_OW(); break;
            case 8: setViseme_U_UW(); break;
            case 9: setViseme_AW(); break;
            case 10: setViseme_fv(); break;
            default: setViseme_AA_AO_OW(); break;

        }
    }



    public void setVisemeNeutral()
    {
        MouthOpenWeight = 0;
        UpperLipOutWeight = 0;
        LowerLipDown_LeftWeight = 0;
        LowerLipDown_RightWeight = 0;
        TongueUpWeight = 0;
        MouthNarrow_LeftWeight = 0;
        MouthNarrow_RightWeight = 0;
        LowerLipOutWeight = 0;
        MouthUpWeight = 0;
        LowerLipInWeight = 0;
        UpperLipInWeight = 0;
        UpperLipUp_LeftWeight = 0;
        UpperLipUp_RightWeight = 0;
    }

    public void setFaceNeutral()
    {
        //AU1
        BrowsOuterLower_LeftWeight = 0;
        BrowsOuterLower_RightWeight = 0;
        //AU2
        BrowsUp_LeftWeight = 0;
        BrowsUp_RightWeight = 0;
        //AU4
        BrowsIn_LeftWeight = 0;
        BrowsIn_RightWeight = 0;
        BrowsDown_LeftWeight = 0;
        BrowsDown_RightWeight = 0;
        //AU5
        EyesWide_LeftWeight = 0;
        EyesWide_RightWeight = 0;
        //AU6
        Squint_RightWeight = 0;
        Squint_LeftWeight = 0;
        //AU12
        Smile_RightWeight = 0;
        Smile_LeftWeight = 0;
        //AU15
        Frown_RightWeight = 0;
        Frown_LeftWeight = 0;
        //AU20
        MouthDownWeight = 0;
        //AU26
        JawDownWeight = 0;
    }
    /*
     * On vient animer les Blendshapes des lèvres à l'aide de l'interpolation entre nos deux valeurs
     */
    public void lerpViseme(float lerp)
    {
        Mesh m = SkinnedMeshRendererTarget.sharedMesh;
        int i = getBlendShapeIndex(SkinnedMeshRendererTarget, "MouthOpen");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(MouthOpenWeightBack, MouthOpenWeight, lerp));

        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "MouthUp");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(MouthUpWeightBack, MouthUpWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "LowerLipOut");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(LowerLipOutWeightBack, LowerLipOutWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "MouthNarrow_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(MouthNarrow_LeftWeightBack, MouthNarrow_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "MouthNarrow_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(MouthNarrow_RightWeightBack, MouthNarrow_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "UpperLipOut");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(UpperLipOutWeightBack, UpperLipOutWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "LowerLipIn");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(LowerLipInWeightBack, LowerLipInWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "UpperLipIn");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(UpperLipInWeightBack, UpperLipInWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "LowerLipDown_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(LowerLipDown_LeftWeightBack, LowerLipDown_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "LowerLipDown_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(LowerLipDown_RightWeightBack, LowerLipDown_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "UpperLipUp_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(UpperLipUp_LeftWeightBack, UpperLipUp_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "UpperLipUp_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(UpperLipUp_RightWeightBack, UpperLipUp_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "TongueUp");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(TongueUpWeightBack, TongueUpWeight, lerp));

    }

    /*
     * On vient animer les BlendShapes du visage (correspondant plus ou moins aux AUs) à l'aide de l'interpolation
     */
    public void lerpFace(float lerp)
    {
        Mesh m = SkinnedMeshRendererTarget.sharedMesh;

        int i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsUp_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsUp_RightWeightBack, BrowsUp_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsUp_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsUp_LeftWeightBack, BrowsUp_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Smile_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(Smile_RightWeightBack, Smile_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Smile_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(Smile_LeftWeightBack, Smile_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsOuterLower_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsOuterLower_LeftWeightBack, BrowsOuterLower_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsOuterLower_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsOuterLower_RightWeightBack, BrowsOuterLower_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsIn_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsIn_LeftWeightBack, BrowsIn_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsIn_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsIn_RightWeightBack, BrowsIn_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsDown_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsDown_LeftWeightBack, BrowsDown_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsDown_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(BrowsDown_RightWeightBack, BrowsDown_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "EyesWide_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(EyesWide_LeftWeightBack, EyesWide_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "EyesWide_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(EyesWide_RightWeightBack, EyesWide_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Squint_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(Squint_LeftWeightBack, Squint_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Squint_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(Squint_RightWeightBack, Squint_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Frown_Left");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(Frown_LeftWeightBack, Frown_LeftWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Frown_Right");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(Frown_RightWeightBack, Frown_RightWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "MouthDown");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(MouthDownWeightBack, MouthDownWeight, lerp));
        i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Jaw_Down");
        SkinnedMeshRendererTarget.SetBlendShapeWeight(i, (int)Mathf.Lerp(JawDownWeightBack, JawDownWeight, lerp));


    }


    public void setViseme_AA_AO_OW()
    {
        setVisemeNeutral();
        LowerLipOutWeight = 75;
        MouthNarrow_LeftWeight = 75;
        MouthNarrow_RightWeight = 75;
        //MouthOpenWeight = 30;
        UpperLipOutWeight = 75;


    }
    public void setViseme_R_ER()
    {
        setVisemeNeutral();
        LowerLipOutWeight = 75;
        MouthNarrow_LeftWeight = 75;
        MouthNarrow_RightWeight = 75;
        //MouthOpenWeight = 30;
        UpperLipOutWeight = 75;

    }
    public void setViseme_m_b_p_x()
    {
        setVisemeNeutral();
        LowerLipInWeight = 75;
        UpperLipInWeight = 75;

    }
    public void setViseme_N_NG_CH_TH_ZH_DH_j_s()
    {
        setVisemeNeutral();
        //MouthOpenWeight = 30;

    }
    public void setViseme_y_EH_IY()
    {
        setVisemeNeutral();
        LowerLipDown_LeftWeight = 30;
        LowerLipDown_RightWeight = 30;
        //MouthOpenWeight = 15;
        UpperLipUp_LeftWeight = 30;
        UpperLipUp_RightWeight = 30;
        //Smile_RightWeight = 30;
        //Smile_LeftWeight = 30;

    }
    public void setViseme_L_EL()
    {
        setVisemeNeutral();
        //MouthOpenWeight = 60;
        TongueUpWeight = 75;

    }
    public void setViseme_w()
    {
        setVisemeNeutral();
        //MouthOpenWeight = 25;
        MouthNarrow_LeftWeight = 75;
        MouthNarrow_RightWeight = 75;

    }
    public void setViseme_IH_AH_AE()
    {
        setVisemeNeutral();
        //MouthOpenWeight = 50;
        Smile_RightWeight = 50;
        Smile_LeftWeight = 50;

    }
    public void setViseme_U_UW()
    {
        setVisemeNeutral();
        MouthNarrow_LeftWeight = 75;
        MouthNarrow_RightWeight = 75;
        LowerLipOutWeight = 75;
        UpperLipInWeight = 75;

    }
    public void setViseme_AW()
    {
        setVisemeNeutral();
        //MouthOpenWeight = 30;
        MouthUpWeight = 30;
        UpperLipUp_LeftWeight = 30;
        UpperLipUp_RightWeight = 75;

    }
    public void setViseme_fv()
    {
        setVisemeNeutral();
        LowerLipInWeight = 75;
        UpperLipInWeight = 75;
        UpperLipUp_LeftWeight = 50;
        UpperLipUp_RightWeight = 50;

    }



}