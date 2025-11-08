using Reqnroll;

namespace WhateverDotNet.Reqnroll.StepDefinitions;

[Binding]
public class CommonSteps
{
    private readonly List<int> _numbers = new List<int>();
    private int _result = 0;
    
    [Given(@"I have entered {int} into the calculator")]
    public void IEnteredIntoTheCalculator(int number)
    {
        _numbers.Add(number);
    }

    [When("I press add")]
    public void WhenIPressAdd()
    {
        _numbers.ForEach(i => _result += i);
    }

    [Then("the result should be {int} on the screen")]
    public void ThenTheResultShouldBeOnTheScreen(int expectedResult)
    {
        if (_result != expectedResult)
        {
            throw new Exception($"Expected result to be {expectedResult}, but was {_result}.");
        }
    }
}