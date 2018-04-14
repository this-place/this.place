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

    public void ResetScore()
    {
        _primaryRecord = PrimaryCollectiblesCollected;
        _secondaryRecord = SecondaryCollectiblesCollected;

        List<bool> tempList = new List<bool>();
        foreach (bool cs in PrimaryCollectiblesCollected)
        {
            tempList.Add(false);
        }
        _primaryRecord = tempList;

        tempList = new List<bool>();
        foreach (bool cs in SecondaryCollectiblesCollected)
        {
            tempList.Add(false);
        }
        _secondaryRecord = tempList;
    }

    public List<bool> WipeScore()
    {
        if (IsPuzzle)
        {
            List<bool> tempList = new List<bool>();
            foreach (bool cs in PrimaryCollectiblesCollected)
            {
                tempList.Add(false);
            }
            PrimaryCollectiblesCollected = tempList;

            tempList = new List<bool>();
            foreach (bool cs in SecondaryCollectiblesCollected)
            {
                tempList.Add(false);
            }
            SecondaryCollectiblesCollected = tempList;

            tempList = new List<bool>();
            foreach (bool cs in PrimaryCollectiblesCollected)
            {
                tempList.Add(false);
            }
            _primaryRecord = tempList;

            tempList = new List<bool>();
            foreach (bool cs in SecondaryCollectiblesCollected)
            {
                tempList.Add(false);
            }
            _secondaryRecord = tempList;
            return new List<bool>(PrimaryCollectiblesCollected);
        }

        List<bool> toReturn = new List<bool>();

        foreach (CollectibleScore cs in SubScores)
        {
            toReturn.AddRange(cs.WipeScore());
        }

        return toReturn;
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
    
    public List<bool> GetPrimaryRecordInStage()
    {
        if (IsPuzzle)
        {
            return new List<bool>(_primaryRecord);
        }

        List<bool> toReturn = new List<bool>();

        foreach (CollectibleScore cs in SubScores)
        {
            toReturn.AddRange(cs.GetPrimaryRecordInStage());
        }

        return toReturn;
    }

    public List<bool> GetSecondaryRecordInStage()
    {
        if (IsPuzzle)
        {
            return new List<bool>(_secondaryRecord);
        }

        List<bool> toReturn = new List<bool>();

        foreach (CollectibleScore cs in SubScores)
        {
            toReturn.AddRange(cs.GetSecondaryRecordInStage());
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
    