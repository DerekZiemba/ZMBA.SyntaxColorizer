using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Shared;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Roslyn.Utilities;

using VB = Microsoft.CodeAnalysis.VisualBasic;
using CS = Microsoft.CodeAnalysis.CSharp;
using VBKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;
using CSKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace ZMBA.SyntaxColorizer {
  [Serializable]
  public class SyntaxColorizorException : Exception {
		private const string Description = "Exception occurred in ZMBA.SyntaxColorizor";

		protected SyntaxColorizorException(string message, Exception inner) : base(message, inner) { }
		protected SyntaxColorizorException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		protected static string BuildMessage(string desc, string message, string param, string caller, int line, string file) {
			var sb = new StringBuilder(64 + (message?.Length ?? 0));
			sb.Append(desc);
			if(!String.IsNullOrWhiteSpace(message)) { sb.Append(message); }
			if(!String.IsNullOrWhiteSpace(param)) { sb.Append(" | Param: ").Append(param); }
			if(!String.IsNullOrWhiteSpace(caller)) { sb.Append(" | Method: ").Append(caller); }
			if(line > 0) { sb.Append(" | Line: ").Append(line); }
			if(!String.IsNullOrWhiteSpace(file)) { sb.AppendLine().Append(" | File: ").Append(file); }
			return sb.ToString();
		}

    public SyntaxColorizorException() {

    }

    public SyntaxColorizorException(string message) : base(message) {

    }

    public SyntaxColorizorException(string message = null, string param = null, Exception inner = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, [CallerFilePath] string file = null)
  :   base(BuildMessage(Description, message, param, caller, line, file), inner) {
    }
  }

}
