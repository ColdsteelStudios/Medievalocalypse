// ---------------------------------------------------------------------------
// BuildType.cs
// 
// Attached to system object, used to set build type.
// Changes player controls to be appropriate for that build type
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class BuildType : MonoBehaviour
{
    public enum _BuildType
    {
        PC = 0,
        Console = 1,
        Android = 2
    }

    public _BuildType m_buildType;
}