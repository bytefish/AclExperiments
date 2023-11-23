namespace RebacExperiments.Server.Api.Infrastructure.Acl
{
    /// <summary>
    /// A Relation Tuple in the Acl.
    /// </summary>
    public record AclRelation
    {
        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public required AclKey Object { get; set; }

        /// <summary>
        /// Gets or sets the User or Userset.
        /// </summary>
        public required AclKey User { get; set; }

        /// <summary>
        /// Parses a Google Zanzibar Relation to a <see cref="AclRelation"/>.
        /// </summary>
        /// <param name="value">Google Zanzibar Relation</param>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown for invalid Relations</exception>
        public static AclRelation Parse(string value)
        {
            string[] split = value.Split("@");

            AclKey @object = AclKey.Parse(split[0]);

            if (@object.Namespace == null || @object.Id == null || @object.Relation == null)
            {
                throw new Exception("Invalid relation string: " + value);
            }
         
            AclKey user = AclKey.Parse(split[1]);

            bool isUserId = user.Namespace == null && user.Id != null && user.Relation == null;
            bool isUserset = user.Namespace != null && user.Id != null && user.Relation != null;

            if (isUserId || isUserset)
            {
                return new AclRelation 
                {
                    Object = @object,
                    User = user
                };
            }
            
            throw new Exception("Invalid relation string: " + value);
        }
    }
}
