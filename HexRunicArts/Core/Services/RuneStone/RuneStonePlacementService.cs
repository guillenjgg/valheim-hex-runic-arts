using UnityEngine;

namespace HexRunicArts.Core.Services.RuneStone
{
    internal static class RuneStonePlacementService
    {
        private const float SampleDistance = 2f;
        private const float MaximumHeightDifference = 1f;
        private const float ObstacleCheckRadius = 2f;
        private const float ObstacleCheckHeight = 3f;

        internal static bool TryGetValidPosition(Vector3 targetPosition, out Vector3 validPosition)
        {
            validPosition = Vector3.zero;

            if (!Heightmap.GetHeight(targetPosition, out var centerHeight))
            {
                return false;
            }

            var north = targetPosition + Vector3.forward * SampleDistance;
            var south = targetPosition + Vector3.back * SampleDistance;
            var east = targetPosition + Vector3.right * SampleDistance;
            var west = targetPosition + Vector3.left * SampleDistance;

            if (!Heightmap.GetHeight(north, out var northHeight) ||
                !Heightmap.GetHeight(south, out var southHeight) ||
                !Heightmap.GetHeight(east, out var eastHeight) ||
                !Heightmap.GetHeight(west, out var westHeight))
            {
                return false;
            }

            var minimumHeight = Mathf.Min(centerHeight, northHeight, southHeight, eastHeight, westHeight);
            var maximumHeight = Mathf.Max(centerHeight, northHeight, southHeight, eastHeight, westHeight);

            if (maximumHeight - minimumHeight > MaximumHeightDifference)
            {
                return false;
            }

            var obstacleCenter = new Vector3(
                targetPosition.x,
                centerHeight + ObstacleCheckHeight * 0.5f,
                targetPosition.z
            );

            var colliders = Physics.OverlapBox(
                obstacleCenter,
                new Vector3(ObstacleCheckRadius, ObstacleCheckHeight * 0.5f, ObstacleCheckRadius),
                Quaternion.identity,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            );

            foreach (var collider in colliders)
            {
                if (collider.GetComponentInParent<Heightmap>() != null)
                {
                    continue;
                }

                return false;
            }

            validPosition = new Vector3(targetPosition.x, centerHeight, targetPosition.z);

            return true;
        }
    }
}