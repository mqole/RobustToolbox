using System;
using System.Collections.Generic;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects.Components.Localization;

/// <summary>
///     Overrides grammar attributes specified in prototypes or localization files.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
// [Access(typeof(GrammarSystem))] TODO access
public sealed partial class GrammarComponent : Component
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    [DataField, AutoNetworkedField]
    public Dictionary<string, string> Attributes = new();

    [ViewVariables]
    public Pronoun? Pronoun
    {
        get => Attributes.TryGetValue("pronoun", out var p) ?
            _proto.Index<Pronoun>(p) : null;
        [Obsolete("Use GrammarSystem.SetPronoun instead")]
        set => IoCManager.Resolve<IEntityManager>().System<GrammarSystem>().SetPronoun((Owner, this), value);
    }

    [ViewVariables]
    public bool? ProperNoun
    {
        get => Attributes.TryGetValue("proper", out var g) ? bool.Parse(g) : null;
        [Obsolete("Use GrammarSystem.SetProperNoun instead")]
        set => IoCManager.Resolve<IEntityManager>().System<GrammarSystem>().SetProperNoun((Owner, this), value);
    }
}
