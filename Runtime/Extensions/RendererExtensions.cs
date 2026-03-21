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
                case SpriteRenderer spriteRenderer:
                    spriteRenderer.SetShaderUserValue(value);
                    break;
                case SpriteShapeRenderer spriteShapeRenderer:
                    spriteShapeRenderer.SetShaderUserValue(value);
                    break;
                case TilemapRenderer tilemapRenderer:
                    tilemapRenderer.SetShaderUserValue(value);
                    break;
                default:
                    throw new NotImplementedException($"{renderer.GetType()} doesn't support Shader User Value");
            }
        }
    }
}
