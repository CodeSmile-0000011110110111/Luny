using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Luny
{
	[Serializable]
	public readonly struct Variable : IEquatable<Variable>, IEquatable<Boolean>, IEquatable<Double>, IEquatable<String>
	{
		private static Int32 s_UniqueNameID;

		public enum ValueType
		{
			Number,
			Boolean,
			String,
			Reference,
		}

		private const String DefaultName = null;

#if DEBUG || LUNY_DEBUG
		private readonly String _name;
		public String Name => _name ?? DefaultName;
#else
		public String Name => DefaultName;
#endif

		private readonly Double _numValue;
		private readonly Object _string;
		private readonly ValueType _type;

		public ValueType Type => _type;

		public Boolean IsTrue => (_type == ValueType.Boolean || _type == ValueType.Number) && Math.Abs(_numValue) > Double.Epsilon;
		public Boolean IsHigh => _type == ValueType.Number && Math.Abs(_numValue) >= 0.5;
		public Boolean IsNormalized => _type == ValueType.Number && Math.Abs(_numValue) <= 1.0;

		public Double Value => _type switch
		{
			ValueType.Number => _numValue,
			ValueType.Boolean => _numValue,
			var _ => throw new InvalidOperationException($"Attempt to get number value from {_type}: {this}"),
		};
		public Object Object => _type switch
		{
			ValueType.String => _string,
			var _ => _numValue,
		};
		public Boolean IsNull => _type == ValueType.String && _string == null;
		public Boolean IsNullOrEmpty => _type == ValueType.String && String.IsNullOrEmpty((String)_string);
		public Boolean IsNullOrWhitespace => _type == ValueType.String && String.IsNullOrWhiteSpace((String)_string);

		private Variable(Double value, ValueType type, String name = null)
		{
			_numValue = value;
			_string = null;
			_type = type;
#if DEBUG || LUNY_DEBUG
			_name = String.IsNullOrWhiteSpace(name) ? GenerateUniqueName(type, _string, _numValue) : name;
			if (Double.IsNaN(_numValue))
				LunyLogger.LogWarning($"Variable {name}: value is 'NaN' (not a number)");
			if (Double.IsInfinity(_numValue))
				LunyLogger.LogWarning($"Variable {name}: value is 'Infinity' ('division by zero' or value overflow)");
#endif
		}

		private Variable(Object value, ValueType type, String name = null)
		{
			_numValue = 0;
			_string = value;
			_type = type;
#if DEBUG || LUNY_DEBUG
			_name = String.IsNullOrWhiteSpace(name) ? GenerateUniqueName(type, _string, _numValue) : name;
#endif
		}

#if DEBUG || LUNY_DEBUG
		private static String GenerateUniqueName(ValueType type, Object refValue, Double numValue) => null;
		//$"Literal: {(type == ValueType.Number ? numValue : type == ValueType.Boolean ? Math.Abs(numValue) > Double.Epsilon : refValue)}";
#endif

		public static Variable Named(Boolean value, String name) => new(value ? 1.0 : 0.0, ValueType.Boolean, name);
		public static Variable Named(Double value, String name) => new(value, ValueType.Number, name);
		public static Variable Named(Single value, String name) => new(value, ValueType.Number, name);
		public static Variable Named(Int32 value, String name) => new(value, ValueType.Number, name);
		public static Variable Named(Int64 value, String name) => new(value, ValueType.Number, name);
		public static Variable Named(Number value, String name) => new(value, ValueType.Number, name);
		public static Variable Named(String value, String name) => new(value, ValueType.String, name);

		public static Variable Named(Object value, String name) => value switch
		{
			Boolean b => Named(b, name),
			Double d => Named(d, name),
			Single f => Named(f, name),
			Int32 i => Named(i, name),
			Int64 l => Named(l, name),
			Number n => Named(n, name),
			String s => Named(s, name),
			Variable v => new Variable(v._numValue, v._type, name),
			var _ => value == null
				? new Variable(0d, ValueType.Number, name)
				: throw new NotSupportedException($"Unsupported Type: {value.GetType().Name}"),
		};

		public Boolean AsBoolean() => IsTrue;
		public Number AsNumber() => _type == ValueType.Number ? _numValue : 0.0;
		public Single AsSingle() => _type == ValueType.Number ? (Single)_numValue : 0f;
		public Double AsDouble() => _type == ValueType.Number ? _numValue : 0.0;
		public Int32 AsInt32() => _type == ValueType.Number ? (Int32)_numValue : 0;
		public Int64 AsInt64() => _type == ValueType.Number ? (Int64)_numValue : 0L;

		public String AsString() => _type switch
		{
			ValueType.Number => Convert.ToString(_numValue, CultureInfo.InvariantCulture),
			ValueType.Boolean => Convert.ToString(AsBoolean()),
			ValueType.String => _string as String ?? String.Empty,
			var _ => throw new ArgumentOutOfRangeException(_type.ToString()),
		};

		public T As<T>()
		{
			if (TryRead<T>(out var result))
				return result;

			throw new NotSupportedException($"Type {typeof(T).Name} is not supported by {nameof(Variable)} (Current Type: {_type})");
		}

		private Boolean TryRead<T>(out T result)
		{
			var t = typeof(T);

			switch (_type)
			{
				case ValueType.Number:
					if (t == typeof(Single))
					{
						var v = (Single)_numValue;
						result = Unsafe.As<Single, T>(ref v);
						return true;
					}
					if (t == typeof(Int32))
					{
						var v = (Int32)_numValue;
						result = Unsafe.As<Int32, T>(ref v);
						return true;
					}
					if (t == typeof(Int64))
					{
						var v = (Int64)_numValue;
						result = Unsafe.As<Int64, T>(ref v);
						return true;
					}
					if (t == typeof(Double))
					{
						var v = _numValue;
						result = Unsafe.As<Double, T>(ref v);
						return true;
					}
					if (t == typeof(Number))
					{
						var v = (Number)_numValue;
						result = Unsafe.As<Number, T>(ref v);
						return true;
					}
					if (t == typeof(Object))
					{
						result = (T)(Object)_numValue;
						return true;
					}
					break;

				case ValueType.Boolean:
					if (t == typeof(Boolean))
					{
						var v = IsTrue;
						result = Unsafe.As<Boolean, T>(ref v);
						return true;
					}
					if (t == typeof(Object))
					{
						result = (T)(Object)IsTrue;
						return true;
					}
					break;

				case ValueType.String:
					if (t == typeof(String) || t == typeof(Object))
					{
						result = (T)_string;
						return true;
					}
					break;
			}

			result = default;
			return false;
		}

		public static implicit operator Variable(Int32 v) => new(v, ValueType.Number);
		public static implicit operator Variable(Int64 v) => new(v, ValueType.Number);
		public static implicit operator Variable(Single v) => new(v, ValueType.Number);
		public static implicit operator Variable(Double v) => new(v, ValueType.Number);
		public static implicit operator Variable(Boolean v) => new(v ? 1.0 : 0.0, ValueType.Boolean);
		public static implicit operator Variable(Number v) => new(v, ValueType.Number);
		public static implicit operator Variable(String v) => new(v, ValueType.String);

		public static implicit operator Int32(Variable v) => v.AsInt32();
		public static implicit operator Int64(Variable v) => v.AsInt64();
		public static implicit operator Single(Variable v) => v.AsSingle();
		public static implicit operator Double(Variable v) => v.AsDouble();
		public static implicit operator Boolean(Variable v) => v.AsBoolean();
		public static implicit operator Number(Variable v) => v.AsNumber();
		public static implicit operator String(Variable v) => v.AsString();

		[ExcludeFromCodeCoverage]
		public override String ToString() => _type switch
		{
			ValueType.Number => _numValue.ToString("#,##0.###"),
			ValueType.Boolean => Emoji.IsTrue(IsTrue),
			ValueType.String => $"\"{_string}\"",
			var _ => $"<{_type}>",
		};

		public Boolean Equals(Boolean b) => _type == ValueType.Boolean && AsBoolean() == b;
		public Boolean Equals(Double d) => _type == ValueType.Number && _numValue.Equals(d);
		public Boolean Equals(String s) => _type == ValueType.String && String.Equals((String)_string, s);
		public Boolean Equals(Variable other) => _numValue.Equals(other._numValue) && Equals(_string, other._string);

		public override Boolean Equals(Object obj) => obj switch
		{
			Variable other => Equals(other),
			Boolean b => Equals(b),
			Double d => Equals(d),
			Single f => Equals((Double)f),
			Int32 i => Equals((Double)i),
			Int64 l => Equals((Double)l),
			UInt32 ui => Equals((Double)ui),
			UInt64 ul => Equals((Double)ul),
			Number n => Equals((Double)n),
			String s => Equals(s),
			var _ => false,
		};

		public override Int32 GetHashCode() => HashCode.Combine(_numValue, _string, (Int32)_type);

		public static Boolean operator ==(Variable left, Variable right) => left.Equals(right);
		public static Boolean operator !=(Variable left, Variable right) => !left.Equals(right);
	}
}
