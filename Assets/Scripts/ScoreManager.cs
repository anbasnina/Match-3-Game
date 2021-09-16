using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

	private Board board;
	public Text scoreText;
	public int score;


	// Use this for initialization
	void Start()
	{
		board = FindObjectOfType<Board>();
	}

	// Update is called once per frame
	void Update()
	{
		scoreText.text = "Score:" + score;
	}

	public void IncreaseScore(int amountToIncrease)
	{
		score += amountToIncrease;
	
	}

	}

