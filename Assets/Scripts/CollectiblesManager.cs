using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectiblesManager : MonoBehaviour
{

    public CollectibleScore CollectibleScoreObj;
    public List<CollectiblesScript> PrimaryCollectibles;
    public List<CollectiblesScript> SecondaryCollectibles;

    void Start()
    {
        CollectibleScoreObj.StartRecordingScore();
        SceneController.Instance.RegisterCollectibleScore(CollectibleScoreObj);

        //safeguard ourselves
        if (PrimaryCollectibles.Count != CollectibleScoreObj.PrimaryCollectiblesCollected.Count)
        {
            Debug.Log("Make sure to update" + CollectibleScoreObj + " to reflect the correct number of Primary Collectibles");
        }

        if (SecondaryCollectibles.Count != CollectibleScoreObj.SecondaryCollectiblesCollected.Count)
        {
            Debug.Log("Make sure to update" + SecondaryCollectibles + " to reflect the correct number of Secondary Collectibles");
        }

        // update collected details
        UpdateCollectedCollectibles(CollectibleScoreObj.PrimaryCollectiblesCollected, PrimaryCollectibles);
        UpdateCollectedCollectibles(CollectibleScoreObj.SecondaryCollectiblesCollected, SecondaryCollectibles);
    }

    void UpdateCollectedCollectibles(List<bool> collectedMap, List<CollectiblesScript> collectibles)
    {
        for (int i = 0; i < collectedMap.Count; i++)
        {
            if (collectedMap[i])
            {
                collectibles[i].SetCollected();
            }
        }
    }

    void UpdateScore(CollectiblesScript collectiblesScript)
    {
        int index = PrimaryCollectibles.FindIndex(cs => cs == collectiblesScript);
        bool isPrimary = index != -1;
        if (!isPrimary)
        {
            index = SecondaryCollectibles.FindIndex(cs => cs == collectiblesScript);
        }
        CollectibleScoreObj.UpdateCollectedRecord(isPrimary, index);
    }
}
