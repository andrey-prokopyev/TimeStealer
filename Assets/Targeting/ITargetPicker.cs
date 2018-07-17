using UnityEngine;

namespace Targeting
{
    public interface ITargetPicker
    {
        Vector3 PickFor(GameObject pursuer);
    }
}