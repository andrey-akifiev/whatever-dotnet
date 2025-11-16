namespace WhateverDotNet.Engine.Contexts
{
    public class SessionOptions
    {
        public int DefaultNavigationTimeoutInMilliseconds { get; set; }

        public int DefaultActionTimeoutInMilliseconds { get; set; }

        public bool Headless { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}