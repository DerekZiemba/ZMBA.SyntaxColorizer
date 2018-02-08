using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;


namespace ZMBA.SyntaxColorizer {

	//TODO: Pull format settings from a config file eventually. 
	internal abstract class ZMBAFormatDefinition : ClassificationFormatDefinition {

		public ZMBAFormatDefinition(string identifier) {
			this.DisplayName = identifier;
		}
	}
}