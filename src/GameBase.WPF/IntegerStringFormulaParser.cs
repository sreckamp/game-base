//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Globalization;

//namespace GameBase.WPF
//{
//    public class IntegerStringFormulaParser : AbstractStringFormulaParser<int>
//    {
//        public IntegerStringFormulaParser() : base(s_integerGrammer) { }

//        private static readonly IntegerGrammer s_integerGrammer = new IntegerGrammer();

//        private class IntegerGrammer : AbstractGrammer
//        {
//            public IntegerGrammer()
//                : base
//                    (
//                         new Operator("(", OperatorType.GroupingOpen)
//                        ,new Operator(")", OperatorType.GroupingClose)
//                        ,new Operator("+", OperatorType.Binary, 3, typeof(AddProcess))
//                        ,new Operator("-", OperatorType.Binary, 3, typeof(SubtractProcess))
//                        ,new Operator("*", OperatorType.Binary, 2, typeof(MultiplyProcess))
//                        ,new Operator("/", OperatorType.Binary, 2, typeof(DivideProcess))
//                        ,new Operator("%", OperatorType.Binary, 2, typeof(ModuloProcess))
////                        ,new Operator<int>("-", OperatorType.Unary, 1, typeof(NotProcess))
//                    )
//            {
//            }

//            protected override ConstantProcess GetConstantProcess(string token)
//            {
//                var style = NumberStyles.Integer;
//                token = token.ToLower();
//                if (token.StartsWith("0x"))
//                {
//                    token = token.Substring(2);
//                    style=NumberStyles.HexNumber;
//                }
//                try
//                {
//                    return new ConstantProcess(int.Parse(token, style));
//                }
//                catch (FormatException)
//                {
//                }
//                return null;
//            }
//        }

//        private class AddProcess : AbstractBinaryOperator
//        {
//            public AddProcess(IEquationProcessor left, IEquationProcessor right)
//                : base("+", left, right)
//            { }

//            protected override int Execute(int left, int right)
//            {
//                return left + right;
//            }
//        }

//        private class SubtractProcess : AbstractBinaryOperator
//        {
//            public SubtractProcess(IEquationProcessor left, IEquationProcessor right)
//                : base("-", left, right)
//            { }

//            protected override int Execute(int left, int right)
//            {
//                return left - right;
//            }
//        }

//        private class MultiplyProcess : AbstractBinaryOperator
//        {
//            public MultiplyProcess(IEquationProcessor left, IEquationProcessor right)
//                : base("*", left, right)
//            { }

//            protected override int Execute(int left, int right)
//            {
//                return left * right;
//            }
//        }

//        private class DivideProcess : AbstractBinaryOperator
//        {
//            public DivideProcess(IEquationProcessor left, IEquationProcessor right)
//                : base("/", left, right)
//            { }

//            protected override int Execute(int left, int right)
//            {
//                return left / right;
//            }
//        }

//        private class ModuloProcess : AbstractBinaryOperator
//        {
//            public ModuloProcess(IEquationProcessor left, IEquationProcessor right)
//                : base("%", left, right)
//            { }

//            protected override int Execute(int left, int right)
//            {
//                return left % right;
//            }
//        }
//    }
//}
