using System;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

namespace UnityEngine.RSUVBitPacker
{
    public static class RendererExtensions
    {
        public static void SetShaderUserValue(this Renderer renderer, uint value)
        {
            switch (renderer)
            {
                case MeshRenderer meshRenderer:
                    meshRenderer.SetShaderUserValue(value);
                    break;
                case SkinnedMeshRenderer skinnedMeshRenderer:
                    skinnedMeshRenderer.SetShaderUserValue(value);
                    break;
#if UNITY_6000_3_13_OR_NEWER || UNITY_6000_4_4_OR_NEWER || UNITY_6000_5_OR_NEWER
                case SpriteRenderer spriteRenderer:
                    spriteRenderer.SetShaderUserValue(value);
                    break;
                case SpriteShapeRenderer spriteShapeRenderer:
                    spriteShapeRenderer.SetShaderUserValue(value);
                    break;
                case TilemapRenderer tilemapRenderer:
                    tilemapRenderer.SetShaderUserValue(value);
                    break;
#endif
                case null:
                    throw new NullReferenceException();
                default:
                    throw new NotImplementedException($"{renderer.GetType()} doesn't support Shader User Value");
            }
        }

        public static uint GetShaderUserValue(this Renderer renderer) => renderer switch
        {
            MeshRenderer meshRenderer => meshRenderer.GetShaderUserValue(),
            SkinnedMeshRenderer skinnedMeshRenderer => skinnedMeshRenderer.GetShaderUserValue(),
#if UNITY_6000_3_13_OR_NEWER || UNITY_6000_4_4_OR_NEWER || UNITY_6000_5_OR_NEWER
            SpriteRenderer spriteRenderer => spriteRenderer.GetShaderUserValue(),
            SpriteShapeRenderer spriteShapeRenderer => spriteShapeRenderer.GetShaderUserValue(),
            TilemapRenderer tilemapRenderer => tilemapRenderer.GetShaderUserValue(),
#endif
            null => throw new NullReferenceException(),
            _ => throw new NotImplementedException($"{renderer.GetType()} doesn't support Shader User Value")
        };

        public static bool SupportsShaderUserValue(this Renderer renderer) => renderer switch
        {
            MeshRenderer meshRenderer => true,
            SkinnedMeshRenderer skinnedMeshRenderer => true,
#if UNITY_6000_3_13_OR_NEWER || UNITY_6000_4_4_OR_NEWER || UNITY_6000_5_OR_NEWER
            SpriteRenderer spriteRenderer => true,
            SpriteShapeRenderer spriteShapeRenderer => true,
            TilemapRenderer tilemapRenderer => true,
#endif
            null => throw new NullReferenceException(),
            _ => false
        };
    }
}
