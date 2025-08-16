using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Prototypes;

[Prototype]
public sealed partial class Pronoun : IPrototype
{
    // NOTE:
    // Most of these datafields should remain required:false to enable support for non en-US languages.

    /// <summary>
    ///     Prototype ID of the pronoun set.
    /// </summary>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    ///     Subject pronoun.
    ///     Example: "I think SHE is very nice".
    /// </summary>
    [DataField]
    public string Subject { get; private set; } = default!;

    /// <summary>
    ///     Object pronoun.
    ///     Example: "I met HER recently".
    /// </summary>
    [DataField]
    public string Object { get; private set; } = default!;

    /// <summary>
    ///     Not used in en-US.
    ///     Example: "to HIM", "for HER".
    /// </summary>
    [DataField]
    public string DatObj { get; private set; } = default!;

    /// <summary>
    ///     Not used in en-US.
    ///     Example: "у него" (Russian), "seines Vaters" (German).
    /// </summary>
    [DataField]
    public string Genitive { get; private set; } = default!;

    /// <summary>
    ///     Possessive pronoun.
    ///     Example: "She told me that the house is HERS".
    /// </summary>
    [DataField]
    public string PossPronoun { get; private set; } = default!;

    /// <summary>
    ///     Possessive determiner (adjective) pronoun.
    ///     Example: "Is this HER dog?"
    /// </summary>
    [DataField]
    public string PossAdj { get; private set; } = default!;

    /// <summary>
    ///     Reflexive pronoun.
    ///     Example: "She said she would rather do it HERSELF".
    /// </summary>
    [DataField]
    public string Reflexive { get; private set; } = default!;

    /// <summary>
    ///     Does this pronoun conjugate plurally?
    ///     Example: false for "he IS", true for "they ARE".
    /// </summary>
    [DataField]
    public bool Plural { get; private set; } = false;
}
