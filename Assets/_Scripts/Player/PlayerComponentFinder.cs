using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponentFinder : MonoBehaviour
{
    public Player Player
    {
        get
        {
            if (!m_Player){
                GameObject playerObj=GameObject.Find("Player");
                if(playerObj!=null){
                    m_Player=playerObj.GetComponent<Player>();
                }
            }
            return m_Player;
        }
    }

    private Player m_Player;
}
