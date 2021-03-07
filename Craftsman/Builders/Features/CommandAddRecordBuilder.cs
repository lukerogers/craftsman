﻿namespace Craftsman.Builders.Features
{
    using Craftsman.Enums;
    using Craftsman.Exceptions;
    using Craftsman.Helpers;
    using Craftsman.Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using static Helpers.ConsoleWriter;

    public class CommandAddRecordBuilder
    {
        public static void CreateCommand(string solutionDirectory, Entity entity, string contextName, string projectBaseName)
        {
            try
            {
                var classPath = ClassPathHelper.FeaturesClassPath(solutionDirectory, $"{Utilities.AddEntityFeatureClassName(entity.Name)}.cs", entity.Plural, projectBaseName);

                if (!Directory.Exists(classPath.ClassDirectory))
                    Directory.CreateDirectory(classPath.ClassDirectory);

                if (File.Exists(classPath.FullClassPath))
                    throw new FileAlreadyExistsException(classPath.FullClassPath);

                using (FileStream fs = File.Create(classPath.FullClassPath))
                {
                    var data = "";
                    data = GetCommandFileText(classPath.ClassNamespace, entity, contextName, solutionDirectory, projectBaseName);
                    fs.Write(Encoding.UTF8.GetBytes(data));
                }

                GlobalSingleton.AddCreatedFile(classPath.FullClassPath.Replace($"{solutionDirectory}{Path.DirectorySeparatorChar}", ""));
            }
            catch (FileAlreadyExistsException e)
            {
                WriteError(e.Message);
                throw;
            }
            catch (Exception e)
            {
                WriteError($"An unhandled exception occurred when running the API command.\nThe error details are: \n{e.Message}");
                throw;
            }
        }

        public static string GetCommandFileText(string classNamespace, Entity entity, string contextName, string solutionDirectory, string projectBaseName)
        {
            var className = Utilities.AddEntityFeatureClassName(entity.Name);
            var addCommandName = Utilities.CommandAddName(entity.Name);
            var readDto = Utilities.GetDtoName(entity.Name, Dto.Read);
            var createDto = Utilities.GetDtoName(entity.Name, Dto.Creation);
            var manipulationValidator = Utilities.ValidatorNameGenerator(entity.Name, Validator.Manipulation);

            var entityName = entity.Name;
            var entityNameLowercase = entity.Name.LowercaseFirstLetter();
            var primaryKeyPropName = entity.PrimaryKeyProperty.Name;
            var commandProp = $"{entityName}ToAdd";
            var newEntityProp = $"{entityNameLowercase}ToAdd";

            var fkIncludes = Utilities.GetForeignKeyIncludes(entity);

            var entityClassPath = ClassPathHelper.EntityClassPath(solutionDirectory, "", projectBaseName);
            var dtoClassPath = ClassPathHelper.DtoClassPath(solutionDirectory, "", entity.Name, projectBaseName);
            var exceptionsClassPath = ClassPathHelper.CoreExceptionClassPath(solutionDirectory, "", projectBaseName);
            var contextClassPath = ClassPathHelper.DbContextClassPath(solutionDirectory, "", projectBaseName);

            return @$"namespace {classNamespace}
{{
    using {entityClassPath.ClassNamespace};
    using {dtoClassPath.ClassNamespace};
    using {exceptionsClassPath.ClassNamespace};
    using {contextClassPath.ClassNamespace};
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class {className}
    {{
        public class {addCommandName} : IRequest<{readDto}>
        {{
            public {createDto} {commandProp} {{ get; set; }}

            public {addCommandName}({createDto} {newEntityProp})
            {{
                {commandProp} = {newEntityProp};
            }}
        }}

        public class CustomCreate{entityName}Validation : {manipulationValidator}<{createDto}>
        {{
            public CustomCreate{entityName}Validation()
            {{
            }}
        }}

        public class Handler : IRequestHandler<{addCommandName}, {readDto}>
        {{
            private readonly {contextName} _db;
            private readonly IMapper _mapper;

            public Handler({contextName} db, IMapper mapper)
            {{
                _mapper = mapper;
                _db = db;
            }}

            public async Task<{readDto}> Handle({addCommandName} request, CancellationToken cancellationToken)
            {{
                var {entityNameLowercase} = _mapper.Map<{entityName}> (request.{commandProp});
                _db.{entity.Plural}.Add({entityNameLowercase});
                var saveSuccessful = await _db.SaveChangesAsync() > 0;

                if (saveSuccessful)
                {{
                    // include marker -- to accomodate adding includes with craftsman commands, the next line must stay as `var result = await _db.{entity.Plural}`. -- do not delete this comment
                    return await _db.{entity.Plural}{fkIncludes}
                        .ProjectTo<{readDto}>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync({entity.Lambda} => {entity.Lambda}.{primaryKeyPropName} == {entityNameLowercase}.{primaryKeyPropName});
                }}
                else
                {{
                    // add log
                    throw new Exception(""Unable to save the new record. Please check the logs for more information."");
                }}
            }}
        }}
    }}
}}";
        }
    }
}
