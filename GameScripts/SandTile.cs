using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniJam_Warmth.GameScripts;

public class Sand : Entity
{
    private Texture2D _sprite;
    public override Texture2D sprite => _sprite;
    public override Vector2 origin => Vector2.Zero;

    public Sand(Vector2 position) : base()
    {
        this.position = position;
        _sprite = MainGame.Instance.SandTextures.Random();
    }
}