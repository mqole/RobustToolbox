using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Linguini.Bundle;
using Linguini.Shared.Types.Bundle;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Robust.Shared.Localization
{
    internal abstract partial class LocalizationManager
    {
        [Dependency] IPrototypeManager _proto = default!;
        private static readonly Regex RegexWordMatch = new Regex(@"\w+");

        private void AddBuiltInFunctions(FluentBundle bundle)
        {
            // Grammatical pronouns
            AddCtxFunction(bundle, "SUBJECT", FuncSubject);
            AddCtxFunction(bundle, "OBJECT", FuncObject);
            AddCtxFunction(bundle, "DAT-OBJ", FuncDatObj);
            AddCtxFunction(bundle, "GENITIVE", FuncGenitive);
            AddCtxFunction(bundle, "POSS-ADJ", FuncPossAdj);
            AddCtxFunction(bundle, "POSS-PRONOUN", FuncPossPronoun);
            AddCtxFunction(bundle, "REFLEXIVE", FuncReflexive);
            AddCtxFunction(bundle, "COUNTER", FuncCounter);

            // Conjugation
            AddCtxFunction(bundle, "CONJUGATE-BE", FuncConjugateBe);
            AddCtxFunction(bundle, "CONJUGATE-HAVE", FuncConjugateHave);
            AddCtxFunction(bundle, "CONJUGATE-BASIC", FuncConjugateBasic);

            // Proper nouns
            AddCtxFunction(bundle, "PROPER", FuncProper);
            AddCtxFunction(bundle, "THE", FuncThe);

            // Misc
            AddCtxFunction(bundle, "ATTRIB", args => FuncAttrib(bundle, args));
            AddCtxFunction(bundle, "CAPITALIZE", FuncCapitalize);
            AddCtxFunction(bundle, "INDEFINITE", FuncIndefinite);
        }

        /// <summary>
        /// Returns the name of the entity passed in, prepended with "the" if it is not a proper noun.
        /// </summary>
        private ILocValue FuncThe(LocArgs args)
        {
            return new LocValueString(GetString("zzzz-the", ("ent", args.Args[0])));
        }

        /// <summary>
        /// Returns the string passed in, with the first letter capitalized.
        /// </summary>
        private ILocValue FuncCapitalize(LocArgs args)
        {
            var input = args.Args[0].Format(new LocContext());
            if (!String.IsNullOrEmpty(input))
                return new LocValueString(input[0].ToString().ToUpper() + input.Substring(1));
            else return new LocValueString("");
        }

        private static readonly string[] IndefExceptions = { "euler", "heir", "honest" };
        private static readonly char[] IndefCharList = { 'a', 'e', 'd', 'h', 'i', 'l', 'm', 'n', 'o', 'r', 's', 'x' };
        private static readonly Regex[] IndefRegexes =
        {
            new ("^e[uw]"),
            new ("^onc?e\b"),
            new ("^uni([^nmd]|mo)"),
            new ("^u[bcfhjkqrst][aeiou]")
        };

        private static readonly Regex IndefRegexFjo =
            new("(?!FJO|[HLMNS]Y.|RY[EO]|SQU|(F[LR]?|[HL]|MN?|N|RH?|S[CHKLMNPTVW]?|X(YL)?)[AEIOU])[FHLMNRSX][A-Z]");

        private static readonly Regex IndefRegexU = new("^U[NK][AIEO]");

        private static readonly Regex IndefRegexY =
            new("^y(b[lor]|cl[ea]|fere|gg|p[ios]|rou|tt)");

        private static readonly char[] IndefVowels = { 'a', 'e', 'i', 'o', 'u' };

        private ILocValue FuncIndefinite(LocArgs args)
        {
            ILocValue val = args.Args[0];
            if (val.Value == null)
                return new LocValueString("an");

            string? word;
            string? input;
            if (val.Value is EntityUid entity)
            {
                if (TryGetEntityLocAttrib(entity, "indefinite", out var indef))
                    return new LocValueString(indef);

                input = _entMan.GetComponent<MetaDataComponent>(entity).EntityName;
            }
            else
            {
                input = val.Format(new LocContext());
            }

            if (String.IsNullOrEmpty(input))
                return new LocValueString("");

            var a = new LocValueString("a");
            var an = new LocValueString("an");

            var m = RegexWordMatch.Match(input);
            if (m.Success)
            {
                word = m.Groups[0].Value;
            }
            else
            {
                return an;
            }

            var wordi = word.ToLower();
            if (IndefExceptions.Any(anword => wordi.StartsWith(anword)))
            {
                return an;
            }

            if (wordi.StartsWith("hour") && !wordi.StartsWith("houri"))
                return an;

            if (wordi.Length == 1)
            {
                return wordi.IndexOfAny(IndefCharList) == 0 ? an : a;
            }

            if (IndefRegexFjo.Match(word)
                .Success)
            {
                return an;
            }

            foreach (var regex in IndefRegexes)
            {
                if (regex.IsMatch(wordi))
                    return a;
            }

            if (IndefRegexU.IsMatch(word))
            {
                return a;
            }

            if (word == word.ToUpper())
            {
                return wordi.IndexOfAny(IndefCharList) == 0 ? an : a;
            }

            if (wordi.IndexOfAny(IndefVowels) == 0)
            {
                return an;
            }

            return IndefRegexY.IsMatch(wordi) ? an : a;
        }

        /// <summary>
        /// Returns the pronoun prototype used by the entity.
        /// </summary>
        private PronounPrototype FuncPronoun(LocArgs args)
        {

            if (args.Args.Count < 1) return _proto.Index<PronounPrototype>("it"); // TODO validate this exists

            ILocValue entity0 = args.Args[0];
            if (entity0.Value is EntityUid entity)
            {
                if (_entMan.TryGetComponent(entity, out GrammarComponent? grammar) && grammar.Pronoun != null)
                {
                    return  grammar.Pronoun;
                }

                if (TryGetEntityLocAttrib(entity, "pronoun", out var pronoun))
                {
                    return _proto.Index<PronounPrototype>(pronoun);
                }
            }

            return _proto.Index<PronounPrototype>("it"); // TODO validate this exists;
        }

        /// <summary>
        /// Returns the respective subject pronoun (he, she, they, it) for the entity's pronoun ID.
        /// </summary>
        private ILocValue FuncSubject(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.Subject));
        }

        /// <summary>
        /// Returns the respective object pronoun (him, her, them, it) for the entity's pronoun ID.
        /// </summary>
        private ILocValue FuncObject(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.Object));
        }

        /// <summary>
        /// Returns the dative form pronoun for the entity's pronoun ID.
        /// This method is intended for languages with a dative case, where indirect objects
        /// (e.g., "to him," "for her") require specific forms. Not applicable for en-US locale.
        /// </summary>
        private ILocValue FuncDatObj(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.DatObj));
        }

        /// <summary>
        /// Returns the respective genitive form (pronoun or possessive adjective) for the entity's pronoun ID.
        /// This is used in languages with a genitive case to indicate possession or related relationships,
        /// e.g., "у него" (Russian), "seines Vaters" (German).
        private ILocValue FuncGenitive(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.Genitive));
        }

        /// <summary>
        /// Returns the respective possessive adjective (his, her, their, its) for the entity's pronoun ID.
        /// </summary>
        private ILocValue FuncPossAdj(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.PossAdj));
        }

        /// <summary>
        /// Returns the respective possessive pronoun (his, hers, theirs, its) for the entity's pronoun ID.
        /// </summary>
        private ILocValue FuncPossPronoun(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.PossPronoun));
        }

        /// <summary>
        /// Returns the respective reflexive pronoun (himself, herself, themselves, itself) for the entity's pronoun ID.
        /// </summary>
        private ILocValue FuncReflexive(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.Reflexive));
        }

        /// <summary>
        /// Returns the counter or measure word for the entity. Not used in English, common in East Asian languages.
        /// </summary>
        private ILocValue FuncCounter(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string prefix = pronoun.Custom ?
                pronoun.ID : "";

            return new LocValueString(prefix + GetString(pronoun.Counter));
        }

        /// <summary>
        /// Returns the respective conjugated form of "to be" (is/are) for the entity's pronoun ID.
        /// </summary>
        private ILocValue FuncConjugateBe(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string conjugate = pronoun.Plural ?
                "-plural" : "-singular";

            return new LocValueString("be" + conjugate);
        }

        /// <summary>
        /// Returns the respective conjugated form of "to have" (has/have) for the entity's pronoun ID.
        /// </summary>
        private ILocValue FuncConjugateHave(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string conjugate = pronoun.Plural ?
                "-plural" : "-singular";

            return new LocValueString("have" + conjugate);
        }

        /// <summary>
        /// Returns the basic conjugated form of a verb. Plural pronouns use the base verb (first value), others conjugate (second value).
        /// e.g. run -> he runs/she runs/they run/it runs
        /// </summary>
        private ILocValue FuncConjugateBasic(LocArgs args)
        {
            var pronoun = FuncPronoun(args);
            string conjugate = pronoun.Plural ?
                "-plural" : "-singular";

            return new LocValueString("verb" + conjugate);
        }

        private ILocValue FuncAttrib(FluentBundle bundle, LocArgs args)
        {
            if (args.Args.Count < 2) return new LocValueString("other");

            ILocValue entity0 = args.Args[0];
            if (entity0.Value is EntityUid entity)
            {
                ILocValue attrib0 = args.Args[1];
                if (TryGetEntityLocAttrib(entity, attrib0.Format(new LocContext(bundle)), out var attrib))
                {
                    return new LocValueString(attrib);
                }
            }

            return new LocValueString("other");
        }

        /// <summary>
        /// Returns whether the passed in entity's name is proper or not.
        /// </summary>
        private ILocValue FuncProper(LocArgs args)
        {
            if (args.Args.Count < 1) return new LocValueString("false");

            ILocValue entity0 = args.Args[0];
            if (entity0.Value is EntityUid entity)
            {
                if (_entMan.TryGetComponent(entity, out GrammarComponent? grammar) && grammar.ProperNoun.HasValue)
                {
                    return new LocValueString(grammar.ProperNoun.Value.ToString().ToLowerInvariant());
                }

                if (TryGetEntityLocAttrib(entity, "proper", out var proper))
                {
                    return new LocValueString(proper);
                }
            }

            return new LocValueString("false");
        }


        private void AddCtxFunction(FluentBundle ctx, string name, LocFunction function)
        {
            ctx.AddFunctionOverriding(name, (args, options) => CallFunction(function, ctx, args, options));
        }

        private IFluentType CallFunction(
            LocFunction function,
            FluentBundle bundle,
            IList<IFluentType> positionalArgs,
            IDictionary<string, IFluentType> namedArgs)
        {
            var args = new ILocValue[positionalArgs.Count];
            for (var i = 0; i < args.Length; i++)
            {
                args[i] = positionalArgs[i].ToLocValue();
            }

            var options = new Dictionary<string, ILocValue>(namedArgs.Count);
            foreach (var (k, v) in namedArgs)
            {
                options.Add(k, v.ToLocValue());
            }

            var argStruct = new LocArgs(args, options);
            return function.Invoke(argStruct).FluentFromVal(new LocContext(bundle));
        }

        public void AddFunction(CultureInfo culture, string name, LocFunction function)
        {
            var bundle = _contexts[culture];

            bundle.AddFunctionOverriding(name, (args, options)
                => CallFunction(function, bundle, args, options));
        }
    }

    internal sealed class FluentLocWrapperType : IFluentType
    {
        public readonly ILocValue WrappedValue;
        private readonly LocContext _context;

        public FluentLocWrapperType(ILocValue wrappedValue, LocContext context)
        {
            WrappedValue = wrappedValue;
            _context = context;
        }

        public string AsString()
        {
            return WrappedValue.Format(_context);
        }

        public bool IsError()
        {
            return false;
        }

        public bool Matches(IFluentType other, IScope scope)
        {
            if (other is FluentLocWrapperType otherWrapper)
            {
                return (WrappedValue, otherWrapper.WrappedValue) switch
                {
                    (LocValueNone, LocValueNone) => true,
                    (LocValueDateTime l, LocValueDateTime d) => l.Value.Equals(d.Value),
                    (LocValueTimeSpan l, LocValueTimeSpan d) => l.Value.Equals(d.Value),
                    (LocValueNumber l, LocValueNumber d) => l.Value.Equals(d.Value),
                    (LocValueString l, LocValueString d) => l.Value.Equals(d.Value),
                    (LocValueEntity l, LocValueEntity d) => l.Value.Equals(d.Value),
                    ({ } l, { } d) => Equals(l, d),
                    _ => false,
                };
            }

            return false;

        }

        public IFluentType Copy()
        {
            return this;
        }
    }

    static class LinguiniAdapter
    {
        internal static ILocValue ToLocValue(this IFluentType arg)
        {
            return arg switch
            {
                FluentNone => new LocValueNone(""),
                FluentNumber number => new LocValueNumber(number),
                FluentString str => new LocValueString(str),
                FluentLocWrapperType value => value.WrappedValue,
                _ => throw new ArgumentOutOfRangeException(nameof(arg)),
            };
        }

        public static IFluentType FluentFromObject(this object obj, LocContext context)
        {
            return obj switch
            {
                ILocValue wrap => new FluentLocWrapperType(wrap, context),
                EntityUid entity => new FluentLocWrapperType(new LocValueEntity(entity), context),
                IFluentEntityUid entity => new FluentLocWrapperType(new LocValueEntity(entity.FluentOwner), context),
                DateTime dateTime => new FluentLocWrapperType(new LocValueDateTime(dateTime), context),
                TimeSpan timeSpan => new FluentLocWrapperType(new LocValueTimeSpan(timeSpan), context),
                Color color => (FluentString)color.ToHex(),
                bool or Enum => (FluentString)obj.ToString()!.ToLowerInvariant(),
                string str => (FluentString)str,
                byte num => (FluentNumber)num,
                sbyte num => (FluentNumber)num,
                short num => (FluentNumber)num,
                ushort num => (FluentNumber)num,
                int num => (FluentNumber)num,
                uint num => (FluentNumber)num,
                long num => (FluentNumber)num,
                ulong num => (FluentNumber)num,
                double dbl => (FluentNumber)dbl,
                float dbl => (FluentNumber)dbl,
                _ => (FluentString)obj.ToString()!,
            };
        }

        public static IFluentType FluentFromVal(this ILocValue locValue, LocContext context)
        {
            return locValue switch
            {
                LocValueNone => FluentNone.None,
                LocValueNumber number => (FluentNumber)number.Value,
                LocValueString str => (FluentString)str.Value,
                _ => new FluentLocWrapperType(locValue, context),
            };
        }
    }

    internal interface IFluentEntityUid
    {
        internal EntityUid FluentOwner { get; }
    };
}
