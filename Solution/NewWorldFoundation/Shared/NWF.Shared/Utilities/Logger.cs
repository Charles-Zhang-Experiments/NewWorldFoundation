﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NWF.Shared.Utilities
{
    public sealed class Logger : TextWriter, IDisposable
    {
        #region Construction and Destruction
        /// <summary>
        /// Starts logging.
        /// </summary>
        /// <param name="identifier">Filename (without extension)</param>
        /// <param name="print">Print while logging.</param>
        /// <param name="generateTextbuilder">Generate string builder so caller decides what to do with log.</param>
        /// <param name="outputHandler">Custom printing routine (when Console.WriteLine is called).</param>
        /// <param name="doNotWriteAnything">Do not write to log file.</param>
        public Logger(string identifier = null, bool print = true, bool generateTextbuilder = false, Action<string> outputHandler = null, bool doNotWriteAnything = false)
        {
            identifier ??= DateTime.Now.ToString("yyyy-MM-dd_hhmmss");
            OutputPath = Path.GetFullPath(identifier + ".log");
            Print = print;
            OutputHandler = outputHandler ?? DefaultOutputHandler;
            DoNotWriteAnything = doNotWriteAnything;

            if (!File.Exists(OutputPath) & !DoNotWriteAnything)
                File.WriteAllText(OutputPath, string.Empty);
            if (generateTextbuilder)
                Builder = new();

            Previous = Console.Out;
            Console.SetOut(this);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Console.SetOut(Previous);
            Console.WriteLine($"Log file generated at: {OutputPath}");
        }
        private TextWriter Previous { get; }
        private bool Print { get; }
        private StringBuilder Builder { get; }
        private string OutputPath { get; }
        private Action<string> OutputHandler { get; }
        public bool DoNotWriteAnything { get; }
        #endregion

        #region Default Output
        public void DefaultOutputHandler(string text)
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string logText = $"[{dateTime}] {text}";
            if (Print)
            {
                Console.SetOut(Previous);
                Console.Write(logText);
                Console.SetOut(this);
            }
            if (!DoNotWriteAnything && OutputPath != null)
                File.AppendAllText(OutputPath, logText);
            if (Builder != null)
                Builder.Append(logText);
        }
        public override string ToString()
            => Builder.ToString().TrimEnd();
        #endregion

        #region Writing Routines
        public override Encoding Encoding => Encoding.UTF8;
        public override void Write(bool value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(char value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(char[] buffer, int index, int count)
        {
            OutputHandler.Invoke(new string(buffer.Skip(index).Take(count).ToArray()));
        }
        public override void Write(char[] buffer)
        {
            OutputHandler.Invoke(new string(buffer));
        }
        public override void Write(decimal value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(double value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(float value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(int value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(long value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(object value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(ReadOnlySpan<char> buffer)
        {
            OutputHandler.Invoke(buffer.ToString());
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0)
        {
            OutputHandler.Invoke(string.Format(format, arg0));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0, object arg1)
        {
            OutputHandler.Invoke(string.Format(format, arg0, arg1));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0, object arg1, object arg2)
        {
            OutputHandler.Invoke(string.Format(format, arg0, arg1, arg2));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, params object[] arg)
        {
            OutputHandler.Invoke(string.Format(format, arg));
        }
        public override void Write(string value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(StringBuilder value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(uint value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void Write(ulong value)
        {
            OutputHandler.Invoke(value.ToString());
        }
        public override void WriteLine()
        {
            OutputHandler.Invoke(Environment.NewLine);
        }
        public override void WriteLine(bool value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            OutputHandler.Invoke(new string(buffer.Skip(index).Take(count).ToArray()) + Environment.NewLine);
        }
        public override void WriteLine(char[] buffer)
        {
            OutputHandler.Invoke(new string(buffer) + Environment.NewLine);
        }
        public override void WriteLine(decimal value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(double value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(float value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(int value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(long value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(object value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            OutputHandler.Invoke(buffer.ToString() + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0)
        {
            OutputHandler.Invoke(string.Format(format, arg0) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0, object arg1)
        {
            OutputHandler.Invoke(string.Format(format, arg0, arg1) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0, object arg1, object arg2)
        {
            OutputHandler.Invoke(string.Format(format, arg0, arg1, arg2) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, params object[] arg)
        {
            OutputHandler.Invoke(string.Format(format, arg) + Environment.NewLine);
        }
        public override void WriteLine(string value)
        {
            OutputHandler.Invoke(value + Environment.NewLine);
        }
        public override void WriteLine(StringBuilder value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(uint value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(ulong value)
        {
            OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        #endregion
    }
}
