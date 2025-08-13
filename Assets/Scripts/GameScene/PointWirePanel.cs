using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointWirePanel : MonoBehaviour
{
    public Transform LeftPanel;
    public Transform RightPanel;
    public GameObject Wire;
    public GameObject WirePoint;
    public Canvas canvas;
    
    public List<WireColorMapping> colorPalette;

    public void CreatePanel()
    {
        //Создание списка цветов
        List<WireColor> availableColors = new();
        foreach (var mapping in colorPalette)
        {
            availableColors.Add(mapping.wireColorEnum);
        }

        //Создание случайного порядка цветов
        for (int i = 0; i < availableColors.Count; i++)
        {
            WireColor temp = availableColors[i];
            int randomIndex = Random.Range(i, availableColors.Count);
            availableColors[i] = availableColors[randomIndex];
            availableColors[randomIndex] = temp;
        }

        int colorIndex = 0;
        for (int a = 0; a < 4; a++)
        {
            //Общие проверки и данные
            if(colorIndex >= availableColors.Count) break;
            WireColor assignedColorEnum = availableColors[colorIndex];
            Color assignedColorValue = GetColorFromEnum(assignedColorEnum);
            
            //Настройка правой панели
            
            
            //Настройка левой панели
            GameObject wire = Instantiate(Wire, transform.position, Quaternion.identity);
            DontDestroyOnLoad(wire.gameObject);
            wire.transform.SetParent(LeftPanel, false);
            wire.transform.position = LeftPanel.position + new Vector3(0, 350 - a * 175, 0);
            WireDrag wireD = wire.GetComponentInChildren<WireDrag>();
            wireD.wireColor = assignedColorEnum;
            wire.GetComponent<Image>().color = assignedColorValue;
            wire.GetComponentInChildren<Image>().color = assignedColorValue;
        }
    }

    private Color GetColorFromEnum(WireColor wireColorEnum)
    {
        foreach (var mapping in colorPalette)
        {
            if (mapping.wireColorEnum == wireColorEnum)
            {
                return mapping.colorValue;
            }
        }
        return Color.white;
    }
}