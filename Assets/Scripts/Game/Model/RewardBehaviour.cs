using UnityEngine;
using View;

public class RewardBehaviour
{
    private RewardVariables _variables;
    private RewardView _view;

    public RewardBehaviour(RewardView view)
    {
        _view = view;
        _variables = _view.Variables;
        _view.Variables.DucksMax.text = _variables.MaxDucks.ToString();
    }

    public void CompletedLevel(bool result) { _variables.IsLevelComplete = result; }

    public void CaughtDuck() { _variables.DuckCount += 1;
        _view.Variables.DucksCollected.text = _variables.DuckCount.ToString(); 
        _view.Variables.DucksMax.text = _variables.MaxDucks.ToString(); 
        Debug.Log($"{_variables.DuckCount} ducks/ {_variables.DucksNeeded} ducks TO complete level \n{_variables.DuckCount} ducks/ {_variables.MaxDucks}"); }
    public void LostDuck() { _variables.DuckCount -= 1; }

    public void AddMistake() 
    {
        _variables.MistakeCount += 1;
        Debug.Log($"{_variables.MistakeCount}");
    }

    public bool CheckMistakes() =>  _variables.MistakeCount <= _variables.MaxMistakes;

    public bool CheckDucks() => _variables.DuckCount >= _variables.DucksNeeded;

    public bool CheckMaxDucks() => _variables.DuckCount >= _variables.MaxDucks;
}
