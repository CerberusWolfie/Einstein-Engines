using Content.Client.Stylesheets;
using Content.Shared.StatusIcon;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using System.Numerics;

namespace Content.Client.Access.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class AgentIDCardWindow : DefaultWindow
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IEntitySystemManager _entitySystem = default!;
        private readonly SpriteSystem _spriteSystem;

        private const int JobIconColumnCount = 10;

        public event Action<string>? OnNameChanged;
        public event Action<string>? OnJobChanged;

        public event Action<ProtoId<StatusIconPrototype>>? OnJobIconChanged;

        public AgentIDCardWindow()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);
            _spriteSystem = _entitySystem.GetEntitySystem<SpriteSystem>();

            NameLineEdit.OnTextEntered += e => OnNameChanged?.Invoke(e.Text);
            NameLineEdit.OnFocusExit += e => OnNameChanged?.Invoke(e.Text);

            JobLineEdit.OnTextEntered += e => OnJobChanged?.Invoke(e.Text);
            JobLineEdit.OnFocusExit += e => OnJobChanged?.Invoke(e.Text);
        }

        public void SetAllowedIcons(HashSet<string> icons, string currentJobIconId)
        {
            IconGrid.DisposeAllChildren();

            var jobIconGroup = new ButtonGroup();
            var i = 0;
            foreach (var jobIconId in icons)
            {
                if (!_prototypeManager.TryIndex<StatusIconPrototype>(jobIconId, out var jobIcon))
                {
                    continue;
                }

                String styleBase = StyleBase.ButtonOpenBoth;
                var modulo = i % JobIconColumnCount;
                if (modulo == 0)
                    styleBase = StyleBase.ButtonOpenRight;
                else if (modulo == JobIconColumnCount - 1)
                    styleBase = StyleBase.ButtonOpenLeft;

                // Generate buttons
                var jobIconButton = new Button
                {
                    Access = AccessLevel.Public,
                    StyleClasses = { styleBase },
                    MaxSize = new Vector2(42, 28),
                    Group = jobIconGroup,
                    Pressed = i == 0,
                };

                // Generate buttons textures
                var jobIconTexture = new TextureRect
                {
                    Texture = _spriteSystem.Frame0(jobIcon.Icon),
                    TextureScale = new Vector2(2.5f, 2.5f),
                    Stretch = TextureRect.StretchMode.KeepCentered,
                };

                jobIconButton.AddChild(jobIconTexture);
                jobIconButton.OnPressed += _ => OnJobIconChanged?.Invoke(jobIcon.ID);
                IconGrid.AddChild(jobIconButton);

                if (jobIconId.Equals(currentJobIconId))
                    jobIconButton.Pressed = true;

                i++;
            }
        }

        public void SetCurrentName(string name)
        {
            NameLineEdit.Text = name;
        }

        public void SetCurrentJob(string job)
        {
            JobLineEdit.Text = job;
        }
    }
}
