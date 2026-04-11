using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Luny
{
	public sealed class LunyStackTrace
	{
		private readonly List<StackFrame> _frames;
		public IReadOnlyList<StackFrame> Frames => _frames.AsReadOnly();
		public Int32 Count => _frames.Count;

		public StackFrame this[Int32 index] => index >= 0 && index < _frames.Count ? _frames[index] : default;

		public LunyStackTrace(StackFrame rootFrame)
		{
			_frames = new List<StackFrame>();
			_frames.Add(rootFrame);
		}

		public LunyStackTrace([CallerMemberName] String name = "", [CallerFilePath] String path = "", [CallerLineNumber] Int32 line = 0,
			Int32 column = 0)
			: this(new StackFrame(name, path, line, column)) {}

		public LunyStackTrace(String name, System.Diagnostics.StackFrame frame)
			: this(name, frame.GetFileName(), frame.GetFileLineNumber(), frame.GetFileColumnNumber()) {}

		public LunyStackTrace Add(StackFrame frame)
		{
			_frames.Add(frame);
			return this;
		}

		public LunyStackTrace Add([CallerMemberName] String name = "")
		{
			var lastIndex = _frames.Count - 1;
			Add(new StackFrame(name, _frames[lastIndex].Path, _frames[lastIndex].Line));
			return this;
		}

		public LunyStackTrace Add([CallerMemberName] String name = "", [CallerFilePath] String path = "", [CallerLineNumber] Int32 line = 0,
			Int32 column = 0) => Add(new StackFrame(name, path, line, column));

		public override String ToString()
		{
			var sb = new StringBuilder();
			foreach (var frame in _frames)
				sb.AppendLine(frame.ToString());
			return sb.ToString();
		}

		public void Add(String name, System.Diagnostics.StackFrame frame) =>
			Add(new StackFrame(name, frame.GetFileName(), frame.GetFileLineNumber(), frame.GetFileColumnNumber()));
	}
}
