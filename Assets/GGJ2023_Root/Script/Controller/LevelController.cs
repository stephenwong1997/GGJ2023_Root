using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Transform cameraDefaultPos;

    [SerializeField] List<SourceController> sourceControllers;

    [Header("Run time reference")]
    public Transform rootParent;
    public RootDrawer currentRoot;
    public List<RootDrawer> roots;
    public Transform currentContainer;
    public GameObject seedSprite;
    public GameObject cropSprite;

    public void SetSeedToCrop()
    {
        if (seedSprite == null || cropSprite == null)
        {
            Debug.LogError("No seed/crop sprite!");
            return;
        }

        seedSprite.SetActive(false);
        cropSprite.SetActive(true);
    }

    public void SetCropToSeed()
    {
        if (seedSprite == null || cropSprite == null)
        {
            Debug.LogError("No seed/crop sprite!");
            return;
        }

        seedSprite.SetActive(true);
        cropSprite.SetActive(false);
    }

    public void ResetSources()
    {
        SetCropToSeed();

        for (int i = 0; i < sourceControllers.Count; i++)
        {
            sourceControllers[i].Reset();
        }
    }

    public void ResetRoots()
    {
        Destroy(rootParent.GetChild(0).gameObject);
        GameObject newContainer = Instantiate(RootManager.instance.emptyContainer, rootParent);
        currentContainer = newContainer.transform;
        roots.Clear();
        roots.Add(currentContainer.GetComponentInChildren<RootDrawer>());
        currentRoot = roots[0];
    }

}
