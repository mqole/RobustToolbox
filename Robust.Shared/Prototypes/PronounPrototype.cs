using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Prototypes;

[Prototype]
public sealed partial class Pronoun : IPrototype
{
    // Entities without a grammar component will most frequently be inanimate objects, so we use 'it' as a default value.

    /// <summary>
    ///     Prototype ID of the pronoun set.
    /// </summary>
    [IdDataField]
    public string ID { get; private set; } = "it";

    /// <summary>
    ///     Is this pronoun user defined, or engine defined?
    ///     Used to determine whether we construct Loc strings, or just use the string itself as the pronoun.
    /// </summary>
    [DataField]
    public bool Custom { get; private set; } = false;

    /// <summary>
    ///     Subject pronoun.
    ///     Example: "I think SHE is very nice".
    /// </summary>
    [DataField]
    public LocId Subject { get; private set; } = "-subject";

    /// <summary>
    ///     Object pronoun.
    ///     Example: "I met HER recently".
    /// </summary>
    [DataField]
    public LocId Object { get; private set; } = "-object";

    /// <summary>
    ///     Dative form pronoun.
    ///     Not used in en-US.
    ///     Example: "to HIM", "for HER".
    /// </summary>
    [DataField]
    public LocId DatObj { get; private set; } = "-dative";

    /// <summary>
    ///     Genitive form pronoun.
    ///     Not used in en-US.
    ///     Example: "у него" (Russian), "seines Vaters" (German).
    /// </summary>
    [DataField]
    public LocId Genitive { get; private set; } = "-genitive";

    /// <summary>
    ///     Possessive pronoun.
    ///     Example: "She told me that the house is HERS".
    /// </summary>
    [DataField]
    public LocId PossPronoun { get; private set; } = "-poss-pronoun";

    /// <summary>
    ///     Possessive determiner (adjective) pronoun.
    ///     Example: "Is this HER dog?"
    /// </summary>
    [DataField]
    public LocId PossAdj { get; private set; } = "-poss-adjective";

    /// <summary>
    ///     Reflexive pronoun.
    ///     Example: "She said she would rather do it HERSELF".
    /// </summary>
    [DataField]
    public LocId Reflexive { get; private set; } = "-reflexive";

    /// <summary>
    ///     Counter or measure word.
    ///     Not used in en-US.
    ///     Example: "两个人", "一本书".
    /// </summary>
    [DataField]
    public LocId Counter { get; private set; } = "-counter";

    // Note regarding conjugation:
    // Currently, RT has built in support for en-US and pt-BR.
    // Both these languages have relatively simple verb conjugation, so only a bool is needed here.
    // If support for other languages is desired this will probably need to be modified.
    /// <summary>
    ///     Does this pronoun conjugate plurally?
    ///     Example: false for "he IS", true for "they ARE".
    /// </summary>
    [DataField]
    public bool Plural { get; private set; } = false;
}
