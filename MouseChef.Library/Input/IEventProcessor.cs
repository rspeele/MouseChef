namespace MouseChef.Input
{
    public interface IEventProcessor
    {
        void DeviceInfo(DeviceInfoEvent evt);
        void Move(MoveEvent evt);
    }
}