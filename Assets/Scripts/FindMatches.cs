using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour {

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

	// Use this for initialization
	void Start () {
        board = FindObjectOfType<Board>();
	}

    public void FindAllMatches(){
        StartCoroutine(FindAllMatchesCo());
    }


    private void AddToListAndMatch(GameObject dot){
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3){
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCo(){
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i ++){
            for (int j = 0; j < board.height; j ++){
                GameObject currentDot = board.allDots[i, j];

                if(currentDot != null){
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if(i > 0 && i < board.width - 1){
                        GameObject leftDot = board.allDots[i - 1, j];

                        GameObject rightDot = board.allDots[i + 1, j];


                        if (leftDot != null && rightDot != null)
                        {
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {

                                GetNearbyPieces(leftDot, currentDot, rightDot);

                            }
                        }

                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];

                        GameObject downDot = board.allDots[i, j - 1];


                        if (upDot != null && downDot != null)
                        {
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {

                                GetNearbyPieces(upDot, currentDot, downDot);

                            }
                        }
                    }


                }
            }
        }

    }

    public void MatchPiecesOfColor(string color){
        for (int i = 0; i < board.width; i ++){
            for (int j = 0; j < board.height; j ++){
                //Check if that piece exists
                if(board.allDots[i, j] != null){
                    //Check the tag on that dot
                    if(board.allDots[i, j].tag == color){
                        //Set that dot to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column){
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i ++){
            if(board.allDots[column, i]!= null){
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

}
