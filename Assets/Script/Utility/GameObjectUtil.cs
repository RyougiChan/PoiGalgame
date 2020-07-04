using UnityEngine;

namespace Assets.Script.Utility
{
    public class GameObjectUtil
    {
        /// <summary>
        /// Find child game component of <paramref name="parent"/> via <paramref name="fullPath"/>
        /// </summary>
        /// <typeparam name="T">Type of the game component</typeparam>
        /// <param name="parent">parent game object</param>
        /// <param name="fullPath">path format in `path1:path2:path3...`</param>
        /// <returns>The component found or null</returns>
        public static T FindComponentByPath<T>(GameObject parent, string fullPath)
        {
            return (null == FindChildTransform(parent, fullPath) ? default(T) : FindChildTransform(parent, fullPath).GetComponent<T>());
        }

        /// <summary>
        /// Find child tramsform of <paramref name="parent"/> via <paramref name="fullPath"/>
        /// </summary>
        /// <param name="parent">parent game object</param>
        /// <param name="fullPath">path format in `path1:path2:path3...`</param>
        /// <returns>The transform or null</returns>
        public static Transform FindChildTransform(GameObject parent, string fullPath)
        {
            Transform tmp = parent.transform;
            string[] ps = fullPath.Split(':');
            foreach (string p in ps)
            {
                if ((tmp = tmp.Find(p)) == null) return null;
            }
            return tmp;
        }

    }
}
