﻿using System;

namespace Chihiro.Attributes
{
    /// <summary>
    /// Services with this class will be started at the bot initialization/startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoStartAttribute : Attribute
    {
        
    }
}