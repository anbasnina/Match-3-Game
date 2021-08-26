using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };


	public int column;
	public int row;
	public int targetX;
	public int targetY;
	public bool isMatched = false;
	private BoardManager board;
	private GameObject otherTile;
	private Vector2 firstTouchPosition;
	private Vector2 finalTouchPosition;
	private Vector2 tempPosition;
	public float swipeAngle = 0;

    void Start()
    {
		board = FindObjectOfType<BoardManager>();
		targetX = (int)transform.position.x;
		targetY = (int)transform.position.y;

		row = targetY;
		column = targetX;
	}

    void Update()
    {
		
		targetX = column;
		targetY = row;
		if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
			//Move Toward the target
			tempPosition = new Vector2(targetX, transform.position.y);
			transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
			//Directry set the position
			tempPosition = new Vector2(targetX, transform.position.y);
			transform.position = tempPosition;
			board.tiles[column, row] = this.gameObject;
		}
		if (Mathf.Abs(targetY - transform.position.y) > .1f)
		{
			//Move Toward the target
			tempPosition = new Vector2(transform.position.x, targetY);
			transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
		}
		else
		{
			//Directry set the position
			tempPosition = new Vector2( transform.position.x, targetY);
			transform.position = tempPosition;
			board.tiles[column, row] = this.gameObject;
		}
	}

    void Awake() {
		render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}


    private void OnMouseDown()
    {
		firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Debug.Log(firstTouchPosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		CalculateAngle();
	}

	void CalculateAngle()
    {
		swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
		//	Debug.Log(swipeAngle);
		MoveSprite();
	}

	void MoveSprite()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.xSize)
        {
			//Right swipe
			otherTile = board.tiles[column + 1, row];
			otherTile.GetComponent<Tile>().column -= 1;
			column += 1;
        }else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.ySize)
		{
			//Up swipe
			otherTile = board.tiles[column, row + 1];
			otherTile.GetComponent<Tile>().row -= 1;
			row += 1;
		}else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
		{
			//Left swipe
			otherTile = board.tiles[column - 1, row];
			otherTile.GetComponent<Tile>().column += 1;
			column -= 1;
		}else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
		{
			//Down swipe
			otherTile = board.tiles[column, row - 1];
			otherTile.GetComponent<Tile>().row += 1;
			row -= 1;
		}
	}

	private GameObject GetAdjacent(Vector2 castDir) {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		if (hit.collider != null) {
			return hit.collider.gameObject;
		}
		return null;
	}

	private List<GameObject> GetAllAdjacentTiles() {
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++) {
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return adjacentTiles;
	}
	

    private List<GameObject> FindMatch(Vector2 castDir) {
		List<GameObject> matchingTiles = new List<GameObject>();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) {
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}
		return matchingTiles;
	}

	private void ClearMatch(Vector2[] paths) {
		List<GameObject> matchingTiles = new List<GameObject>();
		for (int i = 0; i < paths.Length; i++) { matchingTiles.AddRange(FindMatch(paths[i])); }
		if (matchingTiles.Count >= 2) {
			for (int i = 0; i < matchingTiles.Count; i++) {
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			}
			matchFound = true;
		}
	}

	private bool matchFound = false;
	public void ClearAllMatches() {
		if (render.sprite == null)
			return;

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound) {
			render.sprite = null;
			matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles()); //Add this line
			StartCoroutine(BoardManager.instance.FindNullTiles()); //Add this line
			SFXManager.instance.PlaySFX(Clip.Clear);
		}
	}

}