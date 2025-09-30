using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BubbleSort : MonoBehaviour
{
    float[] array;
    List<GameObject> mainObjects;
    public GameObject prefab;

    private Thread sortThread;
    private bool arrayChanged = false;

    // ---------------- NUEVO PARA QUICKSORT ----------------
    float[] quickArray;
    List<GameObject> quickObjects;
    public GameObject quickPrefab;

    private Thread quickThread;
    private bool quickChanged = false;
    private float quickYOffset = -10f;
    // ------------------------------------------------------

    void Start()
    {
        mainObjects = new List<GameObject>();
        array = new float[300];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (float)Random.Range(0, 1000) / 100;
        }

        // QuickSort usa una copia del mismo array inicial
        quickArray = (float[])array.Clone();
        quickObjects = new List<GameObject>();

        // TO DO 4 – Call setup functions
        logArray();
        spawnObjs();
        updateHeights();

        spawnQuickObjs();
        updateQuickHeights();

        // TO DO 5 – Create and start a thread with bubbleSort
        sortThread = new Thread(bubbleSort);
        sortThread.Start();

        // Crear y lanzar hilo para QuickSort
        quickThread = new Thread(() => quickSort(quickArray, 0, quickArray.Length - 1));
        quickThread.Start();
    }

    void Update()
    {
        // TO DO 6 – Update GameObjects in main thread
        if (arrayChanged)
        {
            updateHeights();
            arrayChanged = false;
        }

        if (quickChanged)
        {
            updateQuickHeights();
            quickChanged = false;
        }
    }

    // TO DO 5 – Sorting function
    void bubbleSort()
    {
        int n = array.Length;
        bool swapped;
        for (int i = 0; i < n - 1; i++)
        {
            swapped = false;
            for (int j = 0; j < n - i - 1; j++)
            {
                if (array[j] > array[j + 1])
                {
                    (array[j], array[j + 1]) = (array[j + 1], array[j]);
                    swapped = true;
                    arrayChanged = true;
                    Thread.Sleep(1);
                }
            }
            if (!swapped) break;
        }
        Debug.Log("BubbleSort finished!");
    }

    // ---------------- QUICK SORT ----------------
    void quickSort(float[] arr, int left, int right)
    {
        int i = left, j = right;
        float pivot = arr[(left + right) / 2];

        while (i <= j)
        {
            while (arr[i] < pivot) i++;
            while (arr[j] > pivot) j--;
            if (i <= j)
            {
                (arr[i], arr[j]) = (arr[j], arr[i]);
                quickChanged = true;
                i++;
                j--;
                Thread.Sleep(13); 
            }
        }
        if (left < j) quickSort(arr, left, j);
        if (i < right) quickSort(arr, i, right);
    }
    // ------------------------------------------------------

    // TO DO 1 – Print array
    void logArray()
    {
        string text = "";
        for (int i = 0; i < array.Length; i++)
        {
            text += array[i].ToString("F2") + ", ";
        }
        Debug.Log(text);
    }

    // TO DO 2 – Spawn objects and store them in a list
    void spawnObjs()
    {
        for (int i = 0; i < array.Length; i++)
        {
            GameObject obj = Instantiate(
                prefab,
                new Vector3((float)i / 20, this.gameObject.transform.position.y, 0),
                Quaternion.identity
            );
            mainObjects.Add(obj);
        }
    }

    // TO DO 3 – Update cube heights to match array
    bool updateHeights()
    {
        bool changed = false;
        for (int i = 0; i < array.Length; i++)
        {
            GameObject obj = mainObjects[i];
            Vector3 scale = obj.transform.localScale;
            if (scale.y != array[i])
            {
                scale.y = array[i];
                obj.transform.localScale = scale;

                Vector3 pos = obj.transform.position;
                pos.y = scale.y / 2f;
                obj.transform.position = pos;

                changed = true;
            }
        }
        return changed;
    }

    // ---------------- QUICK SORT HELPERS ----------------
    void spawnQuickObjs()
    {
        for (int i = 0; i < quickArray.Length; i++)
        {
            GameObject obj = Instantiate(
                quickPrefab,
                new Vector3((float)i / 20, this.gameObject.transform.position.y + quickYOffset, 0),
                Quaternion.identity
            );
            quickObjects.Add(obj);
        }
    }

    bool updateQuickHeights()
    {
        bool changed = false;
        for (int i = 0; i < quickArray.Length; i++)
        {
            GameObject obj = quickObjects[i];
            Vector3 scale = obj.transform.localScale;
            if (scale.y != quickArray[i])
            {
                scale.y = quickArray[i];
                obj.transform.localScale = scale;

                Vector3 pos = obj.transform.position;
                pos.y = scale.y / 2f + quickYOffset;
                obj.transform.position = pos;

                changed = true;
            }
        }
        return changed;
    }
}
