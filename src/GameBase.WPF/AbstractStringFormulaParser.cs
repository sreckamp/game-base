using System;
using System.Collections.Generic;

namespace GameBase.WPF
{
    public abstract class AbstractStringFormulaParser<T>
    {
        private readonly Dictionary<string, IEquationProcessor> m_cache = new Dictionary<string, IEquationProcessor>();
        private readonly AbstractGrammer m_grammer;

        protected AbstractStringFormulaParser(AbstractGrammer grammer)
        {
            m_grammer = grammer;
        }

        public bool IsConstant(string token)
        {
            return m_grammer.IsConstant(token);
        }

        public IEquationProcessor BuildProcessor(string formula)
        {
            if (!m_cache.ContainsKey(formula))
            {
                var toParse = formula;
                var values = new Stack<IEquationProcessor>();
                var operators = new Stack<Operator>();
                while (toParse.Length > 0)
                {
                    var idx = toParse.Length;
                    Operator op = null;
                    foreach (var o in m_grammer.Operators)
                    {
                        var tmp = toParse.IndexOf(o.Symbol);
                        if (tmp >= 0 && tmp < idx)
                        {
                            idx = tmp;
                            op = o;
                        }
                    }
                    if (op != null)
                    {
                        if (idx > 0)
                        {
                            values.Push(m_grammer.GetConstantOrVariableProcess(toParse.Substring(0, idx)));
                        }
                        ParseOperator(op, operators, values);
                        toParse = toParse.Substring(idx + op.Symbol.Length);
                    }
                    else
                    {
                        values.Push(m_grammer.GetConstantOrVariableProcess(toParse));
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
            }
            return m_cache[formula];
        }

        private void ParseOperator(Operator op, Stack<Operator> operatorStack, Stack<IEquationProcessor> valueStack)
        {
            if (op != null)
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
        }

        private void ImplementOperator(Operator op, Stack<IEquationProcessor> valueStack)
        {
            var argTypes = new Type[op.ArgumentCount];
            var args = new object[op.ArgumentCount];
            for (var i = 0; i < args.Length; i++)
            {
                argTypes[i] = typeof(IEquationProcessor);
                args[(args.Length - 1) - i] = valueStack.Pop();
            }
            valueStack.Push(op.OperatorImplementation.GetConstructor(argTypes).Invoke(args) as IEquationProcessor);
        }

        protected abstract class AbstractGrammer
        {
            public AbstractGrammer(params Operator[] operators)
            {
                Operators = operators;
            }

            public Operator[] Operators { get; private set; }

            public bool IsConstant(string token)
            {
                return GetConstantProcess(token) != null;
            }

            protected abstract ConstantProcess GetConstantProcess(string token);

            public IEquationProcessor GetConstantOrVariableProcess(string token)
            {
                IEquationProcessor proc = null;
                proc = GetConstantProcess(token) ?? (IEquationProcessor)new VariableProcess(token);
                return proc;
            }
        }

        protected enum OperatorType
        {
            GroupingOpen,
            GroupingClose,
            Unary,
            Binary
        }

        protected class Operator
        {
            public Operator(string symbol, OperatorType operatorType)
                : this(symbol, operatorType, 0, null)
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
                ArgumentCount = 0;
                if (OperatorType == OperatorType.Binary)
                {
                    ArgumentCount = 2;
                }
                else if (OperatorType == OperatorType.Unary)
                {
                    ArgumentCount = 1;
                }
                Precedence = precedence;
                OperatorImplementation = operatorImplementation;
                if (OperatorType != OperatorType.GroupingOpen && OperatorType != OperatorType.GroupingClose)
                {
                    if (Precedence < 1) throw new ArgumentException("Precedence must be 1 or higher.");
                    if (operatorImplementation?.IsSubclassOf(typeof(IEquationProcessor))
                        ?? throw new ArgumentNullException("operatorImplementation"))
                    {
                        throw new NullReferenceException("OperatorImplementation must implement IEquationProcessor.");
                    }
                }
            }

            public int ArgumentCount { get; private set; }
            public string Symbol { get; private set; }
            public OperatorType OperatorType { get; private set; }
            public int Precedence { get; private set; }
            public Type OperatorImplementation { get; private set; }

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

        protected class VariableProcess : IEquationProcessor
        {
            private readonly string m_key;
            public VariableProcess(string key)
            {
                m_key = key;
            }

            public T Calculate(Dictionary<string, T> values)
            {
                if (values.ContainsKey(m_key))
                {
                    return values[m_key];
                }
                return default(T);
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

        protected abstract class AbstractUnaryOperator : IEquationProcessor
        {
            private readonly IEquationProcessor m_value;
            private readonly string m_symbol;

            public AbstractUnaryOperator(string symbol, IEquationProcessor value)
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

        protected abstract class AbstractBinaryOperator : IEquationProcessor
        {
            private readonly IEquationProcessor m_left;
            private readonly IEquationProcessor m_right;
            private readonly string m_symbol;

            protected AbstractBinaryOperator(string symbol, IEquationProcessor left, IEquationProcessor right)
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
