﻿using MapThis.Dto;
using MapThis.Helpers;
using MapThis.Refactorings.MappingGenerator.Dto;
using MapThis.Services.CompoundGenerator.Interfaces;
using MapThis.Services.SingleMethodGenerator.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MapThis.Services.CompoundGenerator
{
    public class ClassMapGenerator : ICompoundMethodGenerator
    {
        private readonly MapInformationDto MapInformationDto;
        private readonly ISingleMethodGeneratorService SingleMethodGeneratorService;
        private readonly CodeAnalysisDependenciesDto CodeAnalisysDependenciesDto;
        private readonly IList<string> ExistingNamespaces;

        public ClassMapGenerator(MapInformationDto mapInformationDto, ISingleMethodGeneratorService singleMethodGeneratorService, CodeAnalysisDependenciesDto codeAnalisysDependenciesDto, IList<string> existingNamespaces)
        {
            SingleMethodGeneratorService = singleMethodGeneratorService;
            MapInformationDto = mapInformationDto;
            CodeAnalisysDependenciesDto = codeAnalisysDependenciesDto;
            ExistingNamespaces = existingNamespaces;
        }

        public GeneratedMethodsDto Generate()
        {
            var generatedMethodsDto = new GeneratedMethodsDto()
            {
                Blocks = GenerateBlocks(),
                Namespaces = GetNamespaces(),
            };

            return generatedMethodsDto;
        }

        private IList<MethodDeclarationSyntax> GenerateBlocks()
        {
            var destination = new List<MethodDeclarationSyntax>() {
                SingleMethodGeneratorService.Generate(MapInformationDto, CodeAnalisysDependenciesDto, ExistingNamespaces)
            };

            foreach (var childMethodGenerator in MapInformationDto.ChildrenMethodGenerators)
            {
                destination.AddRange(childMethodGenerator.Generate().Blocks);
            }

            return destination;
        }

        private IList<string> GetNamespaces()
        {
            var namespaces = new List<INamespaceSymbol>();

            var sourceType = MapInformationDto.MethodInformation.SourceType;
            var targetType = MapInformationDto.MethodInformation.TargetType;

            var sourceNamespace = sourceType.ContainingNamespace;
            var targetNamespace = targetType.ContainingNamespace;

            if (!ExistingNamespaces.Contains(sourceNamespace.ToDisplayString()) && !sourceType.IsSimpleTypeWithAlias())
            {
                namespaces.Add(sourceNamespace);
            }
            if (!ExistingNamespaces.Contains(targetNamespace.ToDisplayString()) && !targetType.IsSimpleTypeWithAlias())
            {
                namespaces.Add(targetNamespace);
            }

            var namespacesString = namespaces
                .Where(x => x != null && !x.IsGlobalNamespace)
                .Select(x => x.ToDisplayString())
                .ToList();

            foreach (var childMethodGenerator in MapInformationDto.ChildrenMethodGenerators)
            {
                namespacesString.AddRange(childMethodGenerator.Generate().Namespaces);
            }

            namespacesString = namespacesString
                .GroupBy(x => x)
                .Select(x => x.Key)
                .OrderBy(x => x)
                .ToList();

            return namespacesString;
        }

    }
}
