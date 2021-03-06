﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcCivPersonalityManager : MonoBehaviour
{
    //[Header("Tuning")]
    //[SerializeField] float walkSpeed = 10f;
    [Header("Personality")]
    public NpcBehaviorPersonality_SC personality;

    [Header("Current Emotion")]
    public NpcBehaviorEmotion_SC currentEmotion;

    [Header("Possible Emotions")]
    public NpcBehaviorEmotion_SC normal;
    public NpcBehaviorEmotion_SC frightened;

    [Header("PossiblePersonalities")]
    public List<NpcBehaviorPersonality_SC> allPersonalities = new List<NpcBehaviorPersonality_SC>();

    float emotionTimeCounter = 0;

    void Awake()
    {
        currentEmotion = normal;
    }

    private void Update()
    {
        if (currentEmotion != normal)
        {
            emotionTimeCounter+=Time.deltaTime;

            if(emotionTimeCounter >= personality.resetEmotionTime)
            {
                GameObject frontWalkway = this.gameObject.GetComponent<NpcCivMoveWalk>().RayCastDown();
                if ( frontWalkway != null && frontWalkway.tag == "Concrete")
                {
                    currentEmotion = normal;
                    emotionTimeCounter = 0;
                }
            }
        }
    }

    /// <summary>
    /// Sets the personality of this NPC.
    /// </summary>
    /// <param name="p">The personality to set.</param>
    public void SetPersonality(NpcBehaviorPersonality_SC p)
    {
        this.personality = p;
    }

    /// <summary>
    /// Sets the personality of this NPC.
    /// </summary>
    public void SetPersonality()
    {
        //Set a random personality to the NPC.
        SetPersonality(allPersonalities[(int)Random.Range(0, allPersonalities.Count)]);
    }

    public void CheckForScared()
    {
        if (Vector3.Distance(this.transform.position, PlayerManager.Instance.gameObject.transform.position) < personality.distanceCheck)
        {
            this.currentEmotion = frightened;
        }
    }
}
