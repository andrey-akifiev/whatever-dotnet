namespace WhateverDotNet.TestApp.Components.Models;

public interface ISampleProvider<TSelf>
    where TSelf: ISampleProvider<TSelf>
{
    static abstract TSelf CreateSample();
}