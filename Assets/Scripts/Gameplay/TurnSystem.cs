using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


public class TurnSystem : MonoBehaviour
{
    public bool isPlayerTurn;
    public int playerTurn;
    public int opponentTurn;
    public bool playerPlayed = false;
    public bool skip = false;
    public TextMeshProUGUI turnText;

    bool isTurnWarningActive = false;
    string turnWarning;

    public bool isWildTurn;
    public CardColor wildColor;
    public TextMeshProUGUI wildColorText;

    public GameObject youWin;
    public GameObject youLose;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPlayerTurn = false;
        playerTurn = 0;
        opponentTurn = 0;
    }

    public void Reset() {
        youWin.SetActive(false);
        youLose.SetActive(false);
        isPlayerTurn = false;
        playerTurn = 0;
        opponentTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        if (isTurnWarningActive) turnText.text = turnWarning;
        else {
            if (isPlayerTurn) {
                if (playerDeck.cardsToDraw > 0) turnText.text = "DRAW " + playerDeck.cardsToDraw + " CARD(S)";
                else turnText.text = "PLAY A CARD!";
            } else {
                if (playerDeck.cardsToDraw > 0) turnText.text = "OPPONENT DRAWS " + playerDeck.cardsToDraw + " CARD(S)";
                else turnText.text = opponentTurn > 0 ? "OPPONENT PLAYS" : "PLEASE WAIT, SETTING UP...";
            }
        }


        if (isWildTurn) {
            switch (wildColor) {    
                case CardColor.RED:
                    wildColorText.text = "COLOR: <color=#FF0000>RED</color>";
                    break;
                case CardColor.BLUE:
                    wildColorText.text = "COLOR: <color=#007EFF>BLUE</color>";
                    break;
                case CardColor.GREEN:
                    wildColorText.text = "COLOR: <color=#28BC00>GREEN</color>";
                    break;
                case CardColor.YELLOW:
                    wildColorText.text = "COLOR: <color=#FFCC00>YELLOW</color>";
                    break;
                default:
                    wildColorText.text = "COLOR: " + wildColor.ToString();

                    break;
            }
        }
        else wildColorText.text = "";
    }



    public void EndPlayerTurn() {
        isPlayerTurn = false;
        if (GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>().playerClones.Count == 0) {
            youWin.transform.SetAsLastSibling();
            youWin.SetActive(true);
            return;
        }
        ++opponentTurn;
        if (opponentTurn > 0) StartCoroutine(OpponentAI());
    }

    public void EndOpponentTurn() {
        if (GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>().opponentClones.Count == 0) {
            youLose.transform.SetAsLastSibling();
            youLose.SetActive(true);
            return;
        }
        ++playerTurn;
        if (skip) {
            playerPlayed = false;
            skip = false;
            ++opponentTurn;
            if (opponentTurn > 0) StartCoroutine(OpponentAI());
        } else {
            isPlayerTurn = true;
            playerPlayed = false;
        }
    }

    IEnumerator OpponentAI() {
        ColorPicker colorPicker = GameObject.Find("ColorPicker").GetComponent<ColorPicker>();
        yield return new WaitUntil(() => !colorPicker.active);

        yield return new WaitForSeconds(0.5f);
        PlayerDeck playerDeck = GameObject.Find("PlayerDeck").GetComponent<PlayerDeck>();
        Card topDiscard = playerDeck.discardPile.Last();
        switch (topDiscard.num) {
            case CardNum.SKIP:
                if (!playerPlayed) goto default;
                break;
            default:
                if (playerDeck.cardsToDraw > 0) {
                    GameObject cardToPlay = null;
                    List<CardColor> colors = new List<CardColor>();
                    foreach (GameObject card in playerDeck.opponentClones) {
                        Card realCard = card.GetComponent<DisplayCard>().displayCard;
                        if ((realCard.num == CardNum.DRAW2 || realCard.num == CardNum.DRAW4) && playerDeck.isCardPlayable(realCard)) {
                            cardToPlay = card;
                            break;
                        } else colors.Add(card.GetComponent<DisplayCard>().displayCard.color);
                    }
                    if (cardToPlay) {
                        playerDeck.PlayCardO(cardToPlay);
                        Debug.Log("[Opponent] Played card: " + cardToPlay.GetComponent<DisplayCard>().displayCard.color + " " + cardToPlay.GetComponent<DisplayCard>().displayCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                        if (cardToPlay.GetComponent<DisplayCard>().displayCard.color == CardColor.WILD) {
                            SetWildTurn(colors.GroupBy(i=>i).OrderByDescending(grp=>grp.Count()).Select(grp=>grp.Key).First());
                            playerDeck.discardPile.Last().color = wildColor;
                        }
                        topDiscard = playerDeck.discardPile.Last();
                        if (topDiscard.num == CardNum.DRAW2) playerDeck.cardsToDraw += 2;
                        else if (topDiscard.num == CardNum.DRAW4) playerDeck.cardsToDraw += 4;
                    }
                    else {
                        while (playerDeck.cardsToDraw > 0) {
                            playerDeck.cardsToDraw -= 1;
                            playerDeck.DrawCardO();
                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                    break;
                }
                yield return new WaitForSeconds(0.5f);
                bool played = false;
                while (!played) {
                    GameObject wild = null;
                    GameObject cardToPlay = null;
                    List<CardColor> colors = new List<CardColor>();
                    foreach (GameObject card in playerDeck.opponentClones) {
                        if (card.GetComponent<DisplayCard>().displayCard.color == CardColor.WILD) wild = card;
                        else if (card.GetComponent<DisplayCard>().displayCard.color == topDiscard.color) {
                            cardToPlay = card;
                            break;
                        } else if (card.GetComponent<DisplayCard>().displayCard.num == topDiscard.num) {
                            cardToPlay = card;
                            break;
                        } else colors.Add(card.GetComponent<DisplayCard>().displayCard.color);
                    }
                    if (cardToPlay) {
                        Debug.Log("[Opponent] Played card: " + cardToPlay.GetComponent<DisplayCard>().displayCard.color + " " + cardToPlay.GetComponent<DisplayCard>().displayCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                        playerDeck.PlayCardO(cardToPlay);
                        played = true;
                    }

                    else if (wild) {
                        Debug.Log("[Opponent] Played card: " + wild.GetComponent<DisplayCard>().displayCard.color + " " + wild.GetComponent<DisplayCard>().displayCard.num + " on " + topDiscard.color + " " + topDiscard.num);
                        playerDeck.PlayCardO(wild);
                        SetWildTurn(colors.GroupBy(i=>i).OrderByDescending(grp=>grp.Count()).Select(grp=>grp.Key).First());
                        playerDeck.discardPile.Last().color = wildColor;
                        played = true;
                    }
                    else {
                        playerDeck.DrawCardO();
                        Card drawnCard = playerDeck.opponentClones.Last().GetComponent<DisplayCard>().displayCard;
                        if (playerDeck.isCardPlayable(drawnCard)) {
                            yield return new WaitForSeconds(0.25f);
                            playerDeck.PlayCardO(playerDeck.opponentClones.Last());
                        }
                        played = true;
                    }
                    if (!wild) isWildTurn = false;

                }
                topDiscard = playerDeck.discardPile.Last();
                if (topDiscard.num == CardNum.DRAW2) playerDeck.cardsToDraw += 2;
                else if (topDiscard.num == CardNum.DRAW4) playerDeck.cardsToDraw += 4;
                else if (topDiscard.num == CardNum.SKIP) skip = true;
                break;
        }
        EndOpponentTurn();
    }

    public void SetTurnWarning(string warningText) {
        isTurnWarningActive = true;
        turnWarning = warningText;
        StartCoroutine(TurnWarningTimer());
    }

    IEnumerator TurnWarningTimer() {
        yield return new WaitForSeconds(5f);
        isTurnWarningActive = false;
    }

    public void SetWildTurn(CardColor color) {
        wildColor = color;
        isWildTurn = true;
    }
}
