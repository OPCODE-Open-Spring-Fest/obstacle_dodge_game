using UnityEngine;

public static class DeathHelper
{
    public static void TriggerDeath(GameObject player)
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
            }
            else
            {
                LastLevelRecorder.SaveAndLoad("GameOver");
                return;
            }
        }

        PlayerDeathAnimator deathAnimator = player.GetComponent<PlayerDeathAnimator>();
        if (deathAnimator != null)
        {
            deathAnimator.PlayDeathAnimation();
        }
        else
        {
            LastLevelRecorder.SaveAndLoad("GameOver");
        }
    }
}

