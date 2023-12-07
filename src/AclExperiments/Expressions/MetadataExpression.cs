using System.Text.Json.Serialization;

namespace AclExperiments.Expressions
{
    public record MetadataExpression : UsersetExpression
    {
        [JsonPropertyName("relations")]
        public Dictionary<string, MetadataRelationExpression> Relations { get; set; } = new();
    }

    public record MetadataRelationExpression : UsersetExpression
    {
        [JsonPropertyName("directly_related_types")]
        public List<DirectlyRelatedType> DirectlyRelatedTypes { get; set; } = new();
    }

    public record DirectlyRelatedType : UsersetExpression
    {
        [JsonPropertyName("namespace")]
        public required string Namespace { get; set; }

        [JsonPropertyName("relation")]
        public string? Relation { get; set; }
    }
}
