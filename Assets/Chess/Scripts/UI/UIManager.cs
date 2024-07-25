using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private GameObject blackPawnPromotionPanel;
    [SerializeField] private GameObject whitePawnPromotionPanel;
    [SerializeField] public List<Sprite> availablePromotionSprites;

    private ChessPiece _pawnPiece;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        HidePawnPromotionPanel();
    }

    //Called when the pawn promotion panel is to be shown
    public void ShowPawnPromotionPanel(bool isWhite, ChessPiece pawnPiece)
    {
        _pawnPiece = pawnPiece;

        if (isWhite)
        {
            whitePawnPromotionPanel.SetActive(true);
        }
        else
        {
            blackPawnPromotionPanel.SetActive(true);
        }
    }

    public void HidePawnPromotionPanel()
    {
        whitePawnPromotionPanel.SetActive(false);
        blackPawnPromotionPanel.SetActive(false);
    }

    public void Promote(int index)
    {
        HidePawnPromotionPanel();
        string piece = "";

        if (index == 0 || index == 4)
        {
            piece = "Queen";
        }
        else if (index == 1 || index == 5)
        {
            piece = "Rook";
        }
        else if (index == 2 || index == 6)
        {
            piece = "Bishop";
        }
        else if (index == 3 || index == 7)
        {
            piece = "Knight";
        }

        ChessBoardPlacementHandler.Instance.PromotePawn(piece, availablePromotionSprites[index], _pawnPiece);
    }
}