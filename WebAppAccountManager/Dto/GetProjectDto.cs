using System.Collections.Generic;

namespace WebAppAccountManager.Dto
{
    public class GetProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<GetEmployeeOfProjectDto> Employees { get; set; }
        

    }
}
