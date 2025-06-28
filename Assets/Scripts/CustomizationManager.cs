using UnityEngine; 

public static class CustomizationManager
{
    public static int SelectedHandIndex { get; private set; }
    public static int SelectedDartIndex { get; private set; }

    public static void SetHand(int index)
    {
        SelectedHandIndex = index;
        PlayerPrefs.SetInt("HandIndex", index);
        PlayerPrefs.Save(); 
    }

    public static void SetDart(int index)
    {
        SelectedDartIndex = index;
        PlayerPrefs.SetInt("DartIndex", index);
        PlayerPrefs.Save(); 
    }

    public static void LoadSelections()
    {
        SelectedHandIndex = PlayerPrefs.GetInt("HandIndex", 0);
        SelectedDartIndex = PlayerPrefs.GetInt("DartIndex", 0);
    }
}
