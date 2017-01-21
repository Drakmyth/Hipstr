using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace Hipstr.Client.Attachments
{
	public static class HipChatMessageParser
	{
		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
			"Text",
			typeof(string),
			typeof(HipChatMessageParser),
			new PropertyMetadata(default(string), OnTextChangedAsync));

		public static readonly DependencyProperty TeamProperty = DependencyProperty.RegisterAttached(
			"Team",
			typeof(Team),
			typeof(HipChatMessageParser),
			new PropertyMetadata(default(Team)));

		private static readonly IHipChatService _hipChatService;

		static HipChatMessageParser()
		{
			_hipChatService = IoCContainer.Resolve<IHipChatService>();
		}

		public static string GetText(DependencyObject target)
		{
			return (string)target.GetValue(TextProperty);
		}

		public static void SetText(DependencyObject target, string value)
		{
			target.SetValue(TextProperty, value);
		}

		public static Team GetTeam(DependencyObject target)
		{
			return (Team)target.GetValue(TeamProperty);
		}

		public static void SetTeam(DependencyObject target, Team value)
		{
			target.SetValue(TeamProperty, value);
		}

		private static async void OnTextChangedAsync(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var textBlock = sender as RichTextBlock;
			if (textBlock == null) return;

			textBlock.Blocks.Clear();
			IEnumerable<Inline> inlines = await ParseTextToInlinesAsync(sender, (string)e.NewValue);
			var paragraph = new Paragraph();
			paragraph.Inlines.AddRange(inlines);
			textBlock.Blocks.Add(paragraph);
		}

		private static async Task<IEnumerable<Inline>> ParseTextToInlinesAsync(DependencyObject sender, string text)
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
						inlines.Add(await ResolveEmoticonAsync(sender, token));
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

		private static async Task<Inline> ResolveEmoticonAsync(DependencyObject sender, ReplaceToken token)
		{
			string shortcut = token.Text.Substring(1, token.Text.Length - 2);
			Emoticon emoticon = await _hipChatService.GetSingleEmoticon(shortcut, GetTeam(sender));

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
			// TODO: Resolve to Hyperlink instead of Run
			return new Run {Text = token.Text};
		}

		private static bool LiesBetween(int value, int start, int end)
		{
			return value >= start && value <= end;
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