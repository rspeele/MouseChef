namespace MouseChef.Input
{
    public class Event
    {
        /// <summary>
        /// Determins which of the event data properties will be populated (all but one will be null).
        /// </summary>
        public EventType Type { get; set; }
        /// <summary>
        /// Mouse movement data, if Type == EventType.Move.
        /// </summary>
        public MoveEvent Move { get; set; }
        /// <summary>
        /// Device data, if Type == EventType.DeviceInfo.
        /// </summary>
        public DeviceInfoEvent DeviceInfo { get; set; }
    }
}
