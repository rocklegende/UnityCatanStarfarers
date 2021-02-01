﻿using System;

public class NotEnoughResourcesException : Exception
{
    public NotEnoughResourcesException()
    {
    }

    public NotEnoughResourcesException(string message)
        : base(message)
    {
    }

    public NotEnoughResourcesException(string message, Exception inner)
        : base(message, inner)
    {
    }
}