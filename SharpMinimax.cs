using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SharpAiToolkit
{
    public interface ISharpMove
    {
    }
    
    public interface ISharpState<TMove> where TMove : ISharpMove
    {
        public ISharpState Next(TMove move);
    }
    
    public class SharpCacheValue<TMove, TState>
        where TMove : ISharpMove
        where TState : ISharpState<TMove>
    {
        public TState State;
        public IEnumberable<Tuple<TMove, TState>> Nexts;
        
        public bool IsNew;
        public int SelfFitness;
        public int ExpectedFitness;
        public int ExpectedFitnessDepth;
        
        SharpCacheValue(TState state)
        {
            State = state;
            Nexts = null;
            IsNew = true;
            SelfFitness = 0;
            ExpectedFitness = 0;
            ExpectedFitnessDepth = 0;
        }
    }

    public class SharpMinimax<TMove, TState>
        where TMove : ISharpMove
        where TState : ISharpState<TMove>
    {
        private Func<TState, bool> isTerminal;
        private Func<TState, int> calcFitness;
        private Func<TState, IEnumerable<TMove>> moveCalc;
        private LRUCache<TState, SharpCacheValue<TMove, TState>> stateCache;
        
        public Minimax(
            Func<TState, bool> isTerminal,
            Func<TState, int> calcFitness,
            Func<TState, IEnumerable<TMove>> moveCalc,
            int cacheCapacity)
        {
            this.isTerminal = isTerminal;
            this.calcFitness = calcFitness;
            this.childCalc = childCalc;
            
            this.stateCache = new LRUCache<TState, SharpCacheValue<TMove, TState>>(cacheCapacity);
            this.stateCache.OnMiss = state => new SharpCacheValue<TMove, TState>(state);
        }
        
        public TMove Search(TState state, int depth)
        {
            return Search(state, depth, int.MinValue, int.MaxValue);
        }
        
        public TMove Search(TState state, int depth, bool maximizingPlayer, int a, int b)
        {
            var cacheValue = stateCache[state];
            SearchInto(cacheValue, depth, maximizingPlayer, a, b));
            return cacheValue.Nexts.MaxByValue(next => stateCache[next.Second].ExpectedFitness).First;
        }
        
        // Need to document what this is and isn't doing
        private void SearchInto(SharpCacheValue<TMove, TState> cacheValue, int depth, bool maximizingPlayer, int a, int b)
        {
            if (!cacheValue.IsNew && cacheValue.ExpectedFitnessDepth >= depth)
            {
                // We've totally done this search before and maybe even went deeper
                return;
            }
            
            if ((depth == 0 || isTerminal(cacheValue.state))
            {
                if (cacheValue.IsNew)
                {
                    cacheValue.IsNew = false;
                    cacheValue.Fitness = calcFitness(next.State);
                    cacheValue.ExpectedFitness = cacheValue.Fitness;
                    cacheValue.ExpectedFitnessDepth = depth;
                }
                return;
            }
            
            if (cacheValue.Nexts == null)
            {
                cacheValue.Nexts = childCalc(cacheValue.state)
                    .Select(move => new Tuple<TMove, TState>(move, cacheValue.state.Next(move)))
                    .ToLazyList();
            }
            
            var bestNextValue = GetBest(
                OrderBestFirst(cacheValue.Nexts.Select(next => stateCache[next.State]), maximizingPlayer)
                .Select(value => SearchInto(value, depth - 1, !maximizingPlayer, a, b))
                .While(value => !ShouldABPrune(value.ExpectedFitness, maximizingPlayer, ref a, ref b)));
            
            cacheValue.ExpectedFitness = bestNextValue.ExpectedFitness;
            cacheValue.ExpectedFitnessDepth = depth;
        }
        
        // Putting expected best choices first we can AB prune more frequently
        private OrderBestFirst(IEnumerable<SharpCacheValue<TMove, TState>> values, bool max)
        {
            if (max)
            {
                return values.OrderByDesc(value => value.IsNew ? int.MinValue : value.ExpectedFitness);
            }
            else
            {
                return values.OrderBy(value => value.IsNew ? int.MaxValue : value.ExpectedFitness);
            }
        }
        
        // Update alpha or beta and return true if there is no need to expore more options
        private ShouldABPrune(int fitness, bool max, ref int a, ref int b)
        {
            if (max)
            {
                a = Math.Max(a, fitness);
            }
            else
            {
                b = Math.Min(b, fitness);
            }
            return a > b;
        }
        
        private GetBest(IEnumerable<SharpCacheValue<TMove, TState>> values, bool max)
        {
            if (max)
            {
                return values.MaxByValue(v => v.ExpectedFitness);
            }
            else
            {
                return values.MinByValue(v => v.ExpectedFitness);
            }
        }
    }
}