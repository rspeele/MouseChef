namespace MouseChef.Input
{
    public class MoveEvent
    {
        public int DeviceId { get; set; }
        public long Us { get; set; }
        public int Dx { get; set; }
        public int Dy { get; set; }
    }
}