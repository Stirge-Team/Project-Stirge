using System;
using UnityEngine;

namespace Stirge.Tools
{
    public static class JumpFormula
    {
        public static float EvaluateVelocity(float jumpHeight)
        {
            float g = -Physics.gravity.magnitude / 2f;
            float m = Mathf.Sqrt(-jumpHeight / g);

            // f(x) = g(x - m)^2 + j, where f(x) is height in world
            // as f(x) is height at time x, that means that f'(x) gives us velocity at time x
            // so if we want the velocity at time 0, we can use f'(0)
            // however I don't know how to get f'(x), so instead we can just use rise over run
            // but we can't do rise over run at 0 because you can't divide by 0
            // therefore, velocity ~= f(Epsilon) / Epsilon, as Epsilon is nearly 0 but not (cannot be zero as will cause divide by 0 error)
            // this gives us the closest value of the velocity at time 0 seconds, which would be the velocity required to reach a height of jumpHeight
            // values differ by ~0.0005 so its pretty much the same
            return (g * Mathf.Pow(Mathf.Epsilon - m, 2f) + jumpHeight) / Mathf.Epsilon;
        }
    }
}
