namespace DomainQueryMapper.Tests.TestObjects.Domain
{
    public class Appraiser
    {
        public int AppraiserId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public User User { get; set; }
    }
}