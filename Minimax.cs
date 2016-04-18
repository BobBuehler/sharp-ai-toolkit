using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SharpAiToolkit
{
    public static class Minimax
    {
        public static T Minimax(T node, int depth, Func<T, bool> isTerminal, Func<T, int> hCalc, Func<T, IEnumerable<T>> childCalc)
        {
            return childCalc(node).MaxByValue(child => Step(child, depth - 1, isTerminal, hCalc, childCalc));
        }
        
        public static int Step(T node, int depth, bool maximizingPlayer, Func<T, bool> isTerminal, Func<T, int> hCalc, Func<T, IEnumerable<T>> childCalc)
        {
            if (depth == 0 || isTerminal(node))
            {
                return hCalc(node);
            }
            
            if (maximizingPlayer)
            {
                return childCalc(node).Min(child => Set(child, depth - 1, false, isTerminal, hCalc, childCalc));
            }
            else
            {
                return childCalc(node).Max(child => Set(child, depth - 1, true, isTerminal, hCalc, childCalc));
            }
        }
    }
}