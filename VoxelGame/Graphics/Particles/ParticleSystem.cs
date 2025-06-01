using SFML.Graphics;

namespace VoxelGame.Graphics.Particles
{
    public class ParticleSystem : Transformable, Drawable
    {
        public int MaxParticleCount { get; private set; } = 100;

        public List<Particle> Particles { get; private set; }

        public ParticleSystem(int maxParticleCount = 100)
        {
            MaxParticleCount = maxParticleCount;

            Particles = new List<Particle>();
        }

        public ParticleSystem(params Particle[] particles) : this(particles.Length)
        {
            Particles = new List<Particle>(particles);
            foreach (var particle in particles)
            {
                particle.ParticleSystem = this;
            }
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i]?.Update(deltaTime);
            }
        }

        public void AddParticle(Particle particle)
        {
            particle.ParticleSystem = this;
            Particles.Add(particle);
        }

        public void RemoveParticle(Particle particle)
        {
            if (Particles.Contains(particle))
            {
                Particles.Remove(particle);
            }
        }

        public Particle GetParticle(int index)
        {
            if (index < 0 || index >= Particles.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            return Particles[index];
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            foreach (var particle in Particles)
            {
                if (particle.IsAlive)
                {
                    particle.Draw(target, states);
                }
            }
        }
    }
}