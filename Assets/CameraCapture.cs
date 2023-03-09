using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/*
 * Ce script permet de faire des screenshots de différents niveau d'expression sur certains blendShape
 */
public class CameraCapture : MonoBehaviour
{
    public int fileCounter;
    public KeyCode screenshotKey;
    public Camera Camera;
    public SkinnedMeshRenderer SkinnedMeshRendererTarget = null; ///< As the name implies

    private void LateUpdate()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            int i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsDown_Left");
            SkinnedMeshRendererTarget.SetBlendShapeWeight(i,100 - (10*fileCounter));
            i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsDown_Right");
            SkinnedMeshRendererTarget.SetBlendShapeWeight(i, 100 - (10 * fileCounter)); 
            i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsIn_Left");
            SkinnedMeshRendererTarget.SetBlendShapeWeight(i, 100 - (10 * fileCounter)); 
            i = getBlendShapeIndex(SkinnedMeshRendererTarget, "BrowsIn_Left");
            SkinnedMeshRendererTarget.SetBlendShapeWeight(i, 100 - (10 * fileCounter)); 
            i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Smile_Left");
            SkinnedMeshRendererTarget.SetBlendShapeWeight(i, 10 * fileCounter);
            i = getBlendShapeIndex(SkinnedMeshRendererTarget, "Smile_Right");
            SkinnedMeshRendererTarget.SetBlendShapeWeight(i, 10 * fileCounter);

            Capture();
        }
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

    public void Capture()
    {
        Camera.enabled = true;
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;

        Camera.Render();

        Texture2D image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;

        byte[] bytes = image.EncodeToPNG();
        Destroy(image);

        File.WriteAllBytes(Application.dataPath + "/Screenshots/" + fileCounter + ".png", bytes);
        fileCounter++;
        if (fileCounter >= 11)
            fileCounter = 0;
        Camera.enabled = false;
    }
}
