using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Linq.Expressions;
using System.Diagnostics;

namespace GameBase.WPF
{
    public class MathStringFormulaParser<T> : AbstractStringFormulaParser<T>
    {
        public MathStringFormulaParser() : base(s_grammer)
        {
        }

        private static readonly MathGrammer s_grammer = new MathGrammer();
        private static Dictionary<Type, IOperators> s_operators = new Dictionary<Type,IOperators>();

        static MathStringFormulaParser()
        {
            s_operators[typeof(SByte)] = s_operators[typeof(sbyte)] = new sbyteOperators();
            s_operators[typeof(Byte)] = s_operators[typeof(byte)] = new byteOperators();
            s_operators[typeof(Int16)] = s_operators[typeof(short)] = new shortOperators();
            s_operators[typeof(UInt16)] = s_operators[typeof(ushort)] = new ushortOperators();
            s_operators[typeof(Int32)] = s_operators[typeof(int)] = new intOperators();
            s_operators[typeof(UInt32)] = s_operators[typeof(uint)] = new uintOperators();
            s_operators[typeof(Int64)] = s_operators[typeof(long)] = new longOperators();
            s_operators[typeof(UInt64)] = s_operators[typeof(ulong)] = new ulongOperators();
            s_operators[typeof(float)] = new floatOperators();
            s_operators[typeof(Double)] = s_operators[typeof(double)] = new doubleOperators();
            s_operators[typeof(Decimal)] = s_operators[typeof(decimal)] = new decimalOperators();
        }

        private class MathGrammer : AbstractGrammer
        {
            public MathGrammer()
                : base
                    (
                         new Operator("(", OperatorType.GroupingOpen)
                        ,new Operator(")", OperatorType.GroupingClose)
                        ,new Operator("+", OperatorType.Binary, 3, typeof(AddProcess))
                        ,new Operator("-", OperatorType.Binary, 3, typeof(SubtractProcess))
                        ,new Operator("*", OperatorType.Binary, 2, typeof(MultiplyProcess))
                        ,new Operator("/", OperatorType.Binary, 2, typeof(DivideProcess))
//                        ,new Operator<int>("-", OperatorType.Unary, 1, typeof(NotProcess))
                    )
            {
            }

            protected override ConstantProcess GetConstantProcess(string token)
            {
                IOperators<T> ops = (IOperators<T>)s_operators[typeof(T)];
                if(ops.IsParsable(token))
                {
                    return new ConstantProcess(ops.Parse(token));
                }
                return null;
            }
        }

        private class AddProcess : AbstractBinaryOperator
        {
            public AddProcess(IEquationProcessor left, IEquationProcessor right)
                : base("+", left, right)
            { }

            protected override T Execute(T left, T right)
            {
                IOperators<T> ops = (IOperators<T>)s_operators[typeof(T)];
                return ops.Add(left,right);
            }

        }

        private class SubtractProcess : AbstractBinaryOperator
        {
            public SubtractProcess(IEquationProcessor left, IEquationProcessor right)
                : base("-", left, right)
            { }

            protected override T Execute(T left, T right)
            {
                IOperators<T> ops = (IOperators<T>)s_operators[typeof(T)];
                return ops.Subtract(left, right);
            }
        }

        private class MultiplyProcess : AbstractBinaryOperator
        {
            public MultiplyProcess(IEquationProcessor left, IEquationProcessor right)
                : base("*", left, right)
            { }

            protected override T Execute(T left, T right)
            {
                IOperators<T> ops = (IOperators<T>)s_operators[typeof(T)];
                return ops.Multiply(left, right);
            }
        }

        private class DivideProcess : AbstractBinaryOperator
        {
            public DivideProcess(IEquationProcessor left, IEquationProcessor right)
                : base("/", left, right)
            { }

            protected override T Execute(T left, T right)
            {
                IOperators<T> ops = (IOperators<T>)s_operators[typeof(T)];
                return ops.Divide(left, right);
            }
        }

