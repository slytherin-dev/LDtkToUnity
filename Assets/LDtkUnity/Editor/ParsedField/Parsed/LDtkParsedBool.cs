﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LDtkUnity.Editor
{
    [UsedImplicitly]
    internal class LDtkParsedBool : ILDtkValueParser
    {
        bool ILDtkValueParser.TypeName(FieldInstance instance) => instance.IsBool;

        public object ImportString(object input)
        {
            //bool can never be null but just in case
            if (input == null)
            {
                LDtkDebug.LogWarning("Bool field was unexpectedly null");
                return false;
            }
            
            return Convert.ToBoolean(input);
        }
    }
}