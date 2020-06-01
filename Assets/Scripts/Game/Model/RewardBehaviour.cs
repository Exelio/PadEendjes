using System;
using UnityEngine;
using UnityEngine.UI;
using View;

public class RewardBehaviour
{
    private RewardVariables _variables;
    private RewardView _view;

    private int _coins = 0;

    public RewardBehaviour(RewardView view)
    {
        _view = view;
        _variables = _view.Variables;
        ChangeText();
    }

    public void CompletedLevel()
    {
        ChangeUI();
        _variables.EndPanel.SetActive(true);
    }

    private void ChangeUI()
    {
        //mistakes
        string text = $"Maximum {_variables.MaxMistakes} fouten, {_variables.MistakeCount} fouten gemaakt";
        if (CheckMistakes())
            ChangeTextAndStar(_variables.Star, text, _variables.Mistakes, _variables.StarMistakes);
        else
            ChangeTextAndStar(_variables.NoStar, text, _variables.Mistakes, _variables.StarMistakes);

        //max ducks
        text = $"{_variables.DuckCount} / {_variables.MaxDucks} eendjes gevonden";
        if (CheckMaxDucks())
            ChangeTextAndStar(_variables.Star, text, _variables.FoundAllDucks, _variables.StarFoundAllDucks);
        else
            ChangeTextAndStar(_variables.NoStar, text, _variables.FoundAllDucks, _variables.StarFoundAllDucks);

        //ducks needed
        text = $"{_variables.DuckCount} / {_variables.DucksNeeded} gevraagde eendjes gevonden";
        if (CheckEnoughDucks())
            ChangeTextAndStar(_variables.Star, text, _variables.LevelComplete, _variables.StarLevelComplete);
        else
            ChangeTextAndStar(_variables.NoStar, text, _variables.LevelComplete, _variables.StarLevelComplete);

        CheckStars();
    }

    private void CheckStars()
    {
        int starCount = 0;

        if (CheckEnoughDucks() && CheckMaxDucks() && CheckMistakes()) starCount = 3;
        else if ((CheckEnoughDucks() && CheckMaxDucks()) || (CheckEnoughDucks() && CheckMistakes())) starCount = 2;
        else starCount = 1;

        for (int i = 0; i < _variables.TopPanelStars.Length; i++)
        {
            if (i < starCount) _variables.TopPanelStars[i].sprite = _variables.Star;
            else _variables.TopPanelStars[i].sprite = _variables.NoStar;
        }
    }

    private void ChangeTextAndStar(Sprite image, string text, Text uiText, Image UIimage)
    {
        UIimage.sprite = image;
        uiText.text = text;
    }

    public void CaughtDuck(int duckcount)
    {
        _variables.DuckCount = duckcount;
        ChangeText();
    }

    public void ChangeText()
    {
        _view.Variables.DuckCounter.text = _variables.DuckCount + "/" + _view.MaxDuckAmount;
        _variables.CoinCounter.text = _coins.ToString();
    }

    public void LostDuck() { _variables.DuckCount--; ChangeText(); }

    public void AddMistake() 
    {
        _variables.MistakeCount += 1;
    }

    public bool CheckMistakes() =>  _variables.MistakeCount <= _variables.MaxMistakes;

    public bool CheckEnoughDucks() => _variables.DuckCount >= _variables.DucksNeeded;

    public bool CheckMaxDucks() => _variables.DuckCount >= _variables.MaxDucks;

    internal void AddCoin(int obj)
    {
        _coins += obj;
        ChangeText();
    }
}