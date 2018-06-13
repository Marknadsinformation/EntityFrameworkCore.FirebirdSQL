﻿/*
 *          Copyright (c) 2017-2018 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSql
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 *
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using EntityFrameworkCore.FirebirdSql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;

namespace EFCore.FirebirdSql.FunctionalTests.TestUtilities
{
    public class FirebirdDatabaseCleaner : RelationalDatabaseCleaner
    {
        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
            => new FbDatabaseModelFactory(
                new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                    loggerFactory,
                    new LoggingOptions(),
                    new DiagnosticListener("Fake")));

        protected override bool AcceptIndex(DatabaseIndex index)
            => false;

        protected override string BuildCustomSql(DatabaseModel databaseModel)
            => string.Empty;

        protected override string BuildCustomEndingSql(DatabaseModel databaseModel)
            => string.Empty;

        protected override DropTableOperation Drop(DatabaseTable table)
            => AdDOptimizedAnnotation(base.Drop(table), table);

        protected override DropForeignKeyOperation Drop(DatabaseForeignKey foreignKey)
            => AdDOptimizedAnnotation(base.Drop(foreignKey), foreignKey.Table);

        protected override DropIndexOperation Drop(DatabaseIndex index)
            => AdDOptimizedAnnotation(base.Drop(index), index.Table);

        private static TOperation AdDOptimizedAnnotation<TOperation>(TOperation operation, DatabaseTable table)
            where TOperation : MigrationOperation => operation;
    }
}
