grammar UsersetRewrite;

namespace
    : 'name' ':' namespaceName=STRING relation* EOF
    ;

relation
    : 'relation' '{' 'name' ':' relationName=STRING usersetRewrite? '}'
    ;

usersetRewrite
    : 'userset_rewrite' '{' userset '}'
    ;

userset
    : childUserset
    | computedUserset
    | setOperationUserset
    | thisUserset
    | tupleToUserset
    ;

childUserset
    : 'child' '{' userset '}'
    ;

computedUserset
    : 'computed_userset' '{' (usersetNamespaceRef | usersetObjectRef | usersetRelationRef)+ '}'
    ;

usersetNamespaceRef
    : 'namespace' ':' ref=(STRING | TUPLE_USERSET_NAMESPACE)
    ;

usersetObjectRef
    : 'object' ':' ref=(STRING | TUPLE_USERSET_OBJECT)
    ;

usersetRelationRef
    : 'relation' ':' ref=(STRING | TUPLE_USERSET_RELATION)
    ;

thisUserset
    : '_this' '{' '}'
    ;

tupleToUserset
    : 'tuple_to_userset' '{' tupleset computedUserset '}'
    ;

tupleset
    : 'tupleset' '{' (namespaceRef | objectRef | relationRef)+ '}'
    ;

namespaceRef
    : 'namespace' ':' ref=STRING
    ;

objectRef
    : 'object' ':' ref=STRING
    ;

relationRef
    : 'relation' ':' ref=STRING
    ;

setOperationUserset
    : op=(UNION | INTERSECT | EXCLUDE) '{' userset* '}'
    ;

UNION
    : 'union'
    ;

INTERSECT
    : 'intersect'
    ;

EXCLUDE
    : 'exclude'
    ;

TUPLE_USERSET_NAMESPACE
    : '$TUPLE_USERSET_NAMESPACE'
    ;

TUPLE_USERSET_OBJECT
    : '$TUPLE_USERSET_OBJECT'
    ;

TUPLE_USERSET_RELATION
    : '$TUPLE_USERSET_RELATION'
    ;

STRING
   : '"' ~["]* '"'
   | '\'' ~[']* '\''
   ;

SINGLE_LINE_COMMENT
   : '//' .*? (NEWLINE | EOF) -> skip
   ;

MULTI_LINE_COMMENT
   : '/*' .*? '*/' -> skip
   ;

IDENTIFIER
   : IDENTIFIER_START IDENTIFIER_PART*
   ;
fragment IDENTIFIER_START
   : [\p{L}]
   | '$'
   | '_'
   ;
fragment IDENTIFIER_PART
   : IDENTIFIER_START
   | [\p{M}]
   | [\p{N}]
   | [\p{Pc}]
   | '\u200C'
   | '\u200D'
   ;
fragment NEWLINE
   : '\r\n'
   | [\r\n\u2028\u2029]
   ;

WS
   : [ \t\n\r\u00A0\uFEFF\u2003] + -> skip
   ;