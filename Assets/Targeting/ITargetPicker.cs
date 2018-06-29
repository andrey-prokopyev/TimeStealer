using UnityEngine;

namespace Targeting
{
    public interface ITargetPicker
    {
        GameObject PickFor(GameObject pursuer);
    }
}