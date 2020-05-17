using System;
using System.Collections.Generic;

namespace GameBase.WPF
{
    public abstract class AbstractStringFormulaParser<T> where T:struct
    {
        private static readonly Operator Nop = new Operator(string.Empty, OperatorType.Nop);
        private readonly Dictionary<string, IEquationProcessor> m_cache = new Dictionary<string, IEquationProcessor>();
        private readonly Grammar m_grammar;

        protected AbstractStringFormulaParser(Grammar grammar)
        {
            m_grammar = grammar;
        }

        public bool IsConstant(string token)
        {
            return m_grammar.IsConstant(token);
        }

        public IEquationProcessor BuildProcessor(string formula)
        {
            if (m_cache.ContainsKey(formula)) return m_cache[formula];
            var toParse = formula;
            var values = new Stack<IEquationProcessor>();
            var operators = new Stack<Operator>();
            while (toParse.Length > 0)
            {
                var idx = toParse.Length;
                var op = Nop;
                foreach (var o in m_grammar.Operators)
                {
                    var tmp = toParse.IndexOf(o.Symbol, StringComparison.Ordinal);
                    if (tmp < 0 || tmp >= idx) continue;
                    idx = tmp;
                    op = o;
                }
                if (op != null)
                {
                    if (idx > 0)
                    {
                        values.Push(m_grammar.GetConstantOrVariableProcess(toParse.Substring(0, idx)));
                    }
                    ParseOperator(op, operators, values);
                    toParse = toParse.Substring(idx + op.Symbol.Length);
                }
                else
                {
                    values.Push(m_grammar.GetConstantOrVariableProcess(toParse));
                    toParse = string.Empty;
                }
            }
            while (operators.Count > 0)
            {
                ImplementOperator(operators.Pop(), values);
            }
            if (values.Count > 1)
            {
                throw new FormatException("Formula not parsable.");
            }
            m_cache[formula] = values.Pop();
            return m_cache[formula];
        }

        private static void ParseOperator(Operator op, Stack<Operator> operatorStack, Stack<IEquationProcessor> valueStack)
        {
            if (op.OperatorType == OperatorType.GroupingClose)
            {
                while (operatorStack.Peek().OperatorType != OperatorType.GroupingOpen)
                {
                    ImplementOperator(operatorStack.Pop(), valueStack);
                }

                operatorStack.Pop();
            }
            else
            {
                if (op.OperatorType != OperatorType.GroupingOpen)
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek().Precedence > op.Precedence)
                    {
                        ImplementOperator(operatorStack.Pop(), valueStack);
                    }
                }

