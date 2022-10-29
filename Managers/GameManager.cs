using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class GameManager : MonoBehaviourPun
{
    private PlayerController player;
    private Door doorExit;

    public GameObject gameOverPanel;
    public static GameManager instance;

    public bool gameOver;

    public List<Enemy> enemies = new List<Enemy>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //player = FindObjectOfType<PlayerController>();
        //doorExit = FindObjectOfType<Door>();
    }

    private void Update()
    {
        if(player!=null)
            gameOver = player.isDead;
        UIManager.instance.SetGameOverPanel(gameOver);
            
    }
    public void IsEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void IsPlayer(PlayerController controller)  //观察者模式，不是主动找，而是对方来
    {
        player = controller;
    }

    public void IsDoorExit(Door door)
    {
        doorExit = door;
    }

    public void EnemyDead(Enemy enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            doorExit.OpenDoor();
            SaveData();
        }
    }

    public void RestartScene()
    {
        PlayerPrefs.DeleteKey("playerHealth");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        PhotonNetwork.Instantiate("PlayerControl", new Vector3(-11, 1, 0), Quaternion.identity, 0);
    }
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("sceneIndex"))
        {
            int sceneIndex = PlayerPrefs.GetInt("sceneIndex");
            if (sceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(sceneIndex);
            else
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }
        else
            NewGame();
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public float LoadHealth()
    {
        if(!PlayerPrefs.HasKey("playerHealth"))
        {
            PlayerPrefs.SetFloat("playerHealth",3f);
        }
        float currentHealth= PlayerPrefs.GetFloat("playerHealth");
        UIManager.instance.UpdateHealth(currentHealth);
        return currentHealth;
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("playerHealth", player.health);
        PlayerPrefs.SetInt("sceneIndex", SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
