namespace AclExperiment.CheckExpand.Models
{
    public record AclObject
    {
        public required string Namespace { get; set; }

        public required string Id { get; set; }
    }
}
