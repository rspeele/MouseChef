namespace MouseChef.Input
{
    public interface IEventProcessor
    {
        void StoreEvent(Event evt);
        void DeviceInfo(DeviceInfoEvent evt);
        void Move(MoveEvent evt);
    }
}