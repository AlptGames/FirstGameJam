using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public AudioSource metalDoorOpen;
    public AudioSource metalBroke;
    public AudioSource pickup;
    public AudioSource fleshyOpen;
    public AudioSource fleshyPickup;

    public TMP_Text thoughts;
    public Animator animator;

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
            if (hit.collider.gameObject.CompareTag("Key3"))
            {
                StartCoroutine(Thoughts("Is it... breathing..?"));
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
            AddItem("Rust Key", obj, 0);
            pickup.Play();
            return;
        }

        if (obj.CompareTag("Door1"))
        {
            if (obj.CompareTag("Door1"))
            {
                if (Inventory.Contains("Rust Key"))
                {
                    RemoveItem("Rust Key");
                    metalDoorOpen.Play();
                    Destroy(obj); // Удаляем дверь
                    
                    // Ищем объект иконки в UI по имени
                    Transform keyUI = itemsList.transform.Find("Rust Key");
                    if (keyUI != null)
                    {
                        Destroy(keyUI.gameObject); // Удаляем ВЕСЬ объект иконки, а не только трансформ
                    }
                }
                else
                {
                    StartCoroutine(Thoughts("Just a rusty prison door.."));
                }
            }
        }

        if (obj.CompareTag("Key2"))
        {
            // Сначала добавляем в инвентарь, потом удаляем
            AddItem("Prison Key", obj, 1);
            pickup.Play();
            return;
        }

        if (obj.CompareTag("Door2"))
        {
            if (obj.CompareTag("Door2"))
            {
                if (Inventory.Contains("Prison Key"))
                {
                    RemoveItem("Prison Key");
                    metalDoorOpen.Play();
                    Destroy(obj); // Удаляем дверь

                    // Ищем объект иконки в UI по имени
                    Transform keyUI = itemsList.transform.Find("Prison Key");
                    if (keyUI != null)
                    {
                        Destroy(keyUI.gameObject); // Удаляем ВЕСЬ объект иконки, а не только трансформ
                    }
                }
                else
                {
                    StartCoroutine(Thoughts("Prison gate"));
                }
            }
        }

        if (obj.CompareTag("Key3"))
        {
            // Сначала добавляем в инвентарь, потом удаляем
            AddItem("Fleshy Key", obj, 2);
            fleshyPickup.Play();
            StartCoroutine(Thoughts("Ew.."));
            return;
        }

        if (obj.CompareTag("Door3"))
        {
            if (obj.CompareTag("Door3"))
            {
                if (Inventory.Contains("Fleshy Key"))
                {
                    RemoveItem("Fleshy Key");
                    fleshyOpen.Play();
                    Destroy(obj); // Удаляем дверь
                    StartCoroutine(Thoughts("I am tired of these keys"));

                    // Ищем объект иконки в UI по имени
                    Transform keyUI = itemsList.transform.Find("Fleshy Key");
                    if (keyUI != null)
                    {
                        Destroy(keyUI.gameObject); // Удаляем ВЕСЬ объект иконки, а не только трансформ
                    }
                }
                else
                {
                    StartCoroutine(Thoughts("This door is breathing.."));
                }    
            }
        }

        if (obj.CompareTag("Key4"))
        {
            // Сначала добавляем в инвентарь, потом удаляем
            AddItem("Magic Key", obj, 3);
            pickup.Play();
            return;
        }

        if (obj.CompareTag("Door4"))
        {
            if (obj.CompareTag("Door4"))
            {
                if (Inventory.Contains("Magic Key"))
                {
                    RemoveItem("Magic Key");
                    metalDoorOpen.Play();
                    Destroy(obj); // Удаляем дверь

                    // Ищем объект иконки в UI по имени
                    Transform keyUI = itemsList.transform.Find("Magic Key");
                    if (keyUI != null)
                    {
                        Destroy(keyUI.gameObject); // Удаляем ВЕСЬ объект иконки, а не только трансформ
                    }
                }
                else
                {
                    StartCoroutine(Thoughts("What a strange sound..."));
                }
            }
        }

        if (obj.CompareTag("Lom"))
        {
            // Сначала добавляем в инвентарь, потом удаляем
            AddItem("Crowbar", obj, 4);
            StartCoroutine(Thoughts("Oh, finally something useful"));
            pickup.Play();
            return;
        }

        if (obj.CompareTag("DoorLom"))
        {
            if (obj.CompareTag("DoorLom"))
            {
                if (Inventory.Contains("Crowbar"))
                {
                    RemoveItem("Crowbar");
                    Destroy(obj); // Удаляем дверь
                    metalBroke.Play();
                    StartCoroutine(Thoughts("This is an exit!"));
                    animator.SetTrigger("Dark");
                    StartCoroutine(Wait(7f));

                    // Ищем объект иконки в UI по имени
                    Transform keyUI = itemsList.transform.Find("Crowbar");
                    if (keyUI != null)
                    {
                        Destroy(keyUI.gameObject); // Удаляем ВЕСЬ объект иконки, а не только трансформ
                    }
                }
                else
                {
                    StartCoroutine(Thoughts("I need something to open it"));
                }
            }
        }
    }

    public void AddItem(string itemName, GameObject ObjToDestroy, int spriteID)
    {
        Inventory.Add(itemName);
        Debug.Log(itemName);
        GameObject newItem = Instantiate(itemPrefab);
        Image newItemImageScript = newItem.transform.Find("ItemSprite").GetComponent<Image>();
        TMP_Text newItemNaming = newItem.transform.Find("ItemNaming").GetComponent<TMP_Text>();
        newItem.transform.SetParent(itemsList.transform);
        newItem.name = itemName;
        newItemNaming.text = itemName;
        newItemImageScript.sprite = itemSprites[spriteID];
        Destroy(ObjToDestroy);
    }

    public void RemoveItem(string itemName)
    {
        Inventory.Remove(itemName);
    }

    IEnumerator Thoughts(string t)
    {
        thoughts.text = t;
        yield return new WaitForSeconds(3f);
        thoughts.text = string.Empty;
    }
    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("Church");
    }
}