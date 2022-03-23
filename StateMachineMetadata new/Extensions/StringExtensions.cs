using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Extensions
{
    public static class StringExtensions
    {
        public static string GetCoreName(this string origName)
        {
            return origName.Replace("State", "").Replace("Diagram", "").Trim();
        }

        public static string ToCamelCase(this string origName)
        {
            if (origName == null || origName.Length < 2) return origName;

            var firstLetter = origName.Substring(0,1).ToLower();
            var restOfTheName = origName.Substring(1);
            return firstLetter + restOfTheName;
        }
        #region Valid Identifiers
        public static string ToValidCSharpName(this string origName)
        {
            // Split any string at the semi-collon character locations if any, and elect the first one to transform
            var firstToken = origName.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).First().Trim();
            if (string.IsNullOrEmpty(firstToken)) return null;
            // Replace end-of-line with underscore  chars
            firstToken = firstToken.Replace("\n", "_").Replace("\n\r", "_");
            //Replace any invalid character with underscore
            var validChars = firstToken.ToCharArray().Select(c => !char.IsLetterOrDigit(c) && c != '_' ? '_' : c);
            var validIdentifier = (new string(validChars.ToArray())).Trim('_');

            if (validIdentifier.IsValidIdentifier()) return validIdentifier;
            else return null;
        }

        public static bool IsValidIdentifier(this string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            if (!char.IsLetter(text[0]) && text[0] != '_') return false;

            for (int ix = 1; ix < text.Length; ++ix)
            {
                if (!char.IsLetterOrDigit(text[ix]) && text[ix] != '_') return false;
            }

            // Valid identifiers must have at least one alpha character in this application
            return text.ToCharArray().ToList().Any(c => char.IsLetter(c)) ? true : false;

        }

        #endregion Valid Identifiers
    }
}
