using System;
using System.Collections.Generic;
using System.Globalization;

namespace GameBase.WPF
{
    public class MathStringFormulaParser<T> : AbstractStringFormulaParser<T>
    {
        public MathStringFormulaParser() : base(SGrammer)
        {
        }

        private static readonly MathGrammer SGrammer = new MathGrammer();
        private static Dictionary<Type, IOperators> _sOperators = new Dictionary<Type,IOperators>();

        static MathStringFormulaParser()
        {
            _sOperators[typeof(SByte)] = _sOperators[typeof(sbyte)] = new SbyteOperators();
            _sOperators[typeof(Byte)] = _sOperators[typeof(byte)] = new ByteOperators();
            _sOperators[typeof(Int16)] = _sOperators[typeof(short)] = new ShortOperators();
            _sOperators[typeof(UInt16)] = _sOperators[typeof(ushort)] = new UshortOperators();
            _sOperators[typeof(Int32)] = _sOperators[typeof(int)] = new IntOperators();
            _sOperators[typeof(UInt32)] = _sOperators[typeof(uint)] = new UintOperators();
            _sOperators[typeof(Int64)] = _sOperators[typeof(long)] = new LongOperators();
            _sOperators[typeof(UInt64)] = _sOperators[typeof(ulong)] = new UlongOperators();
            _sOperators[typeof(float)] = new FloatOperators();
            _sOperators[typeof(Double)] = _sOperators[typeof(double)] = new DoubleOperators();
            _sOperators[typeof(Decimal)] = _sOperators[typeof(decimal)] = new DecimalOperators();
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
                var ops = (IOperators<T>)_sOperators[typeof(T)];
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
                var ops = (IOperators<T>)_sOperators[typeof(T)];
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
                var ops = (IOperators<T>)_sOperators[typeof(T)];
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
                var ops = (IOperators<T>)_sOperators[typeof(T)];
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
                var ops = (IOperators<T>)_sOperators[typeof(T)];
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
                var ops = (IOperators<T>)_sOperators[typeof(T)];
                return ops.Modulo(left, right);
            }
        }

        private interface IOperators { }
        private interface IOperators<TK> : IOperators
        {
            TK Add(TK var1, TK var2);
            TK Subtract(TK var1, TK var2);
            TK Multiply(TK var1, TK var2);
            TK Divide(TK var1, TK var2);
            TK Modulo(TK var1, TK var2);
            TK Negate(TK var);
            bool IsParsable(string token);
            TK Parse(string token);
        }

#pragma warning disable IDE1006
        private class SbyteOperators : IOperators<sbyte>
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
                return sbyte.TryParse(token, style, null, out var val);
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
        private class ByteOperators : IOperators<byte>
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
                return byte.TryParse(token, style, null, out var val);
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
        private class ShortOperators : IOperators<short>
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
                return short.TryParse(token, style, null, out var val);
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
        private class UshortOperators : IOperators<ushort>
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
                return ushort.TryParse(token, style, null, out var val);
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
        private class IntOperators : IOperators<int>
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
                return int.TryParse(token, style, null, out var val);
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
        private class UintOperators : IOperators<uint>
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
                return uint.TryParse(token, style, null, out var val);
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
        private class LongOperators : IOperators<long>
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
                return long.TryParse(token, style, null, out var val);
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
        private class UlongOperators : IOperators<ulong>
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
                return ulong.TryParse(token, style, null, out var val);
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
        private class FloatOperators : IOperators<float>
#pragma warning restore IDE1006
        {
            public float Add(float var1, float var2) { return var1 + var2; }
            public float Subtract(float var1, float var2) { return var1 - var2; }
            public float Multiply(float var1, float var2) { return var1 * var2; }
            public float Divide(float var1, float var2) { return var1 / var2; }
            public float Modulo(float var1, float var2) { return var1 % var2; }
            public float Negate(float var) { return -var; }
            public bool IsParsable(string token) { return float.TryParse(token, out var val); }
            public float Parse(string token) { return float.Parse(token); }
        }

#pragma warning disable IDE1006
        private class DoubleOperators : IOperators<double>
#pragma warning restore IDE1006
        {
            public double Add(double var1, double var2) { return var1 + var2; }
            public double Subtract(double var1, double var2) { return var1 - var2; }
            public double Multiply(double var1, double var2) { return var1 * var2; }
            public double Divide(double var1, double var2) { return var1 / var2; }
            public double Modulo(double var1, double var2) { return var1 % var2; }
            public double Negate(double var) { return -var; }
            public bool IsParsable(string token) { return double.TryParse(token, out var val); }
            public double Parse(string token) { return double.Parse(token); }
        }

#pragma warning disable IDE1006
        private class DecimalOperators : IOperators<decimal>
#pragma warning restore IDE1006
        {
            public decimal Add(decimal var1, decimal var2) { return var1 + var2; }
            public decimal Subtract(decimal var1, decimal var2) { return var1 - var2; }
            public decimal Multiply(decimal var1, decimal var2) { return var1 * var2; }
            public decimal Divide(decimal var1, decimal var2) { return var1 / var2; }
            public decimal Modulo(decimal var1, decimal var2) { return var1 % var2; }
            public decimal Negate(decimal var) { return -var; }
            public bool IsParsable(string token) { return decimal.TryParse(token, out var val); }
            public decimal Parse(string token) { return decimal.Parse(token); }
        }
    }
}
