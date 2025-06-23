using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenuUI : MonoBehaviour
{
    public Image handPreviewImage;
    public Image dartPreviewImage;
    public Sprite[] handSprites;
    public Sprite[] dartSprites;

    void Start()
    {
        CustomizationManager.LoadSelections();
        UpdatePreviews();
    }

    public void SetHand1() { CustomizationManager.SetHand(0); UpdatePreviews(); }
    public void SetHand2() { CustomizationManager.SetHand(1); UpdatePreviews(); }
    public void SetHand3() { CustomizationManager.SetHand(2); UpdatePreviews(); }

    public void SetDart1() { CustomizationManager.SetDart(0); UpdatePreviews(); }
    public void SetDart2() { CustomizationManager.SetDart(1); UpdatePreviews(); }
    public void SetDart3() { CustomizationManager.SetDart(2); UpdatePreviews(); }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void UpdatePreviews()
    {
        handPreviewImage.sprite = handSprites[CustomizationManager.SelectedHandIndex];
        dartPreviewImage.sprite = dartSprites[CustomizationManager.SelectedDartIndex];
    }
}
