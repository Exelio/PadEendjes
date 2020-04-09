using View;

public class RewardBehaviour
{
    private RewardVariables _variables;
    private RewardView _view;

    public RewardBehaviour(RewardView view)
    {
        _view = view;
        _variables = _view.Variables;
    }

    public void CompletedLevel(bool result) { _variables.IsLevelComplete = result; }

    public void CaughtDuck() { _variables.DuckCount += 1; }
    public void LostDuck() { _variables.DuckCount -= 1; }

    public void AddMistake() { _variables.MistakeCount += 1; }

    public bool CheckMistakes() =>  _variables.MistakeCount <= _variables.MaxMistakes;

    public bool CheckDucks() => _variables.DuckCount >= _variables.DucksNeeded;
}
