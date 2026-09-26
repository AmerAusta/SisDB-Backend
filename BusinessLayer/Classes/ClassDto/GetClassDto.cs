namespace BusinessLayer.Classes.ClassDto
{
    public class GetClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public string Branch { get; set; } = null!;
        public string AcademicYear { get; set; } = null!;
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
    }
}