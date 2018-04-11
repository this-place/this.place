using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu]
public class CollectibleScore : ScriptableObject
{
    [Header("Sub Scores (Based on Level)")]
    public bool IsPuzzle;
    public List<CollectibleScore> SubScores;

    public List<bool> PrimaryCollectiblesCollected;
    public List<bool> SecondaryCollectiblesCollected;

    private List<bool> _primaryRecord;
    private List<bool> _secondaryRecord;

    public void SaveScore()
    {
        PrimaryCollectiblesCollected = _primaryRecord;
        SecondaryCollectiblesCollected = _secondaryRecord;
    }

    public void StartRecordingScore()
    {
        _primaryRecord = new List<bool>(PrimaryCollectiblesCollected);
        _secondaryRecord = new List<bool>(SecondaryCollectiblesCollected);
    }

    public void UpdateCollectedRecord(bool isPrimary, int index)
    {
        if (isPrimary)
        {
            _primaryRecord[index] = true;
        }
        else
        {
            _secondaryRecord[index] = true;
        }
    }

    public List<bool> GetPrimaryCollectedInStage()
    {
        if (IsPuzzle)
        {
            return new List<bool>(PrimaryCollectiblesCollected);
        }

        List<bool> toReturn = new List<bool>();

        foreach (CollectibleScore cs in SubScores)
        {
            toReturn.AddRange(cs.GetPrimaryCollectedInStage());
        }

        return toReturn;
    }

    public List<bool> GetSecondaryCollectedInStage()
    {
        if (IsPuzzle)
        {
            return new List<bool>(SecondaryCollectiblesCollected);
        }

        List<bool> toReturn = new List<bool>();

        foreach (CollectibleScore cs in SubScores)
        {
            toReturn.AddRange(cs.GetSecondaryCollectedInStage());
        }

        return toReturn;
    }
}
    