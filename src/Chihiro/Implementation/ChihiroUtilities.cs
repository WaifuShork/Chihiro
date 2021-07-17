using System;
using System.Linq;
using DSharpPlus.Entities;

namespace Chihiro.Implementation
{
    public static class ChihiroUtilities
    {
        public static bool HasMentionPrefix(this DiscordMessage message, DiscordUser currentUser, out string output)
        {
            var contentSpan = message.Content.AsSpan();
            if (contentSpan.Length > 17 && contentSpan[0] == '<' && contentSpan[1] == '@')
            {
                var closingBracketIndex = contentSpan.IndexOf('>');
                if (closingBracketIndex != -1)
                {
                    var idSpan = contentSpan[2] == '!'
                        ? contentSpan.Slice(3, closingBracketIndex - 3)
                        : contentSpan.Slice(2, closingBracketIndex - 2);
                    if (ulong.TryParse(idSpan, out var id) && id == currentUser.Id)
                    {
                        output = new string(contentSpan.Slice(closingBracketIndex + 1));
                        return true;
                    }
                }
            }

            output = string.Empty;
            return false;
        }

        public static string ToFirstUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
    }
}