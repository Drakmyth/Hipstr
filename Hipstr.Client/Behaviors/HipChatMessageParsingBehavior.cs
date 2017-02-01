using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace Hipstr.Client.Behaviors
{
	public sealed class HipChatMessageParsingBehavior : Behavior<RichTextBlock>
	{
		public static readonly DependencyProperty TeamProperty = DependencyProperty.Register(
			"Team",
			typeof(Team),
			typeof(HipChatMessageParsingBehavior),
			new PropertyMetadata(default(Team)));

		public Team Team
		{
			get { return (Team)GetValue(TeamProperty); }
			set { SetValue(TeamProperty, value); }
		}

		private IHipChatService _hipChatService;

		protected override void OnAttached()
		{
			base.OnAttached();
			_hipChatService = IoCContainer.Resolve<IHipChatService>();
			AssociatedObject.Loaded += AssociatedObject_OnLoaded;
		}

		private void AssociatedObject_OnLoaded(object sender, RoutedEventArgs e)
		{
			AssociatedObject.Loaded -= AssociatedObject_OnLoaded;
			AssociatedObject.DataContextChanged += AssociatedObject_OnDataContextChanged;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.DataContextChanged -= AssociatedObject_OnDataContextChanged;
		}

		private async void AssociatedObject_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
		{
			if (args.NewValue == null) return;
			await ParseMessage((Message)args.NewValue);
		}

		private async Task ParseMessage(Message message)
		{
			AssociatedObject.Blocks.Clear();

			AddParagraph(await ParseTextToInlinesAsync(message.Text));
			AddParagraph(ResolveMessageLinks(message));
			AddParagraph(ResolveFile(message.File));
		}

		private void AddParagraph(IList<Inline> inlines)
		{
			if (!inlines.Any()) return;

			var paragraph = new Paragraph();
			paragraph.Inlines.AddRange(inlines);
			AssociatedObject.Blocks.Add(paragraph);
		}

		private static IList<Inline> ResolveFile(MessageFile file)
		{
			IList<Inline> inlines = new List<Inline>();
			if (file == null) return inlines;

			Inline inline = new InlineUIContainer
			{
				Child = new HyperlinkButton
				{
					NavigateUri = file.Uri,
					Content = new Image
					{
						Source = new BitmapImage(file.ThumbnailUri)
					}
				}
			};
			inlines.Add(inline);
			return inlines;
		}

		private async Task<IList<Inline>> ParseTextToInlinesAsync(string text)
		{
			IList<Inline> inlines = new List<Inline>();
			IList<ReplaceToken> tokens = new List<ReplaceToken>();

			// TODO: Handle Emoji (emoticons not surrounded by parentheses)
			tokens.AddRange(ParseEmoticonTokens(text));
			tokens.AddRange(ParseMentionTokens(text));
			tokens.AddRange(ParseHyperlinkTokens(text));

			// Emoticons and Mentions can't overlap, but Hyperlinks can and should always take precedence
			IEnumerable<ReplaceToken> hyperlinkTokens = tokens.Where(t => t.Type == ReplaceToken.TokenType.Hyperlink).ToList();
			IEnumerable<ReplaceToken> tokensToRemove = tokens
				.Where(t => t.Type != ReplaceToken.TokenType.Hyperlink)
				.Where(t => hyperlinkTokens.Where(h => LiesBetween(t.StartIndex, h.StartIndex, h.EndIndex)).Any())
				.ToList();

			foreach (ReplaceToken token in tokensToRemove)
			{
				tokens.Remove(token);
			}

			IEnumerable<ReplaceToken> orderedTokens = tokens.OrderBy(t => t.StartIndex);

			var currentIndex = 0;
			foreach (ReplaceToken token in orderedTokens)
			{
				if (currentIndex < token.StartIndex)
				{
					inlines.Add(new Run {Text = text.Substring(currentIndex, token.StartIndex - currentIndex)});
				}

				switch (token.Type)
				{
					case ReplaceToken.TokenType.Emoticon:
						inlines.Add(await ResolveEmoticonAsync(token));
						break;
					case ReplaceToken.TokenType.Mention:
						inlines.Add(ResolveMention(token));
						break;
					case ReplaceToken.TokenType.Hyperlink:
						inlines.Add(ResolveHyperlink(token));
						break;
					default:
						throw new ArgumentOutOfRangeException($"Unknown Token Type - {token.Type}", nameof(token.Type));
				}
				currentIndex = token.EndIndex + 1;
			}

			if (currentIndex < text.Length)
			{
				inlines.Add(new Run {Text = text.Substring(currentIndex)});
			}

			return inlines;
		}

		private static IList<Inline> ResolveMessageLinks(Message message)
		{
			IList<Inline> inlines = new List<Inline>();
			foreach (MessageImage image in message.Images)
			{
				var inline = new InlineUIContainer
				{
					Child = new HyperlinkButton
					{
						NavigateUri = image.ImageUri,
						Content = new Image
						{
							Source = new BitmapImage(image.ImageUri)
						}
					}
				};

				inlines.Add(inline);
			}
			return inlines;
		}

		private static bool LiesBetween(int value, int start, int end)
		{
			return value >= start && value <= end;
		}

		private static IEnumerable<ReplaceToken> ParseEmoticonTokens(string text)
		{
			var tokens = new List<ReplaceToken>();
			var emoticonRegex = new Regex(@"\(\w+\)");
			MatchCollection emoticonMatches = emoticonRegex.Matches(text);
			foreach (Match match in emoticonMatches)
			{
				tokens.Add(new ReplaceToken(match.Index, ReplaceToken.TokenType.Emoticon, match.Value));
			}
			return tokens;
		}

		private static IEnumerable<ReplaceToken> ParseMentionTokens(string text)
		{
			var tokens = new List<ReplaceToken>();
			var mentionRegex = new Regex(@"@\w+");
			MatchCollection mentionMatches = mentionRegex.Matches(text);
			foreach (Match match in mentionMatches)
			{
				tokens.Add(new ReplaceToken(match.Index, ReplaceToken.TokenType.Mention, match.Value));
			}
			return tokens;
		}

		private static IEnumerable<ReplaceToken> ParseHyperlinkTokens(string text)
		{
			var tokens = new List<ReplaceToken>();
			var hyperlinkRegex = new Regex(@"(^|\s)(http|https|ftp|ftps)\:\/\/\S*", RegexOptions.IgnoreCase);
			MatchCollection hyperlinkMatches = hyperlinkRegex.Matches(text);
			foreach (Match match in hyperlinkMatches)
			{
				tokens.Add(new ReplaceToken(match.Index, ReplaceToken.TokenType.Hyperlink, match.Value));
			}
			return tokens;
		}

		private async Task<Inline> ResolveEmoticonAsync(ReplaceToken token)
		{
			string shortcut = token.Text.Substring(1, token.Text.Length - 2);
			Emoticon emoticon = await _hipChatService.GetSingleEmoticon(shortcut, Team);

			Inline inline;
			if (emoticon == null)
			{
				inline = new Run {Text = token.Text};
			}
			else
			{
				inline = new InlineUIContainer
				{
					Child = new Image
					{
						Source = new BitmapImage(emoticon.Url),
						Width = emoticon.Width,
						Height = emoticon.Height
					}
				};
			}
			return inline;
		}

		private static Inline ResolveMention(ReplaceToken token)
		{
			// TODO: Check if user exists, and if so style the Run appropriately
			return new Run {Text = token.Text};
		}

		private static Inline ResolveHyperlink(ReplaceToken token)
		{
			if (token.Text.Contains("@")) // IE7+ and Edge consider links with '@' in them a security hole, and so won't resolve them.
			{
				return new Run {Text = token.Text};
			}

			Uri navigateUri;
			bool validUri = Uri.TryCreate(Uri.EscapeUriString(token.Text), UriKind.Absolute, out navigateUri);

			if (!validUri) return new Run {Text = token.Text};

			var hyperlink = new Hyperlink {NavigateUri = navigateUri};
			hyperlink.Inlines.Add(new Run {Text = token.Text});
			return hyperlink;
		}

		private class ReplaceToken
		{
			public enum TokenType
			{
				Emoticon,
				Mention,
				Hyperlink
			}

			public int StartIndex { get; }
			public TokenType Type { get; }
			public string Text { get; }
			public int EndIndex => StartIndex + Text.Length - 1;

			public ReplaceToken(int startIndex, TokenType type, string text)
			{
				StartIndex = startIndex;
				Type = type;
				Text = text;
			}
		}
	}
}