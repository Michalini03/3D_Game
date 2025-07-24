using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Zpg.models
{
    class Collectible : BetterObj
    {
        public bool isCollected = false;

        private float baseY;

        private float elapsedTime = 0.0f;

        public Collectible(string filename, Vector3 position) : base(filename, position)
        {
            this.position = position;
            this.baseY = position.Y; // Store the initial Y position for bobbing effect
        }

        public int Collect()
        {
            isCollected = true;
            return 1;
        }

        public void Animation(float deltaTime)
        {
            elapsedTime += deltaTime;

            // Bobbing up and down (sin wave)
            Vector3 newPosition = position;
            newPosition.Y = baseY + (float)Math.Sin(elapsedTime * 2f) * 0.1f; // baseY is the starting Y position
            position = newPosition;

            // Continuous rotation
            rotationAngle += deltaTime * 2f; // Rotate faster by increasing multiplier
            if (rotationAngle > MathHelper.TwoPi)
                rotationAngle -= MathHelper.TwoPi;
        }

        public bool Check(Camera player)
        {
            Vector2 playerPos = new Vector2(player.pos.X, player.pos.Z);
            Vector2 collectiblePos = new Vector2(position.X, position.Z);
            float distance = Vector2.Distance(playerPos, collectiblePos);
            if (distance < 0.5f || isCollected)
            {
                return true;
            }
            else return false;
        }
    }
}
