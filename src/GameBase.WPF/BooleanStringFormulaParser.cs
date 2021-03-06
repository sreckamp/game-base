﻿using System.Collections.Generic;

namespace GameBase.WPF
{
    public class BooleanStringFormulaParser : StringFormulaParser<bool>
    {
        public BooleanStringFormulaParser() : base(SGrammar) { }

        private static readonly BooleanGrammar SGrammar = new BooleanGrammar();

        private class BooleanGrammar : Grammar
        {
            private readonly Dictionary<string, ConstantProcess> m_constants =
                new Dictionary<string, ConstantProcess>();

            public BooleanGrammar()
                : base(
                        new Operator("(", OperatorType.GroupingOpen),
                        new Operator(")", OperatorType.GroupingClose),
                        new Operator("!", OperatorType.Unary, 4, typeof(NotProcess)),
                        new Operator("^", OperatorType.Binary, 3, typeof(XorProcess)),
                        new Operator("&&", OperatorType.Binary, 2, typeof(AndProcess)),
                        new Operator("*", OperatorType.Binary, 2, typeof(AndProcess)),
                        new Operator("||", OperatorType.Binary, 1, typeof(OrProcess)),
                        new Operator("+", OperatorType.Binary, 1, typeof(OrProcess))
                    )
            {
                m_constants.Add("T", new ConstantProcess(true));
                m_constants.Add("TRUE", new ConstantProcess(true));
                m_constants.Add("F", new ConstantProcess(false));
                m_constants.Add("FALSE", new ConstantProcess(false));
            }

            protected override ConstantProcess GetConstantProcess(string token)
            {
                return m_constants.ContainsKey(token) ? m_constants[token] : null;
            }
        }

        private class AndProcess : BinaryOperator
        {
            public AndProcess(IEquationProcessor left, IEquationProcessor right)
                : base("&&", left, right)
            { }

            protected override bool Execute(bool left, bool right)
            {
                return left && right;
            }
        }

        private class OrProcess : BinaryOperator
        {
            public OrProcess(IEquationProcessor left, IEquationProcessor right)
                : base("||", left, right)
            { }

            protected override bool Execute(bool left, bool right)
            {
                return left || right;
            }
        }

        private class XorProcess : BinaryOperator
        {
            public XorProcess(IEquationProcessor left, IEquationProcessor right)
                : base("^", left, right)
            { }

            protected override bool Execute(bool left, bool right)
            {
                return left ^ right;
            }
        }

        private class NotProcess : UnaryOperator
        {
            public NotProcess(IEquationProcessor value)
                : base("!", value)
            { }

            protected override bool Execute(bool value)
            {
                return !value;
            }
        }
    }
}
