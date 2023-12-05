CREATE UNIQUE INDEX [UX_RelationTuple_UniqueObjectSubjectRelation] 
    ON [Identity].[RelationTuple] (
        [Namespace], 
        [Object], 
        [Relation], 
        [SubjectNamespace],
        [Subject],
        [SubjectRelation])