using UnityEngine;

public class TestRewardScript : MonoBehaviour
{

    public float scoreToGemMultiplier = 10;
    public saveLoadManager saveLoadManager;
    public Score debugScore;

    saveData saveData;

    [ContextMenu("Give Reward in Gems")]
    public void giveRewardGems(Score playerScore)
    {
        saveData = saveLoadManager.loadData();
        saveData.MoneyData.player_gems += (int)Mathf.Floor((float)playerScore.score * scoreToGemMultiplier);
        saveLoadManager.saveData(saveData);
    }

    [ContextMenu("Give Debug Reward in Gems")]
    public void giveDebugReward()
    {
        giveRewardGems(debugScore);
    }
}
