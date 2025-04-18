﻿using DotNetMultiTenant.Web.Core.Extensions;
using DotNetMultiTenant.Web.Data.Entities;
using DotNetMultiTenant.Web.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DotNetMultiTenant.Web.Data
{
    public class DataContext : IdentityDbContext
    {
        private string _tenantId;

        public DataContext(DbContextOptions<DataContext> options, ITenantService tenentService) : base (options)
        {
            _tenantId = tenentService.GetTenat();
        }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<CompanyUserPermission> CompanyUserPermissions => Set<CompanyUserPermission>();
        public DbSet<CompanyUserConnection> CompanyUserConnections => Set<CompanyUserConnection>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CompanyUserPermission>()
                   .HasKey(cup => new { cup.CompanyId, cup.UserId, cup.Permission });

            // Relación con Company
            builder.Entity<CompanyUserPermission>()
                .HasOne(cup => cup.Company)
                .WithMany(c => c.CompanyUserPermissions)
                .HasForeignKey(cup => cup.CompanyId)
                .OnDelete(DeleteBehavior.Restrict); // Evita cascada

            // Relación con IdentityUser
            builder.Entity<CompanyUserPermission>()
                .HasOne(cup => cup.User)
                .WithMany()
                .HasForeignKey(cup => cup.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Evita cascada



            builder.Entity<CompanyUserConnection>()
                   .HasOne(cuc => cuc.Company)
                   .WithMany()
                   .HasForeignKey(cuc => cuc.CompanyId)
                   .OnDelete(DeleteBehavior.NoAction); // Evita cascada


            builder.Entity<Country>().HasData(
            [
                new Country { Id = 1, Name = "Colombia" },
                new Country { Id = 2, Name = "México" },
                new Country { Id = 3, Name = "República Dominicana" },
            ]);

            foreach (IMutableEntityType entity in builder.Model.GetEntityTypes())
            {
                Type type = entity.ClrType;

                // Valida si la entidad es de tenant
                if (typeof(ITenantEntity).IsAssignableFrom(type))
                {
                    // Creación del filtro global para entidades tenant
                    MethodInfo? method = typeof(DataContext).GetMethod(nameof(BuildGlobalTenentFilter),
                                                                BindingFlags.NonPublic | BindingFlags.Static
                                                                )?.MakeGenericMethod(type);

                    //object? filter = method?.Invoke(null, new object[] { this });
                    object filter = method?.Invoke(null, [this])!;

                    entity.SetQueryFilter((LambdaExpression)filter);
                    entity.AddIndex(entity.FindProperty(nameof(ITenantEntity.TenantId))!);
                }
                else if (type.MustOmitTenantValidation())
                {
                    continue;
                }
                else
                {
                    throw new Exception($"La entidad {entity} no ha sido marcada como Tenant o Common");
                }
                
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (EntityEntry? item in ChangeTracker.Entries()
                                              .Where(e => e.State == EntityState.Added && e.Entity is ITenantEntity))
            {
                if (string.IsNullOrEmpty(_tenantId))
                {
                    throw new Exception("TenantId no encontrado al momento de crear el registro");
                }

                ITenantEntity? entity = item.Entity as ITenantEntity;
                entity!.TenantId = _tenantId;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private static LambdaExpression BuildGlobalTenentFilter<TEntity>(DataContext context) where TEntity : class, ITenantEntity
        {
            Expression<Func<TEntity, bool>> filter = x => x.TenantId == context._tenantId;
            return filter;
        }
    }
}
