using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour {
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button exitButton;

    public Transform canvas;

    private void Start()
    {
        resumeButton.onClick.AddListener(resumeClicked);
        restartButton.onClick.AddListener(restartClicked);
        exitButton.onClick.AddListener(exitClicked);
    }

    void resumeClicked()
    {
        //Debug.Log("clicked!");
        GameManager.Instance.MenuOn = false;
        canvas.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    void restartClicked()
    {
        SceneManager.LoadScene("Level1");
    }

    void exitClicked()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update () {
        if (GameManager.Instance.MenuOn)
        {
            canvas.gameObject.SetActive(true);
            Time.timeScale = 0;
        } else
        {
            canvas.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
	}
}
