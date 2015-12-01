namespace Core.Domain.Hyperv
{
    public class VMCreation : BaseModel
    {
        public string Vmname { get; set; }

        public string Vmpath { get; set; }

        public string Physicaladapter { get; set; }

        public string SwitchName { get; set; }

        public string Maxmem { get; set; }

        public string Minmem { get; set; }

        public string Isopath { get; set; }

        public string Hddsize { get; set; }

    }
}