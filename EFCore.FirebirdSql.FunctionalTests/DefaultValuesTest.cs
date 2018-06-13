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

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class DefaultValuesTest
    {
        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkFirebird()
            .BuildServiceProvider();

        [Fact]
        public void Can_use_Firebird_default_values()
        {
            using (var context = new ChipsContext(_serviceProvider, "DefaultKettleChips"))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var honeyDijon = context.Add(
                    new KettleChips
                    {
                        Name = "Honey Dijon"
                    }).Entity;

                var buffaloBleu = context.Add(
                    new KettleChips
                    {
                        Name = "Buffalo Bleu",
                        BestBuyDate = new DateTime(2111, 1, 11)
                    }).Entity;

                context.SaveChanges();

                Assert.Equal(new DateTime(2035, 9, 25), honeyDijon.BestBuyDate);
                Assert.Equal(new DateTime(2111, 1, 11), buffaloBleu.BestBuyDate);
            }

            using (var context = new ChipsContext(_serviceProvider, "DefaultKettleChips"))
            {
                Assert.Equal(new DateTime(2035, 9, 25), context.Chips.Single(c => c.Name == "Honey Dijon").BestBuyDate);
                Assert.Equal(new DateTime(2111, 1, 11), context.Chips.Single(c => c.Name == "Buffalo Bleu").BestBuyDate);
            }
        }

        private class ChipsContext : DbContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly string _databaseName;

            public ChipsContext(IServiceProvider serviceProvider, string databaseName)
            {
                _serviceProvider = serviceProvider;
                _databaseName = databaseName;
            }

            public DbSet<KettleChips> Chips { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.Entity<KettleChips>(
                    b =>
                    {
                        b.Property(e => e.BestBuyDate)
                            .ValueGeneratedOnAdd()
                            .HasDefaultValue(new DateTime(2035, 9, 25));

                        b.Property(e => e.ChipperId)
                            .IsRequired()
                            .HasDefaultValue("Default");
                    });

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
               => optionsBuilder
                   .UseFirebird(FirebirdTestStore.CreateConnectionString(_databaseName))
                   .UseInternalServiceProvider(_serviceProvider);
        }

        private class KettleChips
        {
            public int Id { get; set; }
            [StringLength(100)]
            public string Name { get; set; }
            public string ChipperId { get; set; }
            public DateTime BestBuyDate { get; set; }
        }

        public DefaultValuesTest()
            => TestStore = FirebirdTestStore.CreateInitialized("DefaultValuesTest");

        protected FirebirdTestStore TestStore { get; }

        public virtual void Dispose() => TestStore.Dispose();
    }
}
