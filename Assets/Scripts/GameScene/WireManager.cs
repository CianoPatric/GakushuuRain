using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour
{
    [SerializeField] private GameObject PrefabPanel;
    private GameObject WirePanel;
    private Transform panel;
    public static WireManager Instance { get; private set; }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        panel = transform;
        OpenPanel();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void OpenPanel()
    {
        if (WirePanel == null)
        {
            WirePanel = Instantiate(PrefabPanel, transform.position, Quaternion.identity);
            DontDestroyOnLoad(WirePanel);
            WirePanel.transform.SetParent(panel, false);
            WirePanel.transform.position = panel.position + new Vector3(0, 90, 0);
            var component = WirePanel.GetComponent<PointWirePanel>();
            component.CreatePanel();
        }
        WirePanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (WirePanel != null)
        {
            WirePanel.SetActive(false);
        }
    }
}

[System.Serializable]
public class WireColorMapping
{
    public WireColor wireColorEnum;
    public Color colorValue;
}