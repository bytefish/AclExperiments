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

DECLARE @google_drive_json NVARCHAR(MAX) = N'
{
  "modelKey": "google-drive",
  "name": "Google Drive",
  "description": "An Authorization Model for Google Drive",
  "namespaces": [
    {
      "name": "user",
      "version": 1,
      "metadata": {
        "relations": {}
      },
      "relations": {}
    },
    {
      "name": "group",
      "version": 1,
      "metadata": {
        "relations": {
          "member": {
            "directly_related_types": [
              {
                "namespace": "user"
              }
            ]
          }
        }
      },
      "relations": {
        "member": {
          "$type": "_this"
        }
      }
    },
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
              },
              {
                "namespace": "group",
                "relation": "member"
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
    },
    {
      "name": "doc",
      "version": 1,
      "metadata": {
        "relations": {
          "parent": {
            "directly_related_types": [
              {
                "namespace": "folder"
              }
            ]
          },
          "owner": {
            "directly_related_types": [
              {
                "namespace": "user"
              }
            ]
          },
          "viewer": {
            "directly_related_types": [
              {
                "namespace": "user"
              },
              {
                "namespace": "group",
                "relation": "member"
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
  ]
}
';

MERGE INTO [Identity].[AuthorizationModel] AS [Target]
USING (VALUES 
       (1, 'google-drive', 'Google Drive', 'Google Drive Example', @google_drive_json, 1, @ValidFrom, @ValidTo)
) AS [Source]
(
     [AuthorizationModelID] 
    ,[ModelKey] 
    ,[Name] 
    ,[Description]       
    ,[Content]  
    ,[LastEditedBy]    
    ,[ValidFrom]       
    ,[ValidTo]         
)
ON (
    [Target].[ModelKey] = [Source].[ModelKey]
)
WHEN NOT MATCHED BY TARGET THEN
    INSERT 
        (
             [AuthorizationModelID] 
            ,[ModelKey] 
            ,[Name] 
            ,[Description]       
            ,[Content]  
            ,[LastEditedBy]    
            ,[ValidFrom]       
            ,[ValidTo]                 
        )
    VALUES 
        (
             [Source].[AuthorizationModelID] 
            ,[Source].[ModelKey] 
            ,[Source].[Name] 
            ,[Source].[Description]       
            ,[Source].[Content]  
            ,[Source].[LastEditedBy]    
            ,[Source].[ValidFrom]       
            ,[Source].[ValidTo]         
        );