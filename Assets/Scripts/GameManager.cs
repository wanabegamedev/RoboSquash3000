using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float playerScore = 0;

    public bool gameInPlay;

    public float scoreMultiplier = 1;

    public float turns = 3;

   [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private TextMeshPro multiplierText;


    [SerializeField] private GameObject ball;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore();
        gameInPlay = true;
        RestartMatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameInPlay)
        {
            multiplierText.text = scoreMultiplier + "X";
        }
      
    }

    public void UpdateScore()
    {
        scoreText.text = playerScore.ToString();
    }


    float CalculateScore()
    {
        playerScore = Mathf.Ceil(playerScore * scoreMultiplier);
        return playerScore;
    }

    public void IncreaseScore(int amount)
    {
        playerScore += amount;
    }

    public void IncreaseMultiplier(float amount)
    {
        scoreMultiplier += amount;
    }

    public void RestartMatch()
    {
        //Don't end the game if the player still has turns left
        if (turns <= 0)
        {
            EndMatch();
        }
        //if the player is out of turns then quit
        else
        {
            CalculateScore();
            //Get a reference to the ball and instantiate one in the middle of the court.
            Instantiate(ball, transform.position, Quaternion.identity);
            
              
            //Reset score and multiplier 
            scoreMultiplier = 1;
            UpdateScore();

            gameInPlay = true;
        }
        
      
    }

    public void EndMatch()
    {
        gameInPlay = false;
        scoreText.text = "Press A to start again!";
        scoreMultiplier = 1;
        playerScore = 0;
    }
}
