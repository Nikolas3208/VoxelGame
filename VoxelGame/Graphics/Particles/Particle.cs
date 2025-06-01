using SFML.Graphics;

namespace VoxelGame.Graphics.Particles
{
    public enum ParticleType
    {
        Default,
        Texture,
        AnimatedTexture,
        Text
    }

    public class Particle : Transformable, Drawable
    {
        private Drawable? _drawable;

        public ParticleType Type { get; set; } = ParticleType.Default;

        public ParticleSystem? ParticleSystem { get; set; }

        public float LifeTime { get; set; } = 100;

        public float ElapsedTime { get; set; } = 0;

        public float TimeStep { get; set; } = 0.1f;

        public bool IsAlive => ElapsedTime < LifeTime;

        private Particle(ParticleType type, float lifeTime, float timeStep)
        {
            Type = type;
            LifeTime = lifeTime;
            TimeStep = timeStep;
        }

        public Particle(string mess, Font font, Color textColor, float lifeTime = 100f, float timeStep = 0.1f) : this(ParticleType.Text, lifeTime, timeStep)
        {
            _drawable = new Text(mess, font)
            {
                FillColor = textColor
            };
        }

        public virtual void Update(float deltaTime)
        {
            ElapsedTime += deltaTime * TimeStep;

            if(!IsAlive)
                ParticleSystem?.RemoveParticle(this);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(_drawable, states);
        }
    }
}
