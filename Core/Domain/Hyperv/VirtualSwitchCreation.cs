namespace Core.Domain
{
    public class VirtualSwitchCreation : BaseModel
    {
        public string SwitchName { get; set; }

        public string PhysicalAdapter { get; set; }

        public string AllowManagementOs { get; set; }

    }
}