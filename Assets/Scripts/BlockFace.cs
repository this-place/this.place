using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockFace
{
    Top,
    Bottom,
    North,
    South,
    East,
    West,
    Unknown
}

public static class BlockFaceMethods
{
    public static BlockFace BlockFaceFromNormal(Vector3 normal)
    {
        BlockFace result =
           normal == Vector3.up ? BlockFace.Top :
           normal == Vector3.down ? BlockFace.Bottom :
           normal == Vector3.forward ? BlockFace.North :
           normal == Vector3.back ? BlockFace.South :
           normal == Vector3.right ? BlockFace.East :
           normal == Vector3.left ? BlockFace.West : BlockFace.Unknown;

        if (result == BlockFace.Unknown)
        {
            Debug.Log("Unknown Face Detected. Make sure North face faces Vector3.forward and Top face faces Vector3.up");
        }

        return result;
    }

    public static Vector3 GetNormal(this BlockFace face)
    {
        switch (face)
        {
            case BlockFace.Top:
                return Vector3.up;
            case BlockFace.Bottom:
                return Vector3.down;
            case BlockFace.North:
                return Vector3.forward;
            case BlockFace.South:
                return Vector3.back;
            case BlockFace.East:
                return Vector3.right;
            case BlockFace.West:
                return Vector3.left;
            default: // unknown
                Debug.Log("Unknown Face Detected. Make sure North face faces Vector3.forward and Top face faces Vector3.up");
                return Vector3.zero;
        }
    }

    public static Vector3[] GetPerpendicularNormals(this BlockFace face)
    {
        switch (face)
        {
            case BlockFace.Top:
            case BlockFace.Bottom:
                return new Vector3[] { Vector3.forward, Vector3.right };
            case BlockFace.North:
            case BlockFace.South:
                return new Vector3[] { Vector3.up, Vector3.right };
            case BlockFace.East:
            case BlockFace.West:
                return new Vector3[] { Vector3.up, Vector3.forward };
            default: // unknown
                Debug.Log("Unknown Face Detected. Make sure North face faces Vector3.forward and Top face faces Vector3.up");
                return new Vector3[2];
        }
    }

    public static BlockFace GetOppositeFace(this BlockFace face)
    {
        switch (face)
        {
            case BlockFace.Top:
                return BlockFace.Bottom;
            case BlockFace.Bottom:
                return BlockFace.Top;
            case BlockFace.North:
                return BlockFace.South;
            case BlockFace.South:
                return BlockFace.North;
            case BlockFace.East:
                return BlockFace.West;
            case BlockFace.West:
                return BlockFace.East;
            default: // unknown
                Debug.Log("Unknown Face Detected. Make sure North face faces Vector3.forward and Top face faces Vector3.up");
                return BlockFace.Unknown;
        }
    }
}
