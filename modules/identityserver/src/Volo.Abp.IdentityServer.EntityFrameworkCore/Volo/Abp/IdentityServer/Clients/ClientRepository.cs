﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Volo.Abp.IdentityServer.Clients
{
    public class ClientRepository : EfCoreRepository<IIdentityServerDbContext, Client, Guid>, IClientRepository
    {
        public ClientRepository(IDbContextProvider<IIdentityServerDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public virtual async Task<Client> FindByCliendIdAsync(
            string clientId, 
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.ClientId == clientId, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Client>> GetListAsync(string sorting, int skipCount, int maxResultCount, bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .IncludeDetails(includeDetails).OrderBy(sorting ?? nameof(Client.ClientName) + " desc")
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetTotalCount()
        {
            return await DbSet.CountAsync();
        }

        public override async Task<Client> UpdateAsync(Client entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {

            var secrets = DbContext.Set<ClientSecret>().Where(s => s.ClientId == entity.Id);

            foreach (var secret in secrets)
            {
                DbContext.Set<ClientSecret>().Remove(secret);
            }

            var claims = DbContext.Set<ClientClaim>().Where(s => s.ClientId == entity.Id);

            foreach (var claim in claims)
            {
                DbContext.Set<ClientClaim>().Remove(claim);
            }

            var grantTypes = DbContext.Set<ClientGrantType>().Where(s => s.ClientId == entity.Id);

            foreach (var grantType in grantTypes)
            {
                DbContext.Set<ClientGrantType>().Remove(grantType);
            }

            var restrictions = DbContext.Set<ClientIdPRestriction>().Where(s => s.ClientId == entity.Id);

            foreach (var restriction in restrictions)
            {
                DbContext.Set<ClientIdPRestriction>().Remove(restriction);
            }

            var properties = DbContext.Set<ClientProperty>().Where(s => s.ClientId == entity.Id);

            foreach (var clientProperty in properties)
            {
                DbContext.Set<ClientProperty>().Remove(clientProperty);
            }

            var scopes = DbContext.Set<ClientScope>().Where(s => s.ClientId == entity.Id);

            foreach (var scope in scopes)
            {
                DbContext.Set<ClientScope>().Remove(scope);
            }

            var corsOrigins = DbContext.Set<ClientCorsOrigin>().Where(s => s.ClientId == entity.Id);

            foreach (var corsOrigin in corsOrigins)
            {
                DbContext.Set<ClientCorsOrigin>().Remove(corsOrigin);
            }

            var redirectUris = DbContext.Set<ClientRedirectUri>().Where(s => s.ClientId == entity.Id);

            foreach (var redirectUri in redirectUris)
            {
                DbContext.Set<ClientRedirectUri>().Remove(redirectUri);
            }

            var postLogoutRedirectUris = DbContext.Set<ClientPostLogoutRedirectUri>().Where(s => s.ClientId == entity.Id);

            foreach (var postLogoutRedirectUri in postLogoutRedirectUris)
            {
                DbContext.Set<ClientPostLogoutRedirectUri>().Remove(postLogoutRedirectUri);
            }

            return await base.UpdateAsync(entity, autoSave, cancellationToken);
        }

        public override IQueryable<Client> WithDetails()
        {
            return GetQueryable().IncludeDetails();
        }
    }
}
