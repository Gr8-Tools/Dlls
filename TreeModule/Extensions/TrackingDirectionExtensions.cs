using TreeModule.Utils;

namespace TreeModule.Extensions
{
    public static class TrackingDirectionExtensions
    {
        public static TrackingDirection GetOpposite(this TrackingDirection trackingDirection)
        {
            switch (trackingDirection)
            {
                case TrackingDirection.ToBrother:
                    return TrackingDirection.ToChild;
                case TrackingDirection.ToChild:
                    return TrackingDirection.ToBrother;
                default: return TrackingDirection.Null;
            }
        }
    }
}