                operatorStack.Push(op);
            }
        }

        private static void ImplementOperator(Operator op, Stack<IEquationProcessor> valueStack)
        {
            var argTypes = new Type[op.ArgumentCount];
            var args = new object[op.ArgumentCount];
            for (var i = 0; i < args.Length; i++)
            {
                argTypes[i] = typeof(IEquationProcessor);
                args[args.Length - 1 - i] = valueStack.Pop();
            }
            valueStack.Push((IEquationProcessor)op.OperatorImplementation.GetConstructor(argTypes).Invoke(args));
        }

        protected abstract class Grammar
        {
            protected Grammar(params Operator[] operators)
            {
                Operators = operators;
            }

            public Operator[] Operators { get; }

            public bool IsConstant(string token)
            {
                return GetConstantProcess(token) != null;
            }

            protected abstract ConstantProcess GetConstantProcess(string token);

            public IEquationProcessor GetConstantOrVariableProcess(string token)
            {
                var proc = GetConstantProcess(token) ?? (IEquationProcessor)new VariableProcess(token);
                return proc;
            }
        }

        protected enum OperatorType
        {
            Nop,
            GroupingOpen,
            GroupingClose,
            Unary,
            Binary
        }

        protected class Operator
        {
            public Operator(string symbol, OperatorType operatorType)
                : this(symbol, operatorType, 0, typeof(NoOperator))
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="symbol"></param>
            /// <param name="operatorType"></param>
            /// <param name="precedence">1 is the lowest (N/A Grouping Operator)</param>
            /// <param name="operatorImplementation"></param>
            public Operator(string symbol, OperatorType operatorType, int precedence, Type operatorImplementation)
            {
                Symbol = symbol;
                OperatorType = operatorType;
                ArgumentCount = OperatorType switch
                {
                    OperatorType.Binary => 2,
                    OperatorType.Unary => 1,
                    _ => 0
                };
                Precedence = precedence;
                OperatorImplementation = operatorImplementation;
                if (OperatorType == OperatorType.GroupingOpen || OperatorType == OperatorType.GroupingClose) return;
                if (Precedence < 1) throw new ArgumentException("Precedence must be 1 or higher.");
                if (operatorImplementation?.IsSubclassOf(typeof(IEquationProcessor))
                    ?? throw new ArgumentNullException(nameof(operatorImplementation)))
                {
                    throw new NullReferenceException("OperatorImplementation must implement IEquationProcessor.");
                }
            }

            public int ArgumentCount { get; }
            public string Symbol { get; }
            public OperatorType OperatorType { get; }
            public int Precedence { get; }
            public Type OperatorImplementation { get; }

            public override string ToString()
            {
                return Symbol;
            }
        }

        public interface IEquationProcessor
        {
            T Calculate(Dictionary<string, T> values);
            string ToValueString(Dictionary<string, T> values);
        }

        protected class ConstantProcess : IEquationProcessor
        {
            private readonly T m_value;
            public ConstantProcess(T value)
            {
                m_value = value;
            }

            public T Calculate(Dictionary<string, T> values)
            {
                return m_value;
            }

            public string ToValueString(Dictionary<string, T> values)
            {
                return ToString();
            }

            public override string ToString()
            {
                return m_value.ToString();
            }
        }

        private class VariableProcess : IEquationProcessor
        {
            private readonly string m_key;
            public VariableProcess(string key)
            {
                m_key = key;
            }

            public T Calculate(Dictionary<string, T> values)
            {
                return values.ContainsKey(m_key) ? values[m_key] : default;
            }

            public string ToValueString(Dictionary<string, T> values)
            {
                return values[m_key].ToString();
            }

            public override string ToString()
            {
                return m_key;
            }
        }

        private class NoOperator : IEquationProcessor
        {
            public T Calculate(Dictionary<string, T> values) => default;

            public string ToValueString(Dictionary<string, T> values) => string.Empty;
        }

        protected abstract class AbstractUnaryOperator : IEquationProcessor
        {
            private readonly IEquationProcessor m_value;
            private readonly string m_symbol;

            protected AbstractUnaryOperator(string symbol, IEquationProcessor value)
            {
                m_value = value;
                m_symbol = symbol;
            }

            public T Calculate(Dictionary<string, T> values)
            {
                return Execute(m_value.Calculate(values));
            }

            protected abstract T Execute(T value);

            public string ToValueString(Dictionary<string, T> values)
            {
                return $"{m_symbol}{m_value.ToValueString(values)}";
            }

            public override string ToString()
            {
                return $"{m_symbol}{m_value}";
            }
        }

        protected abstract class BinaryOperator : IEquationProcessor
        {
            private readonly IEquationProcessor m_left;
            private readonly IEquationProcessor m_right;
            private readonly string m_symbol;

            protected BinaryOperator(string symbol, IEquationProcessor left, IEquationProcessor right)
            {
                m_left = left;
                m_right = right;
                m_symbol = symbol;
            }

            public T Calculate(Dictionary<string, T> values)
            {
                return Execute(m_left.Calculate(values), m_right.Calculate(values));
            }

            protected abstract T Execute(T left, T right);

            public string ToValueString(Dictionary<string, T> values)
            {
                return $"({m_left.ToValueString(values)}{m_symbol}{m_right.ToValueString(values)})";
            }

            public override string ToString()
            {
                return $"({m_left}{m_symbol}{m_right})";
            }
        }
    }
}
