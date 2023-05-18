using System.Reflection;

namespace CitasPlus.Models
{
    public class CreateAppoimentViewModel
    {
        public Guid User_Id { get; set; }
        public string NameClient { get; set; }
        public string DateStart { get; set; }
        public int DateEstimated { get; set; }

    }
}