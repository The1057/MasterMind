using UnityEngine;

enum parentCanvas
{
    archive = 0,
    profile = 1,
    tasks = 2,
    shop = 3
}
public class UltimateImageUpdateScript : MonoBehaviour
{
    public ProfilePicUpload profilePicScript;
    public ProfilePicUpload storePicScript;
    public ProfilePicUpload startComicScript;

    [Header("Icon updating")]
    public ProfilePicUpload archiveIcon;
    public ProfilePicUpload profileIcon;
    public ProfilePicUpload tasksIcon;
    public ProfilePicUpload shopIcon;

    [Header("Required objects")]
    public playerDataClass playerData;
    public CanvasSwitcher1 canvasSwitcher;
    public GameObject archiveCanvas;
    public GameObject profileCanvas;
    public GameObject tasksCanvas;
    public GameObject shopCanvas;


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
                startComicScript.imageIndex = 1;
            break;
            case ("M"):
                profilePicScript.imageIndex = 0;
                startComicScript.imageIndex = 0;
                break;
        }

        storePicScript.imageIndex = playerData.data.storePicIndex;

        if (canvasSwitcher.currentActiveCanvas.transform.IsChildOf(archiveCanvas.transform))
        {
            archiveIcon.imageIndex = 1;
            tasksIcon.imageIndex = 0;
            profileIcon.imageIndex = 0;
            shopIcon.imageIndex = 0;
        } 
        else if (canvasSwitcher.currentActiveCanvas.transform.IsChildOf(profileCanvas.transform))
        {
            archiveIcon.imageIndex = 0;
            tasksIcon.imageIndex = 0;
            profileIcon.imageIndex = 1;
            shopIcon.imageIndex = 0;
        }
        else if (canvasSwitcher.currentActiveCanvas.transform.IsChildOf(tasksCanvas.transform))
        {
            archiveIcon.imageIndex = 0;
            tasksIcon.imageIndex = 1;
            profileIcon.imageIndex = 0;
            shopIcon.imageIndex = 0;
        }
        else if (canvasSwitcher.currentActiveCanvas.transform.IsChildOf(shopCanvas.transform))
        {
            archiveIcon.imageIndex = 0;
            tasksIcon.imageIndex = 0;
            profileIcon.imageIndex = 0;
            shopIcon.imageIndex = 1;
        }
    }
}
