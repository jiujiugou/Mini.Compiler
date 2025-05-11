﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Text
{
    public class VariableSymbol
    {
        public VariableSymbol(string name,bool isReadOnly,Type type)
        {
            Name = name;
            IsReadOnly = isReadOnly;
            Type = type;
        }
        public string Name { get; }
        public bool IsReadOnly { get; }
        public Type Type { get; }       
    }
}