        private class ModuloProcess : AbstractBinaryOperator
        {
            public ModuloProcess(IEquationProcessor left, IEquationProcessor right)
                : base("%", left, right)
            { }

            protected override T Execute(T left, T right)
            {
                IOperators<T> ops = (IOperators<T>)s_operators[typeof(T)];
                return ops.Modulo(left, right);
            }
        }

        private interface IOperators { }
        private interface IOperators<K> : IOperators
        {
            K Add(K var1, K var2);
            K Subtract(K var1, K var2);
            K Multiply(K var1, K var2);
            K Divide(K var1, K var2);
            K Modulo(K var1, K var2);
            K Negate(K var);
            bool IsParsable(string token);
            K Parse(string token);
        }

#pragma warning disable IDE1006
        private class sbyteOperators : IOperators<sbyte>
#pragma warning restore IDE1006
        {
            public sbyte Add(sbyte var1, sbyte var2) { return (sbyte)(var1 + var2); }
            public sbyte Subtract(sbyte var1, sbyte var2) { return (sbyte)(var1 - var2); }
            public sbyte Multiply(sbyte var1, sbyte var2) { return (sbyte)(var1 * var2); }
            public sbyte Divide(sbyte var1, sbyte var2) { return (sbyte)(var1 / var2); }
            public sbyte Modulo(sbyte var1, sbyte var2) { return (sbyte)(var1 % var2); }
            public sbyte Negate(sbyte var) { return (sbyte)-var; }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return sbyte.TryParse(token, style, null, out sbyte val);
            }
            public sbyte Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return sbyte.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class byteOperators : IOperators<byte>
#pragma warning restore IDE1006
        {
            public byte Add(byte var1, byte var2) { return (byte)(var1 + var2); }
            public byte Subtract(byte var1, byte var2) { return (byte)(var1 - var2); }
            public byte Multiply(byte var1, byte var2) { return (byte)(var1 * var2); }
            public byte Divide(byte var1, byte var2) { return (byte)(var1 / var2); }
            public byte Modulo(byte var1, byte var2) { return (byte)(var1 % var2); }
            public byte Negate(byte var) { return (byte)-var; }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return byte.TryParse(token, style, null, out byte val);
            }
            public byte Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return byte.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class shortOperators : IOperators<short>
#pragma warning restore IDE1006
        {
            public short Add(short var1, short var2) { return (short)(var1 + var2); }
            public short Subtract(short var1, short var2) { return (short)(var1 - var2); }
            public short Multiply(short var1, short var2) { return (short)(var1 * var2); }
            public short Divide(short var1, short var2) { return (short)(var1 / var2); }
            public short Modulo(short var1, short var2) { return (short)(var1 % var2); }
            public short Negate(short var) { return (short)-var; }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return short.TryParse(token, style, null, out short val);
            }
            public short Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return short.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class ushortOperators : IOperators<ushort>
#pragma warning restore IDE1006
        {
            public ushort Add(ushort var1, ushort var2) { return (ushort)(var1 + var2); }
            public ushort Subtract(ushort var1, ushort var2) { return (ushort)(var1 - var2); }
            public ushort Multiply(ushort var1, ushort var2) { return (ushort)(var1 * var2); }
            public ushort Divide(ushort var1, ushort var2) { return (ushort)(var1 / var2); }
            public ushort Modulo(ushort var1, ushort var2) { return (ushort)(var1 % var2); }
            public ushort Negate(ushort var) { return (ushort)-var; }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return ushort.TryParse(token, style, null, out ushort val);
            }
            public ushort Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return ushort.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class intOperators : IOperators<int>
#pragma warning restore IDE1006
        {
            public int Add(int var1, int var2) { return var1 + var2; }
            public int Subtract(int var1, int var2) { return var1 - var2; }
            public int Multiply(int var1, int var2) { return var1 * var2; }
            public int Divide(int var1, int var2) { return var1 / var2; }
            public int Modulo(int var1, int var2) { return var1 % var2; }
            public int Negate(int var) { return -var; }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return int.TryParse(token, style, null, out int val);
            }
            public int Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return int.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class uintOperators : IOperators<uint>
#pragma warning restore IDE1006
        {
            public uint Add(uint var1, uint var2) { return var1 + var2; }
            public uint Subtract(uint var1, uint var2) { return var1 - var2; }
            public uint Multiply(uint var1, uint var2) { return var1 * var2; }
            public uint Divide(uint var1, uint var2) { return var1 / var2; }
            public uint Modulo(uint var1, uint var2) { return var1 % var2; }
            public uint Negate(uint var) { return (uint)-var; }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return uint.TryParse(token, style, null, out uint val);
            }
            public uint Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return uint.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class longOperators : IOperators<long>
#pragma warning restore IDE1006
        {
            public long Add(long var1, long var2) { return var1 + var2; }
            public long Subtract(long var1, long var2) { return var1 - var2; }
            public long Multiply(long var1, long var2) { return var1 * var2; }
            public long Divide(long var1, long var2) { return var1 / var2; }
            public long Modulo(long var1, long var2) { return var1 % var2; }
            public long Negate(long var) { return -var; }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return long.TryParse(token, style, null, out long val);
            }
            public long Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return long.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class ulongOperators : IOperators<ulong>
#pragma warning restore IDE1006
        {
            public ulong Add(ulong var1, ulong var2) { return var1 + var2; }
            public ulong Subtract(ulong var1, ulong var2) { return var1 - var2; }
            public ulong Multiply(ulong var1, ulong var2) { return var1 * var2; }
            public ulong Divide(ulong var1, ulong var2) { return var1 / var2; }
            public ulong Modulo(ulong var1, ulong var2) { return var1 % var2; }
            public ulong Negate(ulong var) { throw new NotImplementedException(); }
            public bool IsParsable(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return ulong.TryParse(token, style, null, out ulong val);
            }
            public ulong Parse(string token)
            {
                var style = NumberStyles.Integer;
                token = token.ToLower();
                if (token.StartsWith("0x"))
                {
                    token = token.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                return ulong.Parse(token, style);
            }
        }

#pragma warning disable IDE1006
        private class floatOperators : IOperators<float>
#pragma warning restore IDE1006
        {
            public float Add(float var1, float var2) { return var1 + var2; }
            public float Subtract(float var1, float var2) { return var1 - var2; }
            public float Multiply(float var1, float var2) { return var1 * var2; }
            public float Divide(float var1, float var2) { return var1 / var2; }
            public float Modulo(float var1, float var2) { return var1 % var2; }
            public float Negate(float var) { return -var; }
            public bool IsParsable(string token) { return float.TryParse(token, out float val); }
            public float Parse(string token) { return float.Parse(token); }
        }

#pragma warning disable IDE1006
        private class doubleOperators : IOperators<double>
#pragma warning restore IDE1006
        {
            public double Add(double var1, double var2) { return var1 + var2; }
            public double Subtract(double var1, double var2) { return var1 - var2; }
            public double Multiply(double var1, double var2) { return var1 * var2; }
            public double Divide(double var1, double var2) { return var1 / var2; }
            public double Modulo(double var1, double var2) { return var1 % var2; }
            public double Negate(double var) { return -var; }
            public bool IsParsable(string token) { return double.TryParse(token, out double val); }
            public double Parse(string token) { return double.Parse(token); }
        }

#pragma warning disable IDE1006
        private class decimalOperators : IOperators<decimal>
#pragma warning restore IDE1006
        {
            public decimal Add(decimal var1, decimal var2) { return var1 + var2; }
            public decimal Subtract(decimal var1, decimal var2) { return var1 - var2; }
            public decimal Multiply(decimal var1, decimal var2) { return var1 * var2; }
            public decimal Divide(decimal var1, decimal var2) { return var1 / var2; }
            public decimal Modulo(decimal var1, decimal var2) { return var1 % var2; }
            public decimal Negate(decimal var) { return -var; }
            public bool IsParsable(string token) { return decimal.TryParse(token, out decimal val); }
            public decimal Parse(string token) { return decimal.Parse(token); }
        }
    }
}
