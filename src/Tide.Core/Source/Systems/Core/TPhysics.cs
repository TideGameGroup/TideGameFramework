using Microsoft.Xna.Framework;
using System;

namespace Tide.Core
{
    public struct TPhysicsConstructorArgs
    {
        public int numPhysicsSubsteps;
    }

    public class TPhysics : ISystem
    {
        private readonly int numPhysicsSubsteps;

        public TPhysics(TPhysicsConstructorArgs args)
        {
            numPhysicsSubsteps = args.numPhysicsSubsteps;
        }

        public void Draw(TComponentGraph graph, GameTime gameTime)
        {
        }

        public void Update(TComponentGraph graph, GameTime gameTime)
        {
            TimeSpan elapsedStepTime = gameTime.ElapsedGameTime / numPhysicsSubsteps;
            GameTime stepTime = new GameTime(
                gameTime.TotalGameTime - gameTime.ElapsedGameTime,
                gameTime.ElapsedGameTime / numPhysicsSubsteps
                );

            foreach (UComponent script in graph)
            {
                if (script is IPhysicsComponent component && script.IsActive)
                {
                    component.PrePhysics(gameTime);
                }
            }

            for (int n = 0; n < numPhysicsSubsteps; n++)
            {
                stepTime.TotalGameTime += elapsedStepTime * (n + 1);

                foreach (UComponent script in graph)
                {
                    if (script is IPhysicsComponent component && script.IsActive)
                    {
                        component.CollisionUpdate(stepTime);
                    }
                }

                foreach (UComponent script in graph)
                {
                    if (script is IPhysicsComponent component && script.IsActive)
                    {
                        component.PhysicsUpdate(stepTime);
                    }
                }
            }

            foreach (UComponent script in graph)
            {
                if (script is IPhysicsComponent component && script.IsActive)
                {
                    component.PostPhysics(gameTime);
                }
            }
        }
    }
}