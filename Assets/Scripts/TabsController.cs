using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{

    public Image[] tabImages;
    public GameObject[] pages;


    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNum)
    {
        for(int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNum].SetActive(true);
        tabImages[tabNum].color = Color.white;
    }

}
