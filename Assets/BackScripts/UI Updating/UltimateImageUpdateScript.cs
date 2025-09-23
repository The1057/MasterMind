using UnityEngine;

public class UltimateImageUpdateScript : MonoBehaviour
{
    public ProfilePicUpload profilePicScript;
    public ProfilePicUpload storePicScript;
    public playerDataClass playerData;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerData.data.player_gender)
        {
            case ("F"):
                profilePicScript.imageIndex = 1;
            break;
            case ("M"):
                profilePicScript.imageIndex = 0;
            break;
        }

        storePicScript.imageIndex = playerData.data.storePicIndex;
    }
}
