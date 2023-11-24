@echo off

:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

:: ANTLR4 Executable
set ANTLR4_JAR=%~dp0tools\antlr-4.13.1-complete.jar  

:: Parameters for the Code Generator
set PARAM_USERSET_GRAMMAR=%~dp0\RebacExperiments\RebacExperiments.Acl\Ast\UsersetRewrite.g4
set PARAM_OUTPUT_FOLDER=%~dp0\RebacExperiments\RebacExperiments.Acl\Ast\Generated
set PARAM_NAMESPACE=RebacExperiments.Acl.Ast.Generated

:: Run the "Antlr4" Code Generator
java -jar %ANTLR4_JAR%^
    -package %PARAM_NAMESPACE%^
    -visitor^
    -no-listener^
    -Dlanguage=CSharp^
    -Werror^
    -o %PARAM_OUTPUT_FOLDER%^
    %PARAM_USERSET_GRAMMAR%