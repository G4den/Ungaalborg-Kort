using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardTextField : MonoBehaviour
{

    public string Name;
    public TextMeshProUGUI cardText;
    public string prefix;
    TMP_InputField inputText;
    bool setplayerpref = false;



    void Update()
    {

        if (Input.GetKeyDown(KeyCode.ScrollLock))
        {
            PlayerPrefs.DeleteAll();
            Application.LoadLevel(Application.loadedLevel);
        }

        if (setplayerpref)
            PlayerPrefs.SetString(Name, inputText.text);

        if (prefix == "")
            cardText.text = PlayerPrefs.GetString(Name);
        else
            cardText.text = prefix + " " + PlayerPrefs.GetString(Name);
    }

    private IEnumerator Start()
    {
        inputText = GetComponentInParent<TMP_InputField>();
        if (PlayerPrefs.GetString(Name).Length != 0)
            inputText.text = PlayerPrefs.GetString(Name);
        yield return null;
        setplayerpref = true;
    }
}
