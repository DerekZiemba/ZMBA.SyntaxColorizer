using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;


namespace ZMBA.SyntaxColorizer {

	//TODO: Pull format settings from a config file eventually. 
	internal abstract class ZMBAFormatDefinition : ClassificationFormatDefinition {

      public bool IsUnderlined {
         get {
            return TextDecorations == System.Windows.TextDecorations.Underline;
         }
         set {
            TextDecorations = value ? System.Windows.TextDecorations.Underline : (IsUnderlined ? null : TextDecorations);
         }
      }

      public bool IsOverlined {
         get {
            return TextDecorations == System.Windows.TextDecorations.OverLine;
         }
         set {
            TextDecorations = value ? System.Windows.TextDecorations.OverLine : (IsOverlined ? null : TextDecorations);
         }
      }

      public ZMBAFormatDefinition(string identifier) {
			this.DisplayName = identifier;
		}
	}
}