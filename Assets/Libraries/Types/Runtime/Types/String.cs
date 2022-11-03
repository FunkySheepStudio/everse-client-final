using UnityEngine;

namespace FunkySheep.Types
{
    [CreateAssetMenu(menuName = "FunkySheep/Types/String")]
    public class String : Type<System.String>
    {
        /// <summary>
        /// Interpolate au string passing it the variables to replace
        /// </summary>
        /// <param name="variables">The values to set/param>
        /// <param name="names">The names in the interpolated string to replace</param>
        /// <returns></returns>
        public string Interpolate(string[] variables, string[] names)
        {
            string interpolatedValue = this.Value;
            for (int i = 0; i < variables.Length; i++)
            {
                interpolatedValue = interpolatedValue.Replace("{" + names[i] + "}", variables[i]);
            }
            return interpolatedValue;
        }
    }
}