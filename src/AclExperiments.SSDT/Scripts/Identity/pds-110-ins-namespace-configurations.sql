PRINT 'Inserting [Identity].[NamespaceConfiguration] ...'
GO

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'


-----------------------------------------------
-- [Identity].[NamespaceConfiguration]
-----------------------------------------------

DECLARE @user_json NVARCHAR(MAX) = N'
{
  "name": "user",
  "version": 1,
  "metadata": {
    "relations": {}
  },
  "relations": {}
}
';

DECLARE @doc_json NVARCHAR(MAX) = N'
{
  "name": "doc",
  "version": 1,
  "metadata": {
    "relations": {
      "owner": {
        "directly_related_types": [
          {
            "namespace": "user"
          }
        ]
      },
      "parent": {
        "directly_related_types": [
          {
            "namespace": "folder"
          }
        ]
      },
      "viewer": {
        "directly_related_types": [
          {
            "namespace": "user"
          }
        ]
      }
    }
  },
  "relations": {
    "parent": {
      "$type": "_this"
    },
    "owner": {
      "$type": "_this"
    },
    "editor": {
      "$type": "set_operation",
      "operation": "union",
      "children": [
        {
          "$type": "_this"
        },
        {
          "$type": "computed_userset",
          "relation": "owner"
        }
      ]
    },
    "viewer": {
      "$type": "set_operation",
      "operation": "union",
      "children": [
        {
          "$type": "_this"
        },
        {
          "$type": "computed_userset",
          "relation": "owner"
        },
        {
          "$type": "tuple_to_userset",
          "tupleset": {
            "relation": "parent"
          },
          "computed_userset": {
            "relation": "viewer"
          }
        }
      ]
    }
  }
}
';

DECLARE @folder_json NVARCHAR(MAX) = N'
{
  "name": "folder",
  "version": 1,
  "metadata": {
    "relations": {
      "owner": {
        "directly_related_types": [
          {
            "namespace": "user"
          }
        ]
      },
      "parent": {
        "directly_related_types": [
          {
            "namespace": "folder"
          }
        ]
      },
      "viewer": {
        "directly_related_types": [
          {
            "namespace": "user"
          }
        ]
      }
    }
  },
  "relations": {
    "parent": {
      "$type": "_this"
    },
    "owner": {
      "$type": "_this"
    },
    "editor": {
      "$type": "set_operation",
      "operation": "union",
      "children": [
        {
          "$type": "_this"
        },
        {
          "$type": "computed_userset",
          "relation": "owner"
        }
      ]
    },
    "viewer": {
      "$type": "set_operation",
      "operation": "union",
      "children": [
        {
          "$type": "_this"
        },
        {
          "$type": "computed_userset",
          "relation": "owner"
        },
        {
          "$type": "tuple_to_userset",
          "tupleset": {
            "relation": "parent"
          },
          "computed_userset": {
            "relation": "viewer"
          }
        }
      ]
    }
  }
}
';

MERGE INTO [Identity].[NamespaceConfiguration] AS [Target]
USING (VALUES 
       (1, 'user',      1,      @user_json, 1, @ValidFrom, @ValidTo) 
      ,(2, 'folder',    1,      @folder_json, 1, @ValidFrom, @ValidTo) 
      ,(3, 'doc',       1,      @doc_json, 1, @ValidFrom, @ValidTo) 
) AS [Source]
(
     [NamespaceConfigurationID] 
    ,[Name] 
    ,[Version]       
    ,[Content]  
    ,[LastEditedBy]    
    ,[ValidFrom]       
    ,[ValidTo]         
)
ON (
    [Target].[Name] = [Source].[Name] AND [Target].[Version] = [Source].[Version]
)
WHEN NOT MATCHED BY TARGET THEN
    INSERT 
        (
             [NamespaceConfigurationID] 
            ,[Name] 
            ,[Version]       
            ,[Content]  
            ,[LastEditedBy]    
            ,[ValidFrom]       
            ,[ValidTo]         
        )
    VALUES 
        (
             [Source].[NamespaceConfigurationID] 
            ,[Source].[Name] 
            ,[Source].[Version]       
            ,[Source].[Content]  
            ,[Source].[LastEditedBy]    
            ,[Source].[ValidFrom]       
            ,[Source].[ValidTo]         
        );