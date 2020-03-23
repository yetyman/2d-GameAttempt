using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Teams { Player, Enemy, NoTeam, P1, P2, P3, P4, P5, P6, P7, P8 }
[Serializable] public class StringToHeParticipateLookup : SerializableLookup<string, HeParticipate> { }
public class HeParticipate : MonoBehaviour
{
    [SerializeField]
    private Teams Team = Teams.Enemy;//this enum exists to formalize a naming scheme so that several behaviours can easily have the same name without worrying about spelling. it is not necessary
    [SerializeField]
    [Description("Overrides Team Value if given")]
    public string TeamName = null;
    [SerializeField]
    public bool Participating = true;
    public static StringToHeParticipateLookup ParticipatingTeams = new StringToHeParticipateLookup();
    // Start is called before the first frame update
    void Awake()
    {
        if (string.IsNullOrWhiteSpace(TeamName)) TeamName = null;
        TeamName = TeamName ?? GetTeamName(Team);
        
        ParticipatingTeams.Add(TeamName, this);
    }

    private string GetTeamName(Teams team)
    {
        if(team == Teams.NoTeam)
        {
            var noTeamNum = 0;
            while (ParticipatingTeams.ContainsKey(Teams.NoTeam.ToString() + noTeamNum))
                noTeamNum++;
            return Teams.NoTeam.ToString() + noTeamNum;
        }
        return team.ToString();
    }


}
