using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ScrollableList : MonoBehaviour
{
    [Header("References")]
    public GameObject contentParent;
    public GameObject itemPrefab; 
    public RectTransform scrollViewRect;
    
    [Header("Layout Settings")]
    public float spacing = 10f;
    public float itemWidth = 100f; 
    public float itemHeight = 140f;
    private RectTransform contentRect;
    
    [Header("ScrollView Size Limits")]
    public float minScrollViewSize = 100f; // Minimum size
    public float maxScrollViewSize = 500f; // Maximum size (when 5+ items)
    
    void Start()
    {
        contentRect = contentParent.GetComponent<RectTransform>();
        SetupLayoutGroup();
    }
    
    void SetupLayoutGroup()
    {
        if (!contentParent.GetComponent<HorizontalLayoutGroup>())
        {
            var layout = contentParent.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = false; // Items keep their width
            layout.childControlHeight = true; // Items stretch to full height
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.childAlignment = TextAnchor.LowerLeft;
        }
        
        if (!contentParent.GetComponent<ContentSizeFitter>())
        {
            var fitter = contentParent.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // Width grows with items
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained; // Height stays fixed
        }
        
    }
    
    // Main method: Populates the list with building data
    public void PopulateList(BuildingData[] data)
    {
        // Clear old items first
        ClearList();
        
        // Loop through all building data
        foreach (BuildingData buildingData in data)
        {
            // Create the item from prefab
            GameObject item = Instantiate(itemPrefab, contentParent.transform);

            // initialize the placebuildingbutton script
            PlaceBuildingButton pb = item.AddComponent<PlaceBuildingButton>();
            pb.Initialize(buildingData.type);

            Button button = item.GetComponent<Button>();
            Assert.IsNotNull(button, "itemPrefab missing Button");
            button.onClick.RemoveAllListeners();
            // add script to button
            button.onClick.AddListener(pb.OnClick);

            BuildingListItem listItem = item.GetComponent<BuildingListItem>();

            // Fill it with data
            Assert.IsNotNull(listItem, "itemPrefab missing BuildingListItem");
            listItem.Setup(buildingData);

            RectTransform itemRect = item.GetComponent<RectTransform>();
            Assert.IsNotNull(itemRect, "itemPrefab missing RectTransform");

            // For horizontal: set width, height stretches
            LayoutElement layoutElement = item.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = item.AddComponent<LayoutElement>();
            
            layoutElement.preferredWidth = itemWidth;
        }
        
        // Adjust Content size based on number of items
        AdjustContentSize(data.Length);
    }
    
    // Helper method: Clear all items from the list
    public void ClearList()
    {
        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    // Adjusts Content size based on number of items
    void AdjustContentSize(int itemCount)
    {
        if (contentRect == null)
            contentRect = contentParent.GetComponent<RectTransform>();

        // Calculate total width needed
        float totalWidth = (itemWidth * itemCount) + (spacing * (itemCount - 1));
        // Resize the SCROLLVIEW (not just content)
        if (scrollViewRect != null)
        {
            float scrollViewWidth = Mathf.Clamp(totalWidth, minScrollViewSize, maxScrollViewSize);
            scrollViewRect.sizeDelta = new Vector2(scrollViewWidth, scrollViewRect.sizeDelta.y);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }
    
    // Optional: Add single item at runtime
    public void AddItem(BuildingData data)
    {
        GameObject item = Instantiate(itemPrefab, contentParent.transform);
        
        BuildingListItem listItem = item.GetComponent<BuildingListItem>();
        if (listItem != null)
        {
            listItem.Setup(data);
        }
        
        // Update size
        int currentCount = contentParent.transform.childCount;
        AdjustContentSize(currentCount);
    }
}