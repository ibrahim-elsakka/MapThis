﻿using MapThis.Dto;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace MapThis.Services.SingleMethodGenerator.Interfaces
{
    public interface ISingleMethodGeneratorService
    {
        MethodDeclarationSyntax Generate(MapInformationDto mapInformation, CodeAnalysisDependenciesDto codeAnalisysDependenciesDto, IList<string> existingNamespaces);
        MethodDeclarationSyntax Generate(MapCollectionInformationDto childMapCollectionInformation, CodeAnalysisDependenciesDto codeAnalisysDependenciesDto, IList<string> existingNamespaces);
    }
}
