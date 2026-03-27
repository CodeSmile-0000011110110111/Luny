using System;
using System.Runtime.CompilerServices;

namespace Luny
{
	public readonly struct StackFrame
	{
		public String Name { get; init; }
		public String Path { get; init; }
		public String FullPath => System.IO.Path.GetFullPath(Path);
		public String Filename => System.IO.Path.GetFileName(Path);
		public Int32 Line { get; init; }
		public Int32 Column { get; init; }

		public StackFrame([CallerMemberName] String name = "", [CallerFilePath] String path = "", [CallerLineNumber] Int32 line = 0,
			Int32 column = 0)
		{
			Name = name;
			Path = path;
			Line = line;
			Column = column;
		}

		public override String ToString() => $"{Name} (at {Path}:{Line})";
	}
}
