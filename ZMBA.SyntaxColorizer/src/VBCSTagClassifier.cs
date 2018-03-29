using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

using VB = Microsoft.CodeAnalysis.VisualBasic;
using CS = Microsoft.CodeAnalysis.CSharp;
using VBKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;
using CSKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace ZMBA.SyntaxColorizer {


	[Export(typeof(ITaggerProvider))]
	[ContentType("Basic")]
	[ContentType("CSharp")]
	[TagType(typeof(IClassificationTag))]
	internal sealed class VBCSClassificationTagProvider: ITaggerProvider {
		[Import] internal IClassificationTypeRegistryService ClassificationRegistry; // Set via MEF

		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
			//Don't need to pass the buffer because we aren't doing anything too complicated. 
			//Would need it if we had to do some intense tagging on a background thread.
			return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(() => new VBCSTagClassifier(ClassificationRegistry));
		}
	}

	internal class VBCSTagClassifier: ITagger<ClassificationTag> {
#pragma warning disable CS0067 // The event is never used
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore CS0067 // The event is never used

		private readonly FormattingTags Tags;
		private ClassifierContext CachedContext;
		
		internal VBCSTagClassifier(IClassificationTypeRegistryService registry) {
			this.Tags = new FormattingTags(registry);
         TagsChanged += (object sender, SnapshotSpanEventArgs args) => {
            System.Diagnostics.Debugger.Break();
         };
		}

		public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection snapshots) {
			if (snapshots.Count >= 1) {
				SnapshotSpan snapspan = snapshots[0];
				ClassifierContext ctx = ClassifierContext.GetContext(ref this.CachedContext, snapspan);		
				if(ctx != null) {
					List<ClassifiedSpan> ls = ctx.GetClassifiedSpans(snapspan);
					for(int idx = 0; idx < ls.Count; idx++) {
						ClassifiedSpan clsspan = ls[idx];
						TextSpan txt = clsspan.TextSpan;				
						switch(clsspan.ClassificationType) {
							case "string": ClassifyString(ctx, txt); break;
							case "identifier": ClassifyIdentifier(ctx, txt);	break;
							case "keyword": ctx.AssociateTagWithText(Tags.SyntaxKeyword, txt); break;
							case "operator": ClassifyOperator(ctx, txt); break;
							case "number": ctx.AssociateTagWithText(Tags.SyntaxNumber, txt); break;
							case "punctuation": ctx.AssociateTagWithText(Tags.SyntaxPunctuation, txt); break;
							case "comment": ctx.AssociateTagWithText(Tags.SyntaxComment, txt); break;
							case "class name": ctx.AssociateTagWithText(Tags.TypeClass, txt); break;
							case "module name": ctx.AssociateTagWithText(Tags.TypeModule, txt); break;
							case "struct name": ctx.AssociateTagWithText(Tags.TypeStructure, txt); break;
							case "interface name": ctx.AssociateTagWithText(Tags.TypeInterface, txt); break;
							case "type parameter name": ctx.AssociateTagWithText(Tags.TypeGeneric, txt); break;
							case "delegate name": ctx.AssociateTagWithText(Tags.TypeDelegate, txt); break;
							case "enum name": ctx.AssociateTagWithText(Tags.Enum, txt); break;
							case "preprocessor keyword": ctx.AssociateTagWithText(Tags.Preprocessor, txt); break;
							case "preprocessor text": ctx.AssociateTagWithText(Tags.PreprocessorText, txt); break;
							default: continue;
						}
					
						if(ctx.TaggedText != null) {
							TagSpan<ClassificationTag> tag = ctx.TaggedText;
							ctx.TaggedText = null;
							yield return tag;
						}
					}
				}
			}
		}

		private void ClassifyString(ClassifierContext ctx, TextSpan txt) {
			SyntaxNode node = ctx.RootNode.FindOuterMostNode(txt, false);
			if(node == null) {
            ctx.AssociateTagWithText(Tags.String, txt);
         } else {
            switch (node.RawKind) {
               case (int)VBKind.InterpolatedStringExpression:
               case (int)CSKind.InterpolatedVerbatimStringStartToken:
               case (int)CSKind.InterpolatedStringStartToken:
               case (int)CSKind.InterpolatedStringEndToken:
               case (int)CSKind.InterpolatedStringExpression:
                  ctx.AssociateTagWithText(Tags.StringToken, txt);
                  break;
               case (int)VBKind.CharacterLiteralToken:
               case (int)VBKind.CharacterLiteralExpression:
               case (int)VBKind.SingleQuoteToken:
               case (int)CSKind.CharacterLiteralToken:
               case (int)CSKind.CharacterLiteralExpression:
               case (int)CSKind.SingleQuoteToken:
                  ctx.AssociateTagWithText(Tags.StringSingleQuote, txt);
                  break;
               case (int)VBKind.InterpolatedStringTextToken:
               case (int)VBKind.InterpolatedStringText:
               case (int)CSKind.InterpolatedStringTextToken:
               case (int)CSKind.InterpolatedStringText:
               case (int)CSKind.InterpolatedStringToken:
                  ctx.AssociateTagWithText(Tags.StringInterpolated, txt);
                  break;
               case (int)VBKind.StringLiteralToken:
               case (int)VBKind.StringLiteralExpression:
               case (int)VBKind.XmlEntityLiteralToken:
               case (int)VBKind.XmlString:
               case (int)CSKind.StringLiteralToken:
               case (int)CSKind.StringLiteralExpression:
               case (int)CSKind.XmlEntityLiteralToken:
               default:
                  ctx.AssociateTagWithText(Tags.String, txt);
                  break;
            }
         }
		}

      private void ClassifyOperator(ClassifierContext ctx, TextSpan txt) {
         SyntaxNode node = ctx.RootNode.FindOuterMostNode(txt, true);
         if (node != null) {
            ISymbol rawsymbol = ctx.SemanticModel.GetSymbolInfo(node).Symbol ?? ctx.SemanticModel.GetDeclaredSymbol(node);
            if (rawsymbol != null) {
               switch (rawsymbol.Kind) {
                  case SymbolKind.Method: ClassifyIdentifier_Method(ctx, txt, node, (IMethodSymbol)rawsymbol); return;
               }
            }          
         }
         ctx.AssociateTagWithText(Tags.SyntaxOperator, txt);
      }

      private void ClassifyIdentifier(ClassifierContext ctx, TextSpan txt) {
         SyntaxNode node = ctx.RootNode.FindOuterMostNode(txt, true);
         if (node != null) {
            switch (node.RawKind) {
               case (int)VBKind.Attribute:
               case (int)CSKind.Attribute:
                  ctx.AssociateTagWithText(Tags.IdentifierAttribute, txt);
                  return;
               default:
                  ISymbol rawsymbol = ctx.SemanticModel.GetSymbolInfo(node).Symbol ?? ctx.SemanticModel.GetDeclaredSymbol(node);
                  if (rawsymbol != null) {
                     switch (rawsymbol.Kind) {
                        case SymbolKind.Field: ClassifyIdentifier_Field(ctx, txt, node, (IFieldSymbol)rawsymbol); return;
                        case SymbolKind.Method: ClassifyIdentifier_Method(ctx, txt, node, (IMethodSymbol)rawsymbol); return;
                        case SymbolKind.NamedType: ClassifyIdentifier_NamedType(ctx, txt, node, (INamedTypeSymbol)rawsymbol); return;
                        case SymbolKind.Local: ClassifyIdentifier_Local(ctx, txt, node, (ILocalSymbol)rawsymbol); return;
                        case SymbolKind.Parameter: ctx.AssociateTagWithText(Tags.Param, txt); return;
                        case SymbolKind.TypeParameter: ctx.AssociateTagWithText(Tags.TypeGeneric, txt); return;
                        case SymbolKind.DynamicType: ctx.AssociateTagWithText(Tags.TypeDynamic, txt); return;
                        case SymbolKind.Namespace: ctx.AssociateTagWithText(Tags.IdentifierNamespace, txt); return;
                        case SymbolKind.Property: ClassifyIdentifier_Property(ctx, txt, node, (IPropertySymbol)rawsymbol); return;
                        case SymbolKind.Event: ctx.AssociateTagWithText(Tags.IdentifierEvent, txt); return;
                        case SymbolKind.Label: ctx.AssociateTagWithText(Tags.IdentifierLabel, txt); return;
                        case SymbolKind.Preprocessing: ctx.AssociateTagWithText(Tags.PreprocessorText, txt); return;
                     }
                  }
                  break;
            }
         }
         ctx.AssociateTagWithText(Tags.Identifier, txt);
      }
      private void ClassifyIdentifier_Field(ClassifierContext ctx, TextSpan txt, SyntaxNode node, IFieldSymbol symbol) {
			if(symbol.ContainingType.TypeKind == TypeKind.Enum) {
				ctx.AssociateTagWithText(Tags.EnumMember, txt);
			} else {
				if(symbol.IsConst) {
					ctx.AssociateTagWithText(Tags.IdentifierConst, txt);
				} else {
					ctx.AssociateTagWithText(Tags.IdentifierField, txt);
				}
			}
		}
      private void ClassifyIdentifier_Property(ClassifierContext ctx, TextSpan txt, SyntaxNode node, IPropertySymbol symbol) {
         if (symbol.ImplementsInterface()) {
            ctx.AssociateTagWithText(Tags.IdentifierPropertyInterfaceImplementation, txt);
         } else {
            ctx.AssociateTagWithText(Tags.IdentifierProperty, txt);
         }        
      }

      private void ClassifyIdentifier_Method(ClassifierContext ctx, TextSpan txt, SyntaxNode node, IMethodSymbol symbol) {
			switch(node.Parent.RawKind) {
				case (int)VBKind.Attribute:
				case (int)CSKind.Attribute:
				case (int)VBKind.QualifiedName when node.Parent.Parent.RawKind == (int)VBKind.Attribute:
				case (int)CSKind.QualifiedName when node.Parent.Parent.RawKind == (int)CSKind.Attribute:
					ctx.AssociateTagWithText(Tags.IdentifierAttribute, txt);
					break;
				default:              
					if(symbol.MethodKind == MethodKind.Constructor || symbol.MethodKind == MethodKind.StaticConstructor) {
						ctx.AssociateTagWithText(Tags.MethodConstructor, txt);
					} else if(symbol.MethodKind == MethodKind.UserDefinedOperator) {
                  ctx.AssociateTagWithText(Tags.MethodUserDefinedOperator, txt);
               } else if(symbol.IsExtensionMethod) {
						ctx.AssociateTagWithText(Tags.MethodExtension, txt);
					} else if(symbol.IsStatic) {
						ctx.AssociateTagWithText(Tags.MethodStatic, txt);
               } else if (symbol.IsVirtual || symbol.IsOverride) {
                  ctx.AssociateTagWithText(Tags.MethodVirtual, txt);
               } else if (symbol.ImplementsInterface()) {
                  ctx.AssociateTagWithText(Tags.MethodInterfaceImplementation, txt);
               } else {
                  ctx.AssociateTagWithText(Tags.Method, txt);
					}
					break;
			}
		}
		private void ClassifyIdentifier_NamedType(ClassifierContext ctx, TextSpan txt, SyntaxNode node, INamedTypeSymbol symbol) {
			if(symbol.SpecialType != SpecialType.None) {
				ctx.AssociateTagWithText(Tags.Type, txt);
			} else {
				ctx.AssociateTagWithText(Tags.TypeClass, txt);
			}
		}
		private void ClassifyIdentifier_Local(ClassifierContext ctx, TextSpan txt, SyntaxNode node, ILocalSymbol symbol) {
			if(symbol.IsConst) {
				ctx.AssociateTagWithText(Tags.IdentifierConst, txt);
			} else {
				ctx.AssociateTagWithText(Tags.Variable, txt);
			}
		}


	}
}
