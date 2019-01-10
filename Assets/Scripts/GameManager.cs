using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum gameStatus
{
    next, play, gameover, win
}

public class GameManager : Singleton<GameManager> {
    [SerializeField]
    private int totalWaves = 10;
    [SerializeField]
    private Text totalMoneyLabel;
    [SerializeField]
    private Text totalEscapedLabel;
    [SerializeField]
    private Text currentWaveLabel;
    [SerializeField]
    private Text playButtonLabel;
    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button menuButton;

    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int whichEnemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.play;

    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private Enemy[] enemies;
    [SerializeField]
    private int totalEnemies = 3;
    [SerializeField]
    private int enemiesPerSpawn;
    [SerializeField]
    private int maxEnemiesEscaped;

    public List<Enemy> EnemyList = new List<Enemy>();
    const float spawnDelay = 0.5f;
    private int enemiesToSpawn = 0;

    private bool menuOn = false;

    private AudioSource audioSource;

    public bool MenuOn
    {
        get
        {
            return menuOn;
        }
        set
        {
            menuOn = value;
        }
    }

    public AudioSource AudioSource
    {
        get
        {
            return audioSource;
        }
    }

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }

    public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }

    public int TotalMoney
    {
        get
        {
            return totalMoney;
        }
        set
        {
            totalMoney = value;
            totalMoneyLabel.text = totalMoney.ToString();
        }
    }

	// Use this for initialization
	void Start () {
        playButton.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        showMenu();
        menuButton.onClick.AddListener(MenuClicked);
	}

    private void Update()
    {
        handleEscape();
    }

    void MenuClicked()
    {
        if (menuOn)
        {
            menuOn = false;
        } else
        {
            menuOn = true;
        }
    }

    IEnumerator spawn()
    {
        if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (EnemyList.Count < totalEnemies)
                {
                    Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]) as Enemy;
                    newEnemy.transform.position = spawnPoint.transform.position;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyAllEnemies()
    {
        foreach(Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject);
        }
        EnemyList.Clear();
    }

    // adding and subtracting money methods go here
    public void addMoney(int amount)
    {
        TotalMoney += amount; 
    }

    public void subtractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    public void isWaveOver()
    {
        totalEscapedLabel.text = "Escaped " + TotalEscaped + "/" + maxEnemiesEscaped.ToString();
        if((RoundEscaped + TotalKilled) == totalEnemies)
        {
            if(waveNumber <= enemies.Length)
            {
                enemiesToSpawn = waveNumber;
            }
            setCurrentGameState();
            showMenu();
        }
    }

    public void setCurrentGameState()
    {
        if (TotalEscaped >= 10)
        {
            currentState = gameStatus.gameover;
        } else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)
        {
            currentState = gameStatus.play;
        } else if (waveNumber >= totalWaves)
        {
            currentState = gameStatus.win;
        } else
        {
            currentState = gameStatus.next;
        }
    }

    // show menu
    public void showMenu()
    {
        switch (currentState)
        {
            case gameStatus.gameover:
                playButtonLabel.text = "Play again!";
                //gameover state
                audioSource.PlayOneShot(SoundManager.Instance.Gameover);
                waveNumber = 0;
                break;
            case gameStatus.next:
                playButtonLabel.text = "Next wave";
                break;
            case gameStatus.play:
                playButtonLabel.text = "Play";
                break;
            case gameStatus.win:
                playButtonLabel.text = "Play";
                break;
        }
        playButton.gameObject.SetActive(true);
    }

    public void playButtonPressed()
    {
        switch (currentState)
        {
            case gameStatus.next:
                waveNumber += 1;
                totalEnemies += waveNumber;
                break;
            default:
                totalEnemies = 3;
                TotalEscaped = 0;
                TotalMoney = 10;
                enemiesToSpawn = 0;
                TowerManager.Instance.DestroyAllTower();
                TowerManager.Instance.RenameTagsBuildSites();
                totalMoneyLabel.text = TotalMoney.ToString();
                totalEscapedLabel.text = "Escaped " + TotalEscaped + "/" + maxEnemiesEscaped.ToString();
                audioSource.PlayOneShot(SoundManager.Instance.NewGame);
                break;
        }
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWaveLabel.text = "Wave " + (waveNumber + 1);
        StartCoroutine(spawn());
        playButton.gameObject.SetActive(false);
    }

    private void handleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.disableDragSprite();
            TowerManager.Instance.towerBtnPressed = null;
        }
    }
}
