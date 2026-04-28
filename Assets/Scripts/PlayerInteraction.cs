using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionOutline : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Camera playerCamera;
    
    private Outline lastOutline; // Ссылка на последний подсвеченный объект

    public List<string> Inventory = new List<string>();

    public Sprite[] itemSprites;

    public GameObject itemPrefab;
    public GameObject itemsList;

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Пытаемся достать компонент Outline у объекта, на который смотрим
            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline != null)
            {
                // Если мы перевели взгляд на новый объект
                if (lastOutline != outline)
                {
                    ClearOutline(); // Гасим старый
                    outline.enabled = true; // Включаем новый
                    lastOutline = outline;
                }
            }
            else
            {
                ClearOutline(); // Смотрим на объект без обводки
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Выполняем действие
                Interact(hit.collider.gameObject);
            }
        }
        else
        {
            ClearOutline(); // Смотрим в пустоту
        }
    }

    void ClearOutline()
    {
        if (lastOutline != null)
        {
            lastOutline.enabled = false;
            lastOutline = null;
        }
    }

    public void Interact(GameObject obj)
    {
        if (obj.CompareTag("Key1"))
        {
            // Сначала добавляем в инвентарь, потом удаляем
            AddItem("Key1", obj);
            return;
        }

        if (obj.CompareTag("Door1"))
        {
            if (obj.CompareTag("Door1"))
            {
                if (Inventory.Contains("Key1"))
                {
                    RemoveItem("Key1");
                    Destroy(obj); // Удаляем дверь

                    // Ищем объект иконки в UI по имени
                    Transform keyUI = itemsList.transform.Find("Key1");
                    if (keyUI != null)
                    {
                        Destroy(keyUI.gameObject); // Удаляем ВЕСЬ объект иконки, а не только трансформ
                    }
                }
            }
        }
    }

    public void AddItem(string itemName, GameObject ObjToDestroy)
    {
        Inventory.Add(itemName);
        Debug.Log(itemName);
        GameObject newItem = Instantiate(itemPrefab);
        Image newItemImageScript = newItem.transform.Find("ItemSprite").GetComponent<Image>();
        newItem.transform.SetParent(itemsList.transform);
        newItem.name = itemName;
        newItemImageScript.sprite = itemSprites[0];
        Destroy(ObjToDestroy);
    }

    public void RemoveItem(string itemName)
    {
        Inventory.Remove("Key1");
    }
}