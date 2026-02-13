using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ClickManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject animalPanel;
    public TextMeshProUGUI diseaseText;
    public TextMeshProUGUI moneyText;
    public Button treatButton;
    public Button closeButton;

    [Header("Market UI")]
    public GameObject marketPanel;               // Tam ekran market panel
    public Button openMarketButton;              // Market açma butonu
    public Transform marketContent;              // ScrollView Content
    public GameObject marketItemPrefab;          // MarketItem prefab

    [Header("Player")]
    public Transform player;
    public float interactDistance = 5f;

    [Header("Game")]
    public int money = 200;
    public Transform[] animals;

    [Header("Tools")]
    public ToolData[] allTools;
    public List<ToolData> shopItems;            // Market ürünleri

    private string currentTool = "";
    private int currentAnimalIndex = -1;
    private string currentDisease;

    private bool[] animalsHealed;
    private string[] animalDiseases;

    // 🔥 5 HASTALIK
    private string[] diseases = { "Ateş", "Kırık", "İshal", "Enfeksiyon", "Parazit" };
    private int[] treatmentCosts = { 12, 25, 18, 30, 22 };
    private int[] treatmentRewards = { 18, 35, 25, 40, 32 };

    void Start()
    {
        // Animal Panel
        animalPanel.SetActive(false);

        treatButton.onClick.AddListener(TreatAnimal);
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePanel);

        animalsHealed = new bool[animals.Length];
        animalDiseases = new string[animals.Length];

        // Hastalık atama
        for (int i = 0; i < animals.Length; i++)
        {
            int randomChance = Random.Range(0, 100);
            if (randomChance < 10)
                animalDiseases[i] = "Parazit";
            else
                animalDiseases[i] = diseases[Random.Range(0, 4)];
        }

        // Money UI
        UpdateMoneyUI();

        // Market
        marketPanel.SetActive(false);
        if (openMarketButton != null)
            openMarketButton.onClick.AddListener(OpenMarket);

        PopulateMarket();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {
#if UNITY_EDITOR
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#else
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                for (int i = 0; i < animals.Length; i++)
                {
                    if (hit.transform == animals[i])
                    {
                        float dist = Vector3.Distance(player.position, animals[i].position);
                        if (dist <= interactDistance)
                        {
                            currentAnimalIndex = i;
                            OpenAnimalPanel();
                        }
                    }
                }
            }
        }
    }

    #region Animal Panel
    void OpenAnimalPanel()
    {
        if (currentAnimalIndex == -1) return;

        animalPanel.SetActive(true);

        if (animalsHealed[currentAnimalIndex])
        {
            diseaseText.text = "Bu hayvan zaten iyileştirildi!";
            treatButton.interactable = false;
            return;
        }

        treatButton.interactable = true;

        currentDisease = animalDiseases[currentAnimalIndex];

        int index = System.Array.IndexOf(diseases, currentDisease);
        int cost = treatmentCosts[index];

        diseaseText.text = "Hastalık: " + currentDisease + "\nÜcret: $" + cost;
    }

    void TreatAnimal()
    {
        if (currentAnimalIndex == -1) return;

        if (!ToolSuitableForDisease(currentDisease, currentTool))
        {
            diseaseText.text = "Yanlış alet!";
            return;
        }

        int index = System.Array.IndexOf(diseases, currentDisease);
        int cost = treatmentCosts[index];
        int reward = treatmentRewards[index];

        if (money >= cost)
        {
            money -= cost;
            money += reward;

            animalsHealed[currentAnimalIndex] = true;

            diseaseText.text = "İyileştirildi! +" + reward + "$";
            treatButton.interactable = false;

            UpdateMoneyUI();
        }
        else
        {
            diseaseText.text = "Yeterli paran yok!";
        }
    }

    bool ToolSuitableForDisease(string disease, string tool)
    {
        foreach (ToolData toolData in allTools)
        {
            if (toolData.toolName.Trim().ToLower() == tool.Trim().ToLower() &&
                toolData.curesDisease.Trim().ToLower() == disease.Trim().ToLower())
            {
                return true;
            }
        }
        return false;
    }

    void UpdateMoneyUI()
    {
        moneyText.text = "$" + money;
    }

    public void HidePanel()
    {
        animalPanel.SetActive(false);
    }
    #endregion

    #region Market Panel
    public void OpenMarket()
    {
        if (marketPanel != null)
            marketPanel.SetActive(true);
    }

    public void CloseMarket()
    {
        if (marketPanel != null)
            marketPanel.SetActive(false);
    }

    void PopulateMarket()
    {
        if (marketContent == null || marketItemPrefab == null || shopItems == null) return;

        // Önce temizle
        foreach (Transform child in marketContent)
            Destroy(child.gameObject);

        for (int i = 0; i < shopItems.Count; i++)
        {
            GameObject item = Instantiate(marketItemPrefab, marketContent);
            item.transform.localScale = Vector3.one;

            // UI atama
            Image icon = item.GetComponentInChildren<Image>();
            if(icon != null) icon.sprite = shopItems[i].icon;

            TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = shopItems[i].toolName;
                texts[1].text = "$" + shopItems[i].price;
            }

            int index = i;
            Button buyButton = item.GetComponentInChildren<Button>();
            if (buyButton != null)
                buyButton.onClick.AddListener(() => BuyItem(index));
        }
    }

    void BuyItem(int index)
    {
        if (money >= shopItems[index].price)
        {
            money -= shopItems[index].price;
            UpdateMoneyUI();
            Debug.Log("Satın alındı: " + shopItems[index].toolName);
        }
        else
        {
            Debug.Log("Yeterli paran yok!");
        }
    }
    #endregion
}